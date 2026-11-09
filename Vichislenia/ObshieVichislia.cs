using System.Globalization;

namespace Vichislenia
{
    public static class ObshieVichislia
    {
        public static string Format(double value) =>
            value.ToString("0.######", CultureInfo.InvariantCulture);

        public static double Factorial(int n)
        {
            double result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }

        public static double Pk(double rho, int k) =>
            Math.Pow(rho, k) / Factorial(k);

        public static double SumFactorialPowers(double rho, int n)
        {
            double sum = 0;
            for (int k = 0; k <= n; k++)
                sum += Pk(rho, k);
            return sum;
        }

        public static CalculationStep LoadFactor(double lambda, double mu, double rho) =>
            new("Интенсивность нагрузки ρ",
                $"ρ = λ/μ = {Format(lambda)}/{Format(mu)}", rho);

        public static CalculationStep RelativeThroughput(double pOtk, double q) =>
            new("Относительная пропускная способность Q",
                $"Q = 1 - P_отк = 1 - {Format(pOtk)}", q);

        public static CalculationStep AbsoluteThroughput(double lambda, double q, double a) =>
            new("Абсолютная пропускная способность A",
                $"A = λ·Q = {Format(lambda)}·{Format(q)}", a);

        public static CalculationStep EffectiveLambda(double lambda, double q, double lambdaEff) =>
            new("Эффективная интенсивность λ_eff",
                $"λ_eff = λ·(1 - P_отк) = λ·Q = {Format(lambda)}·{Format(q)}", lambdaEff);

        public static CalculationStep RefusalProbability(double pOtk, string formula) =>
            new("Вероятность отказа P_отк", formula, pOtk);
    }
}
