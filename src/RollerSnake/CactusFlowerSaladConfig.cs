using System.Collections.Generic;
using UnityEngine;

namespace RollerSnake {
	public class CactusFlowerSaladConfig : IEntityConfig {
		public const string Id = "CactusFlowerSalad";
		
		public ComplexRecipe Recipe;

		public GameObject CreatePrefab() {
			var cactusSalad = EntityTemplates.CreateLooseEntity(
				id: Id,
				name: RollerSnakeStrings.ITEMS.FOOD.CACTUSFLOWERSALAD.NAME,
				desc: RollerSnakeStrings.ITEMS.FOOD.CACTUSFLOWERSALAD.DESC,
				mass: 1f,
				unitMass: false,
				anim: Assets.GetAnim("cactusflowersalad_kanim"),
				initialAnim: "object",
				sceneLayer: Grid.SceneLayer.Front,
				collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				width: 0.7f,
				height: 0.5f,
				isPickupable: true);

			var foodInfo = new EdiblesManager.FoodInfo(
				id: Id,
				dlcId: DlcManager.VANILLA_ID,
				caloriesPerUnit: 4200000f,
				quality: TUNING.FOOD.FOOD_QUALITY_AMAZING,
				preserveTemperatue: 255.15f,
				rotTemperature: 277.15f,
				spoilTime: 1200f,
				can_rot: true);

			var foodEntity = EntityTemplates.ExtendEntityToFood(
				template: cactusSalad,
				foodInfo: foodInfo);

			var input = new[] {
				new ComplexRecipe.RecipeElement(CactusFlowerConfig.Id, 2f),
				new ComplexRecipe.RecipeElement(LettuceConfig.ID, 4f)
			};
			var output = new[] { new ComplexRecipe.RecipeElement(Id, 1f) };

			var recipeId = ComplexRecipeManager.MakeRecipeID(
				fabricator: GourmetCookingStationConfig.ID,
				inputs: input,
				outputs: output);

			Recipe = new ComplexRecipe(recipeId, input, output) {
				time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME,
				description = RollerSnakeStrings.ITEMS.FOOD.CACTUSFLOWERSALAD.RECIPEDESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { GourmetCookingStationConfig.ID },
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
