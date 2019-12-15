namespace Xmas2019.Library.Infrastructure
{
    public static class RadiusConverter
    {
        public static double _kmToMeter = 1000;
        public static double _feetToMeter = 0.304800610;

        public static double ConvertFeetToMeter(double inputFeet)
        {
            return inputFeet * _feetToMeter;
        }

        public static double ConvertKmToMeter(double inputKm)
        {
            return inputKm * _kmToMeter;
        }
    }
}
