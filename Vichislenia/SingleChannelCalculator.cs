namespace Vichislenia
{
    public static class SingleChannelCalculator
    {
        public static SmoResult Calculate(SmoType type, SmoInput input) => type switch
        {
            SmoType.SingleRefusals => Refusals(input),
            SmoType.SingleLimitedQueue => LimitedQueue(input),
            SmoType.SingleUnlimitedQueue => UnlimitedQueue(input),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        private static SmoResult Refusals(SmoInput input)
        {
            double lambda = input.LambdaBase;
            double mu = input.MuBase;
            double rho = input.Rho;

            var result = new SmoResult { Type = SmoType.SingleRefusals, Rho = rho };
            result.Steps.Add(ObshieVichislia.LoadFactor(lambda, mu, rho));

            double p0 = 1.0 / (1.0 + rho);
            result.Steps.Add(new CalculationStep("Вероятность свободного канала p0",
                $"p0 = 1/(1+ρ) = 1/(1+{ObshieVichislia.Format(rho)})", p0));

            double p1 = rho / (1.0 + rho);
            result.Steps.Add(new CalculationStep("Вероятность занятого канала p1",
                $"p1 = ρ/(1+ρ) = {ObshieVichislia.Format(rho)}/(1+{ObshieVichislia.Format(rho)})", p1));

            double pOtk = p1;
            result.Steps.Add(ObshieVichislia.RefusalProbability(pOtk, "P_отк = p1"));

            double q = 1.0 - pOtk;
            result.Steps.Add(ObshieVichislia.RelativeThroughput(pOtk, q));

            double a = lambda * q;
            result.Steps.Add(ObshieVichislia.AbsoluteThroughput(lambda, q, a));

            result.P0 = p0;
            result.P_otk = pOtk;
            result.Q = q;
            result.A = a;
            return result;
        }

        private static SmoResult LimitedQueue(SmoInput input)
        {
            double lambda = input.LambdaBase;
            double mu = input.MuBase;
            double rho = input.Rho;
            int m = input.QueueLength;

            var result = new SmoResult { Type = SmoType.SingleLimitedQueue, Rho = rho };
            result.Steps.Add(ObshieVichislia.LoadFactor(lambda, mu, rho));

            double p0;
            if (Math.Abs(rho - 1.0) < 1e-9)
            {
                p0 = 1.0 / (m + 2);
                result.Steps.Add(new CalculationStep("Вероятность p0 (ρ=1)",
                    $"p0 = 1/(m+2) = 1/({m}+2)", p0));
            }
            else
            {
                p0 = (1.0 - rho) / (1.0 - Math.Pow(rho, m + 2));
                result.Steps.Add(new CalculationStep("Вероятность p0",
                    $"p0 = (1-ρ)/(1-ρ^(m+2)) = (1-{ObshieVichislia.Format(rho)})/(1-{ObshieVichislia.Format(rho)}^({m}+2))", p0));
            }

            var p = new double[m + 2];
            p[0] = p0;
            for (int k = 1; k <= m + 1; k++)
            {
                p[k] = Math.Pow(rho, k) * p0;
                result.Steps.Add(new CalculationStep($"Вероятность p{k}",
                    $"p{k} = ρ^{k}·p0 = {ObshieVichislia.Format(rho)}^{k}·{ObshieVichislia.Format(p0)}", p[k]));
            }

            double pOtk = p[m + 1];
            result.Steps.Add(ObshieVichislia.RefusalProbability(pOtk, $"P_отк = p{m + 1}"));

            double q = 1.0 - pOtk;
            result.Steps.Add(ObshieVichislia.RelativeThroughput(pOtk, q));

            double a = lambda * q;
            result.Steps.Add(ObshieVichislia.AbsoluteThroughput(lambda, q, a));

            double lambdaEff = lambda * q;
            result.Steps.Add(ObshieVichislia.EffectiveLambda(lambda, q, lambdaEff));

            double lSist = 0;
            for (int k = 0; k <= m + 1; k++)
                lSist += k * p[k];
            result.Steps.Add(new CalculationStep("Среднее число заявок в системе L_сист",
                $"L_сист = Σ(k·p_k), k=0..{m + 1}", lSist));

            double lOch = lSist - (1.0 - p0);
            result.Steps.Add(new CalculationStep("Средняя длина очереди L_оч",
                $"L_оч = L_сист - (1 - p0) = {ObshieVichislia.Format(lSist)} - (1 - {ObshieVichislia.Format(p0)})", lOch));

            double tOch = lOch / lambdaEff;
            result.Steps.Add(new CalculationStep("Среднее время в очереди T_оч",
                $"T_оч = L_оч/λ_eff = {ObshieVichislia.Format(lOch)}/{ObshieVichislia.Format(lambdaEff)}", tOch));

            double tSist = lSist / lambdaEff;
            result.Steps.Add(new CalculationStep("Среднее время в системе T_сист",
                $"T_сист = L_сист/λ_eff = {ObshieVichislia.Format(lSist)}/{ObshieVichislia.Format(lambdaEff)}", tSist));

            result.P0 = p0;
            result.P_otk = pOtk;
            result.Q = q;
            result.A = a;
            result.L_och = lOch;
            result.T_och = tOch;
            result.L_sist = lSist;
            result.T_sist = tSist;
            return result;
        }

        private static SmoResult UnlimitedQueue(SmoInput input)
        {
            double lambda = input.LambdaBase;
            double mu = input.MuBase;
            double rho = input.Rho;

            var result = new SmoResult { Type = SmoType.SingleUnlimitedQueue, Rho = rho };
            result.Steps.Add(ObshieVichislia.LoadFactor(lambda, mu, rho));

            double p0 = 1.0 - rho;
            result.Steps.Add(new CalculationStep("Вероятность свободного канала p0",
                $"p0 = 1 - ρ = 1 - {ObshieVichislia.Format(rho)}", p0));

            double pOtk = 0;
            result.Steps.Add(ObshieVichislia.RefusalProbability(pOtk, "P_отк (очередь неограничена)"));

            double q = 1.0;
            result.Steps.Add(ObshieVichislia.RelativeThroughput(pOtk, q));

            double a = lambda;
            result.Steps.Add(ObshieVichislia.AbsoluteThroughput(lambda, q, a));

            double lOch = rho * rho / (1.0 - rho);
            result.Steps.Add(new CalculationStep("Средняя длина очереди L_оч",
                $"L_оч = ρ²/(1-ρ) = {ObshieVichislia.Format(rho)}²/(1-{ObshieVichislia.Format(rho)})", lOch));

            double lSist = rho / (1.0 - rho);
            result.Steps.Add(new CalculationStep("Среднее число заявок в системе L_сист",
                $"L_сист = ρ/(1-ρ) = {ObshieVichislia.Format(rho)}/(1-{ObshieVichislia.Format(rho)})", lSist));

            double tOch = lOch / lambda;
            result.Steps.Add(new CalculationStep("Среднее время в очереди T_оч",
                $"T_оч = L_оч/λ = {ObshieVichislia.Format(lOch)}/{ObshieVichislia.Format(lambda)}", tOch));

            double tSist = lSist / lambda;
            result.Steps.Add(new CalculationStep("Среднее время в системе T_сист",
                $"T_сист = L_сист/λ = {ObshieVichislia.Format(lSist)}/{ObshieVichislia.Format(lambda)}", tSist));

            result.P0 = p0;
            result.P_otk = pOtk;
            result.Q = q;
            result.A = a;
            result.L_och = lOch;
            result.T_och = tOch;
            result.L_sist = lSist;
            result.T_sist = tSist;
            return result;
        }
    }
}
