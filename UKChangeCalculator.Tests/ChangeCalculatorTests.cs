using Xunit;
using System;
using System.Collections.Generic;
using ChangeCalculator.Services;
using ChangeCalculator.Models;

namespace UKChangeCalculator.Tests
{
    public class ChangeCalculatorTests
    {
        private readonly IChangeCalculatorService _service;

        public ChangeCalculatorTests()
        {
            // Instantiating the refactored service layer
            _service = new UkChangeCalculatorService();
        }

        #region Core Functionality & Standard Scenarios

        [Fact]
        public void CalculateChange_WithStandardScenario_ReturnsCorrectDenominations()
        {
            // Scenario: £20 paid for a £5.50 product. Change required is £14.50.
            // Act
            var result = _service.CalculateChange(20.00m, 5.50m);

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
            // Scenario: Exact payment made.
            // Act
            var result = _service.CalculateChange(10.00m, 10.00m);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region Edge Cases & Boundary Conditions

        [Fact]
        public void CalculateChange_WithSmallestDenominations_ReturnsCorrectBreakdown()
        {
            // Scenario: Change required is £0.03 (3p).
            // Act
            var result = _service.CalculateChange(10.03m, 10.00m);

            // Assert
            Assert.Collection(result,
            item => { Assert.Equal(1, item.Count); Assert.Equal("2p", item.DenominationName); },
            item => { Assert.Equal(1, item.Count); Assert.Equal("1p", item.DenominationName); }
            );
        }

        [Fact]
        public void CalculateChange_WithLargeComplexAmount_ReturnsAccurateBreakdown()
        {
            // Scenario: High change value (£88.88) requiring multiple units across almost all denominations.
            // Act
            var result = _service.CalculateChange(100.00m, 11.12m);

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
        public void CalculateChange_WithPriceGreaterThanGivenAmount_ThrowsArgumentException()
        {
            // Scenario: Guarding against downstream mathematical errors when customer doesn't offer enough cash.
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.CalculateChange(5.00m, 10.00m));
            Assert.Equal("The product price cannot be greater than the money given.", exception.Message);
        }

        #endregion

        #region Data-Driven Tests (Theory)

        [Theory]
        [InlineData(20.00, 0.00, "£20", 1)]
        [InlineData(10.00, 0.00, "£10", 1)]
        [InlineData(5.00, 0.00, "£5", 1)]
        [InlineData(2.00, 0.00, "£2", 1)]
        [InlineData(1.00, 0.00, "£1", 1)]
        [InlineData(0.50, 0.00, "50p", 1)]
        [InlineData(0.20, 0.00, "20p", 1)]
        [InlineData(0.10, 0.00, "10p", 1)]
        [InlineData(0.05, 0.00, "5p", 1)]
        [InlineData(0.02, 0.00, "2p", 1)]
        [InlineData(0.01, 0.00, "1p", 1)]
        public void CalculateChange_WithSingleExactNoteOrCoin_ReturnsOneItem(decimal paid, decimal price, string expectedName, int expectedCount)
        {
            // Act
            var result = _service.CalculateChange(paid, price);

            // Assert
            Assert.Single(result);
            Assert.Equal(expectedCount, result[0].Count);
            Assert.Equal(expectedName, result[0].DenominationName);
        }

        [Theory]
        [InlineData(40.00, 0.00, "£20", 2)]
        [InlineData(30.00, 0.00, "£20", 1)]
        [InlineData(0.04, 0.00, "2p", 2)]
        public void CalculateChange_WithMultiplesOfSameDenomination_CalculatesCorrectCount(decimal paid, decimal price, string expectedName, int expectedCount)
        {
            // Act
            var result = _service.CalculateChange(paid, price);
            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedCount, result[0].Count);
            Assert.Equal(expectedName, result[0].DenominationName);
        }

        #endregion
    }
}