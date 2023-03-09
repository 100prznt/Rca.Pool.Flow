using Rca.Physical;
using Rca.Physical.Helpers;

namespace Rca.Pool.Flow
{
    public class FilterMedium
    {
        /// <summary>
        /// Quartz sand with a grain size of 0.4 to 0.7 mm
        /// </summary>
        public static FilterMedium QuartzSand_04_07 => new(Length.FromMillimetres(0.4), 0.395, 0.74);

        /// <summary>
        /// Sauter diameter [m]
        /// </summary>
        public PhysicalValue SauterDiameter { get; set; }

        /// <summary>
        /// Porosity
        /// </summary>
        public double Porosity { get; set; }

        /// <summary>
        /// Pressure drop form factor PHI_D
        /// </summary>
        public double PressureDropFactor { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public FilterMedium()
        {
            SauterDiameter = PhysicalValue.NaN;
            Porosity = double.NaN;
            PressureDropFactor = double.NaN;
        }

        /// <summary>
        /// New <see cref="FilterMedium"/> from its parameters
        /// </summary>
        /// <param name="d_p">Sauter diameter</param>
        /// <param name="psi">Porosity</param>
        /// <param name="phi_D">Pressure drop form factor PHI_D</param>
        public FilterMedium(PhysicalValue d_p, double psi, double phi_D)
        {
            SauterDiameter = d_p;
            Porosity = psi;
            PressureDropFactor = phi_D;
        }
    }
}
