using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Yui.Api.Kitsu
{
    public static class Kitsu
    {
        public static async Task<Anime> GetAnimeAsync(string name)
        {
            using (var http = new HttpClient())
            {
                var url = Uri.EscapeUriString($"https://kitsu.io/api/edge/anime?filter[text]={name}");
                var s = await http.GetStringAsync(url);
                var anime = Anime.FromJson(s);
                return anime.Meta.Count == 0 ? null : anime;
            }
        }
    }
}