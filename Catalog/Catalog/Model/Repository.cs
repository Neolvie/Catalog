using System;
using System.Collections.Generic;
using System.Linq;

namespace Catalog.Model
{
  public class Repository
  {
    public static Dictionary<string, string> TaskTypes;

    public static List<string> TaskTypeGuids;

    private static Model _model;

    public static Model Model => _model ?? (_model = GetMockModel(10, 50));

    public static Model GetMockModel(int performersCount = 10, int assignmentsCount = 20)
    {
      var model = new Model();

      for (var i = 0; i < performersCount; ++i)
      {
        var performer = GetMockPerformer();

        while (model.Performers.Any(x => x.Id == performer.Id))
          performer = GetMockPerformer();

        model.Performers.Add(GetMockPerformer());
      }

      for (var i = 0; i < assignmentsCount; ++i)
      {
        var asg = GetMockAssignment(model.Performers);

        while (model.Assignments.Any(x => x.Id == asg.Id))
          asg = GetMockAssignment(model.Performers);

        model.Assignments.Add(asg);
      }

      return model;
    }

    public static Assignment GetMockAssignment(List<Performer> performers)
    {
      var asg = new Assignment();

      asg.Created = DateTime.Now.AddDays(-new Random().Next(30));
      asg.Deadline = asg.Created.AddDays(new Random().Next(35));
      asg.Id = new Random().Next(100000);
      asg.InWork = Convert.ToBoolean(new Random().Next(1));
      asg.Modified = asg.Created.AddDays(new Random().Next(29));
      asg.PerformerId = performers[new Random().Next(performers.Count)].Id;
      asg.TaskId = new Random().Next(5000);
      asg.TaskTypeGuid = TaskTypeGuids[new Random().Next(TaskTypeGuids.Count)];
      asg.Name = $"{Guid.NewGuid()}, {Guid.NewGuid()}";

      return asg;
    }

    public static Performer GetMockPerformer()
    {
      var nameGuid = Guid.NewGuid().ToString();

      var performer = new Performer
      {
        Active = true,
        DepartmentId = new Random().Next(5000),
        DepartmentName = Guid.NewGuid().ToString().Substring(0, 8),
        Id = new Random().Next(10000),
        JobTitle = Guid.NewGuid().ToString().Substring(0, 10),
        Name = $"{nameGuid.Substring(0, new Random().Next(5, 9))} {nameGuid.Substring(0, 1)}.{nameGuid.Substring(2, 1)}."
      };

      return performer;
    }

    static Repository()
    {
      TaskTypes = new Dictionary<string, string>();

      TaskTypes.Add("100950d0-03d2-44f0-9e31-f9c8dfdf3829", "Задача на согласование по регламенту");
      TaskTypes.Add("77f43035-9f23-4a19-9882-5a6a2cd5c9c7", "Задача на свободное согласование");
      TaskTypes.Add("c290b098-12c7-487d-bb38-73e2c98f9789", "Задача на исполнение поручения");
      TaskTypes.Add("4ef03457-8b42-4239-a3c5-d4d05e61f0b6", "Задача на рассмотрение документа");

      TaskTypeGuids = TaskTypes.Select(x => x.Key).ToList();
    }
  }
}