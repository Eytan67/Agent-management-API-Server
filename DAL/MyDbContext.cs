using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using AgentManagementAPIServer.Models;

namespace AgentManagementAPIServer.DAL
{
    public class MyDbContext :DbContext
    {

        public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }
        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new
                DbContextOptionsBuilder(), connectionString).Options;
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Target> Targets { get; set; }


    }
}
