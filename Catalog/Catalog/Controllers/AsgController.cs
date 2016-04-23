using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catalog.Model;

namespace Catalog.Controllers
{
  public class AsgController : Controller
  {
    // GET: Asg
    public ActionResult Index(int? id)
    {
      Assignment asg = null;

      if (id.HasValue)
        asg = Model.Repository.Model.Assignments.FirstOrDefault(x => x.Id == id);

      return View(asg);
    }
  }
}