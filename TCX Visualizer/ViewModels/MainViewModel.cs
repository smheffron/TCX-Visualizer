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
        private String name = "Upload an Activity";
        private int selectedLapIndex;
        private int selectedLapInfoIndex;
        private bool activityLoaded = false;
        private String selectedLapData;
        private String statsDisplayName;

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

        public String StatsDisplayName
        {
            get
            {
                if(statsDisplayName != null)
                {
                    return statsDisplayName;
                }
                else
                {
                    return "Overall stats:";
                }
            }
            set
            {

                statsDisplayName = value;
                RaisePropertyChanged("StatsDisplayName");
            }
        }

        public bool ActivityLoaded
        {
            get
            {
                return activityLoaded;
            }
            set
            {
                activityLoaded = value;
                RaisePropertyChanged("ActivityLoaded");
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

        public int SelectedLapIndex
        {
            get
            {
                return selectedLapIndex;
            }
            set
            {
                this.selectedLapIndex = value;
                StatsDisplayName = "Lap " + (selectedLapIndex + 1).ToString() + " stats:";
                updateGraphsAndData(selectedLapIndex);
                RaisePropertyChanged("SelectedLapIndex");
            }
        }

        public String SelectedLapData
        {
            get
            {
                return selectedLapData;
            }
            set
            {
                this.selectedLapData = value;
                RaisePropertyChanged("SelectedLapData");
            }
        }

        public int SelectedLapInfoIndex
        {
            get
            {
                return selectedLapInfoIndex;
            }
            set
            {
                this.selectedLapInfoIndex = value;
                updateLapChart(selectedLapInfoIndex);
                RaisePropertyChanged("SelectedLapInfoIndex");
            }
        }

        public ICommand OpenFileCommand { get; private set; }
        public ICommand LoadEntireAveragesCommand { get; private set; }

        public MainViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenTCXFile);
            LoadEntireAveragesCommand = new RelayCommand(updateGraphsAndDataEntire);
        }

        private void updateLapChart(int selectedLapIndex)
        {
            double min = 0;
            double max = 0;
            if(selectedLapIndex == 0)
            {
                //update time lap graph
                max = 0;
                min = 0;
                List<double> lapList = new List<double>(ActiveActivity.Laps.Select(x => x.TotalTimeSeconds).ToList());
                List<ColumnItem> lapDataPoints = new List<ColumnItem>();
                foreach (double point in lapList)
                {
                    lapDataPoints.Add(new ColumnItem { Value = point / 60, Color = OxyColors.MediumAquamarine });
                }
                max = lapDataPoints.Count;
                min = 0;

                LapData = new BarChartData(lapDataPoints, max, min);

                SelectedLapData = "Time (min)";
            }
            else if (selectedLapIndex == 1)
            {
                //update distance lap graph
                List<double> disLapList = new List<double>(ActiveActivity.Laps.Select(x => x.TotalDistanceMeters).ToList());
                List<ColumnItem> disLapDataPoints = new List<ColumnItem>();
                foreach (double point in disLapList)
                {
                    disLapDataPoints.Add(new ColumnItem { Value = point * 0.000621371, Color = OxyColors.OrangeRed });
                }
                max = disLapDataPoints.Count;
                min = 0;

                LapData = new BarChartData(disLapDataPoints, max, min);

                SelectedLapData = "Distance (mi)";
            }
            else if(selectedLapIndex == 2) {
                //update speed lap graph
                List<double> speedLapList = new List<double>(ActiveActivity.Laps.Select(x => x.AvgSpeed).ToList());
                List<ColumnItem> speedLapDataPoints = new List<ColumnItem>();
                foreach (double point in speedLapList)
                {
                    speedLapDataPoints.Add(new ColumnItem { Value = point, Color = OxyColors.LightBlue });
                }
                max = speedLapDataPoints.Count;
                min = 0;

                LapData = new BarChartData(speedLapDataPoints, max, min);

                SelectedLapData = "Speed (mph)";
            }
            else if (selectedLapIndex == 3)
            {
                //update hr lap graph
                List<double> hrLapList = new List<double>(ActiveActivity.Laps.Select(x => x.AvgHeartRate).ToList());
                List<ColumnItem> hrLapDataPoints = new List<ColumnItem>();
                foreach (double point in hrLapList)
                {
                    hrLapDataPoints.Add(new ColumnItem { Value = point, Color = OxyColors.IndianRed });
                }
                max = hrLapDataPoints.Count;
                min = 0;

                LapData = new BarChartData(hrLapDataPoints, max, min);

                SelectedLapData = "Heart Rate (bpm)";
            }
            else if (selectedLapIndex == 4)
            {
                //update watts lap graph
                List<double> wattsLapList = new List<double>(ActiveActivity.Laps.Select(x => x.AvgPower).ToList());
                List<ColumnItem> wattsLapDataPoints = new List<ColumnItem>();
                foreach (double point in wattsLapList)
                {
                    wattsLapDataPoints.Add(new ColumnItem { Value = point, Color = OxyColor.Parse("#b19cd9") });
                }
                max = wattsLapDataPoints.Count;
                min = 0;

                LapData = new BarChartData(wattsLapDataPoints, max, min);

                SelectedLapData = "Power (W)";
            }
            else if (selectedLapIndex == 5)
            {
                //update cadence lap graph
                List<double> cadenceLapList = new List<double>(ActiveActivity.Laps.Select(x => x.AvgCadence).ToList());
                List<ColumnItem> cadenceLapDataPoints = new List<ColumnItem>();
                foreach (double point in cadenceLapList)
                {
                    cadenceLapDataPoints.Add(new ColumnItem { Value = point, Color = OxyColors.LightGreen });
                }
                max = cadenceLapDataPoints.Count;
                min = 0;

                LapData = new BarChartData(cadenceLapDataPoints, max, min);

                SelectedLapData = "Cadence (rpm)";
            }
            else if (selectedLapIndex == 6)
            {

                //update eg lap graph
                List<double> egLapList = new List<double>(ActiveActivity.Laps.Select(x => x.ElevationGain).ToList());
                List<ColumnItem> egLapDataPoints = new List<ColumnItem>();
                foreach (double point in egLapList)
                {
                    egLapDataPoints.Add(new ColumnItem { Value = point, Color = OxyColors.SandyBrown });
                }
                max = egLapDataPoints.Count;
                min = 0;

                LapData = new BarChartData(egLapDataPoints, max, min);

                SelectedLapData = "Elevation Gain (ft)";
            }
        }

        public void updateGraphsAndDataEntire()
        {
            StatsDisplayName = null;

            ActivityLoaded = true;

            int i = 0;
            double max = 0;
            double min = 0;

            //update watts graph
            if (ActiveType == Sport.Biking)
            {
                List<double> wattsList = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => (y.Extensions as BikeExtension).Watts)).ToList());
                List<DataPoint> wattsDataPoints = new List<DataPoint>();
                foreach (double point in wattsList)
                {
                    TimeSpan time = TimeSpan.FromSeconds(i);

                    wattsDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                    i++;
                }
                max = wattsDataPoints.Count / 60;
                min = 0;

                WattsData = new GraphData(wattsDataPoints, max, min);
            }
            
            // update hr graph
            List<double> hrList = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => y.HeartRate)).ToList());
            List<DataPoint> hrDataPoints = new List<DataPoint>();
            i = 0;
            foreach (double point in hrList)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                hrDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                i++;
            }
            max = hrDataPoints.Count / 60;
            min = 0;

            HeartRateData = new GraphData(hrDataPoints, max, min);

            if(ActiveType == Sport.Biking)
            {
                List<double> speedList = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => (y.Extensions as BikeExtension).Speed)).ToList());
                List<DataPoint> speedDataPoints = new List<DataPoint>();
                i = 0;
                foreach (double point in speedList)
                {
                    TimeSpan time = TimeSpan.FromSeconds(i);

                    speedDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                    i++;
                }
                max = speedDataPoints.Count / 60;
                min = 0;

                SpeedData = new GraphData(speedDataPoints, max, min);
            }
            
            // update cadence graph
            List<double> cadenceList = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => y.Cadence)).ToList());
            List<DataPoint> cadenceDataPoints = new List<DataPoint>();
            i = 0;
            foreach (double point in cadenceList)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                cadenceDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                i++;
            }
            max = cadenceDataPoints.Count / 60;
            min = 0;

            CadenceData = new GraphData(cadenceDataPoints, max, min);

            // update elevation graph
            List<double> elevationList = new List<double>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => y.Altitude)).ToList());
            List<DataPoint> elevationDataPoints = new List<DataPoint>();
            i = 0;
            foreach (double point in elevationList)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                elevationDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                i++;
            }
            max = elevationDataPoints.Count / 60;
            min = 0;

            ElevationData = new GraphData(elevationDataPoints, max, min);
            
            //update hr stats
            if (ActiveActivity != null)
            {
                double avg = ActiveActivity.AvgHeartRate;
                double maxim = ActiveActivity.MaxHeartRate;
                HeartRateInfo = "Average: " + Math.Round(avg, 0) + " bpm\nMax: " + Math.Round(maxim, 0) + " bpm";
            }
            //update cadence stats
            if (ActiveActivity != null)
            {
                if (ActiveActivity is BikeActivity)
                {
                    BikeActivity b = (BikeActivity)ActiveActivity;
                    double avg = b.AvgCadence;
                    double maxim = b.MaxCadence;
                    CadenceInfo = "Average: " + Math.Round(avg, 0) + " rpm\nMax: " + Math.Round(maxim, 0) + " rpm";
                }
            }
            // update speed stats
            if (ActiveActivity != null)
            {
                double avg = ActiveActivity.AvgSpeed;
                double maxim = ActiveActivity.MaxSpeed;
                SpeedInfo = "Average: " + Math.Round(avg, 2) + " mph\nMax: " + Math.Round(maxim, 2) + " mph";
            }
            // update watts stats
            if (ActiveActivity != null)
            {
                if (ActiveActivity is BikeActivity)
                {
                    BikeActivity b = (BikeActivity)ActiveActivity;
                    double avg = b.AvgPower;
                    double maxim = b.MaxPower;
                    WattsInfo = "Average: " + Math.Round(avg, 0) + " W\nMax: " + Math.Round(maxim, 0) + " W";
                }
            }
            //update elevation stats
            if (ActiveActivity != null)
            {
                var ascList = ActiveActivity.Laps.Select(x => x.ElevationGain);
                var descList = ActiveActivity.Laps.Select(x => x.ElevationLoss);

                double asc = 0;
                double desc = 0;

                foreach(double a in ascList)
                {
                    asc += a;
                }
                foreach(double d in descList)
                {
                    desc += d;
                }

                double largest = ActiveActivity.Laps.Max(x => x.BiggestClimb);


                ElevationInfo = "Ascent: " + Math.Round(asc, 0) + " ft\nDescent: " + Math.Round(desc, 0) + " ft\nLargest climb: " + Math.Round(largest, 0) + " ft";
            }
        }

        private void updateGraphsAndData(int selectedLapIndex)
        {
            Lap lap = ActiveActivity.Laps[selectedLapIndex];
            
            //Update watts graph
            List<double> wattsList = new List<double>(lap.Trackpoints.Select(x => (x.Extensions as BikeExtension).Watts)).ToList();
            List<DataPoint> wattsDataPoints = new List<DataPoint>();
            int i = 0;
            foreach (double point in wattsList)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                wattsDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                i++;
            }
            double max = wattsDataPoints.Count / 60;
            double min = 0;

            WattsData = new GraphData(wattsDataPoints, max, min);

            //update hr graph
            List<double> hrList = new List<double>(lap.Trackpoints.Select(y => y.HeartRate)).ToList();
            List<DataPoint> hrDataPoints = new List<DataPoint>();
            i = 0;
            foreach (double point in hrList)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                hrDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                i++;
            }
            max = hrDataPoints.Count / 60;
            min = 0;

            HeartRateData = new GraphData(hrDataPoints, max, min);

            // Update speed graph
            List<double> speedList = new List<double>(lap.Trackpoints.Select(y => (y.Extensions as BikeExtension).Speed)).ToList();
            List<DataPoint> speedDataPoints = new List<DataPoint>();
            i = 0;
            foreach (double point in speedList)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                speedDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                i++;
            }
            max = speedDataPoints.Count / 60;
            min = 0;

            SpeedData = new GraphData(speedDataPoints, max, min);

            // Update Cadence graph
            List<double> cadenceList = new List<double>(lap.Trackpoints.Select(y => y.Cadence)).ToList();
            List<DataPoint> cadenceDataPoints = new List<DataPoint>();
            i = 0;
            foreach (double point in cadenceList)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                cadenceDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                i++;
            }
            max = cadenceDataPoints.Count / 60;
            min = 0;

            CadenceData = new GraphData(cadenceDataPoints, max, min);

            // Updated elevevation graph
            List<double> elevationList = new List<double>(lap.Trackpoints.Select(y => y.Altitude)).ToList();
            List<DataPoint> elevationDataPoints = new List<DataPoint>();
            i = 0;
            foreach (double point in elevationList)
            {
                TimeSpan time = TimeSpan.FromSeconds(i);

                elevationDataPoints.Add(new DataPoint(TimeSpanAxis.ToDouble(time) / 60, point));
                i++;
            }
            max = elevationDataPoints.Count / 60;
            min = 0;

            ElevationData = new GraphData(elevationDataPoints, max, min);

            //Update HR stats
            if (ActiveActivity != null)
            {
                double avg = lap.AvgHeartRate;
                double maxim = lap.MaxHeartRate;
                HeartRateInfo = "Average: " + Math.Round(avg, 0) + " bpm\nMax: " + Math.Round(maxim, 0) + " bpm";
            }
            //update cadence stats
            if (ActiveActivity != null)
            {
                if (ActiveActivity is BikeActivity)
                {
                    double avg = lap.AvgCadence;
                    double maxim = lap.MaxCadence;
                    CadenceInfo = "Average: " + Math.Round(avg, 0) + " rpm\nMax: " + Math.Round(maxim, 0) + " rpm";
                }
            }
            //Update speed stats
            if (ActiveActivity != null)
            {
                double avg = lap.AvgSpeed;
                double maxim = lap.MaxSpeed;
                SpeedInfo = "Average: " + Math.Round(avg, 2) + " mph\nMax: " + Math.Round(maxim, 2) + " mph";
            }
            // update power stats
            if (ActiveActivity != null)
            {
                if (ActiveActivity is BikeActivity)
                {
                    double avg = lap.AvgPower;
                    double maxim = lap.MaxPower;
                    WattsInfo = "Average: " + Math.Round(avg, 0) + " W\nMax: " + Math.Round(maxim, 0) + " W";
                }
            }
            if (ActiveActivity != null)
            {
                var ascList = ActiveActivity.Laps.Select(x => x.ElevationGain);
                var descList = ActiveActivity.Laps.Select(x => x.ElevationLoss);

                double asc = lap.ElevationGain;
                double desc = lap.ElevationLoss;
                double largest = lap.BiggestClimb;

                ElevationInfo = "Ascent: " + Math.Round(asc, 0) + " ft\nDescent: " + Math.Round(desc, 0) + " ft\nLargest climb: " + Math.Round(largest, 0) + " ft";

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

            int lapNum = 0;
            foreach(Lap lap in ActiveActivity.Laps)
            {
                lapNum++;
                lap.DisplayName = "Lap " + lapNum;
            }

            

            //update Name
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

            updateGraphsAndDataEntire();
            updateLapChart(0);
        }
    }
}
