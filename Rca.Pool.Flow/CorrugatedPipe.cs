using Rca.Physical.If97;
using Rca.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rca.Physical.Helpers;

namespace Rca.Pool.Flow
{
    /// <summary>
    /// Represents a piece of corrugated piping
    /// </summary>
    public class CorrugatedPipe : Pipe
    {
        /// <summary>
        /// Width of the space between two corrugations on the inside of the tube
        /// </summary>
        public PhysicalValue InnerWaveSpace { get; set; }

        /// <summary>
        /// Distance between two waves (= space + wave)
        /// </summary>
        public PhysicalValue WaveDistance { get; set; }

        /// <summary>
        /// Wave height
        /// </summary>
        public PhysicalValue WaveHeight { get; set; }


        public CorrugatedPipe()
        {
            InnerWaveSpace = PhysicalValue.NaN;
            WaveDistance = PhysicalValue.NaN;
            WaveHeight = PhysicalValue.NaN;
        }

        /// <summary>
        /// Constructor for new instance of <see cref="CorrugatedPipe"/>
        /// </summary>
        /// <param name="l">Pipe length</param>
        /// <param name="dimensions">Pipe definition</param>
        public CorrugatedPipe(PhysicalValue l, PipeDimension dimensions) : this(dimensions.InnerDiameter, l, dimensions.Roughness, dimensions.WaveHeight, dimensions.WaveDistance, dimensions.InnerWaveSpace)
        {

        }

        /// <summary>
        /// Constructor for new instance of <see cref="CorrugatedPipe"/>
        /// </summary>
        /// <param name="l">Pipe length</param>
        /// <param name="di">Inner pipe diameter</param>
        /// <param name="k">Roughness of the pipe</param>
        /// <param name="h_w">Wave height</param>
        /// <param name="l_w">Distance between two waves (= space + wave)</param>
        /// <param name="s_w">Width of the space between two corrugations on the inside of the tube</param>
        public CorrugatedPipe(PhysicalValue di, PhysicalValue l, PhysicalValue k, PhysicalValue h_w, PhysicalValue l_w, PhysicalValue? s_w = null) : base(di, l, k)
        {
            WaveDistance = l_w;
            WaveHeight = h_w;

            if (s_w is not null)
                InnerWaveSpace = s_w;
            else
                InnerWaveSpace = WaveDistance / 2;
        }


        /// <summary>
        /// Calculate pressure drop
        /// (turbulent flow is assumed)
        /// </summary>
        /// <param name="medium">Medium properties</param>
        /// <returns>Pressure drop</returns>
        public new PhysicalValue CalcPressureDrop(Water medium) => CalcPressureDrop(medium, FlowRate);

        /// <summary>
        /// Calculate pressure drop
        /// (turbulent flow is assumed)
        /// </summary>
        /// <param name="medium">Medium properties</param>
        /// <param name="flowRate">Flowrate (volumetric)</param>
        /// <returns>Pressure drop</returns>
        public new PhysicalValue CalcPressureDrop(Water medium, PhysicalValue flowRate)
        {
            var d = Diameter.ValueAs(PhysicalUnits.Millimetre); // [mm]
            var k = Roughness.ValueAs(PhysicalUnits.Millimetre); // [mm]
            var h = WaveHeight.ValueAs(PhysicalUnits.Millimetre); //Wellenhöhe [mm]
            var b = InnerWaveSpace.ValueAs(PhysicalUnits.Millimetre); //Wellenbreite [mm]
            var l = WaveDistance.ValueAs(PhysicalUnits.Millimetre); //Wellenabstand [mm]


            var rho = medium.Density.ValueAs(PhysicalUnits.KilogramPerCubicMetre); //kg/m^3
            var v = CalcFlowVelocity(flowRate).ValueAs(PhysicalUnits.MetrePerSecond); // [m/s]
            var kv = medium.KineticViscosity.ValueAs(PhysicalUnits.SquareMetrePerSecond); // [m^2/s]
            var re = v * Diameter.GetBaseValue() / kv;


            //Näherungsformel für Wellrohr
            //https://www.schweizer-fn.de/stroemung/druckverlust/druckverlust.php#lambda_wellrohr

            if (re < 50000)
                throw new ArgumentOutOfRangeException($"R_e = {re} >= 50000");

            var hl = h / l;
            if (hl < 0 || hl > 1.2)
                throw new ArgumentOutOfRangeException("Ratio WaveHeight/WaveDistance must be in the range 0.2 to 1.2");

            var lambda = 0.2 * Math.Pow(Math.Pow(h/d,6) * Math.Pow(l/h,7), 0.1);

            var lambda2 = -0.25 / Math.Log(k * Math.Sqrt(d * l / (h * b)));
            var lambda3 = -0.25 / Math.Log10(k * Math.Sqrt(d * l / (h * b)));

            // Formel für hydraulisch rauhes Rohr, nach Nikuradse
            k = h;            
            var lambda4 = Math.Pow(1 / (-2 * Math.Log10(k / (3.71 * d))),2);


            // Druckverlust durch Rohrreibung
            // https://www.schweizer-fn.de/stroemung/druckverlust/druckverlust.php#druckverlustrohr
            double deltaP = lambda * Length.GetBaseValue() * rho * Math.Pow(v, 2) / (Diameter.GetBaseValue() * 2); // [Pa]

            return new(deltaP, PhysicalUnits.Pascal);
        }
    }
}
