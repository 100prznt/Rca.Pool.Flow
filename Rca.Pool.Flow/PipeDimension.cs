﻿using Rca.Physical;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Rca.Pool.Flow
{
    public class PipeDimension
    {
        #region Constants
        
        
        #endregion Constants

        #region Member


        #endregion Member

        #region Properties
        /// <summary>
        /// Leitungstyp
        /// </summary>
        [Category("Typ")]
        [DisplayName("Leitungstyp")]
        public PipeCategories Category { get; set; }

        /// <summary>
        /// Anzeigename
        /// </summary>
        [Category("Name")]
        [DisplayName("Anzeigename")]
        [Description("Der Anzeigename ist ein lesbarer Name zur Unterscheidung verschiedener Leitungen.")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Eindeutiger Name; entspricht Dateiname der Definition
        /// </summary>
        [Category("Name")]
        [DisplayName("Eindeutiger Name")]
        [Description("Entspricht dem Namen der Definitionsdatei.")]
        public string UniqueName
        {
            get
            {
                var fileName = FilePath.Split('\\', '/').Last();
                return fileName.Substring(0, fileName.Length - 4); //Extension (.xml) entfernen
            }
        }

        /// <summary>
        /// Außendurchmesser
        /// </summary>
        [Category("Abmessungen")]
        [DisplayName("Außendurchmesser")]
        public PhysicalValue NominalDiameter { get; set; }

        /// <summary>
        /// Innendurchmesser
        /// </summary>
        [Category("Abmessungen")]
        [DisplayName("Innendurchmesser")]
        public PhysicalValue InnerDiameter { get; set; }

        /// <summary>
        /// Rohrrauheit
        /// </summary>
        [Category("Abmessungen")]
        [DisplayName("Rohrrauheit")]
        [Description("Die Rohrrauheit beschreibt die Unebenheit der Rohrinnenoberfläche. Der Wert drückt vereinfacht die Differenz zwischen vorhandenen Erhöhungen und Vertiefungen aus.")]
        public PhysicalValue Roughness { get; set; }




        /// <summary>
        /// Width of the space between two corrugations on the inside of the tube
        /// </summary>
        public PhysicalValue InnerWaveSpace { get; set; }

        /// <summary>
        /// Distance between two waves (= space + wave)
        /// </summary>
        public PhysicalValue WaveDistance { get; set; }

        /// <summary>
        /// Wave height
        /// </summary>
        public PhysicalValue WaveHeight { get; set; }



        /// <summary>
        /// Nendruck (PN)
        /// </summary>
        [Category("Abmessungen")]
        [DisplayName("Nenndruck (PN)")]
        public PhysicalValue NominalPressure { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public string FilePath { get; set; }

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Empty constructor for PipeDimension
        /// </summary>
        public PipeDimension()
        {

        }

        #endregion Constructor

        #region Services
        /// <summary>
        /// Im Standard-Format (XML) speichern
        /// </summary>
        /// <param name="path">Pfad unter welchem die Datei angelegt wird</param>
        public void ToFile(string path)
        {
            ToXmlFile(path);
        }

        /// <summary>
        /// Im XML Format speichern
        /// </summary>
        /// <param name="path">Pfad unter welchem die Datei angelegt wird</param>
        public void ToXmlFile(string path)
        {
            if (!path.EndsWith(".xml"))
            {
                if (path.EndsWith("\\") || path.EndsWith("/"))
                    path += FilePath.Split('\\', '/').Last();
                else
                    path = path + "\\" + FilePath.Split('\\', '/').Last();
            }

            var xs = new XmlSerializer(typeof(PipeDimension));

            using var sw = new StreamWriter(path);
            xs.Serialize(sw, this);
        }

        /// <summary>
        /// Objekt aus xml-Datei generieren
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PipeDimension FromFile(string path)
        {
            PipeDimension pipe;
            var xs = new XmlSerializer(typeof(PipeDimension));

            using (var sr = new StreamReader(path))
                pipe = (PipeDimension)xs.Deserialize(sr);

            return pipe;
        }

        public static string GetCsvHeader(char seperator = ';')
        {
            var header = new StringBuilder();
            header.Append(nameof(UniqueName));
            header.Append(seperator);
            header.Append(nameof(Category));
            header.Append(seperator);
            header.Append(nameof(NominalDiameter));
            header.Append(seperator);
            header.Append(nameof(InnerDiameter));
            header.Append(seperator);
            header.Append(nameof(Roughness));
            header.Append(seperator);
            header.Append(nameof(NominalPressure));
            header.Append(seperator);
            header.Append(nameof(DisplayName));

            return header.ToString();
        }

        public string ToCsvLine(char seperator = ';')
        {
            var line = new StringBuilder();
            line.Append(UniqueName);
            line.Append(seperator);
            line.Append(Category);
            line.Append(seperator);
            line.Append(NominalDiameter);
            line.Append(seperator);
            line.Append(InnerDiameter);
            line.Append(seperator);
            line.Append(Roughness);
            line.Append(seperator);
            line.Append(NominalPressure);
            line.Append(seperator);
            line.Append(DisplayName);

            return line.ToString();
        }

        public static PipeDimension FromCsvLine(string csvLine, char seperator = ';')
        {
            throw new NotImplementedException();

            //var data = csvLine.Split(seperator);
            //var pipe = new PipeDimension()
            //{
            //    FilePath = data[0] + ".xml", //UniqueName kann nicht direkt gesetzt werden
            //    Category = (PipeCategories)Enum.Parse(typeof(PipeCategories), data[1]),
            //    NominalDiameter = PhysicalValue.Parse(data[2]),
            //    InnerDiameter = PhysicalValue.Parse(data[3]),
            //    Roughness = PhysicalValue.Parse(data[4]),
            //    NominalPressure = PhysicalValue.Parse(data[5]),
            //    DisplayName = data[6]
            //};

            //return pipe;
        }

        #endregion Services

        #region Internal services


        #endregion Internal services

        #region Events


        #endregion Events
    }
}
