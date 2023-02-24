using Rca.Physical;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca.Pool.Flow.Medium
{
    /// <summary>
    /// Medienparameter für Wasser
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DisplayName}")]
    public class Water
    {
        #region Member


        #endregion Member

        #region Properties
        /// <summary>
        /// Name des Mediums
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Temperatur
        /// [°C]
        /// </summary>
        public PhysicalValue Temperature { get; init; }

        /// <summary>
        /// Dichte
        /// [kg/m^3]
        /// </summary>
        public PhysicalValue Density { get; init; }

        /// <summary>
        /// Dynamische Viskosität
        /// [10E-6 kg/m s]
        /// </summary>
        public PhysicalValue DynamicViscosity { get; init; }

        /// <summary>
        /// Kinetische Viskosität
        /// [10E-6 m^2/s]
        /// </summary>
        public PhysicalValue KineticViscosity { get; init; }

        public string DisplayName => string.Format("{0} {1:F1} °C", Name, Temperature);

        #endregion Properties

        #region Static services
        /// <summary>
        /// <para>Wasser bei 5 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At5Celsius => new Water()
        {
            Temperature = new(5, PhysicalUnits.Celsius),
            Density = new(999.97, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(1518.7E-6, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(1.519E-6, PhysicalUnits.SquareMetrePerSecond)
        };

        /// <summary>
        /// <para>Wasser bei 10 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At10Celsius => new Water()
        {
            Temperature = new(10, PhysicalUnits.Celsius),
            Density = new(999.7, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(1306.4, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(1.307, PhysicalUnits.SquareMetrePerSecond)
        };

        /// <summary>
        /// <para>Wasser bei 15 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At15Celsius => new Water()
        {
            Temperature = new(15, PhysicalUnits.Celsius),
            Density = new(999.1, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(1138, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(1.139, PhysicalUnits.SquareMetrePerSecond)
        };

        /// <summary>
        /// <para>Wasser bei 20 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At20Celsius => new Water()
        {
            Temperature = new(20, PhysicalUnits.Celsius),
            Density = new(998.21, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(1002, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(1.004, PhysicalUnits.SquareMetrePerSecond)
        };

        /// <summary>
        /// <para>Wasser bei 25 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At25Celsius => new Water()
        {
            Temperature = new(25, PhysicalUnits.Celsius),
            Density = new(997.05, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(890.45, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(0.893, PhysicalUnits.SquareMetrePerSecond)
        };

        /// <summary>
        /// <para>Wasser bei 30 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At30Celsius => new Water()
        {
            Temperature = new(30, PhysicalUnits.Celsius),
            Density = new(995.65, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(797.68, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(0.801, PhysicalUnits.SquareMetrePerSecond)
        };

        /// <summary>
        /// <para>Wasser bei 35 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At35Celsius => new Water()
        {
            Temperature = new(35, PhysicalUnits.Celsius),
            Density = new(994.03, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(719.62, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(0.724, PhysicalUnits.SquareMetrePerSecond)
        };

        /// <summary>
        /// <para>Wasser bei 40 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At40Celsius => new Water()
        {
            Temperature = new(40, PhysicalUnits.Celsius),
            Density = new(993.22, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(653.25, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(0.658, PhysicalUnits.SquareMetrePerSecond)
        };

        /// <summary>
        /// <para>Wasser bei 45 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At45Celsius => new Water()
        {
            Temperature = new(45, PhysicalUnits.Celsius),
            Density = new(990.21, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(596.32, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(0.602, PhysicalUnits.SquareMetrePerSecond)
        };

        /// <summary>
        /// <para>Wasser bei 50 °C</para>
        /// Stoffwerte übernommen von: http://www.uni-magdeburg.de/isut/LSS/Lehre/Arbeitsheft/IV.pdf
        /// </summary>
        public static Water At50Celsius => new Water()
        {
            Temperature = new(50, PhysicalUnits.Celsius),
            Density = new(988.04, PhysicalUnits.KilogramPerCubicMetre),
            DynamicViscosity = new(547.08, PhysicalUnits.KilogramPerMeterSecond),
            KineticViscosity = new(0.554, PhysicalUnits.SquareMetrePerSecond)
        };
        #endregion Static services

        #region Constructor
        /// <summary>
        /// Empty constructor for Medium
        /// </summary>
        public Water()
        {
            Name = "Wasser";
        }

        #endregion Constructor

        #region Services

        #endregion Services
        
        #region Internal services


        #endregion Internal services

        #region Events


        #endregion Events
    }
}
