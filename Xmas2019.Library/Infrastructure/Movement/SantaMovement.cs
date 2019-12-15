using System;
using Xmas2019.Library.Infrastructure;

namespace Xmas2019.Library.Infrastructure
{
    public class SantaMovement
    {
        public string Direction { get; set; }

        public double Value { get; set; }

        public string Unit { get; set; }

        public SantaMovement ConvertToMeters()
        {
            if (Unit.Equals("meter", StringComparison.InvariantCultureIgnoreCase)) return this;

            if (Unit.Equals("foot", StringComparison.InvariantCultureIgnoreCase))
            {
                SantaMovement copy = new SantaMovement() { Direction = Direction, Unit = "meter" };
                copy.Value = LocationCalculator.ConvertFeetToMeter(Value);
                return copy;
            }

            if (Unit.Equals("kilometer", StringComparison.InvariantCultureIgnoreCase))
            {
                SantaMovement copy = new SantaMovement() { Direction = Direction, Unit = "meter" };
                copy.Value = LocationCalculator.ConvertKmToMeter(Value);
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
