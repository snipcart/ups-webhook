using Snipcart.UPS_Webhook.Models;

namespace Snipcart.UPS_Webhook.Dtos
{ 
    // Check your dashboard webhooks to know all available properties
    public record OrderRequest
    {
        public RequestContent Content { get; set; }
    }

    public record RequestContent
    {
        public string Token { get; set; }
        public string Mode { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public string Lang { get; set; }
        
        public List<OrderItem> Items { get; set; }
        
        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }
    }
}