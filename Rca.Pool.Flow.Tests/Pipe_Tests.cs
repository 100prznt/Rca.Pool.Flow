using Rca.Physical;
using Rca.Physical.Helpers;
using Rca.Physical.If97;
using Rca.Pool.Flow.Mathematics;

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

        [TestMethod]
        public void CalcFlowRateByPressureDrop_Test1()
        {
            Pipe testPipe = new(new(50, PhysicalUnits.Millimetre), new(10, PhysicalUnits.Metre), Roghness);

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));

            var q = testPipe.CalcFlowRateByPressureDrop(water, new PhysicalValue(75, PhysicalUnits.Millibar));

            Assert.AreEqual(13.041749046774402, q.ValueAs(PhysicalUnits.CubicMetrePerHour), 1E-4);
        }

        [TestMethod]
        public void CalcFlowRateByPressureDrop_Test2()
        {
            Pipe testPipe = new(new(40, PhysicalUnits.Millimetre), new(6, PhysicalUnits.Metre), Roghness);

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));

            var q = testPipe.CalcFlowRateByPressureDrop(water, new PhysicalValue(25, PhysicalUnits.Millibar));

            Assert.AreEqual(5.2799588509471, q.ValueAs(PhysicalUnits.CubicMetrePerHour), 1E-4);
        }

        [TestMethod]
        public void CalcFlowRateByPressureDrop_Test3()
        {
            Pipe testPipe = new(Diameter, Length, Roghness);

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));

            var q = testPipe.CalcFlowRateByPressureDrop(water, new PhysicalValue(19.58311, PhysicalUnits.Millibar));


            Assert.AreEqual(10, q.ValueAs(PhysicalUnits.CubicMetrePerHour), 1E-2);
        }


        [TestMethod]
        public void CalcFlowRateByPressureDrop_TestX()
        {
            Pipe pipe5 = new(Diameter, new PhysicalValue(5, PhysicalUnits.Metre), Roghness);
            Pipe pipe10 = new(Diameter, new PhysicalValue(10, PhysicalUnits.Metre), Roghness);
            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));

            var pInterval = Matlab.LinSpace(0, 250);

            using var sw = new StreamWriter("ouput.csv");
            sw.WriteLine($"p;Q5;Q10;Q_sum");
            foreach (var p in pInterval)
            {
                var q1 = pipe5.CalcFlowRateByPressureDrop(water, new PhysicalValue(p, PhysicalUnits.Millibar)).ValueAs(PhysicalUnits.CubicMetrePerHour);
                var q2 = pipe10.CalcFlowRateByPressureDrop(water, new PhysicalValue(p, PhysicalUnits.Millibar)).ValueAs(PhysicalUnits.CubicMetrePerHour);
                sw.WriteLine($"{p};{q1};{q2};{q1+q2}");
            }



        }
    }
}