using Xunit;
using ChangeCalculator;
using System.Collections.Generic;

namespace UKChangeCalculator.Tests
{
    public class ChangeCalculatorTests
    {
        #region Core Functionality & Standard Scenarios

        [Fact]
        public void CalculateChange_WithStandardScenario_ReturnsCorrectDenominations()
        {
            // Scenario: £20 paid for a £5.50 product. Change required is £14.50 (1450 pence).
            // Arrange
            int changeInPence = 1450;

            // Act
            var result = Program.CalculateChange(changeInPence);

            // Assert
            Assert.Collection(result,
            item => { Assert.Equal(1, item.Count); Assert.Equal("£10", item.DenominationName); },
            item => { Assert.Equal(2, item.Count); Assert.Equal("£2", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("50p", item.DenominationName); }
            );
        }

        [Fact]
        public void CalculateChange_WithNoChangeRequired_ReturnsEmptyList()
        {
            // Scenario: Exact payment made. Change required is 0 pence.
            // Arrange
            int changeInPence = 0;

            // Act
            var result = Program.CalculateChange(changeInPence);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region Edge Cases & Boundary Conditions

        [Fact]
        public void CalculateChange_WithSmallestDenominations_ReturnsCorrectBreakdown()
        {
            // Scenario: Change required is 3 pence (3p). Tests correct degradation to minimum currency units.
            // Arrange
            int changeInPence = 3;

            // Act
            var result = Program.CalculateChange(changeInPence);

            // Assert
            Assert.Collection(result,
            item => { Assert.Equal(1, item.Count); Assert.Equal("2p", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("1p", item.DenominationName); }
            );
        }

        [Fact]
        public void CalculateChange_WithLargeComplexAmount_ReturnsAccurateBreakdown()
        {
            // Scenario: High change value (£88.88 -> 8888 pence) requiring multiple units across almost all denominations.
            // Arrange
            int changeInPence = 8888;

            // Act
            var result = Program.CalculateChange(changeInPence);

            // Assert
            Assert.Collection(result,
            item => { Assert.Equal(4, item.Count); Assert.Equal("£20", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("£5", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("£2", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("£1", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("50p", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("20p", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("10p", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("5p", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("2p", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("1p", item.DenominationName); }
            );
        }

        [Fact]
        public void CalculateChange_WithNegativePence_ReturnsEmptyListSafely()
        {
            // Scenario: Guarding against downstream mathematical errors resulting in negative values.
            // Arrange
            int changeInPence = -100;

            // Act
            var result = Program.CalculateChange(changeInPence);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region Data-Driven Tests (Theory)

        [Theory]
        [InlineData(2000, "£20", 1)]
        [InlineData(1000, "£10", 1)]
        [InlineData(500, "£5", 1)]
        [InlineData(200, "£2", 1)]
        [InlineData(100, "£1", 1)]
        [InlineData(50, "50p", 1)]
        [InlineData(20, "20p", 1)]
        [InlineData(10, "10p", 1)]
        [InlineData(5, "5p", 1)]
        [InlineData(2, "2p", 1)]
        [InlineData(1, "1p", 1)]
        public void CalculateChange_WithSingleExactNoteOrCoin_ReturnsOneItem(int changeInPence, string expectedName, int expectedCount)
        {
            // Act
            var result = Program.CalculateChange(changeInPence);

            // Assert
            Assert.Single(result);
            Assert.Equal(expectedCount, result[0].Count);
            Assert.Equal(expectedName, result[0].DenominationName);
        }

        [Theory]
        [InlineData(4000, "£20", 2)]
        [InlineData(3000, "£20", 1)] // Followed by £10 in the subsequent logic assertion
        [InlineData(4, "2p", 2)]
        public void CalculateChange_WithMultiplesOfSameDenomination_CalculatesCorrectCount(int changeInPence, string expectedName, int expectedCount)
        {
            // Act
            var result = Program.CalculateChange(changeInPence);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedCount, result[0].Count);
            Assert.Equal(expectedName, result[0].DenominationName);
        }

        #endregion
    }
}