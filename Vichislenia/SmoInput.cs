namespace Vichislenia
{
    public class SmoInput
    {
        public double Lambda { get; init; }
        public TimeUnit LambdaUnit { get; init; } = TimeUnit.Hour;

        public double? Mu { get; init; }
        public TimeUnit MuUnit { get; init; } = TimeUnit.Hour;

        public double? TObs { get; init; }
        public TimeUnit TObsUnit { get; init; } = TimeUnit.Hour;

        public int Channels { get; init; } = 1;
        public int QueueLength { get; init; } = 0;

        public double LambdaBase => Lambda;

        public double MuBase
        {
            get
            {
                double lambdaUnitsPerHour = UnitsPerHour(LambdaUnit);

                if (Mu.HasValue)
                {
                    double muPerHour = Mu.Value * UnitsPerHour(MuUnit);
                    return muPerHour / lambdaUnitsPerHour;
                }

                if (TObs.HasValue)
                {
                    double tObsHours = TObs.Value / UnitsPerHour(TObsUnit);
                    double tObsBase = tObsHours * lambdaUnitsPerHour;
                    return 1.0 / tObsBase;
                }

                throw new InvalidOperationException("Не задано обслуживание: укажите μ или t_обс.");
            }
        }

        public double Rho => LambdaBase / MuBase;

        private static double UnitsPerHour(TimeUnit unit) => unit switch
        {
            TimeUnit.Minute => 60.0,
            TimeUnit.Hour => 1.0,
            TimeUnit.Day => 1.0 / 24.0,
            _ => throw new ArgumentOutOfRangeException(nameof(unit))
        };
    }
}
