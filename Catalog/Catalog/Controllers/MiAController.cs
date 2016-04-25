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
      var performers = sortedListOfAssignments.Select(s => s.PersonaName).ToArray();
      var overdueData = sortedListOfAssignments.Select(s => new Point()
      {
        Name = "Просроченные",
        Y = s.OverdueAssignments
      }).ToArray();
      var notOverdueData = sortedListOfAssignments.Select(s => new Point()
      {
        Name = "В срок",
        Y = s.NotOverdueAssignments
      }).ToArray();

      var chart = new Highcharts("AssignmentsPerPersonChart")
                .InitChart(new Chart { Type = ChartTypes.Bar, PlotShadow = false, PlotBackgroundColor = null, PlotBorderWidth = null, MarginTop = 50 })
                .SetExporting(new Exporting() { Enabled = false })
                .SetTitle(new Title { Text = "", Align = HorizontalAligns.Left })
                .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.y; }" })
                .SetLegend(new Legend { ItemStyle = "fontWeight: 'normal'" })
                .SetPlotOptions(new PlotOptions
                {
                  Bar = new PlotOptionsBar
                  {
                    AllowPointSelect = true,
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
                  new Series { Type = ChartTypes.Bar, Name = "Просроченные", Data = new Data(overdueData) },
                  new Series { Type = ChartTypes.Bar, Name = "В срок", Data = new Data(notOverdueData) },
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

    private Highcharts GetAssignmentPlot(List<Assignment> assignments)
    {
      var dateBegin = DateTime.Now.AddDays(-30);
      var dateEnd = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

      var assignmentsByDates = new List<DatePoint>();

      foreach (var serie in CalendarEx.GetSeriePeriodsWithMaxPointInPeriod(100, CalendarEx.StepType.Days, dateBegin, dateEnd))
      {
        var datePoint = new DatePoint { Name = serie.PeriodEnd.ToString(CultureInfo.CurrentCulture) };

        var dateAssignments = Helpers.FilterAssignmentsForPeriodWithActive(assignments, serie.PeriodBegin, serie.PeriodEnd);
        datePoint.Total = dateAssignments.Count;

        var overduedateAssignments = dateAssignments.Where(a => a.HasOverdueOnDate(serie.PeriodEnd)).ToList();
        datePoint.Overdue = overduedateAssignments.Count;

        assignmentsByDates.Add(datePoint);
      }

      return null;
    }

    public class DatePoint
    {
      public string Name;
      public int Overdue;
      public int Total;

      public DatePoint(string name, int overdue, int total)
      {
        Name = name;
        Overdue = overdue;
        Total = total;
      }

      public DatePoint()
      {

      }
    }

    private List<ViewModel.Primitives.PersonaWithNumbers> GetAssignmentsByPerson(IEnumerable<Assignment> assignments)
    {
      return assignments
        .GroupBy(a => a.PerformerId)
        .OrderByDescending(x => x.Count())
        .Select(a => new ViewModel.Primitives.PersonaWithNumbers()
        {
          PersonaName = Model.Repository.Model.Performers.FirstOrDefault(p => p.Id == a.Key).Name,
          OverdueAssignments = a.Where(s => s.Overdue > 0).Count(),
          NotOverdueAssignments = a.Where(s => s.Overdue != 0).Count()
        })
        .Where(a => a.OverdueAssignments != 0 && a.NotOverdueAssignments != 0)
        .ToList();
    }
  }
}