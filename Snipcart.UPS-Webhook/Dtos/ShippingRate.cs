namespace Snipcart.UPS_Webhook.Dtos
{
    public class ShippingRate
    {
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public int? GuaranteedDeliveryDays { get; set; }
        public string UserDefinedId { get; set; }
    }
}