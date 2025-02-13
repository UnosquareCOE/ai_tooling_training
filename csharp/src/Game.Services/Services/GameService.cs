using System.Text.Json;
using AutoMapper;
using Game.DAL.Enums;
using Game.Services.Dto;
using Game.Services.Interfaces;
using Game.Services.Utilities;
using Polly;
using Polly.Retry;
using System.Collections.Concurrent;
using Game.DAL.Interfaces;

namespace Game.Services.Services
{
    public class GameService : IGameService
    {
        private static readonly ConcurrentDictionary<Guid, GameDto> Games = new();
        private readonly IIdentifierGenerator _identifierGenerator;
        private readonly IMapper _mapper;
        private readonly IGameContext _gameContext;

        private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        public GameService(IIdentifierGenerator identifierGenerator, IMapper mapper, IGameContext gameContext)
        {
            _identifierGenerator = identifierGenerator;
            _mapper = mapper;
            _gameContext = gameContext;
        }

        public async Task<Guid> CreateGameAsync(string language)
        {
            var newGameWord = await RetrieveWordFromApi(language);
            var newGameId = _identifierGenerator.RetrieveIdentifier();

            var maskedWord = RegexHelper.GuessRegex().Replace(newGameWord, "_");

            var newGame = new DAL.Models.Game
            {
                Id = newGameId,
                RemainingGuesses = 5,
                UnmaskedWord = newGameWord,
                Word = maskedWord,
                Status = GameStatus.InProgress.ToString(),
                IncorrectGuesses = []
            };
            
            _gameContext.Games.Add(newGame);
            await _gameContext.SaveChangesAsync();
            
            return newGameId;
        }

        public async Task<GameDto?> GetGameAsync(Guid gameId)
        {
            var game = await _gameContext.Games.FindAsync(gameId);
            if (game == null)
            {
                return null;
            }

            game.Word = new string(game.UnmaskedWord.Select(c => game.IncorrectGuesses.Contains(c.ToString()) ? c : '_')
                .ToArray());

            return _mapper.Map<GameDto>(game);
        }

        public async Task<MakeGuessDto?> MakeGuess(Guid gameId, string letter)
        {
            var game = await _gameContext.Games.FindAsync(gameId);
            if (game == null)
            {
                throw new KeyNotFoundException("Game not found");
            }

            letter = letter.ToLower();
            var unmaskedWordLower = game.UnmaskedWord.ToLower();

            if (!game.IncorrectGuesses.Contains(letter) && !unmaskedWordLower.Contains(letter))
            {
                game.IncorrectGuesses.Add(letter);
                game.RemainingGuesses--;
            }
            else if (!game.IncorrectGuesses.Contains(letter) && unmaskedWordLower.Contains(letter))
            {
                game.IncorrectGuesses.Add(letter);
            }

            var maskedWord = new string(game.UnmaskedWord
                .Select(c => game.IncorrectGuesses.Contains(c.ToString().ToLower()) ? c : '_').ToArray());

            if (!maskedWord.Contains('_'))
            {
                game.Status = GameStatus.Won.ToString();
            }
            else if (game.RemainingGuesses <= 0)
            {
                game.Status = GameStatus.Lost.ToString();
            }

            game.Word = maskedWord;
            await _gameContext.SaveChangesAsync();
            return _mapper.Map<MakeGuessDto>(game);
        }

        public async Task<bool> DeleteGame(Guid gameId)
        {
            var game = await _gameContext.Games.FindAsync(gameId);
            if (game == null)
            {
                return false;
            }
            _gameContext.Games.Remove(game);
            await _gameContext.SaveChangesAsync();
            return true;
        }

        private async Task<string> RetrieveWordFromApi(string language)
        {
            using var httpClient = new HttpClient();
            var response = await RetryPolicy.ExecuteAsync(() =>
                httpClient.GetAsync($"https://random-word-api.herokuapp.com/word?lang={language}"));

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrieve word from API. Status code: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var words = JsonSerializer.Deserialize<List<string>>(responseContent);
            return words?.FirstOrDefault() ??
                   throw new HttpRequestException("API returned an empty or invalid response");
        }
    }
}