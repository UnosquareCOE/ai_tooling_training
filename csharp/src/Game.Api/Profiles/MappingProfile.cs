using api.ViewModels;
using AutoMapper;
using Game.Services.Dto;

namespace api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigureDtoToViewModel();
        ConfigureViewModelToDto();
    }

    private void ConfigureDtoToViewModel()
    {
        CreateMap<GameDto, GameViewModel>();
        CreateMap<MakeGuessDto, MakeGuessViewModel>();
        CreateMap<GameDto, CreateGameViewModel>()
            .ForMember(d => d.MaskedWord, o => o.MapFrom(x => x.Word))
            .ForMember(d => d.AttemptsRemaining, o => o.MapFrom(x => x.RemainingGuesses));
        
        CreateMap<GameDto, CheckGameStatusViewModel>()
            .ForMember(d => d.MaskedWord, o => o.MapFrom(x => x.Word))
            .ForMember(d => d.AttemptsRemaining, o => o.MapFrom(x => x.RemainingGuesses))
            .ForMember(d => d.Guesses, o => o.MapFrom(x => x.IncorrectGuesses));
    }

    private void ConfigureViewModelToDto()
    {
        CreateMap<GameViewModel, GameDto>();
        CreateMap<MakeGuessViewModel, MakeGuessDto>();
        CreateMap<CreateGameViewModel, GameDto>()
            .ForMember(d => d.Word, o => o.MapFrom(x => x.MaskedWord));
    }
}