using System;
using System.IO;
using Microsoft.Win32;
using Server;

namespace Server.Misc
{
	public class DataPath
	{
		/* If you have not installed Ultima Online,
		 * or wish the server to use a separate set of datafiles,
		 * change the 'CustomPath' value.
		 * Example:
		 *  private static string CustomPath = @"C:\Program Files\Ultima Online";
		 */
		private static string ThreesandUODataPath = Environment.GetEnvironmentVariable("THREESANDUO_CLIENT_FILES_DIR");

        /* The following is a list of files which a required for proper execution:
		 * 
		 * Multi.idx
		 * Multi.mul
		 * VerData.mul
		 * TileData.mul
		 * Map*.mul or Map*LegacyMUL.uop
		 * StaIdx*.mul
		 * Statics*.mul
		 * MapDif*.mul
		 * MapDifL*.mul
		 * StaDif*.mul
		 * StaDifL*.mul
		 * StaDifI*.mul
		 */

        public static void Configure()
		{
			if (ThreesandUODataPath != null ) 
				Core.DataDirectories.Add(ThreesandUODataPath);

			else
			{
				Console.WriteLine( "The required 'THREESANDUO_CLIENT_FILES_DIR' Environment Variable has not been correctly configured. Server will not start..." );
				Console.ReadLine();
			}
		}
	}
}