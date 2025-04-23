using AutoMapper;
using dal;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using service.constants;
using service.dtos;
using service.interfaces;
using service.services;

namespace service.tests;

public class HangmanGameServiceTests
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWordService _wordService;
    private readonly HangmanGameService _service;

    public HangmanGameServiceTests()
    {
        _context = Substitute.For<AppDbContext>();
        _mapper = Substitute.For<IMapper>();
        _wordService = Substitute.For<IWordService>();
        _service = new HangmanGameService(_context, _mapper, _wordService);
    }

    [Fact]
    public void CreateGame_ShouldCreateNewGame()
    {
        // Arrange
        var testWord = "TEST";
        var expectedDto = new GameDto { Word = testWord, Status = "In Progress" };
        _wordService.RetrieveWord().Returns(testWord);
        _mapper.Map<GameDto>(Arg.Any<Game>()).Returns(expectedDto);
        var gamesDbSet = Substitute.For<DbSet<Game>>();
        _context.Games.Returns(gamesDbSet);

        // Act
        var result = _service.CreateGame();

        // Assert
        _context.Games.Received(1).Add(Arg.Is<Game>(g => g.Word == testWord && g.Status == "In Progress"));
        _context.Received(1).SaveChanges();
        Assert.Equal(expectedDto, result);
    }

}