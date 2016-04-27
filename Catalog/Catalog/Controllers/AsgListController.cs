﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catalog.Model;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using Point = DotNet.Highcharts.Options.Point;

namespace Catalog.Controllers
{
  public class AsgListController : Controller
  {
    // GET: AssignmentList
    public ActionResult Index(string taskTypeGuid = "", int page = 1)
    {
      ViewBag.Page = page;
      if (!string.IsNullOrEmpty(taskTypeGuid))
        ViewBag.Subtitle = Model.Repository.TaskTypes[taskTypeGuid];

      var skipPages = (page - 1) * 10;
      var assignments = new List<Assignment>();
      assignments = Model.Repository.Model.Assignments.ToList();
      var filteredAssignments = assignments;

      if (!string.IsNullOrEmpty(taskTypeGuid))
        filteredAssignments = filteredAssignments.Where(a => a.TaskTypeGuid == taskTypeGuid).ToList();

      ViewBag.AllAsgListCount = (int)Math.Ceiling(filteredAssignments.Count / (double)10);
      ViewBag.AsgList = filteredAssignments.Skip(skipPages).Take(10).ToList();
      ViewBag.TaskTypeGuid = taskTypeGuid;

      var chart = new Highcharts("chart")
                .InitChart(new Chart { PlotShadow = false, PlotBackgroundColor = null, PlotBorderWidth = null })
                .SetTitle(new Title { Text = "Задания в разрезе типов задач", Align = HorizontalAligns.Left })
                .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.y; }" })
                .SetLegend(new Legend() { ItemStyle = "fontWeight: 'normal'" })
                .SetPlotOptions(new PlotOptions
                {
                  Pie = new PlotOptionsPie
                  {
                    AllowPointSelect = true,
                    Cursor = Cursors.Pointer,
                    DataLabels = new PlotOptionsPieDataLabels { Enabled = true, Formatter = "function() { return '<b>'+ this.y +'</b>'; }" },
                    ShowInLegend = true,
                  }
                })
                .SetSeries(new Series
                {                 
                  Type = ChartTypes.Pie,
                  Name = "Типы задач",
                  Data = new Data(GetSeries(assignments))
                });

      ViewBag.Chart = chart;

      return View(Model.Repository.Model);
}

private static Point[] GetSeries(IEnumerable<Assignment> assignments)
{
  var group = assignments.GroupBy(x => x.TaskTypeName)
    .OrderByDescending(x => x.Count())
    .Select(x => new Point { Name = x.Key, Y = x.Count() }).ToArray();

  return group;
}

public ActionResult RegenerateList()
{
  Model.Repository.ResetModel();

  return RedirectToAction("Index", "AsgList");
}

public ActionResult GetRandomAsg()
{
  var asg = Model.Repository.Model.Assignments[new Random().Next(Model.Repository.Model.Assignments.Count)];

  return Json(asg, JsonRequestBehavior.AllowGet);
}
  }
}