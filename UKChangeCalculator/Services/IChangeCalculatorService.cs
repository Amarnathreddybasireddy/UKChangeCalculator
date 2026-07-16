using System.Collections.Generic;
using ChangeCalculator.Models;

namespace ChangeCalculator.Services
{
    public interface IChangeCalculatorService
    {
        List<ChangeResult> CalculateChange(decimal totalAmount, decimal productPrice);
    }
}