using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scpmtf_webapi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<SCPObject> SCPs { get; set; }
        public DbSet<ScpGPT3Summary> Summaries { get; set; }
    }
}
