using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace csarc
{
    internal static class Archiver
    {
        public static List<string> FilesNpaths = new List<string>();
        public static List<FileEntry> ArcFiles = new List<FileEntry>();

        public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName)
        {
            ProcessDirectory(sourceDirectoryName);
            WriteFilesToFile(destinationArchiveFileName);
            WriteFileEntriesToFile(destinationArchiveFileName);
            WriteBinarySize(destinationArchiveFileName);
            WriteFileEntriesNumber(destinationArchiveFileName);
            PrintFileSizeNCount(destinationArchiveFileName);
            PrintFileEntries(destinationArchiveFileName);
        }

        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        public static void ProcessFile(string path)
        {
            FilesNpaths.Add(path); //make a list of file path.
        }

        public static void WriteFilesToFile(string destinationArchiveFileName)
        {

            var buffer = new byte[4096];
            try
            {
                var outputStream = new FileStream(destinationArchiveFileName, FileMode.Create);

                try
                {
                   
                    foreach (var file in FilesNpaths)
                    {
                        var inputStream = File.OpenRead(file);
                        int fileSize = 0; //fileSize
                        int bytesRead;
                        Int64 currentPosition = outputStream.Position; //currentLocationInsideTheFile
                        do
                        {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            fileSize += bytesRead;
                            outputStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead > 0);

                        
                        ArcFiles.Add(new FileEntry()
                        {
                            FileName = Path.GetFileName(file),
                            FilePath = file,
                            OffsetStart = currentPosition,
                            Size = fileSize
                        });
                        inputStream.Dispose(); //clear resource 
                    }


                }
                finally
                {
                    outputStream.Dispose();//clear resource 

                }
            }
            finally
            {
            }
        }

        public static void WriteFileEntriesToFile(string destinationArchiveFileName)
        {
            foreach (var readEntry in ArcFiles)
            {
                IFormatter formatter = new BinaryFormatter();
                using (var stream = File.OpenWrite(destinationArchiveFileName))
                {
                    stream.Seek(0, SeekOrigin.End);
                    formatter.Serialize(stream, readEntry);
                }
            }

        }

        public static void WriteBinarySize(string destinationArchiveFileName)
        {
            using (var fileStream = new FileStream(destinationArchiveFileName, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var bw = new BinaryWriter(fileStream))
            {
                int compressedfilesize = 0;
                foreach (var getSizeFile in ArcFiles)
                {
                    compressedfilesize += getSizeFile.Size;
                }
                bw.Write(compressedfilesize);
            }
        }

        public static void WriteFileEntriesNumber(string destinationArchiveFileName)
        {
            using (var fileStream = new FileStream(destinationArchiveFileName, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var bw = new BinaryWriter(fileStream))
            {
                bw.Write(ArcFiles.Count);
            }
        }

        public static void PrintFileSizeNCount(string destinationArchiveFileName)
        {
            var fileSize = new System.IO.FileInfo(destinationArchiveFileName).Length;
            Console.WriteLine("Total Size : "+ToFileSize(fileSize)+" File Count: "+ArcFiles.Count );
        }
        public static string ToFileSize(this long size)
        {
            if (size < 1024)
            {
                return (size).ToString("F0") + " bytes";
            }
            else if ((size >> 10) < 1024)
            {
                return (size / (float)1024).ToString("F1") + " KB";
            }
            else if ((size >> 20) < 1024)
            {
                return ((size >> 10) / (float)1024).ToString("F1") + " MB";
            }
            else if ((size >> 30) < 1024)
            {
                return ((size >> 20) / (float)1024).ToString("F1") + " GB";
            }
            else if ((size >> 40) < 1024)
            {
                return ((size >> 30) / (float)1024).ToString("F1") + " TB";
            }
            else if ((size >> 50) < 1024)
            {
                return ((size >> 40) / (float)1024).ToString("F1") + " PB";
            }
            else
            {
                return ((size >> 50) / (float)1024).ToString("F0") + " EB";
            }
        }

        public static string PercentageOfArchive(double small, double big)
        {
            var result = small/big * 100.0;
            result = Math.Round(result, 2);
            return "" + result + "%";
        }
        public static void PrintFileEntries(string destinationArchiveFileName)
        {
            var fileSize = new System.IO.FileInfo(destinationArchiveFileName).Length;
            foreach (var fileEntry in ArcFiles)
            {
                var modifyPath = fileEntry.FilePath.Remove(0, 3);
                Console.WriteLine(modifyPath + " "+ToFileSize(fileEntry.Size)+ " "+PercentageOfArchive(fileEntry.Size,fileSize));
            }
        }

    }

}