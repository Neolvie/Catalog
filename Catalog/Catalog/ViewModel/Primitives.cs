using System.Collections.Generic;
using DotNet.Highcharts.Options;

namespace Catalog.ViewModel.Primitives
{
  public class PersonaWithNumbers
  {
    public string PersonaName;
    public int OverdueAssignments;
    public int NotOverdueAssignments;
  }

  public class AssignmentsCountPoint
  {
    public string Name;
    public int Overdue;
    public int Total;
    public int InTime
    {
      get
      {
        return this.Total - this.Overdue;
      }
    }

    public AssignmentsCountPoint()
    {

    }

    public AssignmentsCountPoint(string name, int overdue, int total)
    {
      Name = name;
      Overdue = overdue;
      Total = total;
    }

    public Point GetOverduePoint()
    {
      return new Point()
      {
        Name = "Просроченные",
        Y = this.Overdue
      };
    }

    public Point GetInTimePoint()
    {
      return new Point()
      {
        Name = "В срок",
        Y = this.InTime
      };
    }

    public Point GetTotalPoint()
    {
      return new Point()
      {
        Name = "Общее количество",
        Y = this.Overdue
      };
    }
  }
}