using System;
using CLAP;

namespace UpdateVersionInfo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Run<UpdateVersionInfoImpl>(args);
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
