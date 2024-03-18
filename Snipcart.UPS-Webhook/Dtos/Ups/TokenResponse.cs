using System.Text.Json.Serialization;

namespace Snipcart.UPS_Webhook.Dtos.Ups
{
    public record TokenResponse
    {
        // In seconds
        [JsonPropertyName("refresh_token_expires_in")] public string RefreshTokenExpiresIn { get; init; }
        [JsonPropertyName("expires_in")] public string ExpiresIn { get; init; }
        // Timestamps in milliseconds
        [JsonPropertyName("issued_at")]  public string IssuedAt { get; init; }
        [JsonPropertyName("refresh_token_issued_at")] public string RefreshTokenIssuedAt { get; init; }
        [JsonPropertyName("refresh_token_status")] public string RefreshTokenStatus { get; init; }
        [JsonPropertyName("token_type")] public string TokenType { get; init; }
    
        [JsonPropertyName("client_id")] public string ClientId { get; init; }
        [JsonPropertyName("access_token")] public string AccessToken { get; init; }
        [JsonPropertyName("refresh_token")] public string RefreshToken { get; init; }
        public string Scope { get; init; }
        [JsonPropertyName("refresh_count")] public string RefreshCount { get; init; }
        public string Status { get; init; }
    }
}