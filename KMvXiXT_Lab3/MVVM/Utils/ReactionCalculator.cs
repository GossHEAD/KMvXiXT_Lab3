using KMvXiXT_Lab3.MVVM.Model;
using MathNet.Numerics.LinearAlgebra;

namespace KMvXiXT_Lab3.MVVM.Utils
{
     public static class ReactionCalculator
    {
        private const double R = 8.314; // Дж/(моль·К)
        /// <summary>
        /// Расчет концентраций компонентов используя метод Рунге-Кутта
        /// - Объем в литрах (л)
        /// - Расход в л/мин
        /// - Время интегрирования в секундах
        /// - Температура в °C, переводим в К
        /// - Концентрации в моль/л
        /// - Предэкспоненциальный множитель в 1/мин или л/(моль*мин) 
        /// </summary>
        public static List<double[]> CalculateConcentrations(
            List<Component_kmx> components,
            List<Reaction> reactions,
            ReactorParameters parameters)
        {
            var compNames = components.Select(c => c.Name).ToList();
            int nComp = components.Count;
            int nReac = reactions.Count;
            
            double T = parameters.Temperature + 273.15;
            
            
            double totalTimeMinutes = parameters.TotalTimeMinutes;
            double stepMinutes = parameters.StepMinutes;
            double dt = stepMinutes; // шаг моделирования

            int nPoints = (int)(totalTimeMinutes / stepMinutes) + 1;
            
            double[,] nuArr = new double[nComp, nReac];
            var reacReactants = new List<(int compIndex, double power)>[nReac];
            
            for (int j = 0; j < nReac; j++)
            {
                ParseReactionFormula(reactions[j].Formula, compNames, out var reactants, out var products);

                foreach (var r in reactants)
                    nuArr[r.compIndex, j] = -r.coeff;
                foreach (var p in products)
                    nuArr[p.compIndex, j] = p.coeff;

                reacReactants[j] = reactants.Select(x => (x.compIndex, x.coeff)).ToList();
            }

            var nu = Matrix<double>.Build.DenseOfArray(nuArr);

            Vector<double> C = Vector<double>.Build.Dense(components.Select(c => c.InitialConcentration).ToArray());

            var results = new List<double[]>();
            results.Add(C.ToArray());
            
            double[] kArr = new double[nReac];
            for (int j = 0; j < nReac; j++)
            {
                double A_factor = reactions[j].PreExponentialFactor;
                double Ea = reactions[j].ActivationEnergy;
                
                double A_sec = A_factor;

                kArr[j] = A_sec * Math.Exp(-Ea / (R * T));
            }

            Vector<double> RHS(Vector<double> cState)
            {
                double[] reacRates = new double[nReac];
                for (int j = 0; j < nReac; j++)
                {
                    double rate = kArr[j];
                    foreach (var (ci, pow) in reacReactants[j])
                    {
                        rate *= Math.Pow(cState[ci], pow);
                    }
                    reacRates[j] = rate;
                }

                Vector<double> rVec = Vector<double>.Build.DenseOfArray(reacRates);
                Vector<double> dCdt = nu * rVec;
                return dCdt;
            }

            // Метод Рунге-Кутта
            for (int step = 1; step < nPoints; step++)
            {
                var k1 = RHS(C);
                var k2 = RHS(C + k1 * (dt / 2.0));
                var k3 = RHS(C + k2 * (dt / 2.0));
                var k4 = RHS(C + k3 * dt);

                C = C + (dt / 6.0) * (k1 + 2 * k2 + 2 * k3 + k4);

                for (int i = 0; i < nComp; i++)
                {
                    if (C[i] < 0) C[i] = 0;
                }

                results.Add(C.ToArray());
            }

            return results;
        }

        private static void ParseReactionFormula(string formula, List<string> compNames, out List<(int compIndex, double coeff)> reactants, out List<(int compIndex, double coeff)> products)
        {
            reactants = new List<(int compIndex, double coeff)>();
            products = new List<(int compIndex, double coeff)>();

            string[] sides = formula.Split(new[] { '>', '→' }, StringSplitOptions.RemoveEmptyEntries);
            if (sides.Length != 2)
                throw new Exception("Некорректная формула: " + formula);

            string left = sides[0].Trim();
            string right = sides[1].Trim();

            foreach (var part in left.Split('+'))
            {
                ParseComponent(part.Trim(), compNames, reactants);
            }

            foreach (var part in right.Split('+'))
            {
                ParseComponent(part.Trim(), compNames, products);
            }
        }

        private static void ParseComponent(string s, List<string> compNames, List<(int compIndex, double coeff)> list)
        {
            int idx = 0;
            double coeff = 0;
            while (idx < s.Length && Char.IsDigit(s[idx]))
            {
                coeff = coeff * 10 + (s[idx] - '0');
                idx++;
            }
            if (coeff == 0) coeff = 1;

            string comp = s.Substring(idx).Trim();
            int cIndex = compNames.IndexOf(comp);
            if (cIndex < 0)
                throw new Exception("Неизвестный компонент: " + comp);

            list.Add((cIndex, coeff));
        }
    }
}
