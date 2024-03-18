using Snipcart.UPS_Webhook.Dtos;
using Snipcart.UPS_Webhook.Models;

namespace Snipcart.UPS_Webhook.Helpers
{
    public static class ShipmentHelper
    {
        public static List<Dimension> RequestsSplit(List<OrderItem> items)
        {
            var distinctPayloads = items.Select(item => item.Dimension).ToList();

            return RecursivePayloadSplit(distinctPayloads);
        }

        private static List<Dimension> RecursivePayloadSplit(List<Dimension> payloads)
        {
            var payloadList = new List<Dimension>();

            var payload = AggregatePayloads(payloads);

            if (IsValidForUps(payload))
            {
                payloadList.Add(payload);
                return payloadList;
            }

            if (payloads.Count == 1)
            {
                throw new Exception("One or multiple items are exceeding maximum dimensions.");
            }
            
            var firstHalf = payloads.Take(payloads.Count / 2).ToList();
            var secondHalf = payloads.Skip(payloads.Count / 2).ToList();

            payloadList.AddRange(RecursivePayloadSplit(firstHalf));
            payloadList.AddRange(RecursivePayloadSplit(secondHalf));

            return payloadList;
        }
    
        private static Dimension AggregatePayloads(List<Dimension> payloads)
        {
            var optimizedPayloads = payloads.Select(OptimizePayload).ToList();

            var aggregatedPayload = new Dimension
            {
                Length = optimizedPayloads.Max(x => x.Length),
                Width = optimizedPayloads.Max(x => x.Width),
                Height = optimizedPayloads.Sum(x => x.Height),
                Weight = optimizedPayloads.Sum(x => x.Weight)
            };

            return OptimizePayload(aggregatedPayload);
        }

        private static Dimension OptimizePayload(Dimension payload)
        {
            //We put largest size as length
            var tempLenght = payload.Length;
            var tempWidth = payload.Width;
            var tempHeight = payload.Height;

            var dimensions = new[] { tempLenght, tempHeight, tempWidth };
            Array.Sort(dimensions);

            payload.Length = dimensions[2];
            payload.Width = dimensions[1];
            payload.Height = dimensions[0];

            return payload;
        }
    
        private static bool IsValidForUps(Dimension dimension)
        {
            return UnitConverter.GramsToLbs(dimension.Weight) < 150 && dimension.Length < 274;
        }

        public static decimal WeightInKg(decimal weight)
        {
            return Math.Ceiling(weight * 100 / 1000m) / 100;
        }
    }
}