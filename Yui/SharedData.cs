using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yui.Entities;

namespace Yui
{
    public class SharedData
    {
        private ConcurrentDictionary<Language, ConcurrentDictionary<string, string>> _translations =
            new ConcurrentDictionary<Language, ConcurrentDictionary<string, string>>();
        public CancellationTokenSource Cts;

        public string GetTranslationFor(Language lang, string forWhat) => GetTranslations(lang)[forWhat];
        public ConcurrentDictionary<string, string> GetTranslations(Language lang) => _translations[lang];
        internal async Task LoadTranslationsAsync()
        {

            const string file = "translations.json";
            if (!File.Exists(file))
            {
                var o = new ConcurrentDictionary<Language, ConcurrentDictionary<string, string>>();
                var insideDict = new ConcurrentDictionary<string, string>();
                insideDict.TryAdd("example", "example");
                o.TryAdd(Language.EN, insideDict);
                await File.WriteAllTextAsync(file,
                    JsonConvert.SerializeObject(o, Formatting.Indented));
            }
            _translations =
                JsonConvert.DeserializeObject<ConcurrentDictionary<Language, ConcurrentDictionary<string, string>>>(await File.ReadAllTextAsync(file));
        }
    }
}