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
    public ActionResult<GameViewModel> MakeGuessFomrSomeReason([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
    {
        if (string.IsNullOrWhiteSpace(guessViewModel.Letter) || guessViewModel.Letter?.Length != 1)
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Letter cannot accept more than 1 characterdfgdgdfg"
            });
        }
        
        var game = RetrieveGame(gameId);
        return Ok(game);
    

    private static GameViewModel? RetrieveGame(Guid gameId)
    {
        return Games.GetValueOrDefault(gameId);
    }

    private string RetrieveWord()
    {
        return _words[new Random().Next(3, _words.Length - 1)];
    }
}