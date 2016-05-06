using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catalog.Model;

namespace Catalog.Controllers
{
  public class AsgListAjaxController : Controller
  {
    // GET: AsgListAjax
    [HttpGet]
    // id, Subject, Performer, Deadline, Status, Overdue
    public ActionResult GetAssignments(int page, string taskTypeGuid)
    {
      var skipPages = (page - 1) * 10;
      var assignments = new List<Assignment>();
      assignments = Model.Repository.Model.Assignments.ToList();
      var filteredAssignments = assignments;

      if (!string.IsNullOrEmpty(taskTypeGuid))
        filteredAssignments = filteredAssignments.Where(a => a.TaskTypeGuid == taskTypeGuid).ToList();

      if (filteredAssignments.Count > 0)
        return Json(filteredAssignments.Select(i => new { id = i.Id, text = i.Name, performer = i.Performer, deadline = i.Deadline, status = i.Status, overdue = i.Overdue}).ToList(), JsonRequestBehavior.AllowGet);
      else
        return null;
      //ViewBag.AllAsgListCount = (int)Math.Ceiling(filteredAssignments.Count / (double)10);
      //ViewBag.AsgList = filteredAssignments.Skip(skipPages).Take(10).ToList();
      //return View();
    }
  }
}