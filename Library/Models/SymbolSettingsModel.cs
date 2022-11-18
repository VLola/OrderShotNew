namespace Library.Models
{
    public class SymbolSettingsModel
    {
        public string Name { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal StepSize { get; set; }
        public decimal TickSize { get; set; }
    }
}
