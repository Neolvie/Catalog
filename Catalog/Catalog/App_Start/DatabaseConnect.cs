using System.Configuration;

namespace Catalog
{
  public static class DatabaseConnect
  {
    public static void Initialize()
    {
      var conStr = ConfigurationManager.ConnectionStrings["DatabaseConnect"].ConnectionString;

      Shared.NHibernate.Environment.Initialize(conStr);
    }
  }
}