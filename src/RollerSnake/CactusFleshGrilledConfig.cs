using System.Collections.Generic;
using UnityEngine;

namespace RollerSnake {
	public class CactusFleshGrilledConfig : IEntityConfig {
		public const string Id = "CactusFleshGrilled";
		
		public ComplexRecipe Recipe;

		public GameObject CreatePrefab() {
			var cactusFleshGrilled = EntityTemplates.CreateLooseEntity(
				id: Id,
				name: RollerSnakeStrings.ITEMS.FOOD.CACTUSFLESHGRILLED.NAME,
				desc: RollerSnakeStrings.ITEMS.FOOD.CACTUSFLESHGRILLED.DESC,
				mass: 1f,
				unitMass: false,
				anim: Assets.GetAnim("cactusfleshgrilled_kanim"),
				initialAnim: "object",
				sceneLayer: Grid.SceneLayer.Front,
				collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				width: 0.8f,
				height: 0.4f,
				isPickupable: true);

			var foodInfo = new EdiblesManager.FoodInfo(
				id: Id,
				dlcId: DlcManager.VANILLA_ID,
				caloriesPerUnit: 2800000f,
				quality: TUNING.FOOD.FOOD_QUALITY_MEDIOCRE,
				preserveTemperatue: 255.15f,
				rotTemperature: 277.15f,
				spoilTime: 2400f,
				can_rot: true);

			var foodEntity = EntityTemplates.ExtendEntityToFood(
				template: cactusFleshGrilled,
				foodInfo: foodInfo);

			var input = new[] { new ComplexRecipe.RecipeElement(CactusFleshConfig.Id, 2f) };
			var output = new[] { new ComplexRecipe.RecipeElement(Id, 1f) };

			var recipeId = ComplexRecipeManager.MakeRecipeID(
				fabricator: CookingStationConfig.ID,
				inputs: input,
				outputs: output);

			Recipe = new ComplexRecipe(recipeId, input, output) {
				time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME,
				description = RollerSnakeStrings.ITEMS.FOOD.CACTUSFLESHGRILLED.RECIPEDESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { CookingStationConfig.ID },
				sortOrder = 2,
				requiredTech = null
			};

			return foodEntity;
		}

		public string[] GetDlcIds() {
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst) { }

	}
}
