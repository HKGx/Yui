using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yui.Entities;

namespace Yui
{
    public class SharedData
    {
        public ConcurrentBag<string> HugGifs { get; set; } = new ConcurrentBag<string>();
        
        public ConcurrentDictionary<Guild.Languages, Translation> Translations { get; set; }

        private readonly string _currentDirectory = Directory.GetCurrentDirectory();
        
        public CancellationTokenSource CTS { get; set; }
       

        public void ReloadHugs()
        {
            HugGifs.Clear();
            foreach (var file in Directory.GetFiles(_currentDirectory + "/hugs"))
            {
                if (!file.StartsWith("hug"))
                    continue;
                if (file.EndsWith(".gif"))
                {
                    HugGifs.Add(file);
                }
            }
        }
        public async Task LoadTranslationsAsync()
        {
            Translations =
                JsonConvert.DeserializeObject<ConcurrentDictionary<Guild.Languages, Translation>>(
                    await File.ReadAllTextAsync(_currentDirectory + "/data/translations.json"));
        }


    }
}