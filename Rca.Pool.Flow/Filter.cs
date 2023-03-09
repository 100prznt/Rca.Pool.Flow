using Rca.Physical.If97;
using Rca.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.Arm;
using Rca.Physical.Helpers;

namespace Rca.Pool.Flow
{
    public class Filter : PipeBase
    {
        /// <summary>
        /// Contains material parameters of the filter medium to be used
        /// </summary>
        public FilterMedium FilterMedium { get; set; }

        /// <summary>
        /// Filter bed height
        /// </summary>
        public PhysicalValue Height { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Filter()
        {
            FilterMedium = new FilterMedium();
            Height = PhysicalValue.NaN;
        }

        /// <summary>
        /// Calculate the pressure drop inside the filter
        /// </summary>
        /// <param name="medium">Medium properties</param>
        /// <param name="flowRate">Flowrate (volumetric)</param>
        /// <returns>Pressure drop</returns>
        public PhysicalValue CalcPressureDrop(Water medium, PhysicalValue flowRate)
        {
            //Es gibt zwei differierende Modelle zur Beschreibung des Strömungsdruckverlustes in Festbetten:
            // - Modell des hydraulischen Durchmessers (Ergun-Gleichung)
            // - Modell der Einzelpartikelumströmung (Molerus)
            //Quelle: Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen, in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79


            //Modell der Einzelpartikelumströmung (Molerus)
            var deltaL = Height.GetBaseValue(); //Länge des Festbettes [m]
            var v = CalcFlowVelocity(flowRate).GetBaseValue(); //Leerrohrfließgeschwindigkeit [m/s]
            var rho_f = medium.Density.GetBaseValue();
            var ny = medium.KineticViscosity.GetBaseValue(); //Kinematische Viskosität [m^2/s]


            //Parameter des Filtermediums
            var d_p = FilterMedium.SauterDiameter.GetBaseValue();  //Sauterdurchmesser [m]
            var psi = FilterMedium.Porosity;                       //Porosität
            var phiD = FilterMedium.PressureDropFactor;            //Druckverlustformfaktor



            double re = CalcReynoldsNumber(v, d_p, psi, ny);

            double r0delta = CalcR0deltaLengthRatio(psi);

            double eu = CalcEulerNumberWithPressureDropFactor(phiD, re, r0delta);

            double deltaP = CalcPressureDrop(deltaL, v, rho_f, d_p, psi, eu);


            return Pressure.FromPascals(deltaP);
        }

        /// <summary>
        /// Berechnung der Partikelanzahl
        /// </summary>
        /// <param name="deltaL">Länge des Festbettes [m]</param>
        /// <param name="f">Querschnittsfläche des Festbettes [m^2]</param>
        /// <param name="d_p">Sauter-Durchmesser [m]</param>
        /// <param name="psi">Porosität</param>
        /// <returns>Partikelanzahl</returns>
        /// <remarks>
        /// Gleichung (10) Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcAmountOfParticles(double deltaL, double f, double d_p, double psi)
        {
            //Gleichung (10)
            double z = ((1 - psi) * f * deltaL) / (Math.Pow(d_p, 3) * (Math.PI / 6.0)); //Z: Partikelanzahl

            return z;
        }

        /// <summary>
        /// Berechnung der Festbettlänge
        /// </summary>
        /// <param name="deltaP">Strömungsdruckabfall über dem Festbett [Pa]</param>
        /// <param name="v">Leerrohrfluidgeschwindigkeit [m/s]</param>
        /// <param name="rho_f">Fluiddichte [kg/m^3]</param>
        /// <param name="d_p">Sauter-Durchmesser [m]</param>
        /// <param name="psi">Porosität</param>
        /// <param name="eu"></param>
        /// <returns>Länge des Festbettes [m]</returns>
        /// <remarks>
        /// Gleichung (12) Umgestellt, siehe Beispiel in Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcBedLength(double deltaP, double v, double rho_f, double d_p, double psi, double eu)
        {
            //Gleichung (12) Umgestellt
            double deltaL = (4.0 / 3.0) * (deltaP / rho_f * Math.Pow(v, 2)) * d_p * (Math.Pow(psi, 2) / (1 - psi)) * 1 / eu;

            return deltaL;
        }

        /// <summary>
        /// Berechnung des Strömungsdruckabfall über dem Festbett
        /// </summary>
        /// <param name="deltaL">Länge des Festbettes [m]</param>
        /// <param name="v">Leerrohrfluidgeschwindigkeit [m/s]</param>
        /// <param name="rho_f">Fluiddichte [kg/m^3]</param>
        /// <param name="d_p">Sauter-Durchmesser [m]</param>
        /// <param name="psi">Porosität</param>
        /// <param name="eu"></param>
        /// <returns>Strömungsdruckabfall über dem Festbett [Pa]</returns>
        /// <remarks>
        /// Gleichung (12) Umgestellt nach Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcPressureDrop(double deltaL, double v, double rho_f, double d_p, double psi, double eu)
        {
            //Gleichung (12) Umgestellt
            double deltaP = eu / ((4.0 / 3.0) * (d_p / deltaL) * (Math.Pow(psi, 2) / (1.0 - psi)) * (1.0 / (rho_f * Math.Pow(v, 2))));

            return deltaP;
        }

        /// <summary>
        /// Berechnung der Euler-Zahl
        /// </summary>
        /// <param name="re">Partikel-Reynolds-Zahl</param>
        /// <param name="r0delta">Längenverhältnis r_0/delta</param>
        /// <returns>Euler-Zahl</returns>
        /// <remarks>
        /// Gleichung (13) Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcEulerNumber(double re, double r0delta)
        {
            //Gleichung (13)
            double eu_1 = (24 / re) * (1 + 0.692 * (r0delta + 0.5 * Math.Pow(r0delta, 2)));   //Stokes-Umströmung der Partikeln
            double eu_2 = ((4 / Math.Sqrt(re)) * (1 + 0.12 * Math.Pow(r0delta, 1.5)));        //Strömungswiderstand auf Grund der Ausbildung einer Strömungsgrenzschicht an den Partikeln
            double eu_3 = (0.4 + 0.891 * r0delta * Math.Pow(re, -0.1));                       //Ablöseverhalten der Strömung an den Partikeln

            double eu = eu_1 + eu_2 + eu_3;

            return eu;
        }

        /// <summary>
        /// Berechnung des Längenverhältnises r_0/delta
        /// </summary>
        /// <param name="psi">Porosität</param>
        /// <returns>r_0/delta</returns>
        /// <remarks>
        /// Gleichung (14) Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcR0deltaLengthRatio(double psi)
        {
            //Gleichung (14)
            double r0delta = Math.Pow((0.95 / (Math.Pow(1.0 - psi, 1.0 / 3.0))) - 1.0, -1.0);

            return r0delta;
        }

        /// <summary>
        /// Berechnung der Partikel-Reynolds-Zahl
        /// </summary>
        /// <param name="v">Leerrohrfluidgeschwindigkeit [m/s]</param>
        /// <param name="d_p">Sauter-Durchmesser [m]</param>
        /// <param name="psi">Porosität</param>
        /// <param name="ny">Kinematische Viskosität [m^2/s]</param>
        /// <returns>Partikel-Reynolds-Zahl</returns>
        /// <remarks>
        /// Gleichung (15) Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcReynoldsNumber(double v, double d_p, double psi, double ny)
        {
            //Gleichung 15
            double re = (v * d_p) / (psi * ny);

            return re;
        }

        /// <summary>
        /// Berechnung des dimensionslosen Strömungswiderstand einer einzelnen Partikel
        /// </summary>
        /// <param name="re">Partikel-Reynolds-Zahl</param>
        /// <returns>Strömungswiderstand einer einzelnen Partikel</returns>
        /// <remarks>
        /// Gleichung (16) Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcParticleFlowResistance(double re)
        {
            //Gleichung (16)
            double cw = (24.0 / re) + (4.0 / Math.Sqrt(re)) + 0.4; //dimensionslosen Strömungswiderstand einer einzelnen Partikel

            return cw;
        }

        /// <summary>
        /// Approximation der Euler-Zahl für nicht kugelförmige Partikeln mit dem Druckverlustformfactor PHI_D
        /// </summary>
        /// <param name="phiD">Druckverlustformfactor PHI_D</param>
        /// <param name="re">Partikel-Reynolds-Zahl</param>
        /// <param name="r0delta">Längenverhältnis r_0/delta</param>
        /// <returns>Euler-Zahl für nicht kugelförmige Partikeln</returns>
        /// <remarks>
        /// Gleichung (17) Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcEulerNumberWithPressureDropFactor(double phiD, double re, double r0delta)
        {
            //Gleichung (17) Approximationsfunktion der Euler-Zahl für nicht kugelförmige Partikeln
            double eu_phiD_1 = (24.0 / (re * Math.Pow(phiD, 2))) * (1 + 0.685 * (r0delta + 0.5 * Math.Pow(r0delta, 2)));
            double eu_phiD_2 = (4.0 / (Math.Sqrt(re) * Math.Pow(phiD, 1.5))) * (1 + 0.289 * Math.Pow(r0delta, 1.5));
            double eu_phiD_3 = (1.0 / phiD) * (0.4 + 0.514 * r0delta);

            double eu_phiD = eu_phiD_1 + eu_phiD_2 + eu_phiD_3;

            return eu_phiD;
        }

        /// <summary>
        /// Berechnung der Euler-Zahl mit der Druckdiffereinz als Eingangsparameter
        /// </summary>
        /// <param name="deltaP">Strömungsdruckabfall über dem Festbett [Pa]</param>
        /// <param name="deltaL">Länge des Festbettes [m]</param>
        /// <param name="v">Leerrohrfluidgeschwindigkeit [m/s]</param>
        /// <param name="rho_f">Fluiddichte [kg/m^3]</param>
        /// <param name="d_p">Sauter-Durchmesser [m]</param>
        /// <param name="psi">Porosität</param>
        /// <returns>Euler-Zahl (für nicht kugelförmige Partikeln)</returns>
        /// <remarks>
        /// Gleichung (18) Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcEulerNumberByPressureDrop(double deltaP, double deltaL, double v, double rho_f, double d_p, double psi)
        {
            //Gleichung (18) = (12)
            var eu = (4.0 / 3.0) * (deltaP / (rho_f * Math.Pow(v, 2))) * (d_p / deltaL) * (Math.Pow(psi, 2) / (1 - psi));

            return eu;
        }

        /// <summary>
        /// Berechnung des Strömungsdruckabfall über dem Festbett
        /// </summary>
        /// <param name="deltaL">Länge des Festbettes [m]</param>
        /// <param name="v">Leerrohrfluidgeschwindigkeit [m/s]</param>
        /// <param name="rho_f">Fluiddichte [kg/m^3]</param>
        /// <param name="d_p">Sauter-Durchmesser [m]</param>
        /// <param name="psi">Porosität</param>
        /// <param name="eu"></param>
        /// <returns>Strömungsdruckabfall über dem Festbett [Pa]</returns>
        /// <remarks>
        /// Gleichung (12) Umgestellt nach Bück, Andreas/Wirth, Karl-Ernst (2019): L1.6 Druckverlust in durchströmten Schüttungen,
        /// in: VDI-Wärmeatlas, 12. Aufl., Berlin/Heidelberg, Deutschland: Springer, [online] doi:10.1007/978-3-662-52989-8_79
        ///</remarks>
        internal double CalcPressureDrop(double deltaL, double v, double rho_f, double d_p, double psi, double eu)
        {
            //Gleichung (12) Umgestellt
            double deltaP = eu / ((4.0 / 3.0) * (d_p / deltaL) * (Math.Pow(psi, 2) / (1.0 - psi)) * (1.0 / (rho_f * Math.Pow(v, 2))));

            return deltaP;
        }
    }
}
