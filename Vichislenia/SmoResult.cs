namespace Vichislenia
{
    public class SmoResult
    {
        public SmoType Type { get; init; }
        public double Rho { get; set; }
        public double P0 { get; set; }
        public double P_otk { get; set; }
        public double Q { get; set; }
        public double A { get; set; }
        public double L_och { get; set; }
        public double T_och { get; set; }
        public double L_sist { get; set; }
        public double T_sist { get; set; }
        public double? K_zan { get; set; }
        public double? P_wait { get; set; }
        public List<CalculationStep> Steps { get; } = new();
        public string? Error { get; set; }
        public bool Success => Error is null;
    }
}
