﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using TCX_Visualizer.Models;

namespace TCX_Visualizer.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private String filePath;
        public String FilePath
        {
            get {
                return this.filePath;
            }
            set {
                this.filePath = value;
                RaisePropertyChanged("FilePath");
            }
        }
        public ICommand OpenFileCommand { get; private set; }

        public MainViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenTCXFile);
        }

        public void OpenTCXFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "TCX Files (*.tcx)|*.tcx";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FilePath = openFileDialog.FileName;
                    readXml(FilePath);
                }
            }
        }

        private void readXml(string filename)
        {
            XDocument root = XDocument.Load(filename);
            XNamespace ns = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace ns3 = "http://www.garmin.com/xmlschemas/ActivityExtension/v2";

            //IEnumerable<String> sport = from data in root.Descendants(ns + "Activities")
            //           select data.Element(ns + "Activity").Attribute("Sport").Value;

            //  String inputSport = sport.First();

            //   IEnumerable<String> id = from data in root.Descendants(ns + "Activities").Descendants(ns + "Activity")
            //                           select data.Element(ns + "Id").Value;

            // String inputId = id.First();
            //   DateTime idAsDate = DateTime.Parse(inputId);

            //   IEnumerable<String> device = from data in root.Descendants(ns + "Activities").Descendants(ns + "Activity")
            //                    select data.Element(ns + "Creator").Attribute("Type").Value;

            //  String inputDevice = device.First();
            //  Console.WriteLine(inputDevice);

            IEnumerable<Activity> activities = root.Root.Elements(ns + "Activities").Elements(ns + "Activity")
                                            .Select(x => new RunActivity(
                                                                DateTime.Parse((String)x.Element(ns + "Id").Value),
                                                                (String)x.Element(ns + "Creator").Element(ns + "Name").Value,
                                                                x.Elements(ns + "Lap")
                                                                .Select(y => new Lap(
                                                                            DateTime.Parse((String)y.Attribute("StartTime").Value),
                                                                            Double.Parse((String)y.Element(ns + "TotalTimeSeconds").Value),
                                                                            Double.Parse((String)y.Element(ns + "DistanceMeters").Value),
                                                                            Double.Parse((String)y.Element(ns + "MaximumSpeed").Value),
                                                                            y.Element(ns + "Track").Elements(ns + "Trackpoint")
                                                                            .Select(z => new Trackpoint(
                                                                                        DateTime.Parse((String)z.Element(ns + "Time")),
                                                                                        new Coordinate(Double.Parse((String)z.Element(ns + "Position").Element(ns+ "LatitudeDegrees").Value),
                                                                                        Double.Parse((String)z.Element(ns + "Position").Element(ns + "LongitudeDegrees").Value)),
                                                                                        Double.Parse((String)z.Element(ns + "AltitudeMeters").Value),
                                                                                        Double.Parse((String)z.Element(ns + "DistanceMeters").Value),
                                                                                        Double.Parse((String)z.Element(ns + "HeartRateBpm").Element(ns+"Value").Value),
                                                                                        extensions: new RunExtension(Double.Parse((String)z.Element(ns + "Extensions").Element(ns3+"TPX").Element(ns3+ "Speed").Value),
                                                                                        Double.Parse((String)z.Element(ns + "Extensions").Element(ns3 + "TPX").Element(ns3 + "RunCadence").Value))
                                                                                        )
                                                                             ).ToList(),
                                                                            Double.Parse((String)y.Element(ns+"Calories").Value),
                                                                            Double.Parse((String)y.Element(ns+ "AverageHeartRateBpm").Element(ns+"Value").Value),
                                                                            Double.Parse((String)y.Element(ns+ "MaximumHeartRateBpm").Element(ns+"Value").Value)
                                                                        )
                                                                ).ToList()
                                                            ))
                                            .ToList();

            Console.WriteLine(activities.First().Id);
            Console.WriteLine(activities.First().Device);
            Console.WriteLine(activities.First().Laps[2].AvgHeartRate);

        }
    }
}