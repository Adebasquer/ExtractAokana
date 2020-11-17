using System;
using System.Collections.Generic;
using System.IO;

namespace ExtractAokana
{
	//Done by using dnSpy on Assembly-csharp.dll inside managed directory of aokana data
	//Decryption algorithm for these .dat files is in pkmain/pkread classes within that dll

	class Program
	{
		static void Main(string[] args)
		{
			var arguments = new Dictionary<string, string>();

			foreach (string argument in args)
			{
				string[] splitted = argument.Split('=');

				if (splitted.Length == 2)
				{
					arguments[splitted[0]] = splitted[1];
				}
			}
			string fullInputPath = arguments["in"];
			string fullOutputPath = arguments["out"];

			FileAttributes attributes = File.GetAttributes(fullInputPath);
			if (attributes.HasFlag(FileAttributes.Directory))
            { //aokana data folder
				string[] paths = Directory.GetFiles(fullInputPath, "*.dat");
				foreach (string path in paths)
                {
					Console.WriteLine("Extracting: " + path);
					ExportArchive archive = new ExportArchive(path, fullOutputPath);
					archive.export();
				}
            }
			else
            { //extract one .dat file
				ExportArchive archive = new ExportArchive(fullInputPath, fullOutputPath);
				archive.export();
			}
		}
	}
}