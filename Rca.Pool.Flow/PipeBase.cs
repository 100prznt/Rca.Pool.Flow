using Rca.Physical;
using Rca.Physical.Dimensions;
using Rca.Physical.Helpers;
using System.Diagnostics;

namespace Rca.Pool.Flow
{
    /// <summary>
    /// Base class for pipes and other components with circular cross-section for which flow is possible.
    /// </summary>
    [DebuggerDisplay("{DefaultFormattedValue, nq}")]
    public abstract class PipeBase
    {

        #region Fields
        protected string DefaultFormattedValue => $"d = {Diameter?.ToString(true, "N2")}; a = {CrossArea?.ToString(true, "N2")}; Q = {FlowRate?.ToString(true, "N2")}";

        #endregion Fields

        #region Properties
        /// <summary>
        /// Inner pipe diameter
        /// </summary>
        public PhysicalValue Diameter { get; set; }
        /// <summary>
        /// Pipe cross area
        /// </summary>
        public PhysicalValue CrossArea => new(Math.PI * Math.Pow(Diameter.GetBaseValue(), 2) / 4, PhysicalDimensions.Area.GetBaseUnit());

        /// <summary>
        /// Volumetric flow rate
        /// </summary>
        public PhysicalValue FlowRate { private protected get; set; }

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public PipeBase()
        {
            Diameter = PhysicalValue.NaN;
            FlowRate = PhysicalValue.NaN;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="di">Inner pipe diameter</param>
        public PipeBase(PhysicalValue di) : this()
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
