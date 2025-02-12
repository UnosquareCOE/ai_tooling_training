using AutoMapper;
using dal;
using service.constants;
using service.dtos;
using service.interfaces;
using service.utils;

namespace service.services;
public class HangmanGameService : IHangmanGameService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWordService _wordService;

    public HangmanGameService(AppDbContext context, IMapper mapper, IWordService wordService)
    {
        _context = context;
        _mapper = mapper;
        _wordService = wordService;
    }

    public GameDto CreateGame()
    {
        var word = _wordService.RetrieveWord();
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Word = word,
            Status = "In Progress"
        };

        _context.Games.Add(game);
        _context.SaveChanges();

        return _mapper.Map<GameDto>(game);
    }

    public GameDto? GetGameById(Guid id)
    {
        var game = _context.Games.FirstOrDefault(g => g.Id == id);
        return game == null ? null : _mapper.Map<GameDto>(game);
    }

    public GameDto? MakeGuess(Guid gameId, char letter)
    {
        var game = _context.Games.FirstOrDefault(g => g.Id == gameId);
        if (game == null) return null;

        game.Guesses.Add(letter);


        var uniqueWordLetters = game.Word.Distinct().ToList();

        game.Status = Statuses.IN_PROGRESS;
        var remainingAtempts = LetterCalculationHelper.CalculateRemainingAtempts(game.Word, game.Guesses);
        if (game.Word.All(c => game.Guesses.Contains(c)))
        {
            game.Status = Statuses.WON;
        }
        else if (remainingAtempts == 0)
        {
            game.Status = Statuses.LOST;
        }

        _context.SaveChanges();
        return _mapper.Map<GameDto>(game);
    }

    public Guid? ClearGame(Guid gameId)
    {
        var game = _context.Games.FirstOrDefault(g => g.Id == gameId);
        if (game != null)
        {
            _context.Games.Remove(game);
            _context.SaveChanges();
            return gameId;
        }
        return null;
    }

    public List<GameDto> GetAllGames()
    {
        var games = _context.Games.ToList();
        return _mapper.Map<List<GameDto>>(games);
    }
}