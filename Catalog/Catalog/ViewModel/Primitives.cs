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
}