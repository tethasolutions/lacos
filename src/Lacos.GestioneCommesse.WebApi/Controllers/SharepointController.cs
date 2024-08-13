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
            var uri = "https://accounts.accesscontrol.windows.net/136f579a-1be2-44ab-a6de-3d19af66d603/tokens/OAuth/2";
            var values = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", "aabb1280-67e0-4495-ab1c-65ffd96e891f@136f579a-1be2-44ab-a6de-3d19af66d603"),
                    new KeyValuePair<string, string>("client_secret", "IQX8Q~43FlFvK-VkNbkOkbVoswHcuApIZivVFbq_"),
                    new KeyValuePair<string, string>("resource", "00000003-0000-0ff1-ce00-000000000000/tetha365.sharepoint.com@136f579a-1be2-44ab-a6de-3d19af66d603")
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