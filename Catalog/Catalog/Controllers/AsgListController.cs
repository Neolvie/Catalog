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
    public ActionResult Index()
    {
      return View(Model.Repository.Model);
    }

    public ActionResult RegenerateList()
    {
      Model.Repository.ResetModel();

      return RedirectToAction("Index", "AsgList");
    }
  }
}