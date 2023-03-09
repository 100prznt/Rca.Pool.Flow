using Rca.Physical;
using Rca.Physical.Helpers;
using Rca.Physical.If97;

namespace Rca.Pool.Flow.Tests
{
    [TestClass]
    public class CorrugatedPipe_Tests
    {
        readonly PhysicalValue Diameter = new(38, PhysicalUnits.Millimetre);
        readonly PhysicalValue Length = new(2.6, PhysicalUnits.Metre);
        readonly PhysicalValue Roghness = new(0.05, PhysicalUnits.Millimetre);

        [TestMethod]
        public void CalcPressureDrop_Test()
        {                 
            PhysicalValue waveDistance = new(6, PhysicalUnits.Millimetre);
            PhysicalValue waveHeight = new(3, PhysicalUnits.Millimetre);
            PhysicalValue waveSpace = new(3, PhysicalUnits.Millimetre);

            CorrugatedPipe testPipe = new(Diameter, Length, Roghness, waveHeight, waveDistance, waveSpace);

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));
            var density = water.Density; //997,0480319717384 kg/m^3
            var kinViscosity = water.KineticViscosity; //0,8927174788692257E-06 m^2/s

            var result = testPipe.CalcPressureDrop(water, new PhysicalValue(5, PhysicalUnits.CubicMetrePerHour)).ValueAs(PhysicalUnits.Millibar);

            //Reference value from http://druckverlust.de)
            Assert.AreEqual(36.23, result, 1E-2);
        }

        [TestMethod]
        public void CalcPressureDrop2_Test()
        {
            PhysicalValue waveDistance = new(8, PhysicalUnits.Millimetre);
            PhysicalValue waveHeight = new(4, PhysicalUnits.Millimetre);
            PhysicalValue waveSpace = new(4, PhysicalUnits.Millimetre);

            CorrugatedPipe testPipe = new(Diameter, Length, Roghness, waveHeight, waveDistance, waveSpace);

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));
            var density = water.Density; //997,0480319717384 kg/m^3
            var kinViscosity = water.KineticViscosity; //0,8927174788692257E-06 m^2/s

            var result = testPipe.CalcPressureDrop(water, new PhysicalValue(5, PhysicalUnits.CubicMetrePerHour)).ValueAs(PhysicalUnits.Millibar);

            //Reference value from http://druckverlust.de)
            Assert.AreEqual(43.05, result, 1E-2);
        }

        [TestMethod]
        public void CalcPressureDrop3_Test()
        {
            PhysicalValue waveDistance = new(5, PhysicalUnits.Millimetre);
            PhysicalValue waveHeight = new(2.5, PhysicalUnits.Millimetre);
            PhysicalValue waveSpace = new(2.5, PhysicalUnits.Millimetre);

            CorrugatedPipe testPipe = new(Diameter, Length, Roghness, waveHeight, waveDistance, waveSpace);

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));
            var density = water.Density; //997,0480319717384 kg/m^3
            var kinViscosity = water.KineticViscosity; //0,8927174788692257E-06 m^2/s

            var result = testPipe.CalcPressureDrop(water, new PhysicalValue(5, PhysicalUnits.CubicMetrePerHour)).ValueAs(PhysicalUnits.Millibar);

            //Reference value from http://druckverlust.de)
            Assert.AreEqual(32.47, result, 1E-2);
        }

        [Ignore]
        [TestMethod]
        public void CalcPressureDrop4_Test()
        {
            PhysicalValue waveDistance = new(4.5, PhysicalUnits.Millimetre);
            PhysicalValue waveHeight = new(2.25, PhysicalUnits.Millimetre);
            PhysicalValue waveSpace = new(2.25, PhysicalUnits.Millimetre);

            CorrugatedPipe testPipe = new(Diameter, Length, Roghness, waveHeight, waveDistance, waveSpace);

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));
            var density = water.Density; //997,0480319717384 kg/m^3
            var kinViscosity = water.KineticViscosity; //0,8927174788692257E-06 m^2/s

            var result = testPipe.CalcPressureDrop(water, new PhysicalValue(5, PhysicalUnits.CubicMetrePerHour)).ValueAs(PhysicalUnits.Millibar);

            //Reference value from http://druckverlust.de)
            Assert.AreEqual(14.29, result, 1E-2);
        }

        [Ignore]
        [TestMethod]
        public void CalcPressureDrop9_Test()
        {
            PhysicalValue waveDistance = new(4, PhysicalUnits.Millimetre);
            PhysicalValue waveHeight = new(2, PhysicalUnits.Millimetre);
            PhysicalValue waveSpace = new(2, PhysicalUnits.Millimetre);

            CorrugatedPipe testPipe = new(Diameter, Length, Roghness, waveHeight, waveDistance, waveSpace);
            Pipe testPipe2 = new(Diameter, Length, Physical.Helpers.Length.FromMillimetres(2));

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));
            var density = water.Density; //997,0480319717384 kg/m^3
            var kinViscosity = water.KineticViscosity; //0,8927174788692257E-06 m^2/s

            var result = testPipe.CalcPressureDrop(water, new PhysicalValue(5, PhysicalUnits.CubicMetrePerHour)).ValueAs(PhysicalUnits.Millibar);
            var result2 = testPipe2.CalcPressureDrop(water, new PhysicalValue(5, PhysicalUnits.CubicMetrePerHour)).ValueAs(PhysicalUnits.Millibar);

            //Reference value from http://druckverlust.de)
            Assert.AreEqual(13.26, result, 1E-2);
        }
    }
}