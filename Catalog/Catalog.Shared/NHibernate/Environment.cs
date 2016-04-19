using System.Data.SqlClient;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Catalog.Shared.NHibernate
{
  public class Environment
  {
    private const string DefaultConnectionString = @"Data Source=NASTYA-NB\SQLEXPRESS;Initial Catalog=Catalog;User ID=admin;Password=11111";
    private static string _connectionString;

    private static ISessionFactory _sessionFactory;

    public static ISession Session;

    public static ISession OpenSession()
    {
      var session = _sessionFactory.OpenSession();
      session.FlushMode = FlushMode.Auto;
      return session;
    }

    public static bool Initialized { get; set; }

    public static void Initialize(string connectionString = null)
    {
      Initialized = false;

      _connectionString = !string.IsNullOrEmpty(connectionString) ? connectionString : DefaultConnectionString;

      CreateDatabaseIfNotExist(_connectionString);

      _sessionFactory = CreateSessionFactory();
      Session = OpenSession();

      Initialized = true;
    }

    static Environment()
    {    
      Initialize();
    }

    public static void Close()
    {
      if (_sessionFactory == null || _sessionFactory.IsClosed)
        return;

      _sessionFactory.Close();
    }

    private static ISessionFactory CreateSessionFactory()
    {
      return Fluently
        .Configure()
        .Database(MsSqlConfiguration.MsSql2012.ConnectionString(_connectionString))
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Environment>())
        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
        .BuildSessionFactory();
    }

    private static void CreateDatabaseIfNotExist(string connectionString)
    {
      var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      var databaseName = connectionStringBuilder.InitialCatalog;

      connectionStringBuilder.InitialCatalog = "master";

      using (var connection = new SqlConnection(connectionStringBuilder.ToString()))
      {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = string.Format("select * from master.dbo.sysdatabases where name = '{0}'", databaseName);
          using (var reader = command.ExecuteReader())
          {
            if (reader.HasRows)
              return;           
          }

          command.CommandText = string.Format("CREATE DATABASE {0}", databaseName);
          command.ExecuteNonQuery();
        }
      }
    }
  }
}