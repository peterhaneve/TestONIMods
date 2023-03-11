using UnityEngine;
using Klei.AI;

namespace RollerSnake {
	public class RollerSnakeConfig : IEntityConfig {
		public const string Id = "RollerSnake";
		public const string BaseTraitId = "RollerSnakeBaseTrait";
		public const string EggId = "RollerSnakeEgg";

		public const float Hitpoints = 25f;
		public const float Lifespan = 50f;
		public const float FertilityCycles = 30f;
		public const float IncubationCycles = 10f;

		public const float KgEatenPerCycle = 140.0f;
		public const float MinPoopSizeInKg = 25.0f;
		public static float CaloriesPerKg = RollerSnakeTuning.STANDARD_CALORIES_PER_CYCLE /
			KgEatenPerCycle;
		public static float ProducedConversionRate = TUNING.CREATURES.CONVERSION_EFFICIENCY.
			BAD_1;
		public const int EggSortOrder = 700;

		public static float ScaleGrowthTimeCycles = 6.0f;
		public static float GoldAmalgamPerCycle = 10.0f;
		public static Tag EmitElement = SimHashes.GoldAmalgam.CreateTag();

		public static GameObject CreateRollerSnake(string id, string name, string desc,
				string anim_file, bool is_baby) {
			var wildCreature = EntityTemplates.ExtendEntityToWildCreature(
				BaseRollerSnakeConfig.BaseRollerSnake(id, name, desc, anim_file, BaseTraitId,
				is_baby), RollerSnakeTuning.PEN_SIZE_PER_CREATURE);

			var trait = Db.Get().CreateTrait(BaseTraitId, name, name, null, false, null, true,
				true);
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id,
				RollerSnakeTuning.STANDARD_STOMACH_SIZE, name));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id,
				-RollerSnakeTuning.STANDARD_CALORIES_PER_CYCLE / 600.0f, name));
			trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id,
				Hitpoints, name));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, Lifespan,
				name));

			var diet = BaseRollerSnakeConfig.BasicRockDiet(SimHashes.Carbon.CreateTag(),
				CaloriesPerKg, ProducedConversionRate, null, 0.0f);
			BaseRollerSnakeConfig.SetupDiet(wildCreature, diet, CaloriesPerKg,
				MinPoopSizeInKg);

			var scaleMonitor = wildCreature.AddOrGetDef<ScaleGrowthMonitor.Def>();
			scaleMonitor.defaultGrowthRate = 1.0f / (ScaleGrowthTimeCycles * 600.0f);
			scaleMonitor.dropMass = GoldAmalgamPerCycle * ScaleGrowthTimeCycles;
			scaleMonitor.itemDroppedOnShear = EmitElement;
			scaleMonitor.levelCount = 2;
			scaleMonitor.targetAtmosphere = SimHashes.Oxygen;
			wildCreature.AddTag(GameTags.OriginalCreature);

			return wildCreature;
		}

		public GameObject CreatePrefab() {
			var rollerSnake = CreateRollerSnake(Id,
				RollerSnakeStrings.CREATURES.SPECIES.ROLLERSNAKE.NAME,
				RollerSnakeStrings.CREATURES.SPECIES.ROLLERSNAKE.DESC,
				"rollersnake_kanim", false);
			return EntityTemplates.ExtendEntityToFertileCreature(rollerSnake, EggId,
				RollerSnakeStrings.CREATURES.SPECIES.ROLLERSNAKE.EGG_NAME,
				RollerSnakeStrings.CREATURES.SPECIES.ROLLERSNAKE.DESC, "rollersnakeegg_kanim",
				RollerSnakeTuning.EGG_MASS, BabyRollerSnakeConfig.Id, FertilityCycles,
				IncubationCycles, RollerSnakeTuning.EGG_CHANCES_BASE, EggSortOrder);
		}

		public string[] GetDlcIds() {
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}

		public void OnPrefabInit(GameObject prefab) {
		}

		public void OnSpawn(GameObject inst) {
		}
	}
}
