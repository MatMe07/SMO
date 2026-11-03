using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vichislenia
{
    static class MnogokanalVich
    {
        public static (string, List<string>, double) Вероятность_что_канал_свободен(double ro, int k)
        {

            double p0 = 1;
            double znam = 0;
            string p0_str = "1 / (1 + ";
            List<string> p_lst = new List<string>();
            p_lst.Add("1 + ");
            for (int i = 1; i <= k; i++)
            {
                var (str_p, p) = ObshieVichislia.Вероятность(ro, i);
                p_lst.Add(str_p);
                p0_str += str_p + (i == k ? ")" : " + ");
                znam += p;
            }
            p0 /= znam;
            return (p0_str, p_lst, p0);
        }
        public static (string, double) Вероятность_что_канал_свободен(double lm, double mu)
        {
            string p0_str = "p0 = ";
            var (p0_st, p0) = ObshieVichislia.относительную_пропускную_способностьQ(lm, mu);
            p0_str += p0_st;
            return (p0_str, p0);
        }


    }

}
