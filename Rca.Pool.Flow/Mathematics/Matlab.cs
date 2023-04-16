using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca.Pool.Flow.Mathematics
{
    internal class Matlab
    {
        /// <summary>
        /// Generate linearly spaced array
        /// </summary>
        /// <param name="startValue">Interval start value</param>
        /// <param name="endValue">Interval end value</param>
        /// <param name="count">Number of values</param>
        /// <returns></returns>
        public static double[] LinSpace(double startValue, double endValue, double count = 100)
        {
            double delta = endValue - startValue;
            double step = delta / count;

            var result = new List<double>();

            result.Add(startValue);

            for (int i = 2; i < (count - 1); i++)
                result.Add(step * i);

            result.Add(endValue);

            return result.ToArray();
        }
    }
}
