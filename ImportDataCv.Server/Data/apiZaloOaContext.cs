using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using apiZaloOa.Models;

namespace apiZaloOa.Data
{
    public class apiZaloOaContext : DbContext
    {
        public apiZaloOaContext (DbContextOptions<apiZaloOaContext> options)
            : base(options)
        {
        }

        public DbSet<zaloOaTokenModel> zaloOaTokenModel { get; set; } = default!;
    }
}
