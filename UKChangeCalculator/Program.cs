using System;
using System.Globalization;
using ChangeCalculator.Services;

namespace ChangeCalculator
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== UK Currency Change Calculator ===");
            Console.WriteLine("-------------------------------------");

            decimal totalAmount = PromptForDecimal("Enter the total customer money given (e.g., 20 or 20.00): £");
            decimal productPrice = PromptForDecimal("Enter the product price (e.g., 5.50): £");

            // Instantiating the service layer (Separation of Concerns)
            IChangeCalculatorService calculator = new UkChangeCalculatorService();

            try
            {
                var changeResult = calculator.CalculateChange(totalAmount, productPrice);

                Console.WriteLine("\n-------------------------------------");
                if (changeResult.Count == 0)
                {
                    Console.WriteLine("No change required. Exact amount paid.");
                    return;
                }

                Console.WriteLine("Your change is:");
                foreach (var item in changeResult)
                {
                    Console.WriteLine($"{item.Count} x {item.DenominationName}");
                }
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static decimal PromptForDecimal(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();
                input = input?.Replace("£", "").Trim();

                if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result) && result >= 0)

                {
                    return result;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Invalid input. Please enter a valid positive numeric value.");
                Console.ResetColor();
            }
        }
    }
}
