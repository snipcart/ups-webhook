using Microsoft.Extensions.Logging;

namespace Snipcart.UPS_Webhook.Helpers
{
    public class UpsShippingMethods
    {
        private readonly Dictionary<string, string> _shippingMethods = new()
        {
            { "01", "UPS Next Day Air" },
            { "02", "UPS 2nd Day Air" },
            { "03", "UPS Ground" },
            { "07", "UPS Worldwide Express" },
            { "08", "UPS Worldwide Expedited" },
            { "11", "UPS Standard" },
            { "12", "UPS 3 Day Select" },
            { "13", "UPS Next Day Air Saver" },
            { "14", "UPS Next Day Air Early A.M." },
            { "54", "UPS Worldwide Express Plus" },
            { "59", "UPS 2nd Day Air AM" },
            { "65", "UPS World Wide Saver" }
        };

        public string GetMethodName(string serviceCode, ILogger logger)
        {
            if (_shippingMethods.TryGetValue(serviceCode, out var methodName))
                return methodName;
        
            logger.LogWarning("Unknown service code: {serviceCode}", serviceCode);
            return "Unknown service code";
        }
    }
}