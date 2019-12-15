using System;

namespace Xmas2019.Library.Infrastructure
{
    public static class LocationCalculator
    {
        public static double _feetToMeter = 0.304800610;
        public static double _kmToMeter = 1000;
        private static double _meterInDegrees = 1 / (2 * Math.PI / 360 * 6378.137) / 1000;

        public static double CalcLatMeters(double inputLat, double inputMeter)
        {
            return inputLat + inputMeter * _meterInDegrees;
        }

        public static double CalcLonMeters(double inputLon, double inputMeter, double inputLat)
        {
            return inputLon + inputMeter * _meterInDegrees / Math.Cos(inputLat * (Math.PI / 180));
        }

        public static double ConvertFeetToMeter(double inputFeet)
        {
            return inputFeet * _feetToMeter;
        }

        public static double ConvertKmToMeter(double inputKm)
        {
            return inputKm * _kmToMeter;
        }

        public static double MoveY(double lat, double distance)
        {
            if (distance == 0d) return lat;

            return CalcLatMeters(lat, distance);
        }

        public static double MoveX(double lon, double distance, double lat)
        {
            if (distance == 0d) return lon;

            return CalcLonMeters(lon, distance, lat);
        }

    }
}
