using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;

namespace NLoad.App.Features.RunLoadTest
{
    public class LoadTestChart : PlotModel
    {
        public LoadTestChart(List<Heartbeat> heartbeats)
        {
            Background = OxyColors.Transparent;
            PlotAreaBorderThickness = new OxyThickness(1, 0, 0, 1);
            Padding = new OxyThickness(15, 30, 30, 20);
            LegendPosition = LegendPosition.LeftTop;
            LegendPlacement = LegendPlacement.Inside;

            Series.Add(new LineSeries
            {
                ItemsSource = heartbeats,
                DataFieldX = "TotalRuntime",
                DataFieldY = "TotalErrors",
                Color = OxyColors.Red,
                Title = "Errors",
                Smooth = false
            });

            Series.Add(new LineSeries
            {
                ItemsSource = heartbeats,
                DataFieldX = "TotalRuntime",
                DataFieldY = "TotalThreads",
                Color = OxyColors.Green,
                Title = "Threads",
                Smooth = false
            });

            Series.Add(new LineSeries
            {
                ItemsSource = heartbeats,
                DataFieldX = "TotalRuntime",
                DataFieldY = "Throughput",
                Color = OxyColors.DodgerBlue,
                Title = "Throughput",
                Smooth = false
            });
        }
    }
}
