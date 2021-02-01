using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVNInfo
{
    class Config
    {
        public int FallbackTraversal{ get; internal set; }

        public string Path { get; internal set; }

        public string Query { get; internal set; }

        public Mode Mode { get; internal set; }

        public string OutputFile { get; internal set; }

        public Config(string[] args) 
        {
            FallbackTraversal = 0;
            Mode = Mode.HELP;
            for (int i = 0; i < args.Length; ++i) 
            {
                switch (args[i].ToLower()) 
                {
                    case "-h": Mode = Mode.HELP; break;
                    case "-b": Mode = Mode.BRANCH; break;
                    case "-r": Mode = Mode.REVISION; break;
                    case "-q":
                        {
                            Mode = Mode.QUERY;
                            i++;
                            if(i < args.Length) 
                            {
                                Query = args[i];
                            }
                            break;
                        }
                    case "-s":
                        {
                            Mode = Mode.QUERY_SCALAR;
                            i++;
                            if (i < args.Length)
                            {
                                Query = args[i];
                            }
                            break;
                        }
                    case "-p":
                        {
                            i++;
                            if (i < args.Length)
                            {
                                Path = args[i];
                            }
                            break;
                        }
                    case "-f":
                        {
                            i++;
                            if (i < args.Length)
                            {
                                FallbackTraversal = int.Parse(args[i]);
                            }
                            break;
                        }
                    case "-o":
                        {
                            i++;
                            if (i < args.Length)
                            {
                                OutputFile = args[i];
                            }
                            break;
                        }
                }
            }
        }
    }
}
