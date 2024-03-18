using Snipcart.UPS_Webhook.Dtos;

namespace Snipcart.UPS_Webhook.Extensions
{
    public static class OrderRequestExtensions
    {
        public static decimal GetTotalWeight(this RequestContent order)
        {
            return order.Items
                .Where(item => item.Shippable)
                .Sum(item => item.Dimension.Weight * item.Quantity);
        }
    }
}