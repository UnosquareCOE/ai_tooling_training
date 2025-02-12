namespace api.ViewModels;

public class CreateGameRequestViewModel
{
    public CreateGameRequestViewModel()
    {
    }

    public CreateGameRequestViewModel(string language)
    {
        Language = language;
    }

    public string Language { get; set; } = "en";
}