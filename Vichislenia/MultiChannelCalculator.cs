namespace Vichislenia
{
    public static class MultiChannelCalculator
    {
        public static SmoResult Calculate(SmoType type, SmoInput input) => type switch
        {
            SmoType.MultiRefusals => Refusals(input),
            SmoType.MultiLimitedQueue => LimitedQueue(input),
            SmoType.MultiUnlimitedQueue => UnlimitedQueue(input),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        private static SmoResult Refusals(SmoInput input)
        {
            double lambda = input.LambdaBase;
            double mu = input.MuBase;
            double rho = input.Rho;
            int n = input.Channels;

            var result = new SmoResult { Type = SmoType.MultiRefusals, Rho = rho };
            result.Steps.Add(ObshieVichislia.LoadFactor(lambda, mu, rho));

            double sum = ObshieVichislia.SumFactorialPowers(rho, n);
            result.Steps.Add(new CalculationStep("Сумма ряда для p0",
                $"Σ = Σ(ρ^k/k!), k=0..{n}", sum));

            double p0 = 1.0 / sum;
            result.Steps.Add(new CalculationStep("Вероятность p0",
                $"p0 = 1/Σ = 1/{ObshieVichislia.Format(sum)}", p0));

            double pn = ObshieVichislia.Pk(rho, n) * p0;
            double pOtk = pn;
            result.Steps.Add(ObshieVichislia.RefusalProbability(pOtk,
                $"P_отк = p{n} = ρ^{n}/{n}!·p0"));

            double q = 1.0 - pOtk;
            result.Steps.Add(ObshieVichislia.RelativeThroughput(pOtk, q));

            double a = lambda * q;
            result.Steps.Add(ObshieVichislia.AbsoluteThroughput(lambda, q, a));

            double kZan = rho * (1.0 - pOtk);
            result.Steps.Add(new CalculationStep("Среднее число занятых каналов K_зан",
                $"K_зан = ρ·(1-P_отк) = {ObshieVichislia.Format(rho)}·(1-{ObshieVichislia.Format(pOtk)})", kZan));

            result.P0 = p0;
            result.P_otk = pOtk;
            result.Q = q;
            result.A = a;
            result.K_zan = kZan;
            return result;
        }

        private static SmoResult LimitedQueue(SmoInput input)
        {
            double lambda = input.LambdaBase;
            double mu = input.MuBase;
            double rho = input.Rho;
            int n = input.Channels;
            int m = input.QueueLength;

            var result = new SmoResult { Type = SmoType.MultiLimitedQueue, Rho = rho };
            result.Steps.Add(ObshieVichislia.LoadFactor(lambda, mu, rho));

            double factN = ObshieVichislia.Factorial(n);
            double denom = 0;
            for (int k = 0; k <= n; k++)
                denom += ObshieVichislia.Pk(rho, k);
            for (int k = n + 1; k <= n + m; k++)
                denom += Math.Pow(rho, k) / (factN * Math.Pow(n, k - n));

            result.Steps.Add(new CalculationStep("Сумма ряда для p0",
                $"Σ = Σ(ρ^k/k!), k=0..{n} + Σ(ρ^k/(n!·n^(k-n))), k={n + 1}..{n + m}", denom));

            double p0 = 1.0 / denom;
            result.Steps.Add(new CalculationStep("Вероятность p0",
                $"p0 = 1/Σ = 1/{ObshieVichislia.Format(denom)}", p0));

            var p = new double[n + m + 1];
            for (int k = 0; k <= n; k++)
                p[k] = ObshieVichislia.Pk(rho, k) * p0;
            for (int k = n + 1; k <= n + m; k++)
                p[k] = Math.Pow(rho, k) / (factN * Math.Pow(n, k - n)) * p0;

            double pOtk = p[n + m];
            result.Steps.Add(ObshieVichislia.RefusalProbability(pOtk, $"P_отк = p{n + m}"));

            double q = 1.0 - pOtk;
            result.Steps.Add(ObshieVichislia.RelativeThroughput(pOtk, q));

            double a = lambda * q;
            result.Steps.Add(ObshieVichislia.AbsoluteThroughput(lambda, q, a));

            double lambdaEff = lambda * q;
            result.Steps.Add(ObshieVichislia.EffectiveLambda(lambda, q, lambdaEff));

            double lOch = 0;
            for (int k = n + 1; k <= n + m; k++)
                lOch += (k - n) * p[k];
            result.Steps.Add(new CalculationStep("Средняя длина очереди L_оч",
                $"L_оч = Σ((k-n)·p_k), k={n + 1}..{n + m}", lOch));

            double lSist = 0;
            for (int k = 0; k <= n + m; k++)
                lSist += k * p[k];
            result.Steps.Add(new CalculationStep("Среднее число заявок в системе L_сист",
                $"L_сист = Σ(k·p_k), k=0..{n + m}", lSist));

            double tOch = lOch / lambdaEff;
            result.Steps.Add(new CalculationStep("Среднее время в очереди T_оч",
                $"T_оч = L_оч/λ_eff = {ObshieVichislia.Format(lOch)}/{ObshieVichislia.Format(lambdaEff)}", tOch));

            double tSist = lSist / lambdaEff;
            result.Steps.Add(new CalculationStep("Среднее время в системе T_сист",
                $"T_сист = L_сист/λ_eff = {ObshieVichislia.Format(lSist)}/{ObshieVichislia.Format(lambdaEff)}", tSist));

            double kZan = a / mu;
            result.Steps.Add(new CalculationStep("Среднее число занятых каналов K_зан",
                $"K_зан = A/μ = {ObshieVichislia.Format(a)}/{ObshieVichislia.Format(mu)}", kZan));

            result.P0 = p0;
            result.P_otk = pOtk;
            result.Q = q;
            result.A = a;
            result.L_och = lOch;
            result.T_och = tOch;
            result.L_sist = lSist;
            result.T_sist = tSist;
            result.K_zan = kZan;
            return result;
        }

        private static SmoResult UnlimitedQueue(SmoInput input)
        {
            double lambda = input.LambdaBase;
            double mu = input.MuBase;
            double rho = input.Rho;
            int n = input.Channels;

            var result = new SmoResult { Type = SmoType.MultiUnlimitedQueue, Rho = rho };
            result.Steps.Add(ObshieVichislia.LoadFactor(lambda, mu, rho));

            double sum = 0;
            for (int k = 0; k <= n - 1; k++)
                sum += ObshieVichislia.Pk(rho, k);
            double lastTerm = ObshieVichislia.Pk(rho, n) / (1.0 - rho / n);
            double denom = sum + lastTerm;
            result.Steps.Add(new CalculationStep("Сумма ряда для p0",
                $"Σ = Σ(ρ^k/k!), k=0..{n - 1} + (ρ^{n}/{n}!)/(1-ρ/{n})", denom));

            double p0 = 1.0 / denom;
            result.Steps.Add(new CalculationStep("Вероятность p0",
                $"p0 = 1/Σ = 1/{ObshieVichislia.Format(denom)}", p0));

            double pn = ObshieVichislia.Pk(rho, n) * p0;
            double pWait = pn / (1.0 - rho / n);
            result.Steps.Add(new CalculationStep("Вероятность ожидания P_ожид",
                $"P_ожид = p{n}/(1-ρ/{n}) = {ObshieVichislia.Format(pn)}/(1-{ObshieVichislia.Format(rho)}/{n})", pWait));

            double pOtk = 0;
            result.Steps.Add(ObshieVichislia.RefusalProbability(pOtk, "P_отк (очередь неограничена)"));

            double q = 1.0;
            result.Steps.Add(ObshieVichislia.RelativeThroughput(pOtk, q));

            double a = lambda;
            result.Steps.Add(ObshieVichislia.AbsoluteThroughput(lambda, q, a));

            double lOch = pWait * rho / (n - rho);
            result.Steps.Add(new CalculationStep("Средняя длина очереди L_оч",
                $"L_оч = P_ожид·ρ/(n-ρ) = {ObshieVichislia.Format(pWait)}·{ObshieVichislia.Format(rho)}/({n}-{ObshieVichislia.Format(rho)})", lOch));

            double lSist = lOch + rho;
            result.Steps.Add(new CalculationStep("Среднее число заявок в системе L_сист",
                $"L_сист = L_оч + ρ = {ObshieVichislia.Format(lOch)} + {ObshieVichislia.Format(rho)}", lSist));

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
            result.K_zan = rho;
            result.P_wait = pWait;
            return result;
        }
    }
}
