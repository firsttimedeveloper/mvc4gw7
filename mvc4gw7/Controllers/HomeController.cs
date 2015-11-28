using System.Web.Mvc;
using System.Data.SqlClient;

namespace mvc4gw7.Controllers
{  
    public class HomeController : Controller
    {
        //readonly string connectionString= @"Data Source=.\SQLEXPRESS; AttachDBFilename=D:\Maksim\Documents\ExampleSite\ASP.NET\Examples\mvc4\groundwork7(site)\mvc4gw7\App_Data\AppDB.mdf; Initial Catalog=AppDB; Integrated Security=true";
        readonly string connectionString = @"workstation id=AppMedia.mssql.somee.com;packet size=4096;user id=firsttime_SQLLogin_1;pwd=fe1zoshmsh;data source=AppMedia.mssql.somee.com;persist security info=False;initial catalog=AppMedia";            
        
        public ActionResult Index()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"insert into Visits ([UserName], [IP], [DateTime]) values (@UserName, @IP, GETDATE())", connection);
                    command.Parameters.AddWithValue("@UserName", "anonymous");
                    command.Parameters.AddWithValue("@IP", System.Web.HttpContext.Current.Request.UserHostAddress.ToString());
                    command.ExecuteNonQuery();
                }
                
                finally
                {
                    connection.Close();
                }
            }
            
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Products()
        {
            return View();
        }

        public ActionResult Contacts()
        {
            return View();
        }
    }
}
