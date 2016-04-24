﻿using System;
using System.Collections.Generic;
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
                .InitChart(new Chart { PlotShadow = false, PlotBackgroundColor = null, PlotBorderWidth = null })
                .SetExporting(new Exporting() { Enabled = false })
                .SetTitle(new Title { Text = "Задания c нарушением срока", Align = HorizontalAligns.Left })
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
                  MarginTop = 0,
                  Events = new ChartEvents() { Load = "SetCenterText" }
                })
                .SetExporting(new Exporting() { Enabled = false })
                .SetTitle(new Title { Text = "Исполнительская дисциплина", Align = HorizontalAligns.Left })
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
                    new Point() { Y = discipline },
                    new Point() { Y = 100 - discipline }
                  })
                })
                .AddJavascripFunction("SetCenterText", string.Format(@"
                                                        var textX = this.plotLeft + (this.plotWidth  * 0.5);
                                                        var textY = this.plotTop + (this.plotHeight * 0.5);

                                                        var span = '<span id=""pieChartInfoText"" style=""position: relative; text-align: center;"">';
                                                        span += '<span style=""font-size: 48px; font-family: Arial"">{0}%</span><br>';
                                                        span += '</span>';

                                                        $('#addText').append(span);
                                                        span = $('#pieChartInfoText');
                                                        span.css('left', textX + (span.width() * -0.5));
                                                        span.css('top', textY + (span.height() * -0.5));", discipline));

      return chart;
    }

    private static Point[] GetSeries(IEnumerable<Assignment> assignments)
    {
      var group = assignments.GroupBy(x => x.TaskTypeName)
        .OrderByDescending(x => x.Count())
        .Select(x => new Point { Name = x.Key, Y = x.Count() }).ToArray();

      return group;
    }
  }
}