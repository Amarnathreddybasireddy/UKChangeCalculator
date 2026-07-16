namespace ChangeCalculator.Models
{
    public class ChangeResult
    {
        public int Count { get; set; }
        public string DenominationName { get; set; } = string.Empty;

        public ChangeResult(int count, string denominationName)
        {
            Count = count;
            DenominationName = denominationName;
        }
    }
}
