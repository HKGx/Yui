using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Yui.Api.Imgur
{
    public class Client
    {
        public string AppId { get; set; }
        private const string BaseUrl = "https://api.imgur.com/3/";

        public Client(string appId)
        {
            AppId = appId;
        }

        public async Task<Album> GetAlbumImagesFromId(string id)
        {
            using(var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Client-ID " + AppId);
                var response =  await client.GetStringAsync(new Uri(BaseUrl + "album/" + id + "/images"));
                return Album.FromJson(response);
            }
        }
    }
}