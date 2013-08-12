using System.Data.Entity;

namespace Silas.Server.DB
{
    public class ForecastContextInitializer : DropCreateDatabaseAlways<ForecastContext>
    {
    }
}