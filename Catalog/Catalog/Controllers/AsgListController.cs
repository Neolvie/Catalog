using System;
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
    public ActionResult Index(string taskTypeGuid = "", int page = 1)
    {
      return View();
    }

    // GET: AssignmentList
    public ActionResult GetAssignments(string taskTypeGuid = "", int page = 1)
    {
      var asgPerPage = 7;

      ViewBag.Page = page;
      if (!string.IsNullOrEmpty(taskTypeGuid))
        ViewBag.Subtitle = Model.Repository.TaskTypes[taskTypeGuid];

      var skipPages = (page - 1) * asgPerPage;
      var assignments = new List<Assignment>();
      assignments = Model.Repository.Model.Assignments.ToList();
      var filteredAssignments = assignments;

      if (!string.IsNullOrEmpty(taskTypeGuid))
        filteredAssignments = filteredAssignments.Where(a => a.TaskTypeGuid == taskTypeGuid).ToList();

      ViewBag.AllAsgListCount = (int)Math.Ceiling(filteredAssignments.Count / (double)asgPerPage);
      var pageAsgs = filteredAssignments.Skip(skipPages).Take(asgPerPage).ToList();
      ViewBag.AsgList = pageAsgs;
      ViewBag.TaskTypeGuid = taskTypeGuid;

      return PartialView("AsgList", Model.Repository.Model); //View(Model.Repository.Model);
    }

    public ActionResult GetAssignmentsPieChart()
    {
      var assignments = new List<Assignment>();
      assignments = Model.Repository.Model.Assignments.ToList();
      var chart = ViewModel.AssignmentsViewModel.GetAssignmentsByTypePieChart(assignments, this.ControllerContext);

      ViewBag.Chart = chart;
      return PartialView("AsgPieChart", Model.Repository.Model);
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