using FlowCalc.Mathematics;
using Rca.Physical;
using Rca.Physical.Dimensions;
using Rca.Physical.If97;
using Rca.Pool.Flow;
using Rca.Pool.Flow.Mathematics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca.Pool.Flow
{
    /// <summary>
    /// Represents a piece of piping
    /// </summary>
    [DebuggerDisplay("{DefaultFormattedValue, nq}")]
    public class Pipe : PipeBase
    {
        #region Member


        #endregion Member

        #region Fields
        protected new string DefaultFormattedValue => $"{base.DefaultFormattedValue}; l = {Length?.ToString(true, "N2")}; k = {Roughness?.ToString(true, "N2")}";

        #endregion Fields

        #region Properties
        /// <summary>
        /// Pipe length
        /// </summary>
        public PhysicalValue Length { get; set; }

        /// <summary>
        /// Roughness of the inner pipe side
        /// </summary>
        public PhysicalValue Roughness { get; set; }

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Konstruktor
        /// </summary>
        public Pipe()
        {
            Length = PhysicalValue.NaN;
            Roughness = PhysicalValue.NaN;
        }

        /// <summary>
        /// Constructor for new instance of <see cref="Pipe"/>
        /// </summary>
        /// <param name="l">Pipe length</param>
        /// <param name="dimensions">Pipe definition</param>
        public Pipe(PhysicalValue l, PipeDimension dimensions) : this(l, dimensions.InnerDiameter, dimensions.Roughness)
        {

        }

        /// <summary>
        /// Constructor for new instance of <see cref="Pipe"/>
        /// </summary>
        /// <param name="l">Pipe length</param>
        /// <param name="di">Inner pipe diameter</param>
        /// <param name="k">Roughness of the pipe</param>
        public Pipe(PhysicalValue di, PhysicalValue l, PhysicalValue k) : base(di)
        {
            Length = l;
            Roughness = k;
        }

        #endregion Constructor

        #region Services
        /// <summary>
        /// Calculate pressure drop
        /// (turbulent flow is assumed)
        /// </summary>
        /// <param name="medium">Medium properties</param>
        /// <returns>Pressure drop</returns>
        public PhysicalValue CalcPressureDrop(Water medium) => CalcPressureDrop(medium, FlowRate);

        /// <summary>
        /// Calculate pressure drop
        /// (turbulent flow is assumed)
        /// </summary>
        /// <param name="medium">Medium properties</param>
        /// <param name="flowRate">Flowrate (volumetric)</param>
        /// <returns>Pressure drop</returns>
        public PhysicalValue CalcPressureDrop(Water medium, PhysicalValue flowRate)
        {
            var di = Diameter.ValueAs(PhysicalUnits.Metre); // [m]
            var k = Roughness.ValueAs(PhysicalUnits.Metre); // [m]
            var l = Length.ValueAs(PhysicalUnits.Metre); // [m]
            var rho = medium.Density.ValueAs(PhysicalUnits.KilogramPerCubicMetre); //kg/m^3
            var v = CalcFlowVelocity(flowRate).ValueAs(PhysicalUnits.MetrePerSecond); // [m/s]
            var kv = medium.KineticViscosity.ValueAs(PhysicalUnits.SquareMetrePerSecond); // [m^2/s]
            var re = v * di / kv; //Reynolds-Zahl https://de.wikipedia.org/wiki/Reynolds-Zahl

            // Rohrreibungszahl (Lambda) nach Colebrook und White siehe: https://de.wikipedia.org/wiki/Rohrreibungszahl
            // Iterative Berechnung
            double lambda = 0.005; // Startwert für Lambda
            double s = 0.001; // Schrittweite für Antastung (0.001)
            int i = 7; // Anzahl der Richtungswechsel (7)

            double error = double.MaxValue - 1;
            while (i > 0)
            {
                double lastError = error;
                error = 1 / Math.Sqrt(lambda) - (-2 * Math.Log10((2.51 / (re * Math.Sqrt(lambda))) + (k / (3.71 * di))));

                if (Math.Abs(error) >= Math.Abs(lastError))
                {
                    s /= -10; //-10
                    i--;
                }
                lambda += s;
            }


            // Rohrreibungszahl (Lambda) nach Prandtl siehe: https://www.cosmos-indirekt.de//Physik-Schule/Rohrreibungszahl
            // Iterative Berechnung
            //lambda = 0.025; // Startwert für Lambda
            //s = 0.0001; // Schrittweite für Antastung
            //i = 100; // Anzahl der Richtungswechsel

            //error = double.MaxValue - 1;
            //while (i > 0)
            //{
            //    double lastError = error;
            //    error = 1 / Math.Sqrt(lambda) - (-2 * Math.Log10(2.51 / (re * Math.Sqrt(lambda))));

            //    if (Math.Abs(error) >= Math.Abs(lastError))
            //    {
            //        s /= -10;
            //        i--;
            //    }
            //    lambda += s;
            //}


            //Alternative Formel prüfen:
            //Blasius
            //https://www.cosmos-indirekt.de//Physik-Schule/Rohrreibungszahl

            //Näherungsformel für Wellrohr
            //https://www.schweizer-fn.de/stroemung/druckverlust/druckverlust.php#lambda_wellrohr

            // Druckverlust durch Rohrreibung
            // https://www.schweizer-fn.de/stroemung/druckverlust/druckverlust.php#druckverlustrohr
            double deltaP = lambda * l * rho * Math.Pow(v, 2) / (di * 2); // [Pa]

            return new(deltaP, PhysicalUnits.Pascal);
        }

        public PhysicalValue CalcDiameterByPressureDrop(Water medium, PhysicalValue pressureDrop)
        {

            //Generate polynom di(deltaP)
            var diInterval = Matlab.LinSpace(10, 60); //[mm]
            var specDeltaPList = new List<double>();
            foreach (var di in diInterval)
            {
                var specPipe = new Pipe(new PhysicalValue(di, PhysicalUnits.Millimetre), Length, Roughness);
                specDeltaPList.Add(specPipe.CalcPressureDrop(medium, FlowRate).ValueAs(PhysicalUnits.Millibar));
            }

            var p_deltaP_di = Polynom.Polyfit(specDeltaPList.ToArray(), diInterval, 2);

            var diFromPolynominal = p_deltaP_di.Polyval(pressureDrop.ValueAs(PhysicalUnits.Millibar)); //[mm]


            var deltaP = pressureDrop.ValueAs(PhysicalUnits.Pascal); // [Pa]
            var k = Roughness.ValueAs(PhysicalUnits.Metre); // [m]
            var l = Length.ValueAs(PhysicalUnits.Metre); // [m]
            var rho = medium.Density.ValueAs(PhysicalUnits.KilogramPerCubicMetre); //kg/m^3
            var kv = medium.KineticViscosity.ValueAs(PhysicalUnits.SquareMetrePerSecond); // [m^2/s]





            var deltaP_Specific = deltaP / (rho * l);

            return PhysicalValue.NaN;
        }

        public override string ToString()
        {
            return $"{Length.ToString(true, "N2")} (d = {Diameter.ToString(true, "N2")}, k = {Roughness.ToString(true, "N2")})";
        }

        #endregion Services

        #region Internal services


        #endregion Internal services

        #region Events


        #endregion Events
    }
}
