using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
        private Sport activeType;
        private Activity activeActivity;
        private GraphData wattsData;
        private GraphData heartRateData;
        private GraphData speedData;
        private GraphData cadenceData;
        private GraphData elevationData;
        private BarChartData lapData;
        private String heartRateInfo;
        private String cadenceInfo;
        private String speedInfo;
        private String elevationInfo;
        private String wattsInfo;
        private String name;

        public Activity ActiveActivity
        {
            get
            {
                return activeActivity;
            }
            set
            {
                activeActivity = value;
                RaisePropertyChanged("ActiveActivity");
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string HeartRateInfo
        {
            get
            {
                return heartRateInfo;
            }
            set {
                heartRateInfo = value;
                RaisePropertyChanged("HeartRateInfo");
            }
        }

        public string ElevationInfo
        {
            get
            {
                return elevationInfo;
            }
            set
            {
                elevationInfo = value;
                RaisePropertyChanged("ElevationInfo");
            }
        }

        public string SpeedInfo
        {
            get
            {
                return speedInfo;
            }
            set
            {
                speedInfo = value;
                RaisePropertyChanged("SpeedInfo");
            }
        }

        public string WattsInfo
        {
            get
            {
                return wattsInfo;
            }
            set
            {
                wattsInfo = value;
                RaisePropertyChanged("WattsInfo");
            }
        }

        public string CadenceInfo
        {
            get
            {
                return cadenceInfo;
            }
            set
            {
                cadenceInfo = value;
                RaisePropertyChanged("CadenceInfo");
            }
        }


        public GraphData WattsData {
            get
            {
                return wattsData;
            }
            set
            {
                wattsData = value;
                RaisePropertyChanged("WattsData");
            }
        }
        public BarChartData LapData {
            get
            {
                return lapData;
            }
            set
            {
                lapData = value;
                RaisePropertyChanged("LapData");
            }
        }
        public GraphData ElevationData {
            get
            {
                return elevationData;
            }
            set
            {
                elevationData = value;
                RaisePropertyChanged("ElevationData");
            }
        }
        public GraphData CadenceData {
            get
            {
                return cadenceData;
            }
            set
            {
                cadenceData = value;
                RaisePropertyChanged("CadenceData");
            }
        }
        public GraphData SpeedData {
            get
            {
                return speedData;
            }
            set
            {
                speedData = value;
                RaisePropertyChanged("SpeedData");
            }
        }

        public GraphData HeartRateData
        {
            get
            {
                return heartRateData;
            }
            set
            {
                heartRateData = value;
                RaisePropertyChanged("HeartRateData");
            }
        }

        public Sport ActiveType
        {
            get
            {
                return activeType;
            }
            set
            {
                activeType = value;
                RaisePropertyChanged("ActiveType");
            }
        }

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

            IEnumerable<String> sport = from data in root.Descendants(ns + "Activities")
                       select data.Element(ns + "Activity").Attribute("Sport").Value;

            String inputSport = sport.First();

            IEnumerable<Activity> activities = null;

            if (inputSport.Equals("Running"))
            {
                Sport type = Sport.Running;
                ActiveType = type;

                activities = root.Root.Elements(ns + "Activities").Elements(ns + "Activity")
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
                                                    new Coordinate(Double.Parse((String)z.Element(ns + "Position").Element(ns + "LatitudeDegrees").Value),
                                                    Double.Parse((String)z.Element(ns + "Position").Element(ns + "LongitudeDegrees").Value)),
                                                    Double.Parse((String)z.Element(ns + "AltitudeMeters").Value),
                                                    Double.Parse((String)z.Element(ns + "DistanceMeters").Value),
                                                    Double.Parse((String)z.Element(ns + "HeartRateBpm").Element(ns + "Value").Value),                                              
                                                    type,
                                                    extensions: new RunExtension(Double.Parse((String)z.Element(ns + "Extensions").Element(ns3 + "TPX").Element(ns3 + "Speed").Value),
                                                    Double.Parse((String)z.Element(ns + "Extensions").Element(ns3 + "TPX").Element(ns3 + "RunCadence").Value))
                                                    )).ToList(),
                                                type,
                                                Double.Parse((String)y.Element(ns + "Calories").Value)
                                            )).ToList()
                                        )).ToList();
            }
            else if(inputSport.Equals("Biking"))
            {
                Sport type = Sport.Biking;
                ActiveType = type;

                activities = root.Root.Elements(ns + "Activities").Elements(ns + "Activity")
                                      .Select(x => new BikeActivity(
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
                                                    new Coordinate(Double.Parse((String)z.Element(ns + "Position").Element(ns + "LatitudeDegrees").Value),
                                                    Double.Parse((String)z.Element(ns + "Position").Element(ns + "LongitudeDegrees").Value)),
                                                    Double.Parse((String)z.Element(ns + "AltitudeMeters").Value),
                                                    Double.Parse((String)z.Element(ns + "DistanceMeters").Value),
                                                    Double.Parse((String)z.Element(ns + "HeartRateBpm").Element(ns + "Value").Value),
                                                    type,
                                                    Double.Parse((String)z.Element(ns + "Cadence").Value),
                                                    extensions: new BikeExtension(Double.Parse((String)z.Element(ns + "Extensions").Element(ns3 + "TPX").Element(ns3 + "Speed").Value),
                                                    Double.Parse((String)z.Element(ns + "Extensions").Element(ns3 + "TPX").Element(ns3 + "Watts").Value))
                                                    )).ToList(),
                                            type,
                                            Double.Parse((String)y.Element(ns + "Calories").Value)
                                            )).ToList()
                                        )).ToList();
            }
            else
            {
                Console.WriteLine("Unrecognized Sport");
            }

            if(activities.First() != null)
            {
                ActiveActivity = activities.First();
            }

            List<double> l = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => (y.Extensions as BikeExtension).Watts)).ToList());
            List<DataPoint> dataPoints = new List<DataPoint>();
            int i = 0;
            foreach(double point in l)
            {
                dataPoints.Add(new DataPoint(i, point));
                i++;
            }
            double max = dataPoints.Count;
            double min = 0;

            WattsData = new GraphData(dataPoints, max, min);

            List<double> l2 = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => y.HeartRate)).ToList());
            List<DataPoint> dataPoints2 = new List<DataPoint>();
            i = 0;
            foreach (double point in l2)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                dataPoints2.Add(new DataPoint(TimeSpanAxis.ToDouble(time), point));
                i++;
            }
            max = dataPoints2.Count;
            min = 0;

            HeartRateData = new GraphData(dataPoints2, max, min);

            List<double> l3 = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => (y.Extensions as BikeExtension).Speed)).ToList());
            List<DataPoint> dataPoints3 = new List<DataPoint>();
            i = 0;
            foreach (double point in l3)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                dataPoints3.Add(new DataPoint(TimeSpanAxis.ToDouble(time), point));
                i++;
            }
            max = dataPoints3.Count;
            min = 0;

            SpeedData = new GraphData(dataPoints3, max, min);

            List<double> l4 = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => y.Cadence)).ToList());
            List<DataPoint> dataPoints4 = new List<DataPoint>();
            i = 0;
            foreach (double point in l4)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                dataPoints4.Add(new DataPoint(TimeSpanAxis.ToDouble(time), point));
                i++;
            }
            max = dataPoints4.Count;
            min = 0;

            CadenceData = new GraphData(dataPoints4, max, min);

            List<double> l5 = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => y.Altitude)).ToList());
            List<DataPoint> dataPoints5 = new List<DataPoint>();
            i = 0;
            foreach (double point in l5)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                dataPoints5.Add(new DataPoint(TimeSpanAxis.ToDouble(time), point));
                i++;
            }
            max = dataPoints5.Count;
            min = 0;

            ElevationData = new GraphData(dataPoints5, max, min);

            List<double> l6 = new List<double>(ActiveActivity.Laps.Select(x => x.TotalTimeSeconds).ToList());
            List<ColumnItem> dataPoints6 = new List<ColumnItem>();
            i = 0;
            foreach (double point in l6)
            {
                dataPoints6.Add(new ColumnItem { Color = OxyPlot.OxyColors.Aquamarine, Value = point/60, CategoryIndex = i });
                i++;
            }
            max = dataPoints6.Count;
            min = 0;

            LapData = new BarChartData(dataPoints6, max, min);



            if (ActiveActivity != null)
            {
                double avg = ActiveActivity.AvgHeartRate;
                double maxim = ActiveActivity.MaxHeartRate;
                HeartRateInfo = "Average: " + Math.Round(avg, 0) + " bpm\nMax: " + Math.Round(maxim, 0)+" bpm";
            }
            if (ActiveActivity != null)
            {
                if(ActiveActivity is BikeActivity)
                {
                    BikeActivity b = (BikeActivity)ActiveActivity; 
                    double avg = b.AvgCadence;
                    double maxim = b.MaxCadence;
                    CadenceInfo = "Average: " + Math.Round(avg, 0) + " rpm\nMax: " + Math.Round(maxim, 0)+" rpm";
                }
            }
            if (ActiveActivity != null)
            {
                double avg = ActiveActivity.AvgSpeed;
                double maxim = ActiveActivity.MaxSpeed;
                SpeedInfo = "Average: " + Math.Round(avg, 2) + " mph\nMax: " + Math.Round(maxim, 2)+" mph";
            }
            if (ActiveActivity != null)
            {
                if (ActiveActivity is BikeActivity)
                {
                    BikeActivity b = (BikeActivity)ActiveActivity;
                    double avg = b.AvgPower;
                    double maxim = b.MaxPower;
                    WattsInfo = "Average: " + Math.Round(avg, 0) + " W\nMax: " + Math.Round(maxim, 0)+" W";
                }
            }
            if (ActiveActivity != null)
            {
                double asc = 0;
                double desc = 0;
                var altPoints = ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => y.Altitude));

                double prev = altPoints.First();
                foreach(double point in altPoints)
                {
                    if(prev < point)
                    {
                        asc += point - prev;
                    }
                    else if(prev > point)
                    {
                        desc += prev - point;
                    }
                    prev = point;
                }

                ElevationInfo = "Ascent: " + Math.Round(asc, 0) + " m\nDescent: " + Math.Round(desc, 0)+" m";

                if (ActiveActivity != null)
                {
                    if (ActiveActivity is BikeActivity)
                    {
                        Name = (ActiveActivity as BikeActivity).Name;
                    }
                    else if (ActiveActivity is RunActivity)
                    {
                        Name = (ActiveActivity as RunActivity).Name;
                    }
                }
            }
        }
    }
}
