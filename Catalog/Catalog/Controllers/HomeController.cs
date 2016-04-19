using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catalog.Shared.Model;
using Catalog.ViewModel;

namespace Catalog.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }

    public ActionResult Create()
    {
      var category = new Category();
      category.Name = "Просто категория";
      category.Save();

      ViewBag.Message = $"Сущность \"{category.Name}\" (id {category.Id}) создана успешно";

      var model = new CategoriesViewModel();

      return View(model);
    }
  }
}