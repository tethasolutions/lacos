namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncProductDto:SyncBaseDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }

        // immagine della porta rei
        public string? PictureFileName { get; set; }

        // qrcode della porta rei
        public string? QrCodePrefix { get; set; }
        public string? QrCodeNumber { get; set; }

        // associazione della porta rei con il cliente
        public long? CustomerId { get; set; }

        // associazione della porta rei con indirizzo del cliente
        public long? AddressId { get; set; }

        public long? ProductTypeId { get; set; }

        public string? Location { get; set; }
        public string? SerialNumber { get; set; }
        public string? ReiType { get; set; }
        public string? ConstructorName { get; set; }
        public bool? HasPushBar { get; set; }
        public int? Year { get; set; }
        public string? VocType { get; set; }
        public int? NumberOfDoors { get; set; }
    }
}
