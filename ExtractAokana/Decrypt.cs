using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ExtractAokana
{
    
    class Decrypt
    {
        public FileStream fs;
        //key value pair [<filename> -> <position, length, and key necessary to get decrypted binary blob>]
        public Dictionary<string, Decrypt.EncryptedFile> fileDict;
        public Decrypt(string dataFilePath)
        {
            this.fs = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read);
            this.fs.Position = 0L;
            this.fileDict = new Dictionary<string, EncryptedFile>();
            byte[] array = new byte[1024];
            this.fs.Read(array, 0, 1024);
            int numFiles = 0;
            for (int i = 4; i < 255; i++)
            {
                numFiles += BitConverter.ToInt32(array, i * 4);
            }

            byte[] array2 = new byte[16 * numFiles];
            this.fs.Read(array2, 0, 16 * numFiles);
            this.decrypt(array2, 16 * numFiles, BitConverter.ToUInt32(array, 212));
            int b = BitConverter.ToInt32(array2, 12);
            int c = b - (1024 + 16 * numFiles);

            byte[] array3 = new byte[c];
            this.fs.Read(array3, 0, array3.Length);
            this.decrypt(array3, c, BitConverter.ToUInt32(array, 92));

            int fileNameStartIndex = 0;
            for (int i = 0; i < numFiles; i++)
            {
                int metaPos = 16 * i;
                uint length = BitConverter.ToUInt32(array2, metaPos);
                int indexStart = BitConverter.ToInt32(array2, metaPos + 4);
                uint key = BitConverter.ToUInt32(array2, metaPos + 8);
                uint position = BitConverter.ToUInt32(array2, metaPos + 12);

                int fileNameEndIndex;
                for (fileNameEndIndex = indexStart; fileNameEndIndex < array3.Length; fileNameEndIndex++)
                {
                    if (array3[fileNameEndIndex] == 0)
                    {
                        break;
                    }
                }
                string fileName = Encoding.ASCII.GetString(array3, fileNameStartIndex, fileNameEndIndex - fileNameStartIndex).ToLower();

                Decrypt.EncryptedFile fileMeta = default(EncryptedFile);
                fileMeta.position = position;
                fileMeta.key = key;
                fileMeta.length = length;
                this.fileDict.Add(fileName, fileMeta);

                fileNameStartIndex = fileNameEndIndex + 1;

            }

        }
        public struct EncryptedFile
        {
            public uint length;
            public uint position;
            public uint key;
        }
        public struct DecryptedFile
        {
            public string fileName;
            public byte[] blob;
            public bool isEmpty;
        }
        public void decrypt(byte[] byteArray, int length, uint key)
        {
            byte[] temp = new byte[256];
            uint a = key * 7391U + 42828U;
            uint b = a << 17 ^ a;
            for (int i = 0; i < 256; i++)
            {
                a -= key;
                a += b;
                b = a + 56U;
                a *= (b & 239U);
                temp[i] = (byte)a;
                a >>= 1;
            }
            for (int i = 0; i < length; i++)
            {
                byte bt = byteArray[i];
                bt ^= temp[i % 253];
                bt += 3;
                bt += temp[i % 89];
                bt ^= 153;
                byteArray[i] = bt;
            }
        }

        public DecryptedFile getDecryptedFile(string fileName)
        {
            EncryptedFile fencrypted;
            if (!this.fileDict.TryGetValue(fileName, out fencrypted))
            {
                DecryptedFile empty;
                empty.isEmpty = true;
                empty.blob = null;
                empty.fileName = fileName;
                return empty;
            }
            this.fs.Position = (long)((ulong)fencrypted.position);
            byte[] blob = new byte[fencrypted.length];
            this.fs.Read(blob, 0, blob.Length);
            this.decrypt(blob, blob.Length, fencrypted.key);

            DecryptedFile fdecrypted;
            fdecrypted.isEmpty = false;
            fdecrypted.blob = blob;
            fdecrypted.fileName = fileName;

            return fdecrypted;
        }
    }
}
