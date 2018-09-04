using System;
using System.Security.Cryptography;
using FF1Lib;
using RomUtilities;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
	        var flags = new Flags
	        {
		        Treasures = true,
		        IncentivizeIceCave = false,
		        IncentivizeOrdeals = false,
		        Shops = false,
		        MagicShops = false,
		        MagicLevels = false,
		        MagicPermissions = false,
		        Rng = false,
		        EnemyScripts = false,
		        EnemySkillsSpells = false,
		        EnemyStatusAttacks = false,
		        OrdealsPillars = false,

		        EarlySarda = true,
		        EarlySage = true,
		        CrownlessOrdeals = true,
		        NoPartyShuffle = false,
		        SpeedHacks = false,
		        IdentifyTreasures = false,
		        Dash = false,
		        BuyTen = false,

		        HouseMPRestoration = false,
		        WeaponStats = false,
		        ChanceToRun = false,
		        SpellBugs = false,
		        EnemyStatusAttackBug = false,

		        FunEnemyNames = false,
		        PaletteSwap = false,
		        TeamSteak = false,
		        ModernBattlefield = false,
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
