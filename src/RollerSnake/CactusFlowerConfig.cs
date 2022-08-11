using UnityEngine;

namespace RollerSnake {
	public class CactusFlowerConfig : IEntityConfig {
		public const string Id = "CactusFlower";

		public const float UnitsToSpawn = 1.0f;
		
		public GameObject CreatePrefab() {
			var cactusFlower = EntityTemplates.CreateLooseEntity(
				id: Id,
				name: RollerSnakeStrings.ITEMS.FOOD.CACTUSFLOWER.NAME,
				desc: RollerSnakeStrings.ITEMS.FOOD.CACTUSFLOWER.DESC,
				mass: 1f,
				unitMass: false,
				anim: Assets.GetAnim("cactusflower_kanim"),
				initialAnim: "object",
				sceneLayer: Grid.SceneLayer.Front,
				collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				width: 0.6f,
				height: 0.4f,
				isPickupable: true);

			var foodInfo = new EdiblesManager.FoodInfo(
				id: Id,
				dlcId: DlcManager.VANILLA_ID,
				caloriesPerUnit: 1200000f,
				quality: TUNING.FOOD.FOOD_QUALITY_GOOD,
				preserveTemperatue: 255.15f,
				rotTemperature: 277.15f,
				spoilTime: 3600f,
				can_rot: true);

			var foodEntity = EntityTemplates.ExtendEntityToFood(
				template: cactusFlower,
				foodInfo: foodInfo);

			return foodEntity;
		}

		public string[] GetDlcIds() {
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst) { }
	}

}
