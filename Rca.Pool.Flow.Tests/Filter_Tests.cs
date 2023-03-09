using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rca.Physical;
using Rca.Physical.Helpers;
using Rca.Physical.If97;
using Rca.Pool.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca.Pool.Flow.Tests
{
    [TestClass()]
    public class Filter_Tests
    {
        [TestMethod()]
        public void CalcPressureDropTest()
        {
            var filter = new Filter();
            filter.Diameter = Length.FromMillimetres(500);
            filter.Height = Length.FromMillimetres(263);
            filter.FilterMedium = FilterMedium.QuartzSand_04_07;

            var water = new Water();
            water.UpdatePT(Pressure.FromStandardAtmospheres(1), ThermodynamicTemperature.FromCelsius(25));

            var result = filter.CalcPressureDrop(water, VolumetricFlowRate.FromCubicMetrePerHours(5.8));

            Assert.AreEqual(200, result.ValueAs(PhysicalUnits.Millibar), 10);
        }
    }
}