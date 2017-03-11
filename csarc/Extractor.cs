using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace csarc
{
    internal static class Extractor
    {
        public static List<FileEntry> ExtFiles = new List<FileEntry>();
        public static int DirSize = 0;
        public static int FileEntriesOffset = 0;
        
        public static void GetArchiveInfo(string archiveName)
        {
            if (!Path.GetExtension(archiveName).Equals(".csarc"))
                throw new ArgumentException("only accepts .csarc files!"); //check condition
            using (var stream = File.OpenRead(archiveName))
            {
                BinaryReader reader = new BinaryReader(stream);
                stream.Seek(-8, SeekOrigin.End);
                FileEntriesOffset = reader.ReadInt32();
                DirSize = reader.ReadInt32();
            }

            //after we got the directorySize we need to read the FileEntries.
            Deserialize(archiveName, FileEntriesOffset, DirSize);
        }
    
        public static void CopyStream(Stream input, Stream output, Int64 position, int size)
        {
            var buffer = new byte[size];
            input.Seek(position, SeekOrigin.Begin);
            try
            {
                    input.Read(buffer, 0, buffer.Length);
                    output.Write(buffer, 0, size);
              
            }
            finally
            {
                output.Dispose();
            }
            input.Dispose();

        }

        private static void Deserialize(string archiveName,int offset,int numberOfObjects)
        {
            IFormatter formatter = new BinaryFormatter();
            FileEntry me;
            using (var stream = File.OpenRead(archiveName))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                for (int i = numberOfObjects; i > 0; i--)
                {
                    me = (FileEntry)formatter.Deserialize(stream);
                    ExtFiles.Add(me);
                }
               
            }
            
        }

        public static void ExtractToCurrentDir(string archiveName)
        {
            GetArchiveInfo(archiveName);

            foreach (var extractedFile in ExtFiles)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(extractedFile.FilePath.Remove(0, 3)));
                //after directories has been created. if we didn't created this folders it will throw a exception
                //we need to copy the content of the files inside the archive
                var inputStream = File.OpenRead(archiveName);
                var outputStream = File.OpenWrite(extractedFile.FilePath.Remove(0, 3));
                CopyStream(inputStream, outputStream, extractedFile.OffsetStart, extractedFile.Size);

            }

        }

        public static void ExtractSpecificFileToCurrentDir(string archiveName, string fileToExtract)
        {
            GetArchiveInfo(archiveName);
            var result = ExtFiles.FirstOrDefault(o => o.FileName == fileToExtract); //using LINQ is so much easier
            if (result == null) throw new FileNotFoundException();
            Directory.CreateDirectory(Path.GetDirectoryName(result.FilePath.Remove(0, 3)));
            var inputStream = File.OpenRead(archiveName);
            var outputStream = File.OpenWrite(result.FilePath.Remove(0, 3));
            CopyStream(inputStream, outputStream, result.OffsetStart, result.Size);

        }

    }
}

