using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csarc
{
    class Program
    {
        private static void Main(string[] args)
        {
            //checking terms for the program to run 
             CheckArgs(args);
            RunCommand(args);
            Console.WriteLine("\nThank you and goodbye!\nPlease check your current folder!");
           

        }

        static void RunCommand(string[] args)
        {
           if(Path.GetExtension(args[0]) == ".csarc")
                if(args.Length == 1)Extractor.ExtractToCurrentDir(args[0]);
           else
                    Extractor.ExtractSpecificFileToCurrentDir(args[0],args[1]);
           if(Directory.Exists(args[0]))
                if(args.Length>1)
                    if(args[1]!=null)
                    Archiver.CreateFromDirectory(args[0],args[1]);
                    else
                    {
                        return;
                    }
        }



        private static void CheckArgs(string[] args)
        {
            if (args.Length < 1) throw new ArgumentException
                ("Fatal Error!\nplease use this program with the following syntax:\n" +
                    "(extract)Usage: csarc <archive file> \n" +
                    "(extract)Usage: csarc <archive file> [<file to extract>]\n" +
                    "(archive)Usage: csarc <folder name> <output file>\n");
        }




    }
}
