using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using anu_mitkadmim_mamash_leat.Models;

namespace anu_mitkadmim_mamash_leat.Data
{
    public class anu_mitkadmim_mamash_leatContext : DbContext
    {
        public anu_mitkadmim_mamash_leatContext(DbContextOptions<anu_mitkadmim_mamash_leatContext> options)
            : base(options)
        {
        }

        public DbSet<Contact>? Contact { get; set; }
        public DbSet<Message>? Message { get; set; }
        public DbSet<User>? User { get; set; }
        //linq c#
    }
}
