using System;
using System.Collections.Generic;
using ChangeCalculator.Models;

namespace ChangeCalculator.Services
{
    public class UkChangeCalculatorService : IChangeCalculatorService
    {
        private static readonly List<(string Name, int ValueInPence)> Denominations = new()
{
("£20", 2000), ("£10", 1000), ("£5", 500), ("£2", 200), ("£1", 100),
("50p", 50), ("20p", 20), ("10p", 10), ("5p", 5), ("2p", 2), ("1p", 1)
};

        public List<ChangeResult> CalculateChange(decimal totalAmount, decimal productPrice)
        {
            if (productPrice > totalAmount)
            {
                throw new ArgumentException("The product price cannot be greater than the money given.");
            }

            int totalInPence = (int)Math.Round(totalAmount * 100);
            int priceInPence = (int)Math.Round(productPrice * 100);
            int changeRequired = totalInPence - priceInPence;

            var results = new List<ChangeResult>();
            if (changeRequired == 0) return results;

            foreach (var denomination in Denominations)
            {
                if (changeRequired >= denomination.ValueInPence)
                {
                    int count = changeRequired / denomination.ValueInPence;
                    changeRequired %= denomination.ValueInPence;
                    results.Add(new ChangeResult(count, denomination.Name));
                }
                if (changeRequired == 0) break;
            }

            return results;
        }
    }
}
