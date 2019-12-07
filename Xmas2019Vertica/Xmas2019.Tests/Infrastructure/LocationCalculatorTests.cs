using System;
using System.Collections.Generic;
using System.Text;
using Xmas2019.Library.Infrastructure;
using Xunit;

namespace Xmas2019.Tests.Infrastructure
{
    public class LocationCalculatorTests
    {
        [Fact]
        public void ItShouldReturnNewLatitude()
        {
            //Arrange
            double oldLat = 71.639566053691;
            double meters = -7500d;

            //Act
            double newLat = LocationCalculator.CalcLatMeters(oldLat, meters);

            //Assert
            Assert.True(newLat == 71.572192407382033);
        }

        [Fact]
        public void ItShouldReturnNewLongitude()
        {
            //Arrange
            double oldLon = -51.1902823595313;
            double meters = 10000;
            double oldLat = 71.639566053691;

            //Act
            double newLat = LocationCalculator.CalcLonMeters(oldLon, meters, oldLat);

            //Assert
            Assert.True(newLat == -50.905097207707151);
        }

        [Fact]
        public void ItShouldReturnLatIfMeterIsZero()
        {
            //Arrange
            double inputLat = 71.639566053691;
            double inputMeter = 0;

            //Act
            double result = LocationCalculator.MoveY(inputLat, inputMeter);

            //Assert
            Assert.True(result == inputLat);
        }

        [Fact]
        public void ItShouldReturnLonIfMeterIsZero()
        {
            //Arrange
            double inputLon = 71.639566053691;
            double inputLat = 75.639566053691;
            double inputMeter = 0;

            //Act
            double result = LocationCalculator.MoveX(inputLon, inputMeter, inputLat);

            //Assert
            Assert.True(result == inputLon);
        }
    }
}
