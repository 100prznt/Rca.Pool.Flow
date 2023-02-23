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
        /// Rohrquerschnitt
        /// </summary>
        public PhysicalValue CrossArea => new(Math.PI * Math.Pow(Diameter.GetBaseValue(), 2) / 4, PhysicalDimensions.Area.GetBaseUnit());

        /// <summary>
        /// EINGABE PARAMETER
        /// Volumenstrom
        /// </summary>
        public PhysicalValue FlowRate { private get; set; }

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

        #endregion Constructor

        #region Services
        /// <summary>
        /// Strömungsgeschwindigkeit berechnen
        /// </summary>
        /// <returns>Strömungsgeschwindigkeit</returns>
        public PhysicalValue CalcFlowVelocity() => CalcFlowVelocity(FlowRate);

        /// <summary>
        /// Strömungsgeschwindigkeit berechnen
        /// </summary>
        /// <param name="flowRate">Volumenstrom</param>
        /// <returns>Strömungsgeschwindigkeit</returns>
        public PhysicalValue CalcFlowVelocity(PhysicalValue flowRate) => flowRate / CrossArea;

        /// <summary>
        /// Volumenstrom berechnen
        /// </summary>
        /// <param name="flowVelocity">Strömungsgeschwindigkeit</param>
        /// <returns>Volumenstrom</returns>
        public PhysicalValue CalcFlowRate(PhysicalValue flowVelocity) => flowVelocity * CrossArea;


        #endregion Services
    }

}
