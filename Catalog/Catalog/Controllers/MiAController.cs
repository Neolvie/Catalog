using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
      var chart = new Highcharts("OverdueChart")
                .InitChart(new Chart { PlotShadow = false, PlotBackgroundColor = null, PlotBorderWidth = null, MarginTop = 50 })
                .SetExporting(new Exporting() { Enabled = false })
                .SetTitle(new Title { Text = "", Align = HorizontalAligns.Left })
                .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.y; }" })
                .SetLegend(new Legend { ItemStyle = "fontWeight: 'normal'" })
                .SetPlotOptions(new PlotOptions
                {
                  Pie = new PlotOptionsPie
                  {
                    AllowPointSelect = true,
                    Cursor = Cursors.Pointer,
                    DataLabels = new PlotOptionsPieDataLabels { Enabled = false },
                    ShowInLegend = true
                  }
                })
                .SetSeries(new Series
                {
                  Type = ChartTypes.Pie,
                  Name = "Типы задач",
                  Data = new Data(GetSeries(assignments))
                });

      return chart;
    }

    private Highcharts GetPerformerDisciplineChart(IEnumerable<Assignment> assignments)
    {
      var discipline = 66;

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
            new Point() {Y = discipline},
            new Point() {Y = 100 - discipline}
          })
        });

      return chart;
    }

    private Highcharts GetAssignmentsPerPerson(IEnumerable<Assignment> assignments)
    {
      var sortedListOfAssignments = GetAssignmentsByPerson(assignments);
      var performers = sortedListOfAssignments.Select(s => s.Name).ToArray();
      var overdueData = sortedListOfAssignments.Select(s => new Point()
      {
        Name = "Просроченные",
        Y = (int)s.Values[0]
      }).ToArray();
      var notOverdueData = sortedListOfAssignments.Select(s => new Point()
      {
        Name = "В срок",
        Y = (int)s.Values[1]
      }).ToArray();

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
                  new Series { Type = ChartTypes.Bar, Name = "В срок", Data = new Data(notOverdueData) },
                  new Series { Type = ChartTypes.Bar, Name = "Просроченные", Data = new Data(overdueData) },
                });

      return chart;
    }

    private Highcharts GetAssignmentsPlot(IEnumerable<Assignment> assignments)
    {
      var dateBegin = DateTime.Now.Date.AddDays(-30);
      var dateEnd = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

      var assignmentsByDates = new List<ViewModel.Primitives.DatePoint>();

      foreach (var serie in CalendarEx.GetSeriePeriodsWithMaxPointInPeriod(100, CalendarEx.StepType.Days, dateBegin, dateEnd))
      {
        var datePoint = new ViewModel.Primitives.DatePoint { Name = serie.PeriodEnd.ToString("d",CultureInfo.CurrentCulture) };

        var dateAssignments = Helpers.FilterAssignmentsForPeriodWithActive(assignments.ToList(), serie.PeriodBegin, serie.PeriodEnd);
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
                    PlotOptionsSeries = new PlotOptionsSeries()
                    {
                      PointStart = new PointStart(DateTime.Now.AddDays(-30)),
                      PointInterval = 24 * 3600 * 1000 // Один день в миллисекундах.
                    }
                  },             
                });


      return chart;
    }

    private Point[] GetSeries(IEnumerable<Assignment> assignments)
    {
      var group = assignments.GroupBy(x => x.TaskTypeGuid)
        .OrderByDescending(x => x.Count())
        .Select(x => new Point
        {
          Name = Model.Repository.TaskTypes[x.Key],
          Y = x.Count(),
          Events = new PlotOptionsSeriesPointEvents()
          {
            Click = "function() {window.location.href = \""
            + new UrlHelper(this.ControllerContext.RequestContext).Action("Index", "AsgList", new { taskTypeGuid = x.Key })
              + "\"}"
          }
        }).ToArray();

      return group;
    }

    private List<ViewModel.Primitives.MultyYPoint> GetAssignmentsByPerson(IEnumerable<Assignment> assignments)
    {
      return assignments
        .GroupBy(a => a.PerformerId)
        .OrderByDescending(x => x.Count()).Take(10)
        .Select(a => new ViewModel.Primitives.MultyYPoint(Model.Repository.Model.Performers.First(p => p.Id == a.Key).Name, new object[] { a.Count(s => s.Overdue > 0), a.Count(s => s.Overdue == 0) }))
        .Where(a => (int)a.Values[0] != 0 || (int)a.Values[1] != 0)
        .ToList();
    }
  }
}