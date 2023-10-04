using UnityEngine;

namespace RollerSnake {
	[EntityConfigOrder(1)]
	public class BabySteelRollerSnakeConfig : IEntityConfig {
		public const string Id = "RollerSnakeSteelBaby";

		public GameObject CreatePrefab() {
			var rollerSnake = SteelRollerSnakeConfig.CreateSteelRollerSnake(Id, 
				RollerSnakeStrings.CREATURES.SPECIES.ROLLERSNAKE.VARIANT_STEEL.BABY.NAME,
				RollerSnakeStrings.CREATURES.SPECIES.ROLLERSNAKE.VARIANT_STEEL.BABY.DESC,
				"babyrollersnake_kanim", true);
			EntityTemplates.ExtendEntityToBeingABaby(rollerSnake, SteelRollerSnakeConfig.Id);
			return rollerSnake;
		}

		public string[] GetDlcIds() {
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}

		public void OnPrefabInit(GameObject inst) {
		}

		public void OnSpawn(GameObject inst) {
		}
	}
}
