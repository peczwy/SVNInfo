using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVNInfo
{
    class Worker
    {
        internal Config Config { get; private set; }

        internal Worker(Config config) 
        {
            Config = config;
        }

        internal void Work() 
        {
            var output = "";
            switch (Config.Mode)
            {
                case Mode.QUERY:
                    using (var connection = ObtainConnection())
                    {
                        output = PerformQuery(connection, Config.Query);
                    }
                    break;
                case Mode.QUERY_SCALAR:
                    using (var connection = ObtainConnection())
                    {
                        output = PerformScalarQuery(connection, Config.Query);
                    }
                    break;
                case Mode.BRANCH:
                    {
                        using (var connection = ObtainConnection())
                        {
                            output = PerformScalarQuery(connection, "SELECT repos_path FROM NODES WHERE LENGTH(repos_path)>1 LIMIT 1;");
                            var split = output.Split('/');
                            if (split.Length > 0) 
                            {
                                if (split[0].Equals("trunk")) 
                                {
                                    output = "trunk";
                                }
                                if (split[0].StartsWith("branch")) 
                                {
                                    output = split.Length > 1 ? split[1] : split[0];   
                                }
                            }
                        }
                        break;
                    }
                case Mode.REVISION: 
                    {
                        using (var connection = ObtainConnection()) 
                        {
                            output = PerformScalarQuery(connection, "SELECT MAX(revision) FROM NODES;");
                        }
                        break;
                    };
            }
            Console.WriteLine(output);
        }

        private SQLiteConnection ObtainConnection() 
        {
            var dbFile = "";
            var path = Config.Path;
            var trials = Config.FallbackTraversal;
            var log = new StringBuilder("");
            if (path.EndsWith(".svn")) 
            {
                path = path.Substring(0, path.Length - 4);
            }
            do
            {
                dbFile = Path.Combine(path, ".svn", "wc.db");
                log.AppendLine(dbFile);
                if (!File.Exists(dbFile))
                {
                    path = Path.GetFullPath(Path.Combine(path,".."));
                }
            } while (!File.Exists(dbFile) && trials-- > 0);
            if (File.Exists(dbFile)) 
            {
                var connection = new SQLiteConnection($"URI=file:{dbFile}");
                connection.Open();
                return connection;
            }
            throw new ArgumentException($"Database path is invalid (or privilages insufficient) at: {log}");
        }

        private string PerformScalarQuery(SQLiteConnection connection, string query) 
        {
            var cmd = new SQLiteCommand(query, connection);
            return cmd.ExecuteScalar().ToString();
        }

        private string PerformQuery(SQLiteConnection connection, string query)
        {
            var output = new StringBuilder("{");
            var cmd = new SQLiteCommand(query, connection);
            var result = cmd.ExecuteReader();
            var columns = new string[result.FieldCount];
            for(int i = 0; i < columns.Length; ++i) 
            {
                columns[i] = result.GetName(i);
            }
            output.Append("\"result\":[");
            int count = 0;
            while (result.Read())
            {
                if (count > 0) 
                {
                    output.Append(",");
                }
                count++;
                output.Append("{");
                output.Append($"\"_id\":\"{count}\"");
                for (int i = 0; i < columns.Length; ++i)
                {
                    output.Append(",");
                    output.Append($"\"{columns[i]}\":\"{result.GetValue(i).ToString()}\"");
                }
                output.Append("}");
            }
            return output.Append("]}").ToString();
        }

    }
}
