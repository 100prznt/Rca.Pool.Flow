using Rca.Physical;
using Rca.Physical.Helpers;
using Rca.Physical.If97;

namespace Rca.Pool.Flow.Tests
{
    [TestClass]
    public class Pipe_Tests
    {
        readonly PhysicalValue Diameter = new(45.2, PhysicalUnits.Millimetre);
        readonly PhysicalValue Length = new(2.6, PhysicalUnits.Metre);
        readonly PhysicalValue Roghness = new(0.05, PhysicalUnits.Millimetre);

        [TestMethod]
        public void CalcPressureDrop_Test()
        {
            Pipe testPipe = new(Diameter, Length, Roghness);

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));
            var density = water.Density; //997,0480319717384 kg/m^3
            var kinViscosity = water.KineticViscosity; //0,8927174788692257E-06 m^2/s

            var result = testPipe.CalcPressureDrop(water, new PhysicalValue(10, PhysicalUnits.CubicMetrePerHour)).ValueAs(PhysicalUnits.Millibar);

            //Reference value from http://druckverlust.de)
            Assert.AreEqual(19.58311, result, 1E-2);
        }
    }
}