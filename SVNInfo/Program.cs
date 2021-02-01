using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SVNInfo
{
    class Program
    {
        private static string HelpMsg = @"This program is used to print the message about wc.db. 
-h to print this message
-q [query] to execute arbitrary query on wc.db - the result is returned as a json 
-s [query] to execute arbitrary scalar query (first row/first column) on wc.db
-p [path] to set path where .svn resides
-f [int] to set the number of times that the search of .svn folder should traverse towards the root (default is 0)
-r to print the revision number
-b to print the branch
-o [path] to set the output file";

        static void Main(string[] args)
        {
            var config = new Config(args);

            switch (config.Mode) {
                case Mode.HELP: { Console.WriteLine(HelpMsg); break; }
                default: { new Worker(config).Work(); break; }
            }
        }
    }
}
