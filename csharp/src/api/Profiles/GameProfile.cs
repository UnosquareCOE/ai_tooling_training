using api.ViewModels;
using AutoMapper;
using services.Dtos;

namespace api.Profiles;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<CreateGameRequestViewModel, CreateGameRequestDto>();
        CreateMap<CreateGameResponseDto, CreateGameResponseViewModel>();
        CreateMap<GameDto, GameViewModel>();
        CreateMap<GameDto, MakeGuessResponseViewModel>()
            .ForMember(d => d.Guesses, o => o.MapFrom(s => s.IncorrectGuesses))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.MaskedWord, o => o.MapFrom(s => s.Word))
            .ForMember(d => d.AttemptsRemaining, o => o.MapFrom(s => s.RemainingGuesses));
        CreateMap<GuessViewModel, GuessDto>();
        CreateMap<MakeGuessResponseDto, MakeGuessResponseViewModel>();
    }
}