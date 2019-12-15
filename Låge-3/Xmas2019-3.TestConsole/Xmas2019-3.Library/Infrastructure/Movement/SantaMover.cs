using System;
using System.Collections.Generic;
using System.Text;
using Xmas2019_3.Library.Infrastructure.Geo;

namespace Xmas2019_3.Library.Infrastructure.Movement
{
    public static class SantaMover
    {
        public static GeoPoint Move(GeoPoint startingPosition, IEnumerable<SantaMovement> movements)
        {
            GeoPoint currentPosition = startingPosition;
            GeoPoint newPosition = new GeoPoint(0, 0);

            foreach (SantaMovement move in movements)
            {
                double xMeters = 0d;
                double yMeters = 0d;

                switch (move.Direction)
                {
                    case "left":
                        xMeters -= move.Value;
                        break;
                    case "right":
                        xMeters += move.Value;
                        break;
                    case "up":
                        yMeters += move.Value;
                        break;
                    case "down":
                        yMeters -= move.Value;
                        break;
                    default:
                        throw new Exception("direction not supported");
                }

                newPosition.lon = LocationCalculator.MoveX(currentPosition.lon, xMeters, currentPosition.lat);
                newPosition.lat = LocationCalculator.MoveY(currentPosition.lat, yMeters);

                currentPosition = new GeoPoint(newPosition.lat, newPosition.lon);
            }

            return currentPosition;
        }
    }
}
