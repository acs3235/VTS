using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using GalaSoft.MvvmLight.Command;
using SLExtensions.Input;
using Vts.Extensions;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.Model;
using Vts.IO;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Silverlight;
using DataPoint = OxyPlot.DataPoint;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;
using Series = OxyPlot.Series.Series;

namespace Vts.Gui.Silverlight.ViewModel
{
    public class DataPointCollection
    {
        public IDataPoint[] DataPoints { get; set; }
        public String Title { get; set; }
        public string ColorTag { get; set; }
    }

    public class PlotPointCollection : ObservableCollection<Point[]>
    {
        public IList<string> ColorTags { get; set; }

        public PlotPointCollection(Point[][] points, IList<string> colorTags)
            : base(points)
        {
            ColorTags = colorTags;
        }

        public PlotPointCollection()
            : base()
        {
            ColorTags = new List<string>();
        }

        public void Add(Point[] item, string groupName)
        {
            ColorTags.Add(groupName);
            base.Add(item);
        }

        public new void Clear()
        {
            ColorTags.Clear();
            base.Clear();
        }

        public PlotPointCollection Clone()
        {
            return new PlotPointCollection(this.Select(points => points).ToArray(), this.ColorTags.Select(name => name).ToArray());
        }
    }

    /// <summary>
    /// View model implementing Plot panel functionality
    /// </summary>
    public class PlotViewModel : BindableObject
    {
        // change from Point to our own custom class so we can bind to color, style, etc, too

        private PlotModel _plotModel;
        private string _Title;
        private IList<string> _PlotTitles;
        private ReflectancePlotType _PlotType;
        private bool _HoldOn;
        private PlotPointCollection _PlotSeriesCollection;
        private IList<string> _Labels;
        private OptionViewModel<ScalingType> _XAxisSpacingOptionVM;
        private OptionViewModel<ScalingType> _YAxisSpacingOptionVM;
        private OptionViewModel<PlotToggleType> _PlotToggleTypeOptionVM;
        private OptionViewModel<PlotNormalizationType> _PlotNormalizationTypeOptionVM;
        private string _CustomPlotLabel;
        private bool _ShowAxes;
        private bool _showComplexPlotToggle;

        private double _MinYValue;
        private double _MaxYValue;
        private double _MinXValue;
        private double _MaxXValue;
        private bool _AutoScaleX;
        private bool _AutoScaleY;
        private bool _IsComplexPlot;
        private IndependentVariableAxis _CurrentIndependentVariableAxis;

        public PlotViewModel()
        {
            _MinYValue = 1E-9;
            _MaxYValue = 1.0;
            _MinXValue = 1E-9;
            _MaxXValue = 1.0;
            _AutoScaleX = true;
            _AutoScaleY = true;

            RealLabels = new List<string>();
            ImagLabels = new List<string>();
            PhaseLabels = new List<string>();
            AmplitudeLabels = new List<string>(); ;
            Labels = new List<string>();
            PlotTitles = new List<string>();
            DataSeriesCollection = new List<DataPointCollection>();
            PlotSeriesCollection = new PlotPointCollection();
            //IsComplexPlot = false;

            PlotModel = new PlotModel
            {
                Title = "Plot View",
                LegendPlacement = LegendPlacement.Outside
            };
            PlotType = ReflectancePlotType.ForwardSolver;
            _HoldOn = true;
            _ShowAxes = false;
            _showComplexPlotToggle = false;

            XAxisSpacingOptionVM = new OptionViewModel<ScalingType>("XAxisSpacing", false);
            XAxisSpacingOptionVM.PropertyChanged += (sender, args) => UpdatePlotSeries();

            YAxisSpacingOptionVM = new OptionViewModel<ScalingType>("YAxisSpacing", false);
            YAxisSpacingOptionVM.PropertyChanged += (sender, args) => UpdatePlotSeries();

            PlotToggleTypeOptionVM = new OptionViewModel<PlotToggleType>("ToggleType", false);
            PlotToggleTypeOptionVM.PropertyChanged += (sender, args) => UpdatePlotSeries();

            PlotNormalizationTypeOptionVM = new OptionViewModel<PlotNormalizationType>("NormalizationType", false);
            PlotNormalizationTypeOptionVM.PropertyChanged += (sender, args) => UpdatePlotSeries();

            CustomPlotLabel = "";

            Commands.Plot_PlotValues.Executed += Plot_Executed;
            Commands.Plot_SetAxesLabels.Executed += Plot_SetAxesLabels_Executed;

            ClearPlotCommand = new RelayCommand(() => Plot_Cleared(null, null));
            ClearPlotSingleCommand = new RelayCommand(() => Plot_ClearedSingle(null, null));
            ExportDataToTextCommand = new RelayCommand(() => Plot_ExportDataToText_Executed(null, null));
            DuplicateWindowCommand = new RelayCommand(() => Plot_DuplicateWindow_Executed(null, null));
        }

        private void Plot_DuplicateWindow_Executed(object sender, ExecutedEventArgs e)
        {
            var vm = this.Clone();
            Commands.Main_DuplicatePlotView.Execute(vm, vm);
        }

        public RelayCommand ClearPlotCommand { get; set; }
        public RelayCommand ClearPlotSingleCommand { get; set; }
        public RelayCommand ExportDataToTextCommand { get; set; }
        public RelayCommand DuplicateWindowCommand { get; set; }

        private List<DataPointCollection> DataSeriesCollection { get; set; }
        //private IList<IList<IDataPoint>> DataSeriesCollectionToggle { get; set; }
        private IList<string> RealLabels { get; set; }
        private IList<string> ImagLabels { get; set; }
        private IList<string> PhaseLabels { get; set; }
        private IList<string> AmplitudeLabels { get; set; }

        public PlotViewModel Clone()
        {
            return Clone(this);
        }

        public static PlotViewModel Clone(PlotViewModel plotToClone)
        {
            var output = new PlotViewModel();

            Commands.Plot_PlotValues.Executed -= output.Plot_Executed;

            output._Title = plotToClone._Title;
            output._PlotTitles = plotToClone._PlotTitles.ToList();
            output._PlotType = plotToClone._PlotType;
            output._HoldOn = plotToClone._HoldOn;
            output._PlotSeriesCollection = plotToClone._PlotSeriesCollection.Clone();
            output._Labels = plotToClone._Labels.ToList();
            output._CustomPlotLabel = plotToClone._CustomPlotLabel;
            output._ShowAxes = plotToClone._ShowAxes;
            output._MinYValue = plotToClone._MinYValue;
            output._MaxYValue = plotToClone._MaxYValue;
            output._MinXValue = plotToClone._MinXValue;
            output._MaxXValue = plotToClone._MaxXValue;
            output._AutoScaleX = plotToClone._AutoScaleX;
            output._AutoScaleY = plotToClone._AutoScaleY;
            output._IsComplexPlot = plotToClone._IsComplexPlot;
            output._CurrentIndependentVariableAxis = plotToClone._CurrentIndependentVariableAxis;

            output.RealLabels = plotToClone.RealLabels;
            output.ImagLabels = plotToClone.ImagLabels;
            output.PhaseLabels = plotToClone.PhaseLabels;
            output.AmplitudeLabels = plotToClone.AmplitudeLabels;

            output._YAxisSpacingOptionVM.Options[plotToClone._YAxisSpacingOptionVM.SelectedValue].IsSelected = true;
            output._PlotNormalizationTypeOptionVM.Options[plotToClone._PlotNormalizationTypeOptionVM.SelectedValue].IsSelected = true;
            output._PlotToggleTypeOptionVM.Options[plotToClone._PlotToggleTypeOptionVM.SelectedValue].IsSelected = true;
            output._XAxisSpacingOptionVM.Options[plotToClone._XAxisSpacingOptionVM.SelectedValue].IsSelected = true;

            output.DataSeriesCollection =
                  plotToClone.DataSeriesCollection.Select(ds => new DataPointCollection { DataPoints = ds.DataPoints.Select(val => val).ToArray(), ColorTag = ds.ColorTag }).ToList();
            //output.DataSeriesCollectionToggle =
            //    plotToClone.DataSeriesCollectionToggle.Select(ds => (IList<IDataPoint>)ds.Select(val => val).ToList()).ToList();

            return output;
        }

        public PlotModel PlotModel
        {
            get
            {
                return _plotModel;
            }
            set
            {
                _plotModel = value;
                this.OnPropertyChanged("PlotModel");
            }
        }

        public PlotPointCollection PlotSeriesCollection
        {
            get
            {
                return _PlotSeriesCollection;
            }
            set
            {
                _PlotSeriesCollection = value;
                this.OnPropertyChanged("PlotSeriesCollection");
            }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        public ReflectancePlotType PlotType
        {
            get { return _PlotType; }
            set
            {
                _PlotType = value;
                this.OnPropertyChanged("PlotType");
            }
        }

        public bool HoldOn
        {
            get { return _HoldOn; }
            set
            {
                _HoldOn = value;
                OnPropertyChanged("HoldOn");
            }
        }

        public bool ShowAxes
        {
            get { return _ShowAxes; }
            set
            {
                _ShowAxes = value;
                OnPropertyChanged("ShowAxes");
            }
        }

        public bool ShowComplexPlotToggle
        {
            get { return _showComplexPlotToggle; }
            set
            {
                _showComplexPlotToggle = value;
                OnPropertyChanged("ShowComplexPlotToggle");
            }
        }

        public OptionViewModel<ScalingType> XAxisSpacingOptionVM
        {
            get { return _XAxisSpacingOptionVM; }
            set
            {
                _XAxisSpacingOptionVM = value;
                OnPropertyChanged("XAxisSpacingOptionVM");
            }
        }

        public OptionViewModel<ScalingType> YAxisSpacingOptionVM
        {
            get { return _YAxisSpacingOptionVM; }
            set
            {
                _YAxisSpacingOptionVM = value;
                OnPropertyChanged("YAxisSpacingOptionVM");
            }
        }
        public OptionViewModel<PlotToggleType> PlotToggleTypeOptionVM
        {
            get { return _PlotToggleTypeOptionVM; }
            set
            {
                _PlotToggleTypeOptionVM = value;
                OnPropertyChanged("PlotToggleTypeOptionVM");
            }
        }
        public IndependentVariableAxis CurrentIndependentVariableAxis
        {
            get { return _CurrentIndependentVariableAxis; }
            set
            {
                // if user switches independent variable, clear plot
                if (_CurrentIndependentVariableAxis != value)
                {
                    ClearPlot();
                    Commands.TextOutput_PostMessage.Execute("Plot View: plot cleared due to independent axis variable change\r");
                }
                _CurrentIndependentVariableAxis = value;
                OnPropertyChanged("CurrentIndependentVariableAxis");
            }
        }
        public OptionViewModel<PlotNormalizationType> PlotNormalizationTypeOptionVM
        {
            get { return _PlotNormalizationTypeOptionVM; }
            set
            {
                _PlotNormalizationTypeOptionVM = value;
                OnPropertyChanged("PlotNormalizationTypeOptionVM");
            }
        }

        public string CustomPlotLabel
        {
            get { return _CustomPlotLabel; }
            set
            {
                _CustomPlotLabel = value;
                OnPropertyChanged("CustomPlotLabel");
            }
        }

        public IList<string> Labels
        {
            get { return _Labels; }
            set
            {
                _Labels = value;
                this.OnPropertyChanged("Labels");
            }
        }

        public IList<string> PlotTitles
        {
            get { return _PlotTitles; }
            set
            {
                _PlotTitles = value;
                this.OnPropertyChanged("PlotTitles");
            }
        }

        public bool AutoScaleX
        {
            get { return _AutoScaleX; }
            set
            {
                _AutoScaleX = value;
                OnPropertyChanged("AutoScaleX");
                OnPropertyChanged("ManualScaleX");
            }
        }

        public bool AutoScaleY
        {
            get { return _AutoScaleY; }
            set
            {
                _AutoScaleY = value;
                OnPropertyChanged("AutoScaleY");
                OnPropertyChanged("ManualScaleY");
            }
        }

        public bool ManualScaleX
        {
            get { return !_AutoScaleX; }
            set
            {
                _AutoScaleX = !value;
                OnPropertyChanged("ManualScaleX");
                OnPropertyChanged("AutoScaleX");
            }
        }

        public bool ManualScaleY
        {
            get { return !_AutoScaleY; }
            set
            {
                _AutoScaleY = !value;
                OnPropertyChanged("ManualScaleY");
                OnPropertyChanged("AutoScaleY");
            }
        }

        public double MinXValue
        {
            get { return _MinXValue; }
            set
            {
                _MinXValue = value;
                OnPropertyChanged("MinXValue");
            }
        }

        public double MaxXValue
        {
            get { return _MaxXValue; }
            set
            {
                _MaxXValue = value;
                OnPropertyChanged("MaxXValue");
            }
        }

        public double MinYValue
        {
            get { return _MinYValue; }
            set
            {
                _MinYValue = value;
                OnPropertyChanged("MinYValue");
            }
        }

        public double MaxYValue
        {
            get { return _MaxYValue; }
            set
            {
                _MaxYValue = value;
                OnPropertyChanged("MaxYValue");
            }
        }

        protected override void AfterPropertyChanged(string propertyName)
        {
            if ((!AutoScaleX && (propertyName == "MinXValue" || propertyName == "MaxXValue")) ||
                (!AutoScaleY && (propertyName == "MinYValue" || propertyName == "MaxYValue")) ||
                propertyName == "AutoScaleX" ||
                propertyName == "AutoScaleY")
            {
                UpdatePlotSeries();
            }
        }

        void Plot_SetAxesLabels_Executed(object sender, ExecutedEventArgs e)
        {
            if (e.Parameter is PlotAxesLabels)
            {
                var labels = e.Parameter as PlotAxesLabels;
                // set CurrentIndependtVariableAxis prior to setting Title because property
                // might ClearPlot including Title
                CurrentIndependentVariableAxis = labels.IndependentAxis.AxisType;
                Title =
                        labels.DependentAxisName + " [" + labels.DependentAxisUnits + "] versus " +
                        labels.IndependentAxis.AxisLabel + " [" + labels.IndependentAxis.AxisUnits + "]";

                if (labels.ConstantAxes.Length > 0)
                {
                    Title += " at " + labels.ConstantAxes[0].AxisLabel + " = " + labels.ConstantAxes[0].AxisValue + " " + labels.ConstantAxes[0].AxisUnits;
                }
                if (labels.ConstantAxes.Length > 1)
                {
                    Title += " and " + labels.ConstantAxes[1].AxisLabel + " = " + labels.ConstantAxes[1].AxisValue + " " + labels.ConstantAxes[1].AxisUnits;
                }
            }
        }

        /// <summary>
        /// Writes tab-delimited 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Plot_ExportDataToText_Executed(object sender, ExecutedEventArgs e)
        {
            if (_Labels != null && _Labels.Count > 0 && _PlotSeriesCollection != null && _PlotSeriesCollection.Count > 0)
            {
                using (var stream = StreamFinder.GetLocalFilestreamFromSaveFileDialog("txt"))
                {
                    if (stream != null)
                    {
                        using (StreamWriter sw = new StreamWriter(stream))
                        {
                            sw.Write("%");
                            _Labels.ForEach(label => sw.Write(label + " (X)" + "\t" + label + " (Y)" + "\t"));
                            sw.WriteLine();
                            for (int i = 0; i < _PlotSeriesCollection[0].Length; i++)
                            {
                                sw.WriteLine();
                                for (int j = 0; j < _PlotSeriesCollection.Count; j++)
                                {
                                    sw.Write(_PlotSeriesCollection[j][i].X + "\t" + _PlotSeriesCollection[j][i].Y + "\t");
                                }
                            }
                        }
                    }
                }
            }
        }

        void Plot_Cleared(object sender, ExecutedEventArgs e)
        {
            ClearPlot();
            UpdatePlotSeries();
        }

        void Plot_ClearedSingle(object sender, ExecutedEventArgs e)
        {
            ClearPlotSingle();
            UpdatePlotSeries();
        }

        void Plot_Executed(object sender, ExecutedEventArgs e)
        {
            var data = e.Parameter as PlotData[];
            if (data != null)
            {
                AddValuesToPlotData(data);
            }
        }

        //static int labelCounter = 0;
        private void AddValuesToPlotData(PlotData[] plotData)
        {
            if (!_HoldOn)
            {
                ClearPlot();
            }

            var customLabel = CustomPlotLabel.Length > 0 ? "[" + CustomPlotLabel + "]" : "";
            foreach (var t in plotData)
            {
                var points = t.Points;
                var title = customLabel + t.Title;

                DataSeriesCollection.Add(new DataPointCollection { DataPoints = points, Title = title, ColorTag = "ColorTag" });
                //if (DataSeriesCollection.Count > 0 && points[0] is ComplexDataPoint)
                //{
                //    RealLabels.Add(title + "\r(real)" + customLabel);
                //    PhaseLabels.Add(title + "\r(phase)" + customLabel);
                //    ImagLabels.Add(title + "\r(imag)" + customLabel);
                //    AmplitudeLabels.Add(title + "\r(amp)" + customLabel);
                //}
                //else
                //{
                //    Labels.Add(title + customLabel); // has to happen before updating the bound collection
                //}
            }

            //PlotTitles.Add(Title);

            UpdatePlotSeries();
        }

        private void ClearPlot()
        {
            DataSeriesCollection.Clear();
            //PlotModel.Series.Clear();
        }

        private void ClearPlotSingle()
        {
            DataSeriesCollection.RemoveAt(DataSeriesCollection.Count - 1);
            //PlotModel.Series.RemoveAt(PlotModel.Series.Count - 1);
        }

        private void CalculateMinMax(PlotModel plotModel)
        {
            // get min and max values for reference
            if (!AutoScaleX && !AutoScaleY) return;
            var minX = double.PositiveInfinity;
            var maxX = double.NegativeInfinity;
            var minY = double.PositiveInfinity;
            var maxY = double.NegativeInfinity;
            foreach (var point in plotModel.Series.Cast<LineSeries>().SelectMany(series => series.Points))
            {
                if (AutoScaleX)
                {
                    if (point.X > maxX)
                    {
                        maxX = point.X;
                    }
                    if (point.X < minX)
                    {
                        minX = point.X;
                    }
                }
                if (!AutoScaleY) continue;
                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
                if (point.Y < minY)
                {
                    minY = point.Y;
                }
            }
            if (AutoScaleX)
            {
                MinXValue = minX;
                MaxXValue = maxX;
            }
            if (!AutoScaleY) return;
            MinYValue = minY;
            MaxYValue = maxY;
        }

        /// <summary>
        /// Updates the plot. 
        /// </summary>
        private void UpdatePlotSeries()
        {
            PlotModel.Series.Clear();
            ShowComplexPlotToggle = false; // do not show the complex toggle until a complex plot is plotted

            // function to filter the results if we're not auto-scaling
            Func<DataPoint, bool> isWithinAxes = p => (AutoScaleX || (p.X <= MaxXValue && p.X >= MinXValue)) && (AutoScaleY || (p.Y <= MaxYValue && p.Y >= MinYValue));

            // function to filter out any invalid data points
            Func<DataPoint, bool> isValidDataPoint = p => !double.IsInfinity(Math.Abs(p.X)) && !double.IsNaN(p.X) && !double.IsInfinity(Math.Abs(p.Y)) && !double.IsNaN(p.Y);

            //check if any normalization is selected 
            var normToCurve = PlotNormalizationTypeOptionVM.SelectedValue == PlotNormalizationType.RelativeToCurve && DataSeriesCollection.Count > 1;
            var normToMax = PlotNormalizationTypeOptionVM.SelectedValue == PlotNormalizationType.RelativeToMax && DataSeriesCollection.Count > 0;

            foreach (var t in DataSeriesCollection)
            {
                double x;
                double y;
                //check if datapoint is complex
                if (t.DataPoints[0] is ComplexDataPoint)
                {
                    switch (PlotToggleTypeOptionVM.SelectedValue)
                    {
                        case PlotToggleType.Complex:
                            var lineSeriesReal = new LineSeries();
                            var lineSeriesImaginary = new LineSeries();
                            foreach (var dataPoint in t.DataPoints)
                            {
                                var dp = (ComplexDataPoint)dataPoint;
                                x = XAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.X) : dp.X;
                                y = YAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.Y.Real) : dp.Y.Real;
                                lineSeriesReal.Points.Add(new DataPoint(x, y));
                                x = XAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.X) : dp.X;
                                y = YAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.Y.Imaginary) : dp.Y.Imaginary;
                                lineSeriesImaginary.Points.Add(new DataPoint(x, y));
                            }
                            lineSeriesReal.Title = t.Title + " (real)";
                            lineSeriesReal.MarkerType = MarkerType.Circle;
                            PlotModel.Series.Add(lineSeriesReal);
                            lineSeriesImaginary.Title = t.Title + " (imag)";
                            lineSeriesImaginary.MarkerType = MarkerType.Circle;
                            PlotModel.Series.Add(lineSeriesImaginary);
                            break;
                        case PlotToggleType.Phase:
                            var lineSeriesPhase = new LineSeries();
                            foreach (var dataPoint in t.DataPoints)
                            {
                                var dp = (ComplexDataPoint)dataPoint;
                                x = XAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.X) : dp.X;
                                y = YAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.Y.Phase * (180 / Math.PI)) : dp.Y.Phase * (180 / Math.PI);
                                lineSeriesPhase.Points.Add(new DataPoint(x, y));
                            }
                            lineSeriesPhase.Title = t.Title + " (phase)";
                            lineSeriesPhase.MarkerType = MarkerType.Circle;
                            PlotModel.Series.Add(lineSeriesPhase);
                            break;
                        case PlotToggleType.Amp:
                            var lineSeriesAmp = new LineSeries();
                            foreach (var dataPoint in t.DataPoints)
                            {
                                var dp = (ComplexDataPoint)dataPoint;
                                x = XAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.X) : dp.X;
                                y = YAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.Y.Magnitude) : dp.Y.Magnitude;
                                lineSeriesAmp.Points.Add(new DataPoint(x, y));
                            }
                            lineSeriesAmp.Title = t.Title + " (amp)";
                            lineSeriesAmp.MarkerType = MarkerType.Circle;
                            PlotModel.Series.Add(lineSeriesAmp);
                            break;
                    }
                    ShowComplexPlotToggle = true; // right now, it's all or nothing - assume all plots are ComplexDataPoints
                }
                else
                {
                    var lineSeries = new LineSeries();
                    var max = 1.0;
                    if (normToMax)
                    {
                        var points = t.DataPoints.Cast<DoubleDataPoint>().ToArray();
                        max = points.Select(p => p.Y).Max();
                    }
                    double[] tempY = null;
                    if (normToCurve)
                    {
                        tempY = (from DoubleDataPoint dp in DataSeriesCollection[0].DataPoints select dp.Y).ToArray();
                    }
                    var curveIndex = 0;
                    foreach (var dataPoint in t.DataPoints)
                    {
                        var dp = (DoubleDataPoint)dataPoint;
                        switch (PlotNormalizationTypeOptionVM.SelectedValue)
                        {
                            case PlotNormalizationType.RelativeToCurve:
                                x = XAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.X) : dp.X;
                                var curveY = normToCurve && tempY != null ? tempY[curveIndex] : 1.0;
                                y = YAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.Y / curveY) : dp.Y / curveY;
                                break;
                            case PlotNormalizationType.RelativeToMax:
                                x = XAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.X) : dp.X;
                                y = YAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10((dp.Y / max)) : dp.Y / max;
                                break;
                            default:
                                x = XAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.X) : dp.X;
                                y = YAxisSpacingOptionVM.SelectedValue == ScalingType.Log ? Math.Log10(dp.Y) : dp.Y;
                                break;
                        }
                        var point = new DataPoint(x, y);
                        if (isValidDataPoint(point) && isWithinAxes(point))
                        {
                            lineSeries.Points.Add(point);
                        }
                        curveIndex += 1;
                    }
                    lineSeries.Title = t.Title;
                    lineSeries.MarkerType = MarkerType.Circle;
                    PlotModel.Series.Add(lineSeries);
                }
            }
            CalculateMinMax(PlotModel);
            //var xAxis = new LinearAxis { Title = "Reflectance" };
            //var yAxis = new LinearAxis { Title = "Time", Position = AxisPosition.Bottom };
            //PlotModel.Axes.Add(xAxis);
            //PlotModel.Axes.Add(yAxis);
            PlotModel.InvalidatePlot(true);
        }
    }
}
