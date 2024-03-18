namespace Snipcart.UPS_Webhook.Dtos
{
    public record TokenRequest
    {
        public string AuthorizationCode { get; init; }
    }
}