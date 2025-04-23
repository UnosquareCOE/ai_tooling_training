using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api.ViewModels;

public class ResponseErrorViewModel
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("errors")]
    public List<ErrorDetail> Errors { get; set; } = new List<ErrorDetail>();
}

public class ErrorDetail
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}