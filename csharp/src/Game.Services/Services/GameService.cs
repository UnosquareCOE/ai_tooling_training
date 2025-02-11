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
        private readonly string[] _words = { "banana", "canine", "unosquare", "airport" };
        private readonly IIdentifierGenerator _identifierGenerator;
        private readonly IMapper _mapper;

        public GameService(IIdentifierGenerator identifierGenerator, IMapper mapper)
        {
            _identifierGenerator = identifierGenerator;
            _mapper = mapper;
        }

        public Guid CreateGame()
        {
            var newGameWord = RetrieveWord();
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

        public void DeleteGame(Guid gameId)
        {
            Games.Remove(gameId);
        }

        private string RetrieveWord()
        {
            return _words[new Random().Next(0, _words.Length)];
        }

        private static GameDto? RetrieveGame(Guid gameId)
        {
            return Games.GetValueOrDefault(gameId);
        }
    }
}