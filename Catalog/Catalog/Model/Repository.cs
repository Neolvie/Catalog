using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Catalog.Model
{
  public class Repository
  {
    public static Dictionary<string, string> TaskTypes;

    public static List<string> TaskTypeGuids;

    private static Model _model;

    public static Model Model => _model ?? (_model = GetMockModel(11, 70));

    public static Model GetMockModel(int performersCount, int assignmentsCount)
    {
      var model = new Model();

      for (var i = 0; i < performersCount; ++i)
      {
        var performer = GetMockPerformer();

        while (model.Performers.Any(x => x.Id == performer.Id))
          performer = GetMockPerformer();

        model.Performers.Add(performer);
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

      using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
      {
        byte[] data = new byte[4];

        rng.GetBytes(data);
        var randomNumber = Math.Abs(BitConverter.ToInt32(data, 0));
        asg.Created = DateTime.Now.AddDays(-randomNumber % 30);

        rng.GetBytes(data);
        randomNumber = Math.Abs(BitConverter.ToInt32(data, 0));
        int[] deadline = {7, 14};
        asg.Deadline = asg.Created.AddDays(deadline[randomNumber % 2]);

        rng.GetBytes(data);
        randomNumber = Math.Abs(BitConverter.ToInt32(data, 0));
        asg.Id = randomNumber % 100000;

        rng.GetBytes(data);
        randomNumber = Math.Abs(BitConverter.ToInt32(data, 0));
        asg.InWork = Convert.ToBoolean(randomNumber % 2);

        rng.GetBytes(data);
        randomNumber = Math.Abs(BitConverter.ToInt32(data, 0));
        asg.Modified = asg.Created.AddDays(randomNumber % 20);

        rng.GetBytes(data);
        randomNumber = Math.Abs(BitConverter.ToInt32(data, 0));
        asg.PerformerId = performers[randomNumber % performers.Count].Id;

        rng.GetBytes(data);
        randomNumber = Math.Abs(BitConverter.ToInt32(data, 0));
        asg.TaskId = randomNumber % 5000;

        rng.GetBytes(data);
        randomNumber = Math.Abs(BitConverter.ToInt32(data, 0));
        asg.TaskTypeGuid = TaskTypeGuids[randomNumber % TaskTypeGuids.Count];

        asg.Name = $"{Guid.NewGuid()}, {Guid.NewGuid()}";
      }
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
      TaskTypes.Add("0E1FF429-C11D-4140-9DF1-10717CF0E9A7", "Простая задача"); // Этот гуид сгенерил студией. Никакого отношения к простой задаче он не имеет.

      TaskTypeGuids = TaskTypes.Select(x => x.Key).ToList();
    }

    public static void ResetModel()
    {
      _model = null;
    }
  }
}