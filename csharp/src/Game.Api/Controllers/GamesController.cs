using api.RequestModels;
using AutoMapper;
using Game.Services.Interfaces;
using api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;

        public GamesController(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateGame([FromBody] CreateGameRequestModel requestModel)
        {
            var newGameId = await _gameService.CreateGame(requestModel.Language);
            var gameDto = _gameService.GetGame(newGameId);
            var response = _mapper.Map<CreateGameViewModel>(gameDto);

            return Ok(response);
        }

        [HttpGet("{gameId:guid}")]
        public ActionResult<GameViewModel> GetGame([FromRoute] Guid gameId)
        {
            var gameDto = _gameService.GetGame(gameId);
            if (gameDto == null)
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

            var response = _mapper.Map<CheckGameStatusViewModel>(gameDto);
            return Ok(response);
        }

        [HttpPut("{gameId:guid}")]
        public ActionResult<MakeGuessViewModel> MakeGuess([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
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

            try
            {
                var makeGuessDto = _gameService.MakeGuess(gameId, guessViewModel.Letter.ToLower());
                var response = _mapper.Map<MakeGuessViewModel>(makeGuessDto);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
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
}