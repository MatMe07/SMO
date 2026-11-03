using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vichislenia
{
    static class OdnokanalVich
    {

        public static (string, double) Абсолютную_пропускную_способность(double lm, double mu)
        {
            return ObshieVichislia.абсолютную_пропускную_способностьA(lm, mu);
        }

        public static (string, double) Относительную_пропускную_способность(double lm, double mu)
        {
            return ObshieVichislia.относительную_пропускную_способностьQ(lm, mu);
        }

        public static (string, double) Вероятность_отказа(double lm, double mu)
        {
            return ObshieVichislia.вероятность_отказаОдноканалP(lm, mu);
        }

    }
}
