using AutoFixture.Xunit2;
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
        public void ItShouldConvertFeetToMeter(double value)
        {
            //Arrange
            SantaMovement inputMovement = new SantaMovement() { Direction = "up", Unit = "foot", Value = value };

            //Act
            SantaMovement sanitized = inputMovement.ConvertToMeters();

            //Assert
            Assert.True(sanitized.Unit == "meter");
            Assert.True(sanitized.Value == value * LocationCalculator._feetToMeter);
        }

        [Theory]
        [InlineAutoData(52)]
        [InlineAutoData(43)]
        [InlineAutoData(-5)]
        public void ItShouldNotConvertMeterToMeter(double value)
        {
            //Arrange
            SantaMovement inputMovement = new SantaMovement() { Direction = "left", Unit = "meter", Value = value };

            //Act
            SantaMovement sanitized = inputMovement.ConvertToMeters();

            //Assert
            Assert.True(sanitized.Unit == "meter");
            Assert.True(sanitized.Value == inputMovement.Value);
        }

        [Theory]
        [InlineAutoData(52)]
        [InlineAutoData(43)]
        [InlineAutoData(-5)]
        public void ItShouldConvertKmToMeter(double value)
        {
            //Arrange
            SantaMovement inputMovement = new SantaMovement() { Direction = "down", Unit = "kilometer", Value = value };

            //Act
            SantaMovement sanitized = inputMovement.ConvertToMeters();

            //Assert
            Assert.True(sanitized.Unit == "meter");
            Assert.True(sanitized.Value == LocationCalculator._kmToMeter * inputMovement.Value);
        }
    }
}
