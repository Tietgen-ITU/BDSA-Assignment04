using System;
using System.IO;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Assignment4
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = LoadConfiguration();
            var connectionString = configuration.GetConnectionString("Connectionstring");

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseNpgsql(connectionString);
        }

        static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>();

            return builder.Build();
        }
    }
}
