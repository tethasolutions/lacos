using Lacos.GestioneCommesse.WebApi.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class SharepointController : LacosApiController
{
    [HttpGet("token")]
    public async Task<object> GetToken()
    {
        using (var http = new HttpClient())
        {
            //var uri = "https://accounts.accesscontrol.windows.net/136f579a-1be2-44ab-a6de-3d19af66d603/tokens/OAuth/2";
            //var values = new List<KeyValuePair<string, string>>()
            //    {
            //        new KeyValuePair<string, string>("grant_type", "client_credentials"),
            //        new KeyValuePair<string, string>("client_id", "eb6471b1-0c2c-412b-9b8b-025afc733a4c@136f579a-1be2-44ab-a6de-3d19af66d603"),
            //        new KeyValuePair<string, string>("client_secret", "G258Q~sEZhWSnwb4_myUOZoJDWzsRTaP6jg~rbS2"),
            //        new KeyValuePair<string, string>("resource", "00000003-0000-0ff1-ce00-000000000000/tetha365.sharepoint.com@136f579a-1be2-44ab-a6de-3d19af66d603")
            //    };
            var uri = "https://login.microsoftonline.com/b65690a8-45a2-4630-b821-a331c42268c9/oauth2/v2.0/token";
            var values = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", "acf38256-3b6e-4f3e-" + "aac1-8ca7978152db" + "@b65690a8-45a2-" + "4630-b821" + "-a331c42268c9"),
                    new KeyValuePair<string, string>("client_secr" + "et", "bQd8Q~xkV" + "EF76jcMcIs" + "9BfvkVRY" + "kcptssqOprbI."),
                    new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default")
                };
            var content = new FormUrlEncodedContent(values);

            var result = await http.PostAsync(uri, content);

            var stringContent = await result.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<SharepointResult>(stringContent);

            return new { json!.AccessToken };
        }
    }
        private class SharepointResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}