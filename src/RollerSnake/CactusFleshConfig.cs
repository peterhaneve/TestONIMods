﻿using UnityEngine;

namespace RollerSnake {
	public class CactusFleshConfig : IEntityConfig {
		public const string Id = "CactusFlesh";

		public GameObject CreatePrefab() {
			var cactusFlesh = EntityTemplates.CreateLooseEntity(
				id: Id,
				name: RollerSnakeStrings.ITEMS.FOOD.CACTUSFLESH.NAME,
				desc: RollerSnakeStrings.ITEMS.FOOD.CACTUSFLESH.DESC,
				mass: 1f,
				unitMass: false,
				anim: Assets.GetAnim("cactusflesh_kanim"),
				initialAnim: "object",
				sceneLayer: Grid.SceneLayer.Front,
				collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				width: 0.8f,
				height: 0.4f,
				isPickupable: true);

			var foodInfo = new EdiblesManager.FoodInfo(
				id: Id,
				dlcId: DlcManager.VANILLA_ID,
				caloriesPerUnit: 1200000f,
				quality: TUNING.FOOD.FOOD_QUALITY_AWFUL,
				preserveTemperatue: 255.15f,
				rotTemperature: 277.15f,
				spoilTime: 2400f,
				can_rot: true);

			var foodEntity = EntityTemplates.ExtendEntityToFood(
				template: cactusFlesh,
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
