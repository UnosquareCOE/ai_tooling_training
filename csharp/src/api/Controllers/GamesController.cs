using System.Text.RegularExpressions;
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

        Games.Add(newGameId, new GameViewModel
        {
            RemainingGuesses = 3,
            UnmaskedWord = newGameWord,
            Word = GuessRegex().Replace(newGameWord, "_"),
            Status = "In Progress",
            IncorrectGuesses = []
        });

        return Ok(newGameId);
    }

    [HttpGet("{gameId:guid}")]
    public ActionResult<GameViewModel> GetGame([FromRoute] Guid gameId)
    {
        var game = RetrieveGame(gameId);
        return Ok(game);
    }

    [HttpPut("{gameId:guid}")]
    public ActionResult<GameViewModel> MakeGuess([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
    {
        if (string.IsNullOrWhiteSpace(guessViewModel.Letter) || guessViewModel.Letter?.Length != 1)
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Letter cannot accept more than 1 character"
            });
        }
        
        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found"
            });
        }

        var letter = guessViewModel.Letter.ToLower();
        var isCorrectGuess = game.UnmaskedWord!.ToLower().Contains(letter);

        if (isCorrectGuess)
        {
            // Update word display
            var wordArray = game.Word!.ToCharArray();
            var unmaskedArray = game.UnmaskedWord!.ToCharArray();
            
            for (int i = 0; i < unmaskedArray.Length; i++)
            {
                if (unmaskedArray[i].ToString().ToLower() == letter)
                {
                    wordArray[i] = unmaskedArray[i];
                }
            }
            
            game.Word = new string(wordArray);
            
            // Check if word is complete
            if (!game.Word.Contains('_'))
            {
                game.Status = "Won";
            }
        }
        else
        {
            // Add to incorrect guesses and decrease remaining
            if (!game.IncorrectGuesses.Contains(letter))
            {
                game.IncorrectGuesses.Add(letter);
                game.RemainingGuesses--;
            }
            
            // Check if game is lost
            if (game.RemainingGuesses <= 0)
            {
                game.Status = "Lost";
            }
        }

        return Ok(game);
    }

    [HttpDelete("{gameId:guid}")]
    public ActionResult DeleteGame([FromRoute] Guid gameId)
    {
        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found"
            });
        }

        if (game.Status != "In Progress")
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Cannot delete games with status Won or Lost"
            });
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