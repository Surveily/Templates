using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cassandra
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var contractId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            using (var ctx = new SqlContext())
            {
                var sw = Stopwatch.StartNew();

                for (var i = 0d; i < 1000; i++)
                {
                    var unitId = Guid.NewGuid();

                    Console.WriteLine($"<{accountId}>");

                    for (var j = 0; j < 1000; j++)
                    {
                        ctx.Set<UnitData>().Add(new UnitData
                        {
                            UnitId = unitId,
                            Id = Guid.NewGuid(),
                            AccountId = accountId,
                            ContractId = contractId,
                            Occurred = new DateTime(2020, 1, 1).AddMinutes(i * j),
                        });
                    }

                    await ctx.SaveChangesAsync();

                    Console.WriteLine($"</{accountId}>");
                    Console.WriteLine($"Progress = {(i / 1000.0) * 100.0}%");
                }

                sw.Stop();
                Console.WriteLine($"Elapsed = {sw.Elapsed}");
            }
        }
    }
}
