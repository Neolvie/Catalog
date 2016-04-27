using System.Collections.Generic;

namespace Catalog.ViewModel.Primitives
{
  public class PersonaWithNumbers
  {
    public string PersonaName;
    public int OverdueAssignments;
    public int NotOverdueAssignments;
  }

  public class DatePoint
  {
    public string Name;
    public int Overdue;
    public int Total;

    public DatePoint(string name, int overdue, int total)
    {
      Name = name;
      Overdue = overdue;
      Total = total;
    }

    public DatePoint()
    {

    }
  }

  public class MultyYPoint
  {
    public string Name;
    public List<object> Values;

    public MultyYPoint(string name, object[] values)
    {
      this.Name = name;
      Values = new List<object>();
      foreach (var value in values)
        Values.Add(value);
    }
  }
}