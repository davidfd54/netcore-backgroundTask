using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkerMVC.Models;
using static WorkerMVC.Models.Entities;
using Microsoft.Extensions.Configuration;

namespace WorkerMVC.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; }

        private readonly ILogger<HomeController> _logger;


        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            List<oReadNameFile> oNameFile = new List<oReadNameFile>();

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = Configuration.GetConnectionString("Devconnection");
                con.Open();

                SqlCommand cmd = new SqlCommand("SP_READ_NAME_FILE", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    oReadNameFile input = new oReadNameFile();
                    input.NOMBRE = dr["NOMBRE"].ToString();
                    input.FECHA_INGRESO = DateTime.Parse(dr["FECHA_INGRESO"].ToString());

                    oNameFile.Add(input);
                }

                con.Close();
            }

            return View(oNameFile);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
