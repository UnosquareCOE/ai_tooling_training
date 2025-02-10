using System.Text.RegularExpressions;
using api.Constants;
using api.ViewModels;
using api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public partial class GamesController(IIdentifierGenerator identifierGenerator) : ControllerBase
{
    private static readonly Dictionary<Guid, GameViewModel> Games = new();
    private readonly string[] _words = ["banana", "canine", "unosquare", "airport"];

    [GeneratedRegex(@"[a-zA-Z0-9_]")]
    private static partial Regex GuessRegex();

    [HttpPost]
    public ActionResult<Guid> CreateGame()
    {
        var newGameWord = RetrieveWord();
        var newGameId = identifierGenerator.RetrieveIdentifier();
        
        var maskedWord = GuessRegex().Replace(newGameWord, "_");


        Games.Add(newGameId, new GameViewModel
        {
            RemainingGuesses = 5,
            UnmaskedWord = newGameWord,
            Word = maskedWord,
            Status = "In Progress",
            IncorrectGuesses = []
        });
        
        var response = new CreateGameViewModel
        {
            GameId = newGameId,
            MaskedWord = maskedWord,
            AttemptsRemaining = 5
        };

        return CreatedAtAction(nameof(GetGame), new { gameId = newGameId }, response);
    }

    [HttpGet("{gameId:guid}")]
    public ActionResult<GameViewModel> GetGame([FromRoute] Guid gameId)
    {
        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound();
        }

        var maskedWord = new string(game.UnmaskedWord.Select(c => game.IncorrectGuesses.Contains(c.ToString()) ? c : '_').ToArray());

        var response = new CheckGameStatusViewModel
        {
            MaskedWord = maskedWord,
            AttemptsRemaining = game.RemainingGuesses,
            Guesses = game.IncorrectGuesses,
            Status = game.Status
        };

        return Ok(response);
    }

    [HttpPut("{gameId:guid}")]
    public ActionResult<MakeGuessViewModel> MakeGuess([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
    {
        if (string.IsNullOrWhiteSpace(guessViewModel.Letter) || guessViewModel.Letter?.Length != 1)
        {
            return BadRequest(new
            {
                message = "Cannot process guess",
                errors = new[]
                {
                    new { field = "letter", message = "Letter cannot accept more than 1 character" }
                }
                Message = "Letter cannot accept more than 1 character"

            });
        }

        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound();
        }

        var letter = guessViewModel.Letter.ToLower();
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
            game.Status = GameStatus.Won;
        }
        else if (game.RemainingGuesses <= 0)
        {
            game.Status = GameStatus.Lost;
        }

        var response = new MakeGuessViewModel
        {
            MaskedWord = maskedWord,
            AttemptsRemaining = game.RemainingGuesses,
            Guesses = game.IncorrectGuesses,
            Status = game.Status
        };

        return Ok(response);
    }
    [HttpDelete("{gameId:guid}")]
    public IActionResult DeleteGame([FromRoute] Guid gameId)
    {
        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound();
        }

        Games.Remove(gameId);
        return NoContent();
    }
    
    private static GameViewModel? RetrieveGame(Guid gameId)
    {
        return Games.GetValueOrDefault(gameId);
    }

    private string RetrieveWord()
    {
        return _words[new Random().Next(0, _words.Length)];
    }
}