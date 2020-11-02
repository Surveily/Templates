// <copyright file="SqlContext.cs" company="VAR Unit">
// Copyright (c) VAR Unit. All rights reserved.
// </copyright>

using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cassandra
{
    public class SqlContext : DbContext
    {
        private readonly IDbConnection _connection;

        public SqlContext()
        {
        }

        public SqlContext(IDbConnection connection)
        {
            _connection = connection;
        }

        public DbSet<UnitData> Header { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connection = new CassandraConnectionStringBuilder();

                connection.Username = "cassandra";
                connection.Password = "cassandra";
                connection.ClusterName = "My Cluster";
                connection.DefaultKeyspace = "surveily";
                connection.Port = 9042;
                connection.ContactPoints = new string[] { "localhost" };

                optionsBuilder.UseCassandra(connection.ConnectionString, opt =>
                {
                    opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "surveily");
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UsePropertyAccessMode(PropertyAccessMode.Property);
            builder.ForCassandraAddKeyspace("surveily", new KeyspaceReplicationSimpleStrategyClass(1));

            builder.Entity<UnitData>().ToTable(nameof(UnitData), "surveily");
            builder.Entity<UnitData>().Ignore(table => table.ETag);
            builder.Entity<UnitData>().Ignore(table => table.Timestamp);
            builder.Entity<UnitData>().Ignore(table => table.Values);
            builder.Entity<UnitData>().ForCassandraSetClusterColumns(table => new { table.RowKey });
            builder.Entity<UnitData>().HasKey(table => new
            {
                table.PartitionKey,
                table.RowKey
            });
        }
    }
}