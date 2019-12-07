using AutoFixture.Xunit2;
using System;
using Xmas2019.Library.Infrastructure;
using Xunit;

namespace Xmas2019.Tests.Infrastructure
{
    public class SantaMovementTests
    {
        [Theory]
        [InlineAutoData(52)]
        [InlineAutoData(43)]
        [InlineAutoData(-5)]
        public void ItShouldConvertFeetToMeter(double value, SantaMovement inputMovement)
        {
            //Arrange
            inputMovement.Value = value;

            //Act
            inputMovement.Sanitize();

            //Assert
            Assert.True(inputMovement.Direction == inputMovement.Direction);
            Assert.True(inputMovement.Unit == "meter");
            Assert.True(inputMovement.Value == value * LocationCalculator._feetToMeter);
        }
    }
}
