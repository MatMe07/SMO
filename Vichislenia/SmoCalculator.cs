namespace Vichislenia
{
    public static class SmoCalculator
    {
        public static SmoResult Calculate(SmoType type, SmoInput input)
        {
            string? error = Validate(type, input);
            if (error is not null)
                return new SmoResult { Type = type, Error = error };

            return type switch
            {
                SmoType.SingleRefusals or
                SmoType.SingleLimitedQueue or
                SmoType.SingleUnlimitedQueue =>
                    SingleChannelCalculator.Calculate(type, input),
                SmoType.MultiRefusals or
                SmoType.MultiLimitedQueue or
                SmoType.MultiUnlimitedQueue =>
                    MultiChannelCalculator.Calculate(type, input),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        private static string? Validate(SmoType type, SmoInput input)
        {
            if (input.Lambda <= 0)
                return "Интенсивность λ должна быть больше 0.";

            if (!input.Mu.HasValue && !input.TObs.HasValue)
                return "Укажите интенсивность обслуживания μ или время обслуживания t_обс.";

            if (input.Mu is <= 0)
                return "Интенсивность μ должна быть больше 0.";

            if (input.TObs is <= 0)
                return "Время обслуживания t_обс должно быть больше 0.";

            if (input.Channels < 1)
                return "Число каналов n должно быть не меньше 1.";

            if (input.QueueLength < 0)
                return "Длина очереди m должна быть не меньше 0.";

            double rho = input.Rho;

            switch (type)
            {
                case SmoType.SingleLimitedQueue:
                case SmoType.SingleUnlimitedQueue:
                    if (rho >= 1.0)
                        return $"Предельные вероятности существуют при ρ < 1 (ρ = {ObshieVichislia.Format(rho)}).";
                    break;

                case SmoType.MultiUnlimitedQueue:
                    if (rho >= input.Channels)
                        return $"Стационарный режим при ρ < n (ρ = {ObshieVichislia.Format(rho)}, n = {input.Channels}).";
                    break;
            }

            return null;
        }
    }
}
