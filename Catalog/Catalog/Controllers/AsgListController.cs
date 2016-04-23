using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Catalog.Controllers
{
  public class AsgListController : Controller
  {
    // GET: AssignmentList
    public ActionResult Index(int page = 1)
    {
      ViewBag.Page = page;

      var skipPages = (page - 1)*10;

      ViewBag.AsgList = Model.Repository.Model.Assignments.Skip(skipPages).Take(10).ToList();

      return View(Model.Repository.Model);
    }

    public ActionResult RegenerateList()
    {
      Model.Repository.ResetModel();

      return RedirectToAction("Index", "AsgList");
    }
  }
}