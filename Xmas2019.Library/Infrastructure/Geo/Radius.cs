﻿using Xmas2019.Library.Infrastructure;

namespace Xmas2019.Library.Infrastructure.Geo
{
    public class Radius
    {
        public string Unit { get; set; }

        public double Value { get; set; }

        public Radius Normalize()
        {
            if (Unit == "meter") return this;

            Radius radius = new Radius() { Unit = "meter" };

            if (Unit == "foot")
            {
                radius.Value = RadiusConverter.ConvertFeetToMeter(Value);
                return radius;
            }

            radius.Value = RadiusConverter.ConvertKmToMeter(Value);
            return radius;
        }
    }
}
