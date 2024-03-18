using System.Text.Json.Serialization;
using Snipcart.UPS_Webhook.Converters;

namespace Snipcart.UPS_Webhook.Dtos.Ups.Rating
{
    // Done as best as possible, documentation https://developer.ups.com/api/referenceloc=en_US#operation/Rate
    // Is not the same as https://github.com/UPS-API/java-api-examples/blob/main/rating/src/main/resources/scenarios/simpleRateShopResponse.json
    public record ShopRateResponse
    {
        public RateResponse RateResponse { get; set; }
    }

    public record RateResponse
    {
        public Response Response { get; set; }
        public List<RatedShipment> RatedShipment { get; set; }
    }

    public record Response
    {
        // Identifies the success or failure of the transaction. 1 = Successful
        public CodeDescription ResponseStatus { get; set; }
        public List<CodeDescription> Alert { get; set; }
        public List<AlertDetail> AlertDetail { get; set; }
    
        // Object which defaults to empty string instead of null.....
        // Can not exist, be an empty string or have a CustomerContext variable of type string inside
        [JsonConverter(typeof(EmptyStringAsNullConverter<TransactionReference>))]
        public TransactionReference TransactionReference { get; set; }
    }

    public record TransactionReference
    {
        public string CustomerContext { get; set; }
    }

    public record CodeDescription
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public record AlertDetail
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public ElementLevelInformation ElementLevelInformation { get; set; }
    }

    public class ElementLevelInformation
    {
        public string Level { get; set; }
        public List<ElementIdentifier> ElementIdentifier { get; set; }
    }

    public class ElementIdentifier
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public record RatedShipment
    {
        public CodeDescription Disclaimer { get; set; }
        public CodeDescription Service { get; set; }
        public string RateChart { get; set; }
        [JsonConverter(typeof(SingleOrArrayConverter<CodeDescription>))]
        public List<CodeDescription> RatedShipmentAlert { get; set; }
        public BillingWeight BillingWeight { get; set; }
        public Charge TransportationCharges { get; set; }
        public Charge BaseServiceCharge { get; set; }
        public Charge ServiceOptionsCharges { get; set; }
        public Charge TaxCharges { get; set; }
        public Charge TotalCharges { get; set; }
        public GuaranteedDelivery GuaranteedDelivery { get; set; }
        public RatedPackage RatedPackage { get; set; }
    }

    public record BillingWeight
    {
        public CodeDescription UnitOfMeasurement { get; set; }
        public string Weight { get; set; }
    }

    public record Charge
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }

// Not in their API documentation, but it's in the response
    public record GuaranteedDelivery
    {
        public string BusinessDaysInTransit { get; set; }
        public string DeliveryByTime { get; set; }
    }

    public record RatedPackage
    {
        public Charge TransportationCharges { get; set; }
        public Charge BaseServiceCharge { get; set; }
        public Charge ServiceOptionsCharges { get; set; }
        public BillingWeight BillingWeight { get; set; }
        public CodeDescription Accessorial { get; set; }
        [JsonConverter(typeof(SingleOrArrayConverter<ItemizedCharge>))]
        public List<ItemizedCharge> ItemizedCharges { get; set; }
        public Charge TotalCharges { get; set; }
        public string Weight { get; set; }
    }

    public record ItemizedCharge
    {
        public string Code { get; set; }
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
        public string SubType { get; set; }
        public string Description { get; set; }
    }
}