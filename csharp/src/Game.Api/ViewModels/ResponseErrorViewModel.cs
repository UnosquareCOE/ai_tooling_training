using System.Text.Json.Serialization;

namespace api.ViewModels;

public class ResponseErrorViewModel
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("errors")]
    public List<ResponseErrorDetailViewModel> Errors { get; set; } = [];
}

public class ResponseErrorDetailViewModel
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}