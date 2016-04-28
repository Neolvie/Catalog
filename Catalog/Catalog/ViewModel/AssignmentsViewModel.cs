using System;
using System.Collections.Generic;
using Catalog.Model;
using System.Linq;
using System.Web;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using Catalog.Helpers;
using System.Web.Mvc;

namespace Catalog.ViewModel
{
  public class AssignmentsViewModel
  {
    public static Highcharts GetAssignmentsByTypePieChart(IEnumerable<Assignment> assignments, ControllerContext context)
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
                    DataLabels = new PlotOptionsPieDataLabels { Enabled = true, Formatter = "function() { return '<b>'+ this.y +'</b>'; }" },
                    ShowInLegend = true
                  }
                })
                .SetSeries(new Series
                {
                  Type = ChartTypes.Pie,
                  Name = "Типы задач",
                  Data = new Data(GetSeries(assignments, context))
                });

      return chart;
    }

    private static Point[] GetSeries(IEnumerable<Assignment> assignments, ControllerContext context)
    {
      var i = 0;
      var group = assignments.GroupBy(x => x.TaskTypeGuid)
        .OrderByDescending(x => x.Count())
        .Select(x => new Point
        {
          Name = Model.Repository.TaskTypes[x.Key],
          Y = x.Count(),
          Color = ChartColors.GetByIndex(i++),
          Events = new PlotOptionsSeriesPointEvents()
          {
            Click = "function() {window.location.href = \""
            + new UrlHelper(context.RequestContext).Action("Index", "AsgList", new { taskTypeGuid = x.Key })
              + "\"}"
          }
        }).ToArray();

      return group;
    }
  }
}