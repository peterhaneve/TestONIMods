using PeterHan.PLib.Core;
using UnityEngine;

namespace RollerSnake {
	public class AquaBulbConfig : IEntityConfig {
		public const string Id = "AquaBulb";

		public const float DefaultTemperature = 320f;

		public GameObject CreatePrefab() {
			var placedEntity = EntityTemplates.CreatePlacedEntity(
				id: Id,
				name: RollerSnakeStrings.CREATURES.SPECIES.AQUABULB.NAME,
				desc: RollerSnakeStrings.CREATURES.SPECIES.AQUABULB.DESC,
				mass: 1000f,
				anim: Assets.GetAnim("aquabulb_kanim"),
				initialAnim: "idle",
				sceneLayer: Grid.SceneLayer.BuildingFront,
				width: 1,
				height: 2,
				decor: TUNING.DECOR.BONUS.TIER3,
				defaultTemperature: DefaultTemperature);

			placedEntity.AddOrGet<SimTemperatureTransfer>();
			placedEntity.AddOrGet<OccupyArea>().objectLayers = new[] {
				PGameUtils.GetObjectLayer(nameof(ObjectLayer.Building), ObjectLayer.Building)
			};
			placedEntity.AddOrGet<EntombVulnerable>();
			placedEntity.AddOrGet<DrowningMonitor>();
			placedEntity.AddOrGet<Prioritizable>();
			placedEntity.AddOrGet<Uprootable>();
			placedEntity.AddOrGet<UprootedMonitor>();
			placedEntity.AddOrGet<Harvestable>();
			placedEntity.AddOrGet<HarvestDesignatable>();
			placedEntity.AddOrGet<SeedProducer>().Configure(AquaBulbSackConfig.Id,
				SeedProducer.ProductionType.DigOnly);
			placedEntity.AddOrGet<BasicForagePlantPlanted>();
			placedEntity.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
			return placedEntity;
		}

		public string[] GetDlcIds() {
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst) { }
	}
}
