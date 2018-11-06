using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yui.Entities;
using Yui.Entities.Database;

namespace Yui
{
    public class SharedData
    {    
        public ConcurrentDictionary<Guild.Languages, Translation> Translations { get; set; }
       
        public CancellationTokenSource CTS { get; set; }

        public Keys Keys { get; set; }
        
        private readonly string _currentDirectory = Directory.GetCurrentDirectory();

        public async Task LoadTranslationsAsync()
        {
            Translations =
                JsonConvert.DeserializeObject<ConcurrentDictionary<Guild.Languages, Translation>>(
                    await File.ReadAllTextAsync(_currentDirectory + "/data/translations.json"));
        }



    }
}