namespace Snipcart.UPS_Webhook.Helpers
{
    public static class UnitConverter
    {
        public static decimal GramsToLbs(decimal grams) => grams / 453.592M;
        public static decimal GramsToKgs(decimal grams) => grams / 1000M;
        public static decimal LbsToKgs(decimal lbs) => lbs / 2.20462M;
        public static decimal CmsToInches(decimal cms) => cms / 2.54M;
        public static decimal InchesToCms(decimal inches) => inches * 2.54M;
    }
}