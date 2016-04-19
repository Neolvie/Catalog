using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Catalog
{
  public static class Repository
  {
    public static IQueryable<T> Get<T>() where T : Catalog.Shared.Model.Base.Entity
    {
      return Shared.NHibernate.Environment.Session.Query<T>();
    }

    public static void SaveAll<T>(this IEnumerable<T> objects) where T : Catalog.Shared.Model.Base.Entity
    {
      var objectList = objects.ToList();
      if (!objectList.Any())
        return;

      var session = Shared.NHibernate.Environment.Session;
      using (var transact = session.BeginTransaction())
      {
        try
        {
          foreach (var entity in objectList)
          {
            if (entity.Id == 0)
              session.Save(entity);
            else
              session.Update(entity);
          }

          transact.Commit();
        }
        catch (Exception)
        {
          transact.Rollback();
          throw;
        }
      }
    }

    public static void Save<T>(this T entity) where T : Catalog.Shared.Model.Base.Entity
    {
      SaveAll(new [] {entity});
    }
  }
}