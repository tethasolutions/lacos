using Lacos.GestioneCommesse.Domain.Registry;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

public interface INominatimService
{
    public Task<DistanceResult> GetDistanceAsync(string originAddress, string destinationAddress);
}

public class NominatimService : INominatimService
{
    private readonly HttpClient _httpClient;
    private const string NominatimBaseUrl = "https://nominatim.openstreetmap.org";
    private const string OsrmBaseUrl = "https://router.project-osrm.org";

    public NominatimService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        // User-Agent richiesto dalla policy di Nominatim
        if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "YourCompany-CommessaApp/1.0 (contatto: info@yourcompany.it)");
        }

    }
    public async Task<DistanceResult> GetDistanceAsync(string originAddress, string destinationAddress)
    {
        // 1) Geocoding indirizzi
        var originCoords = await GeocodeAsync(originAddress);
        var destCoords = await GeocodeAsync(destinationAddress);

        // 2) Routing con OSRM
        var route = await GetRouteAsync(originCoords, destCoords);

        decimal distanceKm;
        decimal durationMinutes;

        if (route != null)
        {
            distanceKm = (decimal)route.Distance / 1000;
            durationMinutes = (decimal)route.Duration / 60;
        }
        else
        {
            // FALLBACK: distanza “a volo d’uccello”
            distanceKm = CalculateHaversineKm(originCoords.lat, originCoords.lon, destCoords.lat, destCoords.lon);

            // Se vuoi, puoi ipotizzare una durata media (es. 50 km/h → 0.83 km/min)
            durationMinutes = (decimal)distanceKm / 50 * 60;

            Console.WriteLine($"Usato fallback Haversine per distanza tra '{originAddress}' e '{destinationAddress}'.");
        }

        // 3) Verifica Area C sulla destinazione
        bool insideAreaC = IsPointInsideAreaC(destCoords.lat, destCoords.lon);

        return new DistanceResult
        {
            DistanceKm = distanceKm,
            DurationMinutes = durationMinutes,
            IsInsideAreaC = insideAreaC,
            OriginLatitude = originCoords.lat,
            OriginLongitude = originCoords.lon,
            DestinationLatitude = destCoords.lat,
            DestinationLongitude = destCoords.lon
        };
    }


    public async Task<(double lat, double lon)> GeocodeAsync(string address)
    {
        var url = $"{NominatimBaseUrl}/search" +
                  $"?q={Uri.EscapeDataString(address)}" +
                  $"&format=json&addressdetails=0&limit=1";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var results = JsonSerializer.Deserialize<List<NominatimResult>>(json, options);

        var first = results?.FirstOrDefault();
        if (first == null)
            throw new Exception($"Nominatim non ha trovato risultati per l'indirizzo: {address}");

        if (!double.TryParse(first.Lat, System.Globalization.CultureInfo.InvariantCulture, out var lat) ||
            !double.TryParse(first.Lon, System.Globalization.CultureInfo.InvariantCulture, out var lon))
        {
            throw new Exception($"Coordinate non valide restituite da Nominatim per: {address}");
        }

        return (lat, lon);
    }

    private async Task<OsrmRoute?> GetRouteAsync((double lat, double lon) origin, (double lat, double lon) dest)
    {
        var url = $"{OsrmBaseUrl}/route/v1/driving/" +
                  $"{origin.lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                  $"{origin.lat.ToString(System.Globalization.CultureInfo.InvariantCulture)};" +
                  $"{dest.lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                  $"{dest.lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                  "?overview=false";

        try
        {
            using var response = await _httpClient.GetAsync(url);

            // SE il server risponde 4xx/5xx, non tiro giù l’app: gestisco
            if (!response.IsSuccessStatusCode)
            {
                // TODO: sostituisci con il tuo logger
                Console.WriteLine($"OSRM error: {(int)response.StatusCode} {response.ReasonPhrase}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var osrm = JsonSerializer.Deserialize<OsrmRouteResponse>(json, options);

            if (osrm == null || osrm.Code != "Ok" || osrm.Routes == null || !osrm.Routes.Any())
            {
                Console.WriteLine($"OSRM: risposta non valida o nessun percorso. Code={osrm?.Code}");
                return null;
            }

            return osrm.Routes.First();
        }
        catch (HttpRequestException ex)
        {
            // Problemi di rete, DNS, 502 ecc.
            Console.WriteLine($"OSRM HttpRequestException: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OSRM Exception: {ex.Message}");
            return null;
        }
    }

    private bool IsPointInsideAreaC(double lat, double lon)
    {
        var areaCPolygon = new (double lat, double lon)[]
        {
            // Esempio di poligono (da sostituire/raffinare con coordinate migliori)
            (45.4723, 9.1832),
            (45.4710, 9.1970),
            (45.4625, 9.2045),
            (45.4555, 9.1960),
            (45.4545, 9.1810),
            (45.4615, 9.1745),
            (45.4695, 9.1765)
        };

        return IsPointInPolygon(lat, lon, areaCPolygon);
    }
    
    private static bool IsPointInPolygon(double lat, double lon, (double lat, double lon)[] polygon)
    {
        bool inside = false;

        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            var pi = polygon[i];
            var pj = polygon[j];

            bool intersect = ((pi.lat > lat) != (pj.lat > lat)) &&
                             (lon < (pj.lon - pi.lon) * (lat - pi.lat) / (pj.lat - pi.lat) + pi.lon);

            if (intersect)
                inside = !inside;
        }

        return inside;
    }
    private static decimal CalculateHaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const decimal R = 6371; // Raggio della Terra in km

        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        decimal c = (decimal)(2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)));

        return R * c;
    }

    private static double ToRadians(double deg)
    {
        return deg * (Math.PI / 180.0);
    }

}
