using System;
using System.Collections.Generic;
using service.dtos;

namespace service.interfaces
{
    public interface IHangmanGameService
    {
        GameDto CreateGame();
        GameDto? GetGameById(Guid id);
        GameDto? MakeGuess(Guid gameId, char letter);
        List<GameDto> GetAllGames();
        Guid? ClearGame(Guid gameId);
    }
}
