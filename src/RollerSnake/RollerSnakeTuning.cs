using System.Collections.Generic;
using Klei.AI;
using PeterHan.PLib.Detours;
using TUNING;

namespace RollerSnake {
	public static class RollerSnakeTuning {
		public static readonly List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
		{
			new FertilityMonitor.BreedingChance()
			{
				egg = RollerSnakeConfig.EggId.ToTag(),
				weight = 0.98f
			},
			new FertilityMonitor.BreedingChance()
			{
				egg = SteelRollerSnakeConfig.EggId.ToTag(),
				weight = 0.02f
			},
		};

		public static readonly List<FertilityMonitor.BreedingChance> EGG_CHANCES_STEEL = new List<FertilityMonitor.BreedingChance>()
		{
			new FertilityMonitor.BreedingChance()
			{
				egg = RollerSnakeConfig.EggId.ToTag(),
				weight = 0.35f
			},
			new FertilityMonitor.BreedingChance()
			{
				egg = SteelRollerSnakeConfig.EggId.ToTag(),
				weight = 0.65f
			},
		};

		// Mutable to allow other mods to modify the Roller Snake
		public static float STANDARD_CALORIES_PER_CYCLE = 700000f;
		public static float STANDARD_STARVE_CYCLES = 10f;
		public static float STANDARD_STOMACH_SIZE = STANDARD_CALORIES_PER_CYCLE * STANDARD_STARVE_CYCLES;
		public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;
		public static float EGG_MASS = 1f;
	}
}
