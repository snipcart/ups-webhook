namespace Snipcart.UPS_Webhook.Models
{
    public class OrderItem
    {
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceWithoutTaxes { get; set; }
        public int? Weight { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Length { get; set; }
        public bool Shippable { get; set; }
        
        public Dimension Dimension => new()
        {
            Length = Length.GetValueOrDefault(),
            Width = Width.GetValueOrDefault(),
            Height = Height.GetValueOrDefault(),
            Weight = Weight.GetValueOrDefault() // Note : UPS requires a weight, 0 will return an error
        };
    }
}