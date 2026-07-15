using System;
using System.Collections.Generic;
using System.Globalization;

namespace ChangeCalculator
{
    class Program
    {
        // Defined standard UK sterling denominations in pence to avoid floating-point inaccuracies
        private static readonly List<(string Name, int ValueInPence)> Denominations = new()
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

        static void Main(string[] args)
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
            CalculateAndDisplayChange(changeRequired);
        }

        private static decimal PromptForDecimal(string message)
        {
            decimal result;
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                // Strips out £ sign if the user accidentally includes it
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

        private static void CalculateAndDisplayChange(int changeInPence)
        {
            foreach (var denomination in Denominations)
            {
                if (changeInPence >= denomination.ValueInPence)
                {
                    int count = changeInPence / denomination.ValueInPence;
                    changeInPence %= denomination.ValueInPence;

                    Console.WriteLine($"{count} x {denomination.Name}");
                }

                if (changeInPence == 0) break;
            }
        }
    }
}

