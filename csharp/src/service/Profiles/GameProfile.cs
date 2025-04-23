using AutoMapper;
using dal;
using service.dtos;
using service.utils;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<Game, GameDto>()
            .ForMember(dest => dest.MaskedWord, opt => opt.MapFrom(src =>
                new string(src.Word.Select(c => src.Guesses.Contains(c) ? c : '_').ToArray())))
            .ForMember(dest => dest.GuessesRemaining, opt => opt.MapFrom(src =>
                LetterCalculationHelper.CalculateRemainingAtempts(src.Word, src.Guesses)));
        // Add more mappings here
    }
}