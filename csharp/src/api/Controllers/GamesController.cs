using api.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using services.Constants;
using services.Dtos;
using services.Interfaces;
using ResponseErrorDetailViewModel = api.ViewModels.ResponseErrorDetailViewModel;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IMapper _mapper;
    
    public GamesController(IMapper mapper, IGameService gameService)
    {
        (_mapper, _gameService) = (mapper, gameService);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateGame([FromBody] CreateGameRequestViewModel? request)
    {
        request ??= new CreateGameRequestViewModel(language: "en");
        var newGameResponseDto = await _gameService.CreateGame(_mapper.Map<CreateGameRequestDto>(request));
        return Ok(_mapper.Map<CreateGameResponseViewModel>(newGameResponseDto));
    }

    [HttpGet("{gameId:guid}")]
    public ActionResult<GameViewModel> GetGame([FromRoute] Guid gameId)
    {
        var game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The specified game ID does not exist."
                    }
                ]
            });
        }
        return Ok(_mapper.Map<MakeGuessResponseViewModel>(game));
    }

    [HttpPut("{gameId:guid}")]
    public ActionResult<MakeGuessResponseViewModel> MakeGuess([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
    {
        if (string.IsNullOrWhiteSpace(guessViewModel.Letter) || guessViewModel.Letter?.Length != 1)
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Cannot process guess",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "letter",
                        Message = "Letter cannot accept more than 1 character"
                    }
                ]
            });
        }
        
        var game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The specified game ID does not exist."
                    }
                ]
            });
        }
        
        if (game.RemainingGuesses == 0 || game.Status != GameStatuses.InProgress)
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Cannot process guess",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The game is not in progress."
                    }
                ]
            });
        }
        
        var guessResponse = _gameService.MakeGuess(gameId, _mapper.Map<GuessDto>(guessViewModel));
        return Ok(_mapper.Map<MakeGuessResponseViewModel>(guessResponse));
    }
    
    [HttpGet("{gameId:guid}/cheat")]
    public ActionResult<string> Cheat([FromRoute] Guid gameId)
    {
        var game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The specified game ID does not exist."
                    }
                ]
            });
        }

        return Ok(game.UnmaskedWord);
    }
    
    [HttpDelete("{gameId:guid}")]
    public IActionResult DeleteGame([FromRoute] Guid gameId)
    {
        var game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The specified game ID does not exist."
                    }
                ]
            });
        }
        
        var deleted = _gameService.DeleteGame(gameId);
        return deleted ? NoContent() : NotFound();
    }
}