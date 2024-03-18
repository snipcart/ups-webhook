using System.Text.Json.Serialization;
using Snipcart.UPS_Webhook.Dtos.Ups;
using Snipcart.UPS_Webhook.Models.Enums;

namespace Snipcart.UPS_Webhook.Models
{
    public class UpsToken
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int AccountId { get; init; }
        public Mode Mode { get; set; }
        public long RefreshTokenExpiresIn { get; set; }
        public string RefreshTokenStatus { get; set; }
        public string TokenType { get; set; }
        public long IssuedAt { get; set; }
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Scope { get; set; }
        public long RefreshTokenIssuedAt { get; set; }
        public long ExpiresIn { get; set; }
        public int RefreshCount { get; set; }
        public string Status { get; set; }
    
        // Expires a minute before real expiration
        [JsonIgnore]
        public bool IsExpired => IssuedAt + ExpiresIn < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 60000;
    
        [JsonIgnore]
        public bool IsRefreshExpired => RefreshTokenIssuedAt + RefreshTokenExpiresIn < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    
        // Every refresh the refresh token is also refreshed. But if the account is not used, the refresh token will expire and won't be able to refresh it, need reconnect
        [JsonIgnore]
        public bool NeedRefresh => RefreshTokenIssuedAt + RefreshTokenExpiresIn < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - (long)1000 * 60 * 60 * 24 * 30;


        public void MapFromUpsTokenResponseDto(TokenResponse tokenResponse)
        {
            RefreshTokenStatus = tokenResponse.RefreshTokenStatus;
            TokenType = tokenResponse.TokenType;
            ClientId = tokenResponse.ClientId;
            AccessToken = tokenResponse.AccessToken;
            RefreshToken = tokenResponse.RefreshToken;
            Scope = tokenResponse.Scope;
            Status = tokenResponse.Status;
            RefreshCount = int.Parse(tokenResponse.RefreshCount ?? "0");
            
            RefreshTokenExpiresIn = long.Parse(tokenResponse.RefreshTokenExpiresIn ?? "0") * 1000;
            RefreshTokenIssuedAt = long.Parse(tokenResponse.RefreshTokenIssuedAt ?? "0");
            ExpiresIn = long.Parse(tokenResponse.ExpiresIn ?? "0") * 1000;
            IssuedAt = long.Parse(tokenResponse.IssuedAt ?? "0");
        }
    }
}