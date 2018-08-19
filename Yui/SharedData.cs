using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yui.Entities;

namespace Yui
{
    public class SharedData
    {
        public ConcurrentBag<string> HugGifs { get; set; } = new ConcurrentBag<string>();
        public ConcurrentBag<Guild> Guilds { get; set; }
        public ConcurrentDictionary<Guild.Languages, Translation> Translations { get; set; }

        public async Task SaveGuildsAsync() =>
            await File.WriteAllTextAsync(Directory.GetCurrentDirectory() + "/data/guilds.json",
                JsonConvert.SerializeObject(Guilds, Formatting.Indented));

        public async Task ReloadTranslationsAsync() => Translations =
            JsonConvert.DeserializeObject<ConcurrentDictionary<Guild.Languages, Translation>>(
                await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/data/translations.json"));

        public void ReloadHugs()
        {
            HugGifs.Clear();
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory() + "/hugs"))
            {
                if (!file.Contains("hug"))
                    continue;
                if (file.EndsWith(".gif"))
                {
                    HugGifs.Add(file);
                }
            }
        }

        public async Task LoadGuildsAsync()
        {
            Guilds = JsonConvert.DeserializeObject<ConcurrentBag<Guild>>(
                await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/data/guilds.json"));
        }
}
}