using api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.ViewModels;
using service.interfaces;
using AutoMapper;
using api.Validation;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public partial class GamesController(IHangmanGameService gameService, IMapper mapper) : ControllerBase
{

    [HttpPost]
    public ActionResult<CreateGameViewModel> CreateGame()
    {
        var dto = gameService.CreateGame();
        var result = mapper.Map<CreateGameViewModel>(dto);
        return Ok(result);
    }

    [HttpGet("{gameId:guid}")]
    public ActionResult<CheckGameViewModel> GetGame([FromRoute] Guid gameId)
    {
        var guidErrorResponse = ValidationUtils.ValidateGuid(gameId);
        if (guidErrorResponse != null)
        {
            return BadRequest(guidErrorResponse);
        }

        var dto = gameService.GetGameById(gameId);

        if (dto == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found",
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Field = "gameId",
                        Message = "Game not found"
                    }
                }
            });
        }

        var result = mapper.Map<CheckGameViewModel>(dto);
        return Ok(result);
    }

    [HttpPut("{gameId:guid}")]
    public ActionResult<ProcessGameViewModel> MakeGuess([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
    {
        var validationResult = guessViewModel.Validate();
        if (!validationResult.IsValid)
        {
            return BadRequest(ValidationUtils.CreateBadRequestResponse(validationResult));
        }

        var guidErrorResponse = ValidationUtils.ValidateGuid(gameId);
        if (guidErrorResponse != null)
        {
            return BadRequest(guidErrorResponse);
        }

        try
        {
            var dto = gameService.MakeGuess(gameId, guessViewModel.Letter!.Value);
            if (dto == null)
            {
                return NotFound();
            }

            var result = mapper.Map<CheckGameViewModel>(dto);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = e.Message
            });
        }
    }

    [HttpDelete("{gameId}")]
    public IActionResult ClearGame(Guid gameId)
    {
        var guidErrorResponse = ValidationUtils.ValidateGuid(gameId);
        if (guidErrorResponse != null)
        {
            return BadRequest(guidErrorResponse);
        }

        var removedGameId = gameService.ClearGame(gameId);
        if (removedGameId == null)
        {
            return NotFound();
        }
        return NoContent();
    }
}

