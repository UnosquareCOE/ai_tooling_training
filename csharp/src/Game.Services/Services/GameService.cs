using System.Text.Json;
using AutoMapper;
using Game.DAL.Enums;
using Game.Services.Dto;
using Game.Services.Interfaces;
using Game.Services.Utilities;

namespace Game.Services.Services
{
    public class GameService : IGameService
    {
        private static readonly Dictionary<Guid, GameDto> Games = new();
        private readonly IIdentifierGenerator _identifierGenerator;
        private readonly IMapper _mapper;

        public GameService(IIdentifierGenerator identifierGenerator, IMapper mapper)
        {
            _identifierGenerator = identifierGenerator;
            _mapper = mapper;
        }

        public async Task<Guid> CreateGame(string language)
        {
            var newGameWord = await RetrieveWordFromApi(language);
            var newGameId = _identifierGenerator.RetrieveIdentifier();

            var maskedWord = RegexHelper.GuessRegex().Replace(newGameWord, "_");

            Games.Add(newGameId, new GameDto
            {
                GameId = newGameId,
                RemainingGuesses = 5,
                UnmaskedWord = newGameWord,
                Word = maskedWord,
                Status = GameStatus.InProgress.ToString(),
                IncorrectGuesses = new List<string>()
            });

            return newGameId;
        }

        public GameDto? GetGame(Guid gameId)
        {
            var game = Games.GetValueOrDefault(gameId);
            if (game == null)
            {
                return null;
            }

            game.Word = new string(game.UnmaskedWord.Select(c => game.IncorrectGuesses.Contains(c.ToString()) ? c : '_').ToArray());

            return game;
        }

        public MakeGuessDto MakeGuess(Guid gameId, string letter)
        {
            var game = RetrieveGame(gameId);
            if (game == null)
            {
                throw new KeyNotFoundException("Game not found");
            }

            if (!game.IncorrectGuesses.Contains(letter) && !game.UnmaskedWord.Contains(letter))
            {
                game.IncorrectGuesses.Add(letter);
                game.RemainingGuesses--;
            }
            else if (!game.IncorrectGuesses.Contains(letter) && game.UnmaskedWord.Contains(letter))
            {
                game.IncorrectGuesses.Add(letter);
            }

            var maskedWord = new string(game.UnmaskedWord.Select(c => game.IncorrectGuesses.Contains(c.ToString()) ? c : '_').ToArray());

            if (!maskedWord.Contains('_'))
            {
                game.Status = GameStatus.Won.ToString();
            }
            else if (game.RemainingGuesses <= 0)
            {
                game.Status = GameStatus.Lost.ToString();
            }

            return new MakeGuessDto
            {
                MaskedWord = maskedWord,
                AttemptsRemaining = game.RemainingGuesses,
                Guesses = game.IncorrectGuesses,
                Status = game.Status
            };
        }

        public bool DeleteGame(Guid gameId)
        {
            var game = GetGame(gameId);
            if (game == null)
            {
                return false;
            }

            Games.Remove(gameId);
            return true;
            
        }
        private static GameDto? RetrieveGame(Guid gameId)
        {
            return Games.GetValueOrDefault(gameId);
        }
        
        private async Task<string> RetrieveWordFromApi(string language)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync($"https://random-word-api.herokuapp.com/word?lang={language}");
            var words = JsonSerializer.Deserialize<List<string>>(response);
            return words?.FirstOrDefault() ?? "defaultword";
        }
    }
}