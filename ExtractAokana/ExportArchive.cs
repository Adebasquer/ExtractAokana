using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ExtractAokana
{
    class ExportArchive
    {
        Decrypt decryptedData;
        string outpath;
        public ExportArchive(string fullInPath, string fullOutPath)
        {
            this.decryptedData = new Decrypt(fullInPath);
            this.outpath = fullOutPath;
            System.IO.Directory.CreateDirectory(this.outpath);
        }
        public void export()
        {
            foreach (string fileName in decryptedData.fileDict.Keys)
            {
                
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(outpath, fileName)));
                FileStream fs = new FileStream(Path.Combine(outpath, fileName), FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);
                Decrypt.DecryptedFile f = decryptedData.getDecryptedFile(fileName);
                byte[] blob;
                if (f.isEmpty)
                {
                    Console.WriteLine(f.fileName + "was empty");
                }
                else
                {
                    blob = f.blob;
                    foreach (byte b in blob)
                    {
                        bw.Write(b);
                    }
                    bw.Close();
                }
            }
        }
    }
}
