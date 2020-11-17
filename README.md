# ExtractAokana

A command line tool used to decrypt and save sprites, backgrounds, cg art, etc from the *.dat files located in Aokana_Data. Made using dnSpy to look at Assembly-CSharp.dll in the Managed folder of Aokana_Data. The game decrypts the *.dat files in its source code for use with unity, the decryption algorithm is located in the PkRead class.

Aokana's *.dat files contain file hierarchies inside them, so the exported file structure isnt split the same way the .dat files are.

The game's art should be moddable by copy and pasting this extracted data into Aokana_Data and changing the game's file reading classes to bypass PkRead/PkMain and directly read the decrypted files instead using dnSpy or a similar program.
# Quick Usage:

```ExtractAokana.exe in="<full path to Aokana_Data>" out="<full path to output directory>"```

### You can also extract just one *.dat file:

For example:
```ExtractAokana.exe in="<full path to Aokana_Data>\sprites.dat" out="<full path to output directory>"```

Have fun :)
