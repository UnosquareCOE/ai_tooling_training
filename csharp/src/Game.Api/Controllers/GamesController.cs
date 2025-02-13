using api.RequestModels;
using AutoMapper;
using Game.Services.Interfaces;
using api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;

        public GamesController(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateGame([FromBody] CreateGameRequestModel? request)
        {
            request ??= new CreateGameRequestModel(language: "en");
            
            var newGameId = await _gameService.CreateGameAsync(request.Language);
            var gameDto = await _gameService.GetGameAsync(newGameId);

            return Ok(_mapper.Map<CreateGameViewModel>(gameDto));
        }

        [HttpGet("{gameId:guid}")]
        public async Task<ActionResult<CheckGameStatusViewModel>> GetGame([FromRoute] Guid gameId)
        {
            var gameDto = await _gameService.GetGameAsync(gameId);
            if (gameDto == null)
            {
                return NotFound(new ResponseErrorViewModel
                {
                    Message = "Game not found",
                    Errors = new List<ResponseErrorDetailViewModel>
                    {
                        new ResponseErrorDetailViewModel
                        {
                            Field = "gameId",
                            Message = "The specified game ID does not exist."
                        }
                    }
                });
            }

            var response = _mapper.Map<CheckGameStatusViewModel>(gameDto);
            return Ok(response);
        }

        [HttpPut("{gameId:guid}")]
        public async Task<ActionResult<MakeGuessViewModel>> MakeGuess([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
        {
            if (string.IsNullOrWhiteSpace(guessViewModel.Letter) || guessViewModel.Letter?.Length != 1)
            {
                return BadRequest(new ResponseErrorViewModel
                {
                    Message = "Cannot process guess",
                    Errors = new List<ResponseErrorDetailViewModel>
                    {
                        new ResponseErrorDetailViewModel
                        {
                            Field = "letter",
                            Message = "Letter cannot accept more than 1 character"
                        }
                    }
                });
            }

            try
            {
                var makeGuessDto = await _gameService.MakeGuess(gameId, guessViewModel.Letter.ToLower());
                var response = _mapper.Map<MakeGuessViewModel>(makeGuessDto);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{gameId:guid}")]
        public async Task<IActionResult> DeleteGame([FromRoute] Guid gameId)
        {
            var game = await _gameService.GetGameAsync(gameId);
            if (game == null)
            {
                return NotFound(new ResponseErrorViewModel
                {
                    Message = "Game not found",
                    Errors = new List<ResponseErrorDetailViewModel>
                    {
                        new ResponseErrorDetailViewModel
                        {
                            Field = "gameId",
                            Message = "The specified game ID does not exist."
                        }
                    }
                });
            }

            var deleted = await _gameService.DeleteGame(gameId);
            return deleted ? NoContent() : NotFound();
        }
    }
}