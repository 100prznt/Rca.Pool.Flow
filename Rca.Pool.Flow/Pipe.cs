﻿using Rca.Physical;
using Rca.Pool.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca.Pool.Flow
{
    public class Pipe : PipeBase
    {
        #region Member


        #endregion Member

        #region Properties
        /// <summary>
        /// Rohrlänge
        /// </summary>
        public PhysicalValue Length { get; set; }

        /// <summary>
        /// Rohrrauheit
        /// </summary>
        public PhysicalValue Roughness { get; set; }

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Konstruktor
        /// </summary>
        public Pipe() :base()
        {

        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="l">Rohrlängein [m]</param>
        /// <param name="pipeDimension">Rohrdefinition</param>
        public Pipe(PhysicalValue l, PipeDimension pipeDimension) : this(l, pipeDimension.InnerDiameter, pipeDimension.Roughness)
        {

        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="l">Rohrlänge</param>
        /// <param name="di">Innerer Rohrdurchmesser</param>
        /// <param name="k">Rohrrauheit</param>
        public Pipe(PhysicalValue di, PhysicalValue l, PhysicalValue k) : base(di)
        {
            Length = l;
            Roughness = k;
        }

        #endregion Constructor

        #region Services
        /// <summary>
        /// Druckverlust berechnen
        /// (es wird von einer turbulenten Strömung ausgegangen)
        /// </summary>
        /// <param name="medium">Stoffdaten</param>
        /// <returns>Druckverlust in [bar]</returns>
        public double CalcPressureDrop(Medium medium) => CalcPressureDrop(medium, FlowRate);

        /// <summary>
        /// Druckverlust berechnen
        /// (es wird von einer turbulenten Strömung ausgegangen)
        /// </summary>
        /// <param name="medium">Stoffdaten</param>
        /// <param name="flowRate">Volumenstrom in [m^3/h]</param>
        /// <returns>Druckverlust in [bar]</returns>
        public double CalcPressureDrop(Medium medium, double flowRate)
        {
            double di = Diameter / 1000; // [m]
            double k = Roughness / 1000; // [m]
            double v = CalcFlowVelocity(flowRate); // [m/s]
            double kv = medium.KineticViscosity / 1E6; // [m^2/s]
            double re = v * di / kv;

            // Rohrreibungszahl (Lambda) nach Colebrook und White siehe: https://de.wikipedia.org/wiki/Rohrreibungszahl
            // Iterative Berechnung
            double lambda = 0.005; // Startwert für Lambda
            double s = 0.001; // Schrittweite für Antastung
            int i = 7; // Anzahl der Richtungswechsel

            double error = double.MaxValue - 1;
            while (i > 0)
            {
                double lastError = error;
                error = 1 / Math.Sqrt(lambda) - (-2 * Math.Log10((2.51 / (re * Math.Sqrt(lambda))) + (k / (3.71 * di))));

                if (Math.Abs(error) >= Math.Abs(lastError))
                {
                    s /= -10;
                    i--;
                }
                lambda += s;
            }


            // Druckverlust durch Rohrreibung
            // https://www.schweizer-fn.de/stroemung/druckverlust/druckverlust.php#druckverlustrohr
            double deltaP = lambda * Length * medium.Density * Math.Pow(v, 2) / (di * 2); // [Pa]

            double deltaP_Bar = deltaP / 1E5;

            return deltaP_Bar;
        }

        public override string ToString()
        {
            return $"{Length} m (d = {Diameter} mm, k = {Roughness} mm)";
        }

        #endregion Services

        #region Internal services


        #endregion Internal services

        #region Events


        #endregion Events
    }
}