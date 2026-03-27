using System.Text.Json.Serialization;

namespace CbPayTest.Models;

public class CbPayResponse
{
    [JsonPropertyName("generateRefOrder")]
    public string GenerateRefOrder { get; set; }

    [JsonPropertyName("code")]
    public string responseCode { get; set; }

    [JsonPropertyName("msg")]
    public string responseMessage { get; set; }
}

