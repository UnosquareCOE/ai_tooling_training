using AutoMapper;
using Game.Services.Dto;

namespace Game.Services.Profiles;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<DAL.Models.Game, GameDto>();

        CreateMap<DAL.Models.Game, MakeGuessDto>()
            .ForMember(d => d.AttemptsRemaining, o => o.MapFrom(s => s.RemainingGuesses))
            .ForMember(d => d.MaskedWord, o => o.MapFrom(s => s.Word))
            .ForMember(d => d.Guesses, o => o.MapFrom(s => s.IncorrectGuesses))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status));

        CreateMap<DAL.Models.Game, GameDto>()
            .ForMember(d => d.GameId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.RemainingGuesses, o => o.MapFrom(s => s.RemainingGuesses))
            .ForMember(d => d.Word, o => o.MapFrom(s => s.Word));
    }
}