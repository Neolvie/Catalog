using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catalog.Helpers;
using Catalog.Model;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;

namespace Catalog.Controllers
{
  public class MiAController : Controller
  {
    // GET: MiA
    public ActionResult Index()
    {
      ViewBag.DisciplineChart = GetPerformerDisciplineChart(Model.Repository.Model.Assignments);
      ViewBag.OverdueTasks = GetOverduedTasksChart(Model.Repository.Model.Assignments);
      ViewBag.AssignmentsPerPerson = GetAssignmentsPerPerson(Model.Repository.Model.Assignments);
      ViewBag.AssignmentsPlot = GetAssignmentsPlot(Model.Repository.Model.Assignments);

      return View();
    }

    private Highcharts GetOverduedTasksChart(IEnumerable<Assignment> assignments)
    {
      return ViewModel.AssignmentsViewModel.GetAssignmentsByTypePieChart(assignments, this.ControllerContext);
    }

    private Highcharts GetPerformerDisciplineChart(IEnumerable<Assignment> assignments)
    {
      var discipline = new Random().Next(1, 100);

      var chart = new Highcharts("PerformerDisciplineChart")
        .InitChart(new Chart
        {
          PlotShadow = false,
          PlotBackgroundColor = null,
          PlotBorderWidth = null,
          MarginTop = 0
        })
        .SetExporting(new Exporting() { Enabled = false })
        .SetTitle(new Title
        {
          Text = string.Format("{0}%", discipline),
          Align = HorizontalAligns.Center,
          VerticalAlign = VerticalAligns.Middle,
          Y = 8,
          Style = "fontSize: '36px', fontFamily: 'Arial'"
        })
        .SetTooltip(new Tooltip { Enabled = false })
        .SetPlotOptions(new PlotOptions
        {
          Pie = new PlotOptionsPie
          {
            AllowPointSelect = false,
            InnerSize = new PercentageOrPixel(50, true),
            Size = new PercentageOrPixel(75, true),
            Cursor = Cursors.Pointer,
            DataLabels = new PlotOptionsPieDataLabels { Enabled = false },
            ShowInLegend = false
          }
        })
        .SetSeries(new Series
        {
          Type = ChartTypes.Pie,
          Name = "Исполнительская дисциплина",
          Data = new Data(new[]
          {
            new Point() {Y = discipline, Color = ChartColors.Parse("#ffffae18") },
            new Point() {Y = 100 - discipline, Color = ChartColors.Parse("#ffe0e0e0") }
          })
        });

      return chart;
    }

    private Highcharts GetAssignmentsPerPerson(IEnumerable<Assignment> assignments)
    {
      var sortedListOfAssignments = assignments
        .GroupBy(a => a.PerformerId)
        .OrderByDescending(x => x.Count()).Take(10)
        .Select(a => new ViewModel.Primitives.AssignmentsCountPoint(Model.Repository.Model.Performers.First(p => p.Id == a.Key).Name, a.Count(s => s.Overdue > 0), a.Count()))
        .Where(p => p.Overdue != 0 || p.InTime != 0)
        .ToList(); ;
      var performers = sortedListOfAssignments.Select(s => s.Name).ToArray();
      var overdueData = sortedListOfAssignments.Select(s => s.GetOverduePoint()).ToArray();
      var notOverdueData = sortedListOfAssignments.Select(s => s.GetInTimePoint()).ToArray();

      var chart = new Highcharts("AssignmentsPerPersonChart")

                .InitChart(new Chart { Type = ChartTypes.Bar, PlotShadow = false, PlotBackgroundColor = null, PlotBorderWidth = null, Height = 800 })
                .SetExporting(new Exporting() { Enabled = false })
                .SetTitle(new Title { Text = "", Align = HorizontalAligns.Left })
                .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.y; }" })
                .SetLegend(new Legend { ItemStyle = "fontWeight: 'normal'" })
                .SetPlotOptions(new PlotOptions
                {
                  Bar = new PlotOptionsBar
                  {
                    AllowPointSelect = false,
                    Cursor = Cursors.Pointer,
                    ShowInLegend = true,
                    Stacking = Stackings.Normal
                  }
                })
                .SetXAxis(new XAxis
                {
                  Categories = performers
                })
                .SetSeries(new Series[]
                {
                  new Series { Type = ChartTypes.Bar, Name = "В срок", Data = new Data(notOverdueData), Color = ChartColors.Color3 },
                  new Series { Type = ChartTypes.Bar, Name = "Просроченные", Data = new Data(overdueData), Color = ChartColors.Red }
                });

      return chart;
    }

    private Highcharts GetAssignmentsPlot(IEnumerable<Assignment> assignments)
    {
      var dateBegin = DateTime.Now.Date.AddDays(-30);
      var dateEnd = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

      var assignmentsByDates = new List<ViewModel.Primitives.AssignmentsCountPoint>();

      foreach (var serie in CalendarEx.GetSeriePeriodsWithMaxPointInPeriod(100, CalendarEx.StepType.Days, dateBegin, dateEnd))
      {
        var datePoint = new ViewModel.Primitives.AssignmentsCountPoint { Name = serie.PeriodEnd.ToString("d",CultureInfo.CurrentCulture) };

        var dateAssignments = Model.Helpers.FilterAssignmentsForPeriodWithActive(assignments.ToList(), serie.PeriodBegin, serie.PeriodEnd);
        datePoint.Total = dateAssignments.Count;

        var overduedateAssignments = dateAssignments.Where(a => a.HasOverdueOnDate(serie.PeriodEnd)).ToList();
        datePoint.Overdue = overduedateAssignments.Count;

        assignmentsByDates.Add(datePoint);
      }

      var overdueAssignments = assignmentsByDates.Select(a => new Point() { Name = a.Name, Y = a.Overdue }).ToArray();
      var totalAssignments = assignmentsByDates.Select(a => new Point() { Name = a.Name, Y = a.Total }).ToArray();

      var chart = new Highcharts("AssignmentsPlot")
                .InitChart(new Chart { Type = ChartTypes.Spline, PlotShadow = false, PlotBackgroundColor = null, PlotBorderWidth = null })
                .SetExporting(new Exporting() { Enabled = false })
                .SetTitle(new Title { Text = "", Align = HorizontalAligns.Left })
                .SetTooltip(new Tooltip { Shared = true })
                .SetLegend(new Legend { ItemStyle = "fontWeight: 'normal'" })
                .SetPlotOptions(new PlotOptions
                {
                  Line = new PlotOptionsLine
                  {
                    AllowPointSelect = true,
                    Cursor = Cursors.Pointer,                    
                    ShowInLegend = true
                  }
                })
                .SetYAxis(new YAxis()
                {
                  Min = 0,
                  Title = new YAxisTitle() { Text = "Задания" }
                })
                .SetXAxis(new XAxis
                {
                  Type = AxisTypes.Datetime,
                  DateTimeLabelFormats = new DateTimeLabel() { Day = "%d.%m.%Y", Week = "%d.%m.%Y", Month = "%d.%m.%Y" }
                })
                .SetSeries(new []
                {
                  new Series
                  {
                    Type = ChartTypes.Line,
                    Name = "Общее количество",
                    Data = new Data(totalAssignments),
                    Color = ChartColors.Color1,
                    PlotOptionsSeries = new PlotOptionsSeries()
                    {
                      PointStart = new PointStart(DateTime.Now.AddDays(-30)),
                      PointInterval = 24 * 3600 * 1000 // Один день в миллисекундах.
                    }
                  },
                  new Series
                  {
                    Type = ChartTypes.Line,
                    Name = "Просроченные",
                    Data = new Data(overdueAssignments),
                    Color = ChartColors.Red,
                    PlotOptionsSeries = new PlotOptionsSeries()
                    {
                      PointStart = new PointStart(DateTime.Now.AddDays(-30)),
                      PointInterval = 24 * 3600 * 1000 // Один день в миллисекундах.
                    }
                  },             
                });


      return chart;
    }
  }
}