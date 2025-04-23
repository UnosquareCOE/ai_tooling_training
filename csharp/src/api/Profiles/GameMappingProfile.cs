using api.ViewModels;
using AutoMapper;
using service.dtos;
using YourNamespace.ViewModels;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GameDto, CreateGameViewModel>().ForMember(
            dest => dest.GameId,
            opt => opt.MapFrom(src => src.Id.ToString())
        ).ForMember(
            dest => dest.AttemptsRemaining,
            opt => opt.MapFrom(src => src.GuessesRemaining)
        );

        CreateMap<GameDto, CheckGameViewModel>().ForMember(
            dest => dest.AttemptsRemaining,
            opt => opt.MapFrom(src => src.GuessesRemaining)
        );
    }
}