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

        //public static CanePosition MoveSanta(CanePosition currentPosition, SantaMovement move)
        //{
        //    if (currentPosition == null) throw new ArgumentNullException(nameof(currentPosition));
        //    if (move == null) throw new ArgumentNullException(nameof(move));

        //    CanePosition newPos = new CanePosition();

        //    newPos.lat = MoveY(currentPosition, move);
        //    newPos.lon = MoveX(currentPosition, move);

        //    return newPos;
        //}

        //public static double MoveY(CanePosition currentPosition, SantaMovement move)
        //{
        //    if (move.Direction == "left" || move.Direction == "right") return currentPosition.lat;

        //    return CalcLatMeters(currentPosition.lat, move.Value);

        //    //return CalcLatMeters(currentPosition.lat, move.Direction == "down" ? -1 * move.Value : move.Value); //todo should -1 be multiplied to result?
        //}

        public static double MoveY(double lat, double distance)
        {
            if (distance == 0d) return lat;

            return CalcLatMeters(lat, distance);

            //return CalcLatMeters(currentPosition.lat, move.Direction == "down" ? -1 * move.Value : move.Value); //todo should -1 be multiplied to result?
        }

        //public static double MoveX(CanePosition currentPosition, SantaMovement move)
        //{
        //    if (move.Direction == "up" || move.Direction == "down") return currentPosition.lon;

        //    return CalcLonMeters(currentPosition.lon, move.Value, currentPosition.lat);

        //    //return CalcLonMeters(currentPosition.lon, move.Direction == "left" ? -1 * move.Value : move.Value, currentPosition.lat); //todo should -1 be multiplied to result?            
        //}

        public static double MoveX(double lon, double distance, double lat)
        {
            if (distance == 0d) return lon;

            return CalcLonMeters(lon, distance, lat);

            //return CalcLonMeters(currentPosition.lon, move.Direction == "left" ? -1 * move.Value : move.Value, currentPosition.lat); //todo should -1 be multiplied to result?            
        }

    }
}
