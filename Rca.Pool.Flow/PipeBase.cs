using Rca.Physical;
using Rca.Physical.Dimensions;

namespace Rca.Pool.Flow
{
    public class PipeBase
    {
        #region Properties
        /// <summary>
        /// Innerer Rohrdurchmesser
        /// </summary>
        public PhysicalValue Diameter { get; set; }

        /// <summary>
        /// Rohrlänge
        /// </summary>
        public PhysicalValue Length { get; set; }

        /// <summary>
        /// Rohrquerschnitt
        /// </summary>
        public PhysicalValue CrossArea
        {
            get
            {
                return new PhysicalValue(Math.PI * Math.Pow(Diameter.GetBaseValue(), 2) / 4, PhysicalDimensions.Area.GetBaseUnit());
            }
        }

        /// <summary>
        /// EINGABE PARAMETER
        /// Volumenstrom
        /// </summary>
        public PhysicalValue FlowRate { get; set; }

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PipeBase()
        {

        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="di">Innerer Rohrdurchmesser</param>
        public PipeBase(PhysicalValue di)
        {
            Diameter = di;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="l">Rohrlänge</param>
        /// <param name="di">Innerer Rohrdurchmesser</param>
        public PipeBase(PhysicalValue di, PhysicalValue l)
        {
            Length = l;
            Diameter = di;
        }

        #endregion Constructor

        #region Services
        /// <summary>
        /// Strömungsgeschwindigkeit berechnen
        /// </summary>
        /// <returns>Strömungsgeschwindigkeit in [m/s]</returns>
        public PhysicalValue CalcFlowVelocity() => CalcFlowVelocity(FlowRate);

        /// <summary>
        /// Strömungsgeschwindigkeit berechnen
        /// </summary>
        /// <param name="flowRate">Volumenstrom in [m^3/h]</param>
        /// <returns>Strömungsgeschwindigkeit in [m/s]</returns>
        public PhysicalValue CalcFlowVelocity(PhysicalValue flowRate)
        {
            //double q = flowRate / 3600; // [m^3/s]
            //double a = CrossArea / 1E6; // [m^2]

            return flowRate / CrossArea;
        }

        /// <summary>
        /// Volumenstrom berechnen
        /// </summary>
        /// <param name="flowVelocity">Strömungsgeschwindigkeit in [m/s]</param>
        /// <returns>Volumenstrom in [m^3/h]</returns>
        public PhysicalValue CalcFlowRate(PhysicalValue flowVelocity)
        {
            //double a = CrossArea / 1E6; // [m^2]
            //double q = ; // [m^3/s]

            return flowVelocity * CrossArea;
        }

        #endregion Services
    }

}
