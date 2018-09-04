using System;
using System.IO;
using System.Reflection;
using System.Resources;
using RomUtilities;

namespace FF1Lib
{
	public static class AsmReader
	{
		internal static Blob GetAsm(string filename)
		{
			var cSharpAssembly = Assembly.GetExecutingAssembly();

			foreach (var str in cSharpAssembly.GetManifestResourceNames())
			{
				Console.WriteLine(str);
			}

			var resourceStream = cSharpAssembly.GetManifestResourceStream("FF1Lib.asm." + filename + ".pic");
			var ms = new MemoryStream();

			if (resourceStream is null)
			{
				throw new MissingManifestResourceException();
			}

			resourceStream.CopyTo(ms);
			return Blob.FromSBytes((sbyte[])(Array)ms.ToArray());
		}
	}
}
