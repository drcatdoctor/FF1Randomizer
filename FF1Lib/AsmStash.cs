using System;
using System.IO;
using System.Reflection;
using System.Resources;
using RomUtilities;

namespace FF1Lib
{
	public static class AsmStash
	{
		internal static Blob GetAssembledFile(string filename_without_extension)
		{
			var cSharpAssembly = Assembly.GetExecutingAssembly();

			Console.WriteLine("listing manifest resources");
			foreach (var str in cSharpAssembly.GetManifestResourceNames())
			{
				Console.WriteLine(str);
			}
			Console.WriteLine("--- done listing manifest resources ---");

			var resourceStream = cSharpAssembly.GetManifestResourceStream("FF1Lib.asm_autobuild._6502bin." +
			                                                              filename_without_extension + ".bin");
			var ms = new MemoryStream();

			if (resourceStream is null)
			{
				throw new MissingManifestResourceException(filename_without_extension + ".bin not found in manifest " +
				                                           "resources.");
			}

			resourceStream.CopyTo(ms);
			return Blob.FromSBytes((sbyte[])(Array)ms.ToArray());
		}
	}
}
