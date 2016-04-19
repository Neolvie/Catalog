using System.Collections.Generic;
using Catalog.Shared.Model;

namespace Catalog.ViewModel
{
  public class CategoriesViewModel
  {
    public List<Category> Categories; 

    public CategoriesViewModel()
    {
      Categories = new List<Category>();

      Categories.AddRange(Repository.Get<Category>());
    }
  }
}