using System;
using System.Collections.Generic;
using System.Linq;

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

    public static List<Assignment> FilterAssignmentsForPeriodWithActive(List<Assignment> assignments, DateTime date, DateTime dateEnd)
    {
      return assignments.Where(x => (x.Modified >= date && x.Modified <= dateEnd) ||
          (x.Deadline != null && (x.Deadline <= dateEnd && !(!x.InWork && x.Modified < date))) ||
          (x.InWork && x.Created < dateEnd)).ToList();
    }
  }
}