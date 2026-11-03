using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vichislenia
{
    static class ObshieVichislia
    {

        public static (string, double) MU(double t_obs)
        {
            double mu = 1 / t_obs;
            return ($"mu = 1 / t_обс = 1 / {t_obs} = {mu}", mu);
        }

        public static (string, double) Среднее_время_ожидания(double lm)
        {
            double t_oj = 1 / lm;
            return ($"t_oj = 1 / lm = 1 / {lm} = {t_oj}", t_oj);
        }

        public static (string, double) Интенсивность_нагрузки(double lm, double mu)
        {
            double ro = lm / mu;
            return ($"ro = lm/mu = {lm}/{mu} = {ro}", ro);
        }

        public static (string, double) абсолютную_пропускную_способностьA(double lm, double mu)
        {

            double A = lm * mu / (lm + mu);

            return ($"A = lm*mu / (lm + mu) = {lm}*{mu} / ({lm} + {mu}) = {A}", A);
        }

        public static (string, double) вероятность_отказаОдноканалP(double lm, double mu)
        {
            double P = lm / (lm + mu);
            return ($"P_отк = lm / (lm + mu) = {lm} / {lm} + {mu} = {P}", P);
        }

        public static (string, double) относительную_пропускную_способностьQ(double lm, double mu)
        {
            double Q = mu / (lm + mu);
            return ($"Q = mu / (lm + mu) = {mu} / ({lm} + {mu}) = {Q}", Q);

        }
        public static long FactorialRecursive(int n)
        {
            if (n <= 1) return 1;
            return n * FactorialRecursive(n - 1);
        }
        public static (string, double) ВероятностьСВичисл(double ro, int k)
        {
            double p = Math.Pow(ro, k) / FactorialRecursive(k);
            return ($"p{k} = ro^{k} / k! = 2^{k} / {k}!", p);
        }
        public static (string, double) Вероятность(double ro, int k)
        {
            double p = Math.Pow(ro, k) / FactorialRecursive(k);
            return ($"2^{k} / {k}!", p);
        }

    }

}
