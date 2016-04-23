using System;

namespace Catalog.Model
{
  public static class Helpers
  {
    public static int? CalculateDelay(DateTime? deadline, DateTime now)
    {
      if (!deadline.HasValue)
        return 0;

      var dif = Convert.ToInt32((now - deadline.Value).TotalHours);

      return dif < 0 ? 0 : dif;
    }
  }
}