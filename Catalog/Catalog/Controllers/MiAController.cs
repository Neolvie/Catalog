﻿using System;
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
                    ShowInLegend = true,
                    //Events = new PlotOptionsPieEvents { Click = "function() {window.location.href = \""+ new UrlHelper(this.ControllerContext.RequestContext).Action("Index", "AsgList", new { taskTypeGuid = Model.Repository.TaskTypeGuids[0]}) +"\"}" , }
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
  }
}