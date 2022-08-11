using UnityEngine;

namespace RollerSnake {
	public class BabyRollerSnakeConfig : IEntityConfig {
		public const string Id = "RollerSnakeBaby";

		public GameObject CreatePrefab() {
			var rollerSnake = RollerSnakeConfig.CreateRollerSnake(Id,
				RollerSnakeStrings.CREATURES.SPECIES.ROLLERSNAKE.BABY.NAME,
				RollerSnakeStrings.CREATURES.SPECIES.ROLLERSNAKE.BABY.DESC,
				"babyrollersnake_kanim", true);
			EntityTemplates.ExtendEntityToBeingABaby(rollerSnake, RollerSnakeConfig.Id);
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
