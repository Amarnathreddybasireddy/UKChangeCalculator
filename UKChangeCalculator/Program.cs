using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using ChangeCalculator.Services;

namespace ChangeCalculator
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Setup DI Container infrastructure
            var serviceProvider = new ServiceCollection()
            .AddSingleton<IChangeCalculatorService, UkChangeCalculatorService>()
            .BuildServiceProvider();

            // 2. Resolve service via abstraction container instead of 'new' keyword
            var calculatorService = serviceProvider.GetRequiredService<IChangeCalculatorService>();

            // 3. Instantiate presentation manager instance using injection runtime
            var app = new ApplicationEngine(calculatorService);
            app.Run();
        }
    }

    /// <summary>
    /// Encapsulates application runtime flow utilizing Constructor Injection
    /// </summary>
    public class ApplicationEngine
    {
        private readonly IChangeCalculatorService _calculatorService;

        // The dependency is loosely coupled and cleanly injected here
        public ApplicationEngine(IChangeCalculatorService calculatorService)
        {
            _calculatorService = calculatorService ?? throw new ArgumentNullException(nameof(calculatorService));
        }

        public void Run()
        {
            Console.WriteLine("=== UK Currency Change Calculator ===");
            Console.WriteLine("-------------------------------------");

            decimal totalAmount = PromptForDecimal("Enter the total customer money given: £");
            decimal productPrice = PromptForDecimal("Enter the product price: £");

            try
            {
                var changeResult = _calculatorService.CalculateChange(totalAmount, productPrice);

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
