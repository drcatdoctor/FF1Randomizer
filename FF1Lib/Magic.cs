﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int MagicSize = 8;
		public const int MagicCount = 64;
		public const int MagicNameSize = 5;
		public const int MagicPermissionsSize = 8;
		public const int MagicPermissionsCount = 12;
		public const int MagicOutOfBattleSize = 7;
		public const int MagicOutOfBattleCount = 13;

		public const int FireSpellIndex = 4;

		public const int WeaponSize = 8;
		public const int WeaponCount = 40;

		public const int ArmorSize = 4;
		public const int ArmorCount = 40;

		private struct MagicSpell
		{
			public byte Index;
			public Blob Data;
			public Blob Name;
			public byte TextPointer;
		}

		private readonly List<byte> _outOfBattleSpells = new List<byte> { 0, 16, 32, 48, 19, 51, 35, 24, 33, 56, 38, 40, 41 };

		public void ShuffleMagicLevels(MT19337 rng, bool keepPermissions)
		{
			var spells = Get(Offsets.lut_MagicData, MagicSize * MagicCount).Chunk(MagicSize);
			var names = Get(Offsets.itemText_strings_MagicStart, MagicNameSize * MagicCount).Chunk(MagicNameSize);
			var pointers = Get(Offsets.magicBattleText_ptrTable, MagicCount);

			var magicSpells = spells.Select((spell, i) => new MagicSpell
			{
				Index = (byte)i,
				Data = spell,
				Name = names[i],
				TextPointer = pointers[i]
			})
			.ToList();

			// First we have to un-interleave white and black spells.
			var whiteSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 0).ToList();
			var blackSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 1).ToList();

			whiteSpells.Shuffle(rng);
			blackSpells.Shuffle(rng);

			// Now we re-interleave the spells.
			var shuffledSpells = new List<MagicSpell>();
			for (int i = 0; i < MagicCount; i++)
			{
				var sourceIndex = 4 * (i / 8) + i % 4;
				if ((i / 4) % 2 == 0)
				{
					shuffledSpells.Add(whiteSpells[sourceIndex]);
				}
				else
				{
					shuffledSpells.Add(blackSpells[sourceIndex]);
				}
			}

			Put(Offsets.lut_MagicData, shuffledSpells.Select(spell => spell.Data).Aggregate((seed, next) => seed + next));
			Put(Offsets.itemText_strings_MagicStart, shuffledSpells.Select(spell => spell.Name).Aggregate((seed, next) => seed + next));
			Put(Offsets.magicBattleText_ptrTable, shuffledSpells.Select(spell => spell.TextPointer).ToArray());

			if (keepPermissions)
			{
				// Shuffle the permissions the same way the spells were shuffled.
				for (int c = 0; c < MagicPermissionsCount; c++)
				{
					var oldPermissions = Get(Offsets.lut_MagicPermissions_data + c * MagicPermissionsSize, MagicPermissionsSize);

					var newPermissions = new byte[MagicPermissionsSize];
					for (int i = 0; i < 8; i++)
					{
						for (int j = 0; j < 8; j++)
						{
							var oldIndex = shuffledSpells[8 * i + j].Index;
							var oldPermission = (oldPermissions[oldIndex / 8] & (0x80 >> oldIndex % 8)) >> (7 - oldIndex % 8);
							newPermissions[i] |= (byte)(oldPermission << (7 - j));
						}
					}

					Put(Offsets.lut_MagicPermissions_data + c * MagicPermissionsSize, newPermissions);
				}
			}

			// Map old indices to new indices.
			var newIndices = new byte[MagicCount];
			for (byte i = 0; i < MagicCount; i++)
			{
				newIndices[shuffledSpells[i].Index] = i;
			}

			// Fix enemy spell pointers to point to where the spells are now.
			var scripts = Get(Offsets.lut_EnemyAi, ScriptSize * ScriptCount).Chunk(ScriptSize);
			foreach (var script in scripts)
			{
				// Bytes 2-9 are magic spells.
				for (int i = 2; i < 10; i++)
				{
					if (script[i] != 0xFF)
					{
						script[i] = newIndices[script[i]];
					}
				}
			}
			Put(Offsets.lut_EnemyAi, scripts.SelectMany(script => script.ToBytes()).ToArray());

			// Fix weapon and armor spell pointers to point to where the spells are now.
			var weapons = Get(Offsets.lut_WeaponData, WeaponSize * WeaponCount).Chunk(WeaponSize);
			foreach (var weapon in weapons)
			{
				if (weapon[3] != 0x00)
				{
					weapon[3] = (byte)(newIndices[weapon[3] - 1] + 1);
				}
			}
			Put(Offsets.lut_WeaponData, weapons.SelectMany(weapon => weapon.ToBytes()).ToArray());

			var armors = Get(Offsets.lut_ArmorData, ArmorSize * ArmorCount).Chunk(ArmorSize);
			foreach (var armor in armors)
			{
				if (armor[3] != 0x00)
				{
					armor[3] = (byte)(newIndices[armor[3] - 1] + 1);
				}
			}
			Put(Offsets.lut_ArmorData, armors.SelectMany(armor => armor.ToBytes()).ToArray());

			// Fix the crazy out of battle spell system.

			// this is the offset to a CMP operand in a big list of CMP and JMP.
			int outOfBattleSpellOffset = Offsets.func_MagicMenu_Loop + 0x3A;  // 0x3AEC0 + 0x3A = 0x3AEFA

			// the below changes the IDs that are being CMPd against.
			for (int i = 0; i < MagicOutOfBattleCount; i++)
			{
				var oldSpellIndex = _outOfBattleSpells[i];
				var newSpellIndex = newIndices[oldSpellIndex];

				Put(outOfBattleSpellOffset, new[] { (byte)(newSpellIndex + 0xB0) });

				outOfBattleSpellOffset += MagicOutOfBattleSize;
			}

			// Confused enemies are supposed to cast FIRE, so figure out where FIRE ended up.
			int confusedSpellIndexOffset = Offsets.func_Battle_DoEnemyTurn + 0x87;  // 0x33197

			var newFireSpellIndex = shuffledSpells.FindIndex(spell => spell.Data == spells[FireSpellIndex]);
			Put(confusedSpellIndexOffset, new[] { (byte)newFireSpellIndex });
		}
	}
}
