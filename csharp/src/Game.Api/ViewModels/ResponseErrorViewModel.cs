using System.Text.Json.Serialization;

namespace api.ViewModels;

public class ResponseErrorViewModel
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}