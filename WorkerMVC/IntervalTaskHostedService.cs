using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace WorkerMVC
{
    public class IntervalTaskHostedService : IHostedService, IDisposable
    {
        public IConfiguration Configuration { get; }
        private Timer _timer;

        public IntervalTaskHostedService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SaveFile, null, TimeSpan.Zero, TimeSpan.FromSeconds(8));

            return Task.CompletedTask;
        }

        public void SaveFile(object state)
        {
            string nameFile = "a" + new Random().Next(1000) + ".txt";
            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp",nameFile);
            //File.Create(path);

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = Configuration.GetConnectionString("Devconnection");
                con.Open();

                SqlCommand cmd = new SqlCommand("SP_CREATE_NAME_FILE", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("NOMBRE", nameFile));

                cmd.ExecuteNonQuery();

                con.Close();
            }
                

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }


        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
