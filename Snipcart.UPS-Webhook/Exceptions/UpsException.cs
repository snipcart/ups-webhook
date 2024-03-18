using Snipcart.UPS_Webhook.Dtos.Ups.Error;

namespace Snipcart.UPS_Webhook.Exceptions
{
    public class UpsException(ErrorResponse errorResponse) : Exception
    {
        public override string Message
        {
            get
            {
                var messages = errorResponse.Response?.Errors?
                    .Select(x => $"{x.Code} - {x.Message}")
                    .ToArray();

                return messages != null ? string.Join(", ", messages) : base.Message;
            }
        }
    }
}