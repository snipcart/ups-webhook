namespace Snipcart.UPS_Webhook.Dtos.Ups.Error
{
    public record ErrorResponse
    {
        public Response Response { get; set; }
    }

    public record Response
    {
        public List<Error> Errors { get; set; }
    }

    public record Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}