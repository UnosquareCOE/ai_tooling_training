using AutoMapper;
using services.Dtos;

namespace services.Profiles;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<GameDto, MakeGuessResponseDto>()
            .ForMember(d => d.AttemptsRemaining, o => o.MapFrom(s => s.RemainingGuesses))
            .ForMember(d => d.MaskedWord, o => o.MapFrom(s => s.Word))
            .ForMember(d => d.Guesses, o => o.MapFrom(s => s.IncorrectGuesses))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status));

        CreateMap<GameDto, CreateGameResponseDto>()
            .ForMember(d => d.AttemptsRemaining, o => o.MapFrom(s => s.RemainingGuesses))
            .ForMember(d => d.MaskedWord, o => o.MapFrom(s => s.Word))
            .ForMember(d => d.GameId, o => o.Ignore());
    }
}