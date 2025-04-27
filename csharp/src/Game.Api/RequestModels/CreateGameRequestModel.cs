namespace api.RequestModels;

public class CreateGameRequestModel
{
    public CreateGameRequestModel()
    {
    }

    public CreateGameRequestModel(string language)
    {
        Language = language;
    }

    public string Language { get; set; } = "en";
}