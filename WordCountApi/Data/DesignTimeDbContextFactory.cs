﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WordCountApi.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WordDbContext>
    {
        public WordDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<WordDbContext>();
            object value = optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new WordDbContext(optionsBuilder.Options);
        }
    }
}
