using System;
using System.Collections.Generic;
using Xmas2019_3.Library.Infrastructure.Geo;

namespace Xmas2019_3.Library.Infrastructure
{
    public class SantaInformation
    {
        public string Id { get; set; }

        public GeoPoint CanePosition { get; set; }

        public List<SantaMovement> SantaMovements { get; set; }

        public SantaInformation()
        {
            CanePosition = new GeoPoint(0, 0);
            SantaMovements = new List<SantaMovement>();
        }

        public List<SantaMovement> ConvertAllMovementsToMeters()
        {
            List<SantaMovement> result = new List<SantaMovement>();
            foreach (SantaMovement movement in SantaMovements)
            {
                result.Add(movement.ConvertToMeters());
            }
            return result;
        }

        public override string ToString()
        {
            return $"Id: {Id}, CanePosition: {CanePosition.ToString()}" + Environment.NewLine +
                $"Movements:" + Environment.NewLine + string.Join(",", SantaMovements);
        }

        public static SantaInformation Empty()
        {
            return new SantaInformation();
        }
    }
}
