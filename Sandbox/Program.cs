using System;
using System.Security.Cryptography;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
    class Program
    {
        public static void Main(string[] args)
        {
	        //TreasureDistribution.Test();

	        var flags = new Flags
	        {
		        EnemyFormationsUnrunnable = true,
		        EnemyFormationsSurprise = true,
		        EnemyScripts = true,
		        EnemyFormationsFrequency = true,
		        Treasures = true,
		        SpeedHacks = true,
		        Dash = true,
		        BuyTen = true,
		        Music = MusicShuffle.None,
		        PriceScaleFactor = 1.0,
		        EnemyScaleFactor = 1.0,
		        ExpMultiplier = 1.0,
		        ExpBonus = 0
	        };
	        var rom = new FF1Rom("ff1.nes");
	        var rng = RNGCryptoServiceProvider.Create();
	        var seed = new byte[8];
	        rng.GetBytes(seed);
	        rom.Randomize(seed, flags);
	        rom.Save("ff1_randomized_test.nes");
        }
	}
}
