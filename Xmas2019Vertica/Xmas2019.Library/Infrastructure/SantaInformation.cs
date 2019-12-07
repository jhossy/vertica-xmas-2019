using System;
using System.Collections.Generic;

namespace Xmas2019.Library.Infrastructure
{
    public class SantaInformation
    {
        public string Id { get; set; }

        public CanePosition CanePosition { get; set; }

        public List<SantaMovement> SantaMovements { get; set; }

        public SantaInformation()
        {
            CanePosition = new CanePosition();
            SantaMovements = new List<SantaMovement>();
        }

        public List<SantaMovement> ConvertAllMovementsToMeters()
        {
            List<SantaMovement> result = new List<SantaMovement>();
            foreach(SantaMovement movement in SantaMovements)
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

    public class CanePosition
    {
        public double lat { get; set; }

        public double lon { get; set; }

        public CanePosition()
        {
        }

        public override string ToString()
        {
            return $"({lat},{lon})";
        }
    }

    public class SantaMovement
    {
        public string Direction { get; set; }

        public double Value { get; set; }

        public string Unit { get; set; }

        public SantaMovement ConvertToMeters()
        {
            if (Unit.Equals("meter", StringComparison.InvariantCultureIgnoreCase)) return this;
                        
            if(Unit.Equals("foot", StringComparison.InvariantCultureIgnoreCase))
            {
                SantaMovement copy = new SantaMovement() { Direction = this.Direction, Unit = "meter" };
                copy.Value = LocationCalculator.ConvertFeetToMeter(this.Value);
                return copy;
            }

            if(Unit.Equals("kilometer", StringComparison.InvariantCultureIgnoreCase))
            {
                SantaMovement copy = new SantaMovement() { Direction = this.Direction, Unit = "meter" };
                copy.Value = LocationCalculator.ConvertKmToMeter(this.Value);
                return copy;
            }

            throw new Exception("Case not handled: " + Unit);
        }

        public override string ToString()
        {
            return $"Direction: {Direction}, Value: {Value}, Unit: {Unit}";
        }
    }
}
