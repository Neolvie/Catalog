using System;
using System.Collections.Generic;

namespace Catalog.Model
{
  public static class CalendarEx
  {
    /// <summary>
    /// Серия периода.
    /// </summary>
    public class SeriePeriod
    {
      // Начало периода серии.
      public DateTime PeriodBegin { get; set; }

      // Конец периода серии.
      public DateTime PeriodEnd { get; set; }

      // Название серии.
      public string Name { get; set; }

      #region Конструктор

      public SeriePeriod(string name, DateTime periodBegin, DateTime periodEnd)
      {
        Name = name;
        PeriodBegin = periodBegin;
        PeriodEnd = periodEnd;
      }

      #endregion
    }

    private static DateTime cachedPeriodBegin;
    private static DateTime cachedPeriodEnd;
    private static List<SeriePeriod> cachedSeriePeriods;

    /// <summary>
    /// Типы периодизации.
    /// </summary>
    public enum StepType
    {
      Days,
      Month,
      Quarter,
      Year
    }

    /// <summary>
    /// Разбить период на серии.
    /// </summary>
    /// <param name="periodBegin">Начало периода.</param>
    /// <param name="periodEnd">Конец периода.</param>
    /// <returns>Серии периода.</returns>
    public static List<SeriePeriod> GetSeriePeriods(DateTime periodBegin, DateTime periodEnd)
    {
      // Если есть кешированная разбивка за такой-же период - вернуть её.
      if (cachedSeriePeriods != null && cachedPeriodBegin == periodBegin && cachedPeriodEnd == periodEnd)
        return cachedSeriePeriods;

      var days = (int)(periodEnd - periodBegin).TotalDays;
      var stepType = StepType.Days;
      var step = -1;

      if (days <= 14)       // Меньше 2-х недель.
        step = 1;
      else if (days <= 31)  // Меньше месяца.
        step = 3;
      else if (days <= 61)  // Меньше 2-х месяцев.
        step = 7;
      else if (days <= 92)  // Меньше 3-х месяцев.
        step = 10;
      else if (days <= 183) // Меньше 6 месяцев.
        step = 14;
      else if (days <= 366) // Меньше года.
      {
        step = 1;
        stepType = StepType.Month;
      }
      else if (days <= 1096) // Меньше 3-х лет.
      {
        step = 3;
        stepType = StepType.Quarter;
      }

      var result = new List<SeriePeriod>();


      if (step != -1)
        result = GetSeriesWithStep(step, stepType, periodBegin, periodEnd);
      else // Если не определен шаг, значит период - 1 год.
        result = GetSeriesWithStep(1, StepType.Year, periodBegin, periodEnd);

      // Кеширование разбивки, для минимизации последующих вычислений.
      cachedPeriodBegin = periodBegin;
      cachedPeriodEnd = periodEnd;
      cachedSeriePeriods = result;

      return result;
    }

    public static List<SeriePeriod> GetSeriePeriodsWithMaxPointInPeriod(int maxPoint, StepType stepType, DateTime periodBegin, DateTime periodEnd)
    {
      var step = (int)Math.Ceiling((periodEnd - periodBegin).TotalDays / maxPoint);
      return GetSeriesWithStep(step, stepType, periodBegin, periodEnd);
    }

    public static List<SeriePeriod> GetSeriesWithStep(int step, StepType stepType, DateTime periodBegin, DateTime periodEnd)
    {
      // Смещение периода относительно начала
      var date = periodBegin;
      var dateEnd = GetRealPeriodEnd(stepType, periodBegin);

      if (dateEnd > periodEnd)
        dateEnd = periodEnd;

      var series = new List<SeriePeriod>();

      while (dateEnd <= periodEnd)
      {
        var serie = new SeriePeriod(GetSerieName(stepType, date, dateEnd), date, dateEnd);
        series.Add(serie);

        if (dateEnd < periodEnd)
        {
          date = dateEnd.AddSeconds(1);

          switch (stepType)
          {
            case StepType.Days:
              dateEnd = dateEnd.AddDays(step); break;
            case StepType.Month:
              {
                dateEnd = dateEnd.AddMonths(step);
                // Магия для корректировки последнего дня месяца.
                var nextMonth = dateEnd.AddMonths(1);
                dateEnd = nextMonth.AddDays(-nextMonth.Day);
                break;
              }
            case StepType.Quarter:
              {
                dateEnd = dateEnd.AddMonths(step);
                // Магия для корректировки последнего дня месяца.
                var nextMonth = dateEnd.AddMonths(1);
                dateEnd = nextMonth.AddDays(-nextMonth.Day);
                break;
              }
            case StepType.Year:
              {
                dateEnd = dateEnd.AddYears(step);
                break;
              }
            default: dateEnd = dateEnd.AddDays(step); break;
          }

          if (dateEnd > periodEnd)
            dateEnd = periodEnd;
        }
        else
          break;
      }

      return series;
    }

    /// <summary>
    /// Определить конец периода. 
    /// </summary>
    /// <param name="stepType">Тип периодизации.</param>
    /// <param name="date">Дата.</param>
    /// <returns>Дата окончания периода.</returns>
    private static DateTime GetRealPeriodEnd(StepType stepType, DateTime date)
    {
      switch (stepType)
      {
        case StepType.Days:
          return date.AddDays(1).AddSeconds(-1);
        case StepType.Month:
          return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddSeconds(-1);
        case StepType.Quarter:
          {
            var quarter = (int)Math.Ceiling(date.Month / (double)3);

            return new DateTime(date.Year, 1 + (3 * (quarter - 1)), 1).AddMonths(3).AddSeconds(-1);
          }
        case StepType.Year:
          return new DateTime(date.Year, 1, 1).AddYears(1).AddSeconds(-1);
        default:
          return date.AddDays(1).AddSeconds(-1);
      }
    }

    /// <summary>
    /// Получить название серии.
    /// </summary>
    /// <param name="stepType">Тип периодизации.</param>
    /// <param name="date">Дата - для формирования периода.</param>
    /// <param name="dateEnd">Дата - основа для формирования названия.</param>
    /// <returns>Название серии.</returns>
    private static string GetSerieName(StepType stepType, DateTime date, DateTime dateEnd)
    {
      switch (stepType)
      {
        case StepType.Days:
          return date.Date == dateEnd.Date ? dateEnd.ToString("dd.MM.yy") :
            string.Format("  {0} -\r\n  {1}  ", date.ToString("dd.MM.yy"), dateEnd.ToString("dd.MM.yy"));
        case StepType.Month:
          return dateEnd.ToString("MMMM yyyy");
        case StepType.Quarter:
          {
            var quarter = (int)Math.Ceiling(dateEnd.Month / (double)3);
            string quarterName;
            switch (quarter)
            {
              case 1:
                quarterName = "I"; break;
              case 2:
                quarterName = "II"; break;
              case 3:
                quarterName = "III"; break;
              case 4:
                quarterName = "IV"; break;
              default:
                quarterName = string.Empty; break;
            }

            return string.Format("{0} квартал", quarterName, dateEnd.Year);
          }
        case StepType.Year:
          return dateEnd.Year.ToString();
        default:
          return dateEnd.ToString("dd.MM.yy");
      }
    }
  }
}
