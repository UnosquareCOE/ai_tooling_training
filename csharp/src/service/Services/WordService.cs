using service.interfaces;
using Service.Constants;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace service.services
{
    public class WordService : IWordService
    {
        private readonly string[] _words = { "banana", "canine", "unosquare", "airport" };
        private readonly bool _useOfflineMode;

        public WordService()
        {
            var mode = Environment.GetEnvironmentVariable(EnvironmentVariables.UseOfflineMode) ?? Connectivity.Online;
            _useOfflineMode = mode.Equals(Connectivity.Offline, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> RetrieveWord()
        {
            if (_useOfflineMode)
            {
                return RetrieveWordOffline();
            }
            else
            {
                return await RetrieveWordOnline();
            }
        }

        private string RetrieveWordOffline()
        {
            return _words[new Random().Next(0, _words.Length)];
        }

        private async Task<string> RetrieveWordOnline()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync("https://random-word-api.herokuapp.com/word?lang=en");
                return response;
            }
        }
    }
}
