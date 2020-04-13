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
                if(ActiveActivity is RunActivity)
                {
                    return "No Watts data provided :(";
                }
                else
                {
                    return wattsInfo;
                }
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
                min = -1;

                if(lapList[0] < 0)
                {
                    LapData = null;
                }
                else
                {
                    LapData = new BarChartData(lapDataPoints, max, min);
                }

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
                min = -1;
                
                if(disLapList[0] < 0)
                {
                    LapData = null;
                }
                else
                {
                    LapData = new BarChartData(disLapDataPoints, max, min);
                }

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
                min = -1;

                if(speedLapList[0] < 0)
                {
                    LapData = null;
                }
                else
                {
                    LapData = new BarChartData(speedLapDataPoints, max, min);
                }

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
                min = -1;

                if(hrLapList[0] < 0)
                {
                    LapData = null;
                }
                else
                {
                    LapData = new BarChartData(hrLapDataPoints, max, min);
                }

                SelectedLapData = "Heart Rate (bpm)";
            }
            else if (selectedLapIndex == 4)
            {
                if(ActiveActivity is BikeActivity)
                {
                    //update watts lap graph
                    List<double> wattsLapList = new List<double>(ActiveActivity.Laps.Select(x => x.AvgPower).ToList());
                    List<ColumnItem> wattsLapDataPoints = new List<ColumnItem>();
                    foreach (double point in wattsLapList)
                    {
                        wattsLapDataPoints.Add(new ColumnItem { Value = point, Color = OxyColor.Parse("#b19cd9") });
                    }
                    max = wattsLapDataPoints.Count;
                    min = -1;

                    if(wattsLapList[0] < 0)
                    {
                        LapData = null;
                    }
                    else
                    {
                        LapData = new BarChartData(wattsLapDataPoints, max, min);
                    }

                    SelectedLapData = "Power (W)";
                }
                else
                {
                    LapData = null;
                    SelectedLapData = "Power (W)";
                }
            }
            else if (selectedLapIndex == 5)
            {
                if(ActiveActivity is BikeActivity)
                {
                    //update cadence lap graph
                    List<double> cadenceLapList = new List<double>(ActiveActivity.Laps.Select(x => x.AvgCadence).ToList());
                    List<ColumnItem> cadenceLapDataPoints = new List<ColumnItem>();
                    foreach (double point in cadenceLapList)
                    {
                        cadenceLapDataPoints.Add(new ColumnItem { Value = point, Color = OxyColors.LightGreen });
                    }
                    max = cadenceLapDataPoints.Count;
                    min = -1;

                    if(cadenceLapList[0] < 0)
                    {
                        LapData = null;
                    }
                    else
                    {
                        LapData = new BarChartData(cadenceLapDataPoints, max, min);
                    }

                    SelectedLapData = "Cadence (rpm)";
                }
                else if(ActiveActivity is RunActivity)
                {
                    //update cadence lap graph
                    List<double> cadenceLapList = new List<double>(ActiveActivity.Laps.Select(x => x.AvgCadence).ToList());
                    List<ColumnItem> cadenceLapDataPoints = new List<ColumnItem>();
                    foreach (double point in cadenceLapList)
                    {
                        cadenceLapDataPoints.Add(new ColumnItem { Value = point, Color = OxyColors.LightGreen });
                    }
                    max = cadenceLapDataPoints.Count;
                    min = -1;

                    if (cadenceLapList[0] < 0)
                    {
                        LapData = null;
                    }
                    else
                    {
                        LapData = new BarChartData(cadenceLapDataPoints, max, min);
                    }

                    SelectedLapData = "Cadence (spm)";
                }
                
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
                min = -1;

                if(egLapList[0] < 0)
                {
                    LapData = null;
                }
                else
                {
                    LapData = new BarChartData(egLapDataPoints, max, min);
                }

                SelectedLapData = "Elevation Gain (ft)";
            }
        }

        public void updateGraphsAndDataEntire()
        {
            StatsDisplayName = null;

            ActivityLoaded = true;

            double max = 0;
            double min = 0;

            DateTime start = ActiveActivity.Laps.First().Trackpoints.First().Time;

            //update watts graph
            if (ActiveType == Sport.Biking)
            {
                List<DataPoint> wattsDataPoints = new List<DataPoint>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start) , (y.Extensions as BikeExtension).Watts) )).ToList());

                max = wattsDataPoints.Max(x => x.X) +1;
                min = wattsDataPoints.Min(x => x.X);

                if(wattsDataPoints[0].Y >= 0)
                {
                    WattsData = new GraphData(wattsDataPoints, max, min);
                } 
                else
                {
                    WattsData = null;
                }
            }

            // update hr graph
            List<DataPoint> hrDataPoints = new List<DataPoint>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), y.HeartRate)).ToList()));

            max = hrDataPoints.Max(x => x.X) + 1;
            min = hrDataPoints.Min(x => x.X);

            if (hrDataPoints[0].Y >= 0)
            {
                HeartRateData = new GraphData(hrDataPoints, max, min);
            }
            else
            {
                HeartRateData = null;
            }

            //update speed graph
            List<DataPoint> speedDataPoints = new List<DataPoint>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), y.Extensions.Speed)).ToList()));

            max = speedDataPoints.Max(x => x.X) + 1;
            min = speedDataPoints.Min(x => x.X);

            if(speedDataPoints[0].Y >= 0)
            {
                SpeedData = new GraphData(speedDataPoints, max, min);
            }
            else
            {
                SpeedData = null;
            }
            
            
            if(ActiveType == Sport.Biking)
            {
                // update cadence graph
                List<DataPoint> cadenceDataPoints = new List<DataPoint>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), y.Cadence)).ToList()));

                max = cadenceDataPoints.Max(x => x.X) + 1;
                min = cadenceDataPoints.Min(x => x.X);

                if (cadenceDataPoints[0].Y >= 0)
                {
                    CadenceData = new GraphData(cadenceDataPoints, max, min);
                }
                else
                {
                    CadenceData = null;
                }

                
            }
            else if(ActiveType == Sport.Running)
            {
                // update cadence graph
                List<DataPoint> cadenceDataPoints = new List<DataPoint>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), (y.Extensions as RunExtension).Cadence)).ToList()));

                max = cadenceDataPoints.Max(x => x.X) + 1;
                min = cadenceDataPoints.Min(x => x.X);

                if (cadenceDataPoints[0].Y >= 0)
                {
                    CadenceData = new GraphData(cadenceDataPoints, max, min);
                }
                else
                {
                    CadenceData = null;
                }
            }



            // update elevation graph
            List<DataPoint> altitudeDataPoints = new List<DataPoint>(ActiveActivity.Laps.SelectMany(x => x.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), y.Altitude)).ToList()));

            max = altitudeDataPoints.Max(x => x.X) + 1;
            min = altitudeDataPoints.Min(x => x.X);

            if(altitudeDataPoints[0].Y != -1)
            {
                ElevationData = new GraphData(altitudeDataPoints, max, min);
            }
            else
            {
                ElevationData = null;
            }
            
            //update hr stats
            if (ActiveActivity != null)
            {
                double avg = ActiveActivity.AvgHeartRate;
                double maxim = ActiveActivity.MaxHeartRate;

                if(avg < 0)
                {
                    HeartRateInfo = "No HR data provided :(";
                }
                else
                {
                    HeartRateInfo = "Average: " + Math.Round(avg, 0) + " bpm\nMax: " + Math.Round(maxim, 0) + " bpm";
                }
            }
            //update cadence stats
            if (ActiveActivity != null)
            {
                if (ActiveActivity is BikeActivity)
                {
                    BikeActivity b = (BikeActivity)ActiveActivity;
                    double avg = b.AvgCadence;
                    double maxim = b.MaxCadence;
                    if (avg < 0)
                    {
                        CadenceInfo = "No cadence data provided :(";
                    }
                    else
                    {
                        CadenceInfo = "Average: " + Math.Round(avg, 0) + " rpm\nMax: " + Math.Round(maxim, 0) + " rpm";
                    }
                }
                else if(ActiveActivity is RunActivity)
                {
                    RunActivity a = (RunActivity)ActiveActivity;
                    double avg = a.AvgCadence;
                    double maxim = a.MaxCadence;
                    if (avg < 0)
                    {
                        CadenceInfo = "No cadence data provided :(";
                    }
                    else
                    {
                        CadenceInfo = "Average: " + Math.Round(avg, 0) + " spm\nMax: " + Math.Round(maxim, 0) + " spm";
                    }
                }
            }
            // update speed stats
            if (ActiveActivity != null)
            {
                double avg = ActiveActivity.AvgSpeed;
                double maxim = ActiveActivity.MaxSpeed;
                if(avg < 0)
                {
                    SpeedInfo = "No speed data provided :(";
                }
                else
                {
                    SpeedInfo = "Average: " + Math.Round(avg, 2) + " mph\nMax: " + Math.Round(maxim, 2) + " mph";
                }
            }
            // update watts stats
            if (ActiveActivity != null)
            {
                if (ActiveActivity is BikeActivity)
                {
                    BikeActivity b = (BikeActivity)ActiveActivity;
                    double avg = b.AvgPower;
                    double maxim = b.MaxPower;

                    if (avg < 0)
                    {
                        WattsInfo = "No power data provided :(";
                    }
                    else
                    {
                        WattsInfo = "Average: " + Math.Round(avg, 0) + " W\nMax: " + Math.Round(maxim, 0) + " W";
                    }
                }
                else
                {
                    WattsInfo = "No power data provided :(";
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

                if(asc < 0 && desc < 0)
                {
                    ElevationInfo = "No elevation data provided :(";
                }
                else
                {
                    ElevationInfo = "Ascent: " + Math.Round(asc, 0) + " ft\nDescent: " + Math.Round(desc, 0) + " ft\nLargest climb: " + Math.Round(largest, 0) + " ft";
                }
            }
        }

        private void updateGraphsAndData(int selectedLapIndex)
        {

            Lap lap = ActiveActivity.Laps[selectedLapIndex];
            DateTime start = lap.Trackpoints.First().Time;

            double max = 0;
            double min = 0;
            //update watts graph
            if (ActiveType == Sport.Biking)
            {
                List<DataPoint> wattsDataPoints = new List<DataPoint>(lap.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), (y.Extensions as BikeExtension).Watts)).ToList());

                max = wattsDataPoints.Max(x => x.X) +1;
                min = wattsDataPoints.Min(x => x.X);
               
                if (wattsDataPoints[0].Y >= 0)
                {
                    WattsData = new GraphData(wattsDataPoints, max, min);
                }
                else
                {
                    WattsData = null;
                }
            }

            // update hr graph
            List<DataPoint> hrDataPoints = new List<DataPoint>(lap.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), y.HeartRate)).ToList());

            max = hrDataPoints.Max(x => x.X) +1;
            min = hrDataPoints.Min(x => x.X);

            if (hrDataPoints[0].Y >= 0)
            {
                HeartRateData = new GraphData(hrDataPoints, max, min);
            }
            else
            {
                HeartRateData = null;
            }

            //update speed graph
            List<DataPoint> speedDataPoints = new List<DataPoint>(lap.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), y.Extensions.Speed)).ToList());

            max = speedDataPoints.Max(x => x.X) +1;
            min = speedDataPoints.Min(x => x.X);

            if (speedDataPoints[0].Y >= 0)
            {
                SpeedData = new GraphData(speedDataPoints, max, min);
            }
            else
            {
                SpeedData = null;
            }
            
            if (ActiveType == Sport.Biking)
            {
                // update cadence graph
                List<DataPoint> cadenceDataPoints = new List<DataPoint>(lap.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), y.Cadence)).ToList());

                max = cadenceDataPoints.Max(x => x.X) +1;
                min = cadenceDataPoints.Min(x => x.X);

                if(cadenceDataPoints[0].Y >= 0)
                {
                    CadenceData = new GraphData(cadenceDataPoints, max, min);
                } 
                else
                {
                    CadenceData = null;
                }
            }
            else if (ActiveType == Sport.Running)
            {
                // update cadence graph
                List<DataPoint> cadenceDataPoints = new List<DataPoint>(lap.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), (y.Extensions as RunExtension).Cadence)).ToList());

                max = cadenceDataPoints.Max(x => x.X) +1;
                min = cadenceDataPoints.Min(x => x.X);

                if (cadenceDataPoints[0].Y >= 0)
                {
                    CadenceData = new GraphData(cadenceDataPoints, max, min);
                }
                else
                {
                    CadenceData = null;
                }
            }

            // update elevation graph
            List<DataPoint> altitudeDataPoints = new List<DataPoint>(lap.Trackpoints.Select(y => new DataPoint(TimeSpanAxis.ToDouble(y.Time - start), y.Altitude)).ToList());

            max = altitudeDataPoints.Max(x => x.X) +1;
            min = altitudeDataPoints.Min(x => x.X);

            if (altitudeDataPoints[0].Y != -1)
            {
                ElevationData = new GraphData(altitudeDataPoints, max, min);
            }
            else
            {
                ElevationData = null;
            }

            //Update HR stats
            if (ActiveActivity != null)
            {
                double avg = lap.AvgHeartRate;
                double maxim = lap.MaxHeartRate;
                if (avg < 0)
                {
                    HeartRateInfo = "No HR data provided :(";
                }
                else
                {
                    HeartRateInfo = "Average: " + Math.Round(avg, 0) + " bpm\nMax: " + Math.Round(maxim, 0) + " bpm";
                }
            }
            //update cadence stats
            if (ActiveActivity != null)
            {
                if (ActiveActivity is BikeActivity)
                {
                    double avg = lap.AvgCadence;
                    double maxim = lap.MaxCadence;
                    if (avg < 0)
                    {
                        CadenceInfo = "No cadence data provided :(";
                    }
                    else
                    {
                        CadenceInfo = "Average: " + Math.Round(avg, 0) + " spm\nMax: " + Math.Round(maxim, 0) + " spm";
                    }
                }
            }
            //Update speed stats
            if (ActiveActivity != null)
            {
                double avg = lap.AvgSpeed;
                double maxim = lap.MaxSpeed;
                if (avg < 0)
                {
                    SpeedInfo = "No speed data provided :(";
                }
                else
                {
                    SpeedInfo = "Average: " + Math.Round(avg, 2) + " mph\nMax: " + Math.Round(maxim, 2) + " mph";
                }
            }
            // update power stats
            if (ActiveActivity != null)
            {
                if (ActiveActivity is BikeActivity)
                {
                    double avg = lap.AvgPower;
                    double maxim = lap.MaxPower;
                    if (avg < 0)
                    {
                        WattsInfo = "No power data provided :(";
                    }
                    else
                    {
                        WattsInfo = "Average: " + Math.Round(avg, 0) + " W\nMax: " + Math.Round(maxim, 0) + " W";
                    }
                }
                else
                {
                    WattsInfo = "No power data provided :(";
                }
            }

            //update elevation stats
            if (ActiveActivity != null)
            {
                var ascList = ActiveActivity.Laps.Select(x => x.ElevationGain);
                var descList = ActiveActivity.Laps.Select(x => x.ElevationLoss);

                double asc = lap.ElevationGain;
                double desc = lap.ElevationLoss;
                double largest = lap.BiggestClimb;

                if (asc < 0 && desc < 0)
                {
                    ElevationInfo = "No elevation data provided :(";
                }
                else
                {
                    ElevationInfo = "Ascent: " + Math.Round(asc, 0) + " ft\nDescent: " + Math.Round(desc, 0) + " ft\nLargest climb: " + Math.Round(largest, 0) + " ft";
                }

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

                activities = root?.Root?.Elements(ns + "Activities")?.Elements(ns + "Activity")?
                                      .Select(x => new RunActivity(
                                            DateTime.TryParse((String)x?.Element(ns + "Id")?.Value, out DateTime i)? i : DateTime.Now,
                                            (String)x?.Element(ns + "Creator")?.Element(ns + "Name")?.Value,
                                            x.Elements(ns + "Lap")?
                                            .Select(y => new Lap(
                                                DateTime.TryParse((String)y?.Attribute("StartTime")?.Value, out DateTime uu) ? uu : DateTime.Now,
                                                Double.TryParse((String)y?.Element(ns + "TotalTimeSeconds")?.Value, out double u)? u : -1,
                                                Double.TryParse((String)y?.Element(ns + "DistanceMeters")?.Value, out double tt)? tt : -1,
                                                Double.TryParse((String)y?.Element(ns + "MaximumSpeed")?.Value, out double t)? t : -1,
                                                y.Element(ns + "Track")?.Elements(ns + "Trackpoint")?
                                                .Select(z => new Trackpoint(
                                                    DateTime.TryParse((String)z?.Element(ns + "Time"), out DateTime dt)? dt : DateTime.Now,
                                                    new Coordinate(Double.TryParse((String)z?.Element(ns + "Position")?.Element(ns + "LatitudeDegrees")?.Value, out double rr)? rr : -1,
                                                    Double.TryParse((String)z?.Element(ns + "Position")?.Element(ns + "LongitudeDegrees")?.Value, out double r)? r : -1),
                                                    Double.TryParse((String)z?.Element(ns + "AltitudeMeters")?.Value, out double ee)? ee : -1,
                                                    Double.TryParse((String)z?.Element(ns + "DistanceMeters")?.Value, out double e)? e : -1,
                                                    Double.TryParse((String)z?.Element(ns + "HeartRateBpm")?.Element(ns + "Value").Value, out double ww) ? ww : -1,                                              
                                                    type,
                                                    extensions: new RunExtension(Double.TryParse((String)z?.Element(ns + "Extensions")?.Element(ns3 + "TPX")?.Element(ns3 + "Speed")?.Value, out double w)? w : -1,
                                                    Double.TryParse((String)z?.Element(ns + "Extensions")?.Element(ns3 + "TPX")?.Element(ns3 + "RunCadence").Value, out double qq)?qq : -1)
                                                    )).ToList(),
                                                type,
                                                Double.TryParse((String)y?.Element(ns + "Calories")?.Value, out double q)? q : -1
                                            )).ToList()
                                        )).ToList();
            }
            else if(inputSport.Equals("Biking"))
            {
                Sport type = Sport.Biking;
                ActiveType = type;

                activities = root?.Root?.Elements(ns + "Activities")?.Elements(ns + "Activity")?
                                      .Select(x => new BikeActivity(
                                            DateTime.TryParse((String)x?.Element(ns + "Id")?.Value, out DateTime zzz)? zzz : DateTime.Now,
                                            (String)x?.Element(ns + "Creator")?.Element(ns + "Name")?.Value,
                                            x.Elements(ns + "Lap")?
                                            .Select(y => new Lap(
                                                DateTime.TryParse((String)y?.Attribute("StartTime")?.Value, out DateTime xxx)? xxx : DateTime.Now,
                                                Double.TryParse((String)y?.Element(ns + "TotalTimeSeconds")?.Value, out double q)? q : -1,
                                                Double.TryParse((String)y?.Element(ns + "DistanceMeters")?.Value, out double uu)? uu : -1,
                                                Double.TryParse((String)y?.Element(ns + "MaximumSpeed")?.Value, out double u)? u : -1,
                                                y.Element(ns + "Track").Elements(ns + "Trackpoint")
                                                .Select(z => new Trackpoint(
                                                    DateTime.TryParse((String)z?.Element(ns + "Time"), out DateTime rr) ? rr : DateTime.Now,
                                                    new Coordinate(Double.TryParse((String)z?.Element(ns + "Position")?.Element(ns + "LatitudeDegrees")?.Value, out double yy)? yy : -1,
                                                    Double.TryParse((String)z?.Element(ns + "Position")?.Element(ns + "LongitudeDegrees")?.Value, out double sss) ? sss : -1),
                                                    Double.TryParse((String)z?.Element(ns + "AltitudeMeters")?.Value, out double tt) ? tt : -1,
                                                    Double.TryParse((String)z?.Element(ns + "DistanceMeters")?.Value, out double ee) ? ee : -1,
                                                    Double.TryParse((String)z?.Element(ns + "HeartRateBpm")?.Element(ns + "Value")?.Value, out double e) ? e : -1,
                                                    type,
                                                    Double.TryParse((String)z?.Element(ns + "Cadence")?.Value, out double ss) ? ss : -1,
                                                    extensions: new BikeExtension(Double.TryParse((String)z?.Element(ns + "Extensions")?.Element(ns3 + "TPX")?.Element(ns3 + "Speed")?.Value, out double t) ? t : -1,
                                                    Double.TryParse((String)z?.Element(ns + "Extensions")?.Element(ns3 + "TPX")?.Element(ns3 + "Watts")?.Value, out double w) ? w : -1)
                                                    )).ToList(),
                                            type,
                                            Double.TryParse((String)y?.Element(ns + "Calories")?.Value, out double s) ? s : -1
                                            ))?.ToList()
                                        ))?.ToList();
            }
            else
            {
                MessageBox.Show("Unrecognized activity type");
                return;
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
