using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rca.Physical;
using Rca.Pool.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca.Pool.Flow.Tests
{
    [TestClass()]
    public class FilterPrivate_Tests
    {
        Filter m_Filter;

        public FilterPrivate_Tests()
        {
            m_Filter = new Filter();
        }


        [TestMethod()]
        public void CalcR0deltaTest()
        {
            double psi = 0.441; //Porosität

            double r0delta = m_Filter.CalcR0deltaLengthRatio(psi);

            //Referenzwert mit HP Prime Graphic Calculator berechnet nach Gleichung (17) Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen, in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
            Assert.AreEqual(6.52571448065, r0delta, 1E-9);
        }


        [TestMethod()]
        public void CalcEulerNumberWithPressuredropFactorTest1()
        {
            double phiD = 0.8;  //Druckverlustformfactor PHI_D
            double re = 1000;   //Partikel-Reynolds-Zahl
            double r0delta = 6.52571448065; //r_0/delta

            double eu_DeltaD = m_Filter.CalcEulerNumberWithPressureDropFactor(phiD, re, r0delta);

            //Referenzwert mit HP Prime Graphic Calculator berechnet nach Gleichung (17) c
            Assert.AreEqual(6.47328489284, eu_DeltaD, 1E-6);
        }

        [TestMethod()]
        public void CalcEulerNumberWithPressuredropFactorTest2()
        {
            double phiD = 0.8;  //Druckverlustformfactor PHI_D
            double re = 100;   //Partikel-Reynolds-Zahl
            double r0delta = 6.52571448065; //r_0/delta

            double eu_DeltaD = m_Filter.CalcEulerNumberWithPressureDropFactor(phiD, re, r0delta);

            Assert.AreEqual(15.465763270491223, eu_DeltaD, 1E-15);
        }

        [TestMethod()]
        public void CalcEulerNumberWithPressuredropFactorTest3()
        {
            double phiD = 0.8;  //Druckverlustformfactor PHI_D
            double re = 10000;   //Partikel-Reynolds-Zahl
            double r0delta = 6.52571448065; //r_0/delta

            double eu_DeltaD = m_Filter.CalcEulerNumberWithPressureDropFactor(phiD, re, r0delta);

            Assert.AreEqual(5.093198963405808, eu_DeltaD, 1E-15);
        }


        //Aus Beispielrechnung

        [TestMethod()]
        public void BspCalcReynoldsNumberTest()
        {
            double v = 1;   //Leerrohrfluidgeschwindigkeit[m/s]
            double d_p = 0.003; //Sauter-Durchmesser [m]
            double psi = 0.4; //Porosität
            double ny = 14.8e-7;  //Kinematische Viskosität [m^2/s]

            double eu_DeltaD = m_Filter.CalcReynoldsNumber(v, d_p, psi, ny);


            //Referenzwert aus Beispiel
            Assert.AreEqual(5070, eu_DeltaD, 3);
        }

        [TestMethod()]
        public void BspCalcR0deltaTest()
        {
            double psi = 0.4; //Porosität

            double r0delta = m_Filter.CalcR0deltaLengthRatio(psi);

            //Referenzwert aus Beispiel
            Assert.AreEqual(r0delta, 7.91, 1E-2);
        }

        [TestMethod()]
        public void BspCalcEulerNumberTest()
        {
            double re = 5070;      //Partikel-Reynolds-Zahl
            double r0delta = 7.91; //r_0/delta

            double eu_DeltaD = m_Filter.CalcEulerNumber(re, r0delta);

            //Referenzwert aus Beispiel
            Assert.AreEqual(3.74, eu_DeltaD, 1E-2);
        }

        [TestMethod()]
        public void BspCalcBedLengthTest()
        {
            double deltaP = 100000; // [Pa]
            double v = 1;   //Leerrohrfluidgeschwindigkeit[m/s]
            double rho_f = 16.38; // [kg/m^3]
            double d_p = 0.003; // [m]
            double psi = 0.4;
            double eu = 3.74;

            double deltaL = m_Filter.CalcBedLength(deltaP, v, rho_f, d_p, psi, eu);

            //Referenzwert aus Beispiel
            Assert.AreEqual(1.74, deltaL, 1E-2);
        }

        [TestMethod()]
        public void BspCalcEulerNumberWithPressuredropFactorTest()
        {
            double phiD = 0.74;  //Druckverlustformfactor PHI_D
            double re = 5070;      //Partikel-Reynolds-Zahl
            double r0delta = 7.91; //r_0/delta

            double eu_DeltaD = m_Filter.CalcEulerNumberWithPressureDropFactor(phiD, re, r0delta);

            //Referenzwert aus Beispiel
            Assert.AreEqual(6.93, eu_DeltaD, 1E-2);
        }

        [TestMethod()]
        public void BspCalcBedLengthTest2()
        {
            double deltaP = 100000; // [Pa]
            double v = 1;   //Leerrohrfluidgeschwindigkeit[m/s]
            double rho_f = 16.38; // [kg/m^3]
            double d_p = 0.003; // [m]
            double psi = 0.4;
            double eu = 6.93;

            double deltaL = m_Filter.CalcBedLength(deltaP, v, rho_f, d_p, psi, eu);

            //Referenzwert aus Beispiel
            Assert.AreEqual(0.94, deltaL, 1E-2);
        }





        [TestMethod()]
        public void CalcPressureDropTest()
        {
            double deltaL = 1.7411782117664476; // [m]
            double v = 1;   //Leerrohrfluidgeschwindigkeit[m/s]
            double rho_f = 16.38; // [kg/m^3]
            double d_p = 0.003; // [m]
            double psi = 0.4;
            double eu = 3.74;

            //Refernzwert aus Beispiel
            double deltaP = m_Filter.CalcPressureDrop(deltaL, v, rho_f, d_p, psi, eu);

            Assert.AreEqual(100000, deltaP, 1E-2);
        }
    }
}