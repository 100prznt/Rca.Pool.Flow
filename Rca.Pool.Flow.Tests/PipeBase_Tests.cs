using Rca.Physical;
using System.Net.NetworkInformation;

namespace Rca.Pool.Flow.Tests
{
    [TestClass]
    public class PipeBase_Tests
    {
        readonly PhysicalValue Diameter = new(45.2, PhysicalUnits.Millimetre);

        internal class PipeBaseTesting : PipeBase
        {
            public PipeBaseTesting() : base()
            {
            
            }

            public PipeBaseTesting(PhysicalValue Diameter) : base(Diameter)
            {

            }
        }

        [TestMethod]
        public void CalcFlowVelocity_Test()
        {
            PipeBaseTesting testPipe = new (Diameter);

            // Vergleichsergebnis aus Druckverlust 7.0 (http://druckverlust.de)
            Assert.AreEqual(0.866, testPipe.CalcFlowVelocity(new PhysicalValue(5, PhysicalUnits.CubicMetrePerHour)).ValueAs(PhysicalUnits.MetrePerSecond), 1E-3);
        }

        [TestMethod]
        public void CalcFlowRate_Test()
        {
            PipeBaseTesting testPipe = new(Diameter);

            // Vergleichsergebnis aus Druckverlust 7.0 (http://druckverlust.de)
            Assert.AreEqual(10, testPipe.CalcFlowRate(new PhysicalValue(1.731, PhysicalUnits.MetrePerSecond)).ValueAs(PhysicalUnits.CubicMetrePerHour), 1E-3);
        }
    }
}