using System;
using System.Linq;

namespace Catalog.Model
{
  public class Assignment
  {
    public DateTime Created { get; set; }
    public DateTime? Deadline { get; set; }

    public DateTime? FactDeadline { get; set; }

    public bool HasOverdue => Overdue != 0;

    public int Id { get; set; }
    public bool InWork { get; set; }
    public DateTime? Modified { get; set; }
    public string Name { get; set; }

    private int? _overdue;

    public int Overdue
    {
      get
      {
        if (_overdue == null)
        {
          _overdue = InWork
            ? Helpers.CalculateDelay(Deadline, DateTime.Now)
            : Helpers.CalculateDelay(Deadline, Modified.Value);
        }

        return _overdue.Value;
      }
    }

    private Performer _performer;

    public Performer Performer {
      get
      {
        if (_performer == null)
          _performer = Repository.Model.Performers.Single(p => p.Id == this.PerformerId);

        return _performer;
      }
    }
    public int PerformerId { get; set; }

    public string Status => InWork ? "В работе" : "Выполнено";

    public int TaskId { get; set; }
    public string TaskTypeGuid { get; set; }

    private string _taskTypeName;

    public string TaskTypeName {
      get
      {
        if (string.IsNullOrEmpty(_taskTypeName))
          _taskTypeName = Repository.TaskTypes[TaskTypeGuid];

        return _taskTypeName;
      }
    }

    public bool HasOverdueOnDate(DateTime date)
    {
      // Если нет срока - нет просрочки.
      if (!Deadline.HasValue)
        return false;

      // Если проверяемая дата до дедлайна - просрочки нет.
      if (date <= Deadline)
        return false;

      var nextDay = Deadline.Value.AddDays(1);

      // Хитрым способом считаем дату фактической просрочки без AddWorkDays.
      while (FactDeadline == null)
      {
        var delay = Helpers.CalculateDelay(Deadline, nextDay);

        if (delay != 0)
          FactDeadline = nextDay;

        nextDay = nextDay.AddDays(1);
      }

      // Если задание в работе, считаем дедлайн на проверяемую дату относительно фактической даты просрочки.
      if (InWork)
        return FactDeadline < date;

      // Если на проверяемую дату было еще в работе, считаем дедлайн по фактической дате просрочки. Можно объединить с предыдущем, но оставил для понятности.
      if (Modified > date)
        return FactDeadline < date;

      // Если выполнено до проверяемой даты, берем посчитанную просрочку.
      return HasOverdue;
    }
  }
}