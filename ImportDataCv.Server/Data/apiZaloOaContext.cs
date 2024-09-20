using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using apiZaloOa.Models;
using ImportDataCv.Server.Models;

namespace apiZaloOa.Data
{
    public class apiZaloOaContext : DbContext
    {
        public apiZaloOaContext (DbContextOptions<apiZaloOaContext> options)
            : base(options)
        {
        }

        public DbSet<zaloOaTokenModel> zaloOaTokenModel { get; set; } = default!;
        public DbSet<ZaloMessages> ZaloMessages { get; set; } = default!;
        public DbSet<ZaloMessagesUserId> ZaloMessagesUserId { get; set; } = default!;
        public DbSet<MetaToken> MetaToken { get; set; } = default!;
        public DbSet<MessengerMessage> MessengerMessage { get; set; } = default!;



    }
}
