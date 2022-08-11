using System.Collections.Generic;
using UnityEngine;

namespace RollerSnake {
	public class AquaBulbSackConfig : IEntityConfig {
		public const string Id = "AquaBulbSack";

		public GameObject CreatePrefab() {
			var looseEntity = EntityTemplates.CreateLooseEntity(
				id: Id,
				name: RollerSnakeStrings.ITEMS.INDUSTRIAL_PRODUCTS.AQUABULBSACK.NAME,
				desc: RollerSnakeStrings.ITEMS.INDUSTRIAL_PRODUCTS.AQUABULBSACK.DESC,
				mass: 1000f,
				unitMass: true,
				anim: Assets.GetAnim("aquabulbsack_kanim"),
				initialAnim: "object",
				sceneLayer: Grid.SceneLayer.BuildingBack,
				collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				width: 0.4f,
				height: 0.8f,
				isPickupable: true,
				additionalTags: new List<Tag>
				{
					GameTags.IndustrialIngredient,
					GameTags.Organics
				});

			looseEntity.AddOrGet<EntitySplitter>();
			looseEntity.AddOrGet<SimpleMassStatusItem>();
			EntityTemplates.CreateAndRegisterCompostableFromPrefab(looseEntity);
			return looseEntity;
		}

		public string[] GetDlcIds() {
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst) { }
	}
}
