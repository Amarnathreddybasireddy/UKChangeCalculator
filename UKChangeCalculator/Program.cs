using System;
using System.Collections.Generic;
using System.Globalization;

namespace ChangeCalculator
{
    public class Program
    {
        // Made public so the test project can access the standard UK sterling denominations
        public static readonly List<(string Name, int ValueInPence)> Denominations = new()

{
("£20", 2000),
("£10", 1000),
("£5", 500),
("£2", 200),
("£1", 100),
("50p", 50),
("20p", 20),
("10p", 10),
("5p", 5),
("2p", 2),
("1p", 1)
};

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== UK Currency Change Calculator ===");
            Console.WriteLine("-------------------------------------");

            decimal totalAmount = PromptForDecimal("Enter the total customer money given (e.g., 20 or 20.00): £");
            decimal productPrice = PromptForDecimal("Enter the product price (e.g., 5.50): £");

            if (productPrice > totalAmount)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError: The product price cannot be greater than the money given.");
                Console.ResetColor();
                return;
            }

            // Convert amounts to pence integers to guarantee absolute mathematical precision
            int totalInPence = (int)Math.Round(totalAmount * 100);
            int priceInPence = (int)Math.Round(productPrice * 100);
            int changeRequired = totalInPence - priceInPence;

            Console.WriteLine("\n-------------------------------------");
            if (changeRequired == 0)
            {
                Console.WriteLine("No change required. Exact amount paid.");
                return;
            }

            Console.WriteLine("Your change is:");
            var changeResult = CalculateChange(changeRequired);
            foreach (var item in changeResult)
            {
                Console.WriteLine($"{item.Count} x {item.DenominationName}");
            }
        }

        private static decimal PromptForDecimal(string message)
        {
            decimal result;
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();
                input = input?.Replace("£", "").Trim();

                if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out result) && result >= 0)
                {
                    return result;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Invalid input. Please enter a valid positive numeric value.");
                Console.ResetColor();
            }
        }

        // Extracted logic to a public, pure logical method that is easily unit-testable
        public static List<(int Count, string DenominationName)> CalculateChange(int changeInPence)
        {
            var results = new List<(int Count, string DenominationName)>();

            foreach (var denomination in Denominations)
            {
                if (changeInPence >= denomination.ValueInPence)
                {
                    int count = changeInPence / denomination.ValueInPence;
                    changeInPence %= denomination.ValueInPence;

                    results.Add((count, denomination.Name));
                }

                if (changeInPence == 0) break;
            }

            return results;
        }
    }
}