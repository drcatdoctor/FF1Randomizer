// ReSharper disable InconsistentNaming
namespace FF1Lib
{
	public partial class FF1Rom
	{
		private static class Offsets
		{
			// NOTE all of the below (bank math) assumes MMC3 layout. Use at own risk before UpgradeToMMC3()!

			public static int BA(int bank, int addr)
			{
				return bank*0x4000 + addr - (bank == 0x1F ? 0xC000 : 0x8000);
			}

			// Address when bank is loaded, e.g. 0x8000, instead of ROM-packed offset.
			public static int runAddress(int offset)
			{
				if (offset >= 0x7C000)
					return offset - (0x7C000 - 0xC000);
				return offset % 0x4000 + 0x8000;
			}

			// Bank of offset.
			public static int bank(int offset)
			{
				if (offset >= 0x7c000)
					return 0x1F;
				return offset / 0x4000;
			}

			// -- bank 00: map --
			public static readonly int mapTileData = BA(0x00, 0x8800);  // 0x00800
			public static readonly int teleportTable = BA(0x00, 0xAD00);  // 0x02D00
			public static readonly int mapSpriteAssignment = BA(0x00, 0xAE00);  // 0x02E00
			public static readonly int startingGold = BA(0x00, 0xB01C);  // 0x0301C
			public static readonly int treasure = BA(0x00, 0xB100);  // 0x03100
			public static readonly int mapSprites = BA(0x00, 0xB400);  // 0x03400

			// -- bank 04: location maps --
			public static readonly int maps_ptrTable = BA(0x04, 0x8000);  // 0x10000
			public static readonly int maps_data = BA(0x04, 0x8080);  // 0x10080


			// -- bank 0A: text --
			public static readonly int dialogText_ptrTable = BA(0x0A, 0x8000);  // 0x28000
			public static readonly int dialogText_strings = dialogText_ptrTable + 0x200;  // 0x28200
			public static readonly int itemText_ptrTable = BA(0x0A, 0xB700);  // 0x2B700
			public static readonly int itemText_strings = itemText_ptrTable + 0x200;  // 0x2B900
			public static readonly int itemText_strings_GearStart = itemText_strings + 0xBD;  // 0x2B9BD
			public static readonly int itemText_strings_MagicStart = itemText_strings + 0x503; // 0x2BE03

			// -- bank 0B: encounters, exp, enemy names --
			public static readonly int lut_BattleFormations = BA(0x0B, 0x8400);
			public static readonly int lut_BattleRates = BA(0x0B, 0x8C00);
			public static readonly int lut_ExpToAdvance = BA(0x0B, 0x9000);
			public static readonly int data_EnemyNames_ptrTable = BA(0x0B, 0x94E0); //0x2D4E0
			public static readonly int data_EnemyNames_strings  = data_EnemyNames_ptrTable + 0x100; // 0x2D5E0

			// -- bank 0C: battle!, weapon, armor, magic, enemy stats, palettes, scripts --
			public static readonly int lut_WeaponData = BA(0x0C, 0x8000);  // 0x30000
			public static readonly int lut_ArmorData = BA(0x0C, 0x8140);   // 0x30140
			public static readonly int lut_MagicData = BA(0x0C, 0x81E0);   // 0x301E0
			public static readonly int magicBattleText_ptrTable = BA(0x0C, 0x84C0);  // 0x304C0
			public static readonly int data_EnemyStats = BA(0x0C, 0x8520); // 0x30520
			public static readonly int lut_BattlePalettes = BA(0x0C, 0x8F20);  // 0x30F20
			public static readonly int lut_EnemyAi = BA(0x0C, 0x9020);     // 0x31020
			public static readonly int func_ExitBattle = BA(0x0C, 0x92E0);  // 0x312E0
			public static readonly int func_Battle_DoEnemyTurn = BA(0x0C, 0xB197);  // 0x33197

			// -- bank 0D: big text (story, bridge, end); shop prices  --
			public static readonly int lut_StoryText = BA(0x0D, 0xA800); // 0x36800
			public static readonly int data_Prices = BA(0x0D, 0xBC00);  // 0x37C00

			// -- bank 0E: shops, NPC interactions, menus --
			// note: party gen was here, moved to 1E below.
			public static readonly int lut_ShopData = BA(0x0E, 0x8302);  // 0x38302
			public static readonly int lut_MapObjTalkJumpTbl = BA(0x0E, 0x90D3);  // 0x390D3
			public static class NPC
			{
				public static readonly int func_Talk_Sarda = BA(0x0E, 0x93E4);  // 0x393E4
				public static readonly int func_Talk_CanoeSage = BA(0x0E, 0x947D);  // 0x3947D
			}
			public static readonly int lut_MapObjTalkData = BA(0x0E, 0x95D5);  // 0x395D5
			public static readonly int lut_MagicPermissions_data = BA(0x0E, 0xAD18);  // 0x3AD18
			public static readonly int func_MagicMenu_Loop = BA(0x0E, 0xAEC0);

			// old bank 0F is now bank 1F.


			// -- bank 1E: party gen, moved from 0E. --
			public static readonly int lut_PtyGenBuf = BA(0x1E, 0x84AA);
			public static readonly int lut_AllowedClasses = BA(0x1E, 0x8128);


			// -- bank 1F: always in-mem. core stuff. movement, rng, low-level (e.g. bank switching) --
			public static readonly int func_OWCanMove = BA(0x1F, 0xC47D);
			public static readonly int func_SMMove_Battle = BA(0x1F, 0xCDC3);
			public static readonly int RngOffset = BA(0x1F, 0xF100);  // 0x7F100
			public static readonly int BattleRngOffset = BA(0x1F, 0xFCF1);  // 0x7FCF1

		}
	}
}
