namespace Snipcart.UPS_Webhook.Dtos.Ups
{
    public record ValidateClientResponse
    {
        public string Result { get; init; } = default! ;
        public string Type { get; init; } = default! ;
        public string LassoRedirectUrl { get; init; } = default! ;
    }
}