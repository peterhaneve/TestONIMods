using System.Collections.Generic;
using UnityEngine;
using Klei.AI;

namespace RollerSnake {
	public class BaseRollerSnakeConfig {
		public const string NavGridId = "WalkerNavGrid1x1";
		public const string NavGridBabyId = "WalkerBabyNavGrid";
		public const float Mass = 100.0f;
		public const float MoveSpeed = 2.0f;
		public const string OnDeathDropId = "Meat";
		public const int OnDeathDropCount = 1;

		public const string SpeciesId = RollerSnakeConfig.Id + "Species";

		public const float DefaultTemperature = 320f;
		public const float TemperatureLethalLow = 258.15f;
		public const float TemperatureWarningLow = 308.15f;
		public const float TemperatureWarningHigh = 358.15f;
		public const float TemperatureLethalHigh = 448.15f;

		public static GameObject BaseRollerSnake(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null) {
			var snake = EntityTemplates.CreatePlacedEntity(id, name, desc, Mass, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures,
				width: 1,
				height: 1,
				TUNING.DECOR.BONUS.TIER1, new EffectorValues(), SimHashes.Creature, null, DefaultTemperature);
			EntityTemplates.ExtendEntityToBasicCreature(snake, FactionManager.FactionID.Pest, traitId, is_baby ? NavGridBabyId : NavGridId, NavType.Floor, 32,
				moveSpeed: MoveSpeed,
				onDeathDropID: OnDeathDropId,
				onDeathDropCount: OnDeathDropCount,
				false, false,
				lethalLowTemperature: TemperatureLethalLow,
				warningLowTemperature: TemperatureWarningLow,
				warningHighTemperature: TemperatureWarningHigh,
				lethalHighTemperature: TemperatureLethalHigh);
			if (symbolOverridePrefix != null)
				snake.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix);
			snake.AddOrGet<Trappable>();
			snake.AddOrGetDef<CreatureFallMonitor.Def>();
			snake.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
			snake.AddWeapon(1f, 1f);
			EntityTemplates.CreateAndRegisterBaggedCreature(snake, true, true);
			var prefabID = snake.GetComponent<KPrefabID>();
			prefabID.AddTag(GameTags.Creatures.Walker);
			prefabID.prefabInitFn += (inst => inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost));
			bool condition = !is_baby;
			var choreTable = new ChoreTable.Builder()
				.Add(new DeathStates.Def())
				.Add(new AnimInterruptStates.Def())
				.Add(new GrowUpStates.Def())
				.Add(new TrappedStates.Def())
				.Add(new IncubatingStates.Def())
				.Add(new BaggedStates.Def())
				.Add(new FallStates.Def())
				.Add(new StunnedStates.Def())
				.Add(new DebugGoToStates.Def())
				.Add(new FleeStates.Def())
				.Add(new AttackStates.Def(), condition).PushInterruptGroup()
				.Add(new CreatureSleepStates.Def())
				.Add(new FixedCaptureStates.Def())
				.Add(new RanchedStates.Def(), !is_baby)
				.Add(new LayEggStates.Def())
				.Add(new EatStates.Def())
				.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP))
				.Add(new CallAdultStates.Def()).PopInterruptGroup()
				.Add(new IdleStates.Def());
			EntityTemplates.AddCreatureBrain(snake, choreTable, SpeciesId, symbolOverridePrefix);
			snake.AddTag(GameTags.Amphibious);
			return snake;
		}

		public static List<Diet.Info> BasicRockDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced) {
			return new List<Diet.Info>
			{
				new Diet.Info(new HashSet<Tag>
				{
					SimHashes.Sand.CreateTag(),
					SimHashes.SandStone.CreateTag(),
					SimHashes.CrushedRock.CreateTag(),
					SimHashes.SedimentaryRock.CreateTag(),
					SimHashes.IgneousRock.CreateTag(),
					SimHashes.Obsidian.CreateTag(),
					SimHashes.Granite.CreateTag()
				}, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
			};
		}

		public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos, float referenceCaloriesPerKg, float minPoopSizeInKg) {
			var diet = new Diet(diet_infos.ToArray());
			var def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
			def.diet = diet;
			def.minConsumedCaloriesBeforePooping = referenceCaloriesPerKg * minPoopSizeInKg;
			prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
			return prefab;
		}
	}
}
