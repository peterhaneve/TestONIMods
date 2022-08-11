using System.Collections.Generic;
using UnityEngine;

namespace RollerSnake {
	public class CactusFruitConfig : IEntityConfig {
		public const string Id = "CactusFruit";
		public const string SeedId = "CactusFruitSeed";

		public const float DefaultTemperature = 320f;
		public const float TemperatureLethalLow = 258.15f;
		public const float TemperatureWarningLow = 308.15f;
		public const float TemperatureWarningHigh = 358.15f;
		public const float TemperatureLethalHigh = 448.15f;

		public const float IrrigationRate = 0.025f;

		public const float CyclesForGrowth = 12.0f;

		public GameObject CreatePrefab() {
			var fruitAnim = Assets.GetAnim("cactusfruit_kanim");

			var cactusFruit = EntityTemplates.CreatePlacedEntity(
				id: Id,
				name: RollerSnakeStrings.CREATURES.SPECIES.CACTUSFRUIT.NAME,
				desc: RollerSnakeStrings.CREATURES.SPECIES.CACTUSFRUIT.DESC,
				mass: 1f,
				anim: fruitAnim,
				initialAnim: "idle_empty",
				sceneLayer: Grid.SceneLayer.BuildingFront,
				width: 1,
				height: 1,
				decor: TUNING.DECOR.BONUS.TIER1,
				defaultTemperature: DefaultTemperature);

			EntityTemplates.ExtendEntityToBasicPlant(
				template: cactusFruit,
				temperature_lethal_low: TemperatureLethalLow,
				temperature_warning_low: TemperatureWarningLow,
				temperature_warning_high: TemperatureWarningHigh,
				temperature_lethal_high: TemperatureLethalHigh,
				safe_elements: new[] { SimHashes.Oxygen, SimHashes.CarbonDioxide },
				crop_id: CactusFleshConfig.Id,
				baseTraitId: Id + "Original",
				baseTraitName: RollerSnakeStrings.CREATURES.SPECIES.CACTUSFRUIT.NAME);
			EntityTemplates.ExtendPlantToIrrigated(
				template: cactusFruit,
				info: new PlantElementAbsorber.ConsumeInfo {
					tag = GameTags.Water,
					massConsumptionRate = IrrigationRate
				});
			cactusFruit.AddOrGet<StandardCropPlant>();

			var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
				plant: cactusFruit,
				productionType: SeedProducer.ProductionType.Harvest,
				id: SeedId,
				name: RollerSnakeStrings.CREATURES.SPECIES.SEEDS.CACTUSFRUITSEED.NAME,
				desc: RollerSnakeStrings.CREATURES.SPECIES.SEEDS.CACTUSFRUITSEED.DESC,
				anim: Assets.GetAnim("cactusseed_kanim"),
				initialAnim: "object",
				numberOfSeeds: 1,
				additionalTags: new List<Tag>() { GameTags.CropSeed },
				planterDirection: SingleEntityReceptacle.ReceptacleDirection.Top,
				replantGroundTag: new Tag(),
				sortOrder: 2,
				domesticatedDescription: RollerSnakeStrings.CREATURES.SPECIES.SEEDS.CACTUSFRUITSEED.DOMESTICATEDDESC,
				collisionShape: EntityTemplates.CollisionShape.CIRCLE,
				width: 0.2f,
				height: 0.2f);
			EntityTemplates.CreateAndRegisterPreviewForPlant(
				seed: seed,
				id: Id + "_preview",
				anim: fruitAnim,
				initialAnim: "place",
				width: 1,
				height: 1);

			return cactusFruit;
		}

		public string[] GetDlcIds() {
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst) { }
	}
}
