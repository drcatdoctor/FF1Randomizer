namespace FF1Lib
{
	public partial class FF1Rom
	{
		private static class Offsets
		{
			private static int BA(int bank, int addr)
			{
				return bank*0x4000 + (addr - (bank == 0x1F ? 0xC000 : 0x8000));
			}

			public static int lut_PtyGenBuf = BA(0x1E, 0x84AA);
			public static int lut_AllowedClasses = BA(0x1E, 0x8128);
		}
	}
}
