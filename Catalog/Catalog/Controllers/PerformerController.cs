using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catalog.Model;

namespace Catalog.Controllers
{
  public class PerformerController : Controller
  {
    // GET: Performer
    [HttpGet]
    public ActionResult Index(int? id)
    {
      Performer performer = null;

      if (id.HasValue)
        performer = Model.Repository.Model.Performers.FirstOrDefault(x => x.Id == id);

      return View(performer);
    }
  }
}