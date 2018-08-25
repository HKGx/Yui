using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Yui.Api.Booru
{
    public class Gelbooru
    {
        public static async Task<List<GelbooruResult>> GetImages(string tag, int page)
        {
            using (var http = new HttpClient())
            {
                var url = Uri.EscapeUriString(
                    $"https://www.gelbooru.com/index.php?page=dapi&s=post&q=index&json=1&tags={tag}&pid={page}");
                var s = await http.GetStringAsync(url);
                var imgs = GelbooruResult.FromJson(s);
                return imgs.Count == 0 ? null : imgs;
            }
        }
    }
}