using System;
using System.IO;
using System.Threading.Tasks;
using MySqlConnector;

namespace HashGen
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cs = "Server=localhost;User=root;Password=1234;";
            try
            {
                using var conn = new MySqlConnection(cs);
                await conn.OpenAsync();
                
                Console.WriteLine("Executing schema.sql...");
                var schemaPath = @"E:\proyecto_core\database\schema.sql";
                var schemaSql = await File.ReadAllTextAsync(schemaPath);
                
                // Add DROP DATABASE to ensure clean slate
                schemaSql = "DROP DATABASE IF EXISTS devtaskmanager;\n" + schemaSql;
                
                using var cmd1 = new MySqlCommand(schemaSql, conn);
                await cmd1.ExecuteNonQueryAsync();
                
                Console.WriteLine("Executing seed.sql...");
                var seedPath = @"E:\proyecto_core\database\seed.sql";
                var seedSql = await File.ReadAllTextAsync(seedPath);
                
                using var cmd2 = new MySqlCommand(seedSql, conn);
                await cmd2.ExecuteNonQueryAsync();
                
                Console.WriteLine("Database reset and seeded successfully! Passwords are set to 1234.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
