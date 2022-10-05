using System.Collections.Generic;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.PatchManager;
using PeterHan.PLib.UI;
using ProcGen;
using UnityEngine;

namespace RollerSnake {
	public sealed class RollerSnakePatches : UserMod2 {
		internal delegate System.Action CreateDietaryModifier(string id, Tag eggTag,
			TagBits foodTags, float modifierPerCal);

		internal delegate System.Action CreateDietaryModifierNew(string id, Tag eggTag,
			HashSet<Tag> foodTags, float modifierPerCal);

		internal delegate void GenerateCreatureDescriptionContainers(GameObject creature,
			List<ContentContainer> containers);

		internal delegate void GenerateImageContainers(Sprite[] sprites,
			List<ContentContainer> containers, ContentContainer.ContentLayout layout);

		internal static readonly CreateDietaryModifier CREATE_DIETARY_MODIFIER =
			typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS).
			CreateStaticDelegate<CreateDietaryModifier>(nameof(CreateDietaryModifier),
			typeof(string), typeof(Tag), typeof(TagBits), typeof(float));

		internal static readonly CreateDietaryModifierNew CREATE_DIETARY_MODIFIER_NEW =
			typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS).
			CreateStaticDelegate<CreateDietaryModifierNew>(nameof(CreateDietaryModifier),
			typeof(string), typeof(Tag), typeof(HashSet<Tag>), typeof(float));

		internal static readonly GenerateCreatureDescriptionContainers GENERATE_DESC =
			typeof(CodexEntryGenerator).CreateStaticDelegate<GenerateCreatureDescriptionContainers>(
			nameof(GenerateCreatureDescriptionContainers), typeof(GameObject),
			typeof(List<ContentContainer>));

		internal static readonly GenerateImageContainers GENERATE_IMAGE_CONTAINERS =
			typeof(CodexEntryGenerator).CreateStaticDelegate<GenerateImageContainers>(
			nameof(GenerateImageContainers), typeof(Sprite[]), typeof(List<ContentContainer>),
			typeof(ContentContainer.ContentLayout));

		private const string TETRAMENT_ICON = "Asteroid_desert_oasis";

		private const string TEST_DESERT_BIOME_ICON = "biomeIconTestdesert";

		private static void AddFlowerInfoDescriptors(ICollection<Descriptor> descriptors) {
			var flowerTag = CactusFlowerConfig.Id.ToTag();
			var prefab = Assets.GetPrefab(flowerTag);
			float kcal = 0.0f, perUnit = 0.0f;
			string desc = string.Empty;
			if (prefab.TryGetComponent(out Edible edible)) {
				perUnit = edible.FoodInfo.CaloriesPerUnit;
				kcal = CactusFlowerConfig.UnitsToSpawn;
			}
			if (prefab.TryGetComponent(out InfoDescription component2))
				desc = component2.description;
			string amount = GameTags.DisplayAsCalories.Contains(flowerTag) ? GameUtil.
				GetFormattedCalories(kcal) : (GameTags.DisplayAsUnits.Contains(flowerTag) ?
				GameUtil.GetFormattedUnits(CactusFlowerConfig.UnitsToSpawn, GameUtil.TimeSlice.
				None, false) : GameUtil.GetFormattedMass(CactusFlowerConfig.UnitsToSpawn));
			descriptors.Add(new Descriptor(string.Format(STRINGS.UI.UISIDESCREENS.
				PLANTERSIDESCREEN.YIELD, prefab.GetProperName(), amount), string.Format(
				STRINGS.UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, desc,
				GameUtil.GetFormattedCalories(perUnit), GameUtil.GetFormattedCalories(kcal))));
		}

		private static void AddToCodex(Tag speciesTag, string name,
				IDictionary<string, CodexEntry> results) {
			string tagStr = speciesTag.ToString();
			var brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
			var entry = new CodexEntry(nameof(STRINGS.CREATURES), new List<ContentContainer> {
				new ContentContainer(new List<ICodexWidget> {
					new CodexSpacer(),
					new CodexSpacer()
				}, ContentContainer.ContentLayout.Vertical)
			}, name) {
				parentId = nameof(STRINGS.CREATURES)
			};
			CodexCache.AddEntry(tagStr, entry);
			results.Add(tagStr, entry);
			// Find all critters with this tag
			foreach (var prefab in brains)
				if (prefab.GetDef<BabyMonitor.Def>() == null && prefab.TryGetComponent(
						out CreatureBrain brain) && brain.species == speciesTag) {
					Sprite babySprite = null;
					string prefabID = prefab.PrefabID().Name;
					var baby = Assets.TryGetPrefab(prefabID + "Baby");
					var contentContainerList = new List<ContentContainer>(4);
					var first = Def.GetUISprite(prefab, brain.symbolPrefix + "ui").first;
					if (baby != null)
						babySprite = Def.GetUISprite(baby).first;
					if (babySprite != null)
						GENERATE_IMAGE_CONTAINERS.Invoke(new [] { first, babySprite },
							contentContainerList, ContentContainer.ContentLayout.Horizontal);
					else
						contentContainerList.Add(new ContentContainer(new List<ICodexWidget> {
							new CodexImage(128, 128, first)
						}, ContentContainer.ContentLayout.Vertical));
					GENERATE_DESC.Invoke(prefab, contentContainerList);
					entry.subEntries.Add(new SubEntry(prefabID, tagStr, contentContainerList,
							prefab.GetProperName()) {
						icon = first,
						iconColor = Color.white
					});
				}
		}

		[PLibMethod(RunAt.BeforeDbInit)]
		internal static void BeforeDbInit() {
			var tetramentIcon = PUIUtils.LoadSprite("RollerSnake." + TETRAMENT_ICON + ".png");
			var desertBiomeIcon = PUIUtils.LoadSprite("RollerSnake." + TEST_DESERT_BIOME_ICON +
				".png");
			tetramentIcon.name = TETRAMENT_ICON;
			desertBiomeIcon.name = TEST_DESERT_BIOME_ICON;
			Assets.Sprites.Add(TETRAMENT_ICON, tetramentIcon);
			Assets.Sprites.Add(TEST_DESERT_BIOME_ICON, desertBiomeIcon);
			LocString.CreateLocStringKeys(typeof(RollerSnakeStrings.CLUSTER_NAMES));
			LocString.CreateLocStringKeys(typeof(RollerSnakeStrings.CODEX));
			LocString.CreateLocStringKeys(typeof(RollerSnakeStrings.CREATURES));
			LocString.CreateLocStringKeys(typeof(RollerSnakeStrings.ITEMS));
			LocString.CreateLocStringKeys(typeof(RollerSnakeStrings.SUBWORLDS));
			LocString.CreateLocStringKeys(typeof(RollerSnakeStrings.WORLDS));
			TUNING.CROPS.CROP_TYPES.Add(new Crop.CropVal(CactusFleshConfig.Id,
				CactusFruitConfig.CyclesForGrowth * 600.0f));
		}

		private static void SpawnCactusFlower(Crop plant) {
			// Spawn secondary crop product, 1 unit flower per 5 units flesh (default)
			var flower = GameUtil.KInstantiate(Assets.GetPrefab(CactusFlowerConfig.Id),
				plant.transform.position + new Vector3(0.0f, 0.75f, 0.0f),
				Grid.SceneLayer.Ore);
			if (flower != null) {
				flower.SetActive(true);
				// Not a seed, do not roll for mutations
				if (flower.TryGetComponent(out PrimaryElement pe) && plant.TryGetComponent(
						out PrimaryElement plantPE)) {
					pe.Units = CactusFlowerConfig.UnitsToSpawn;
					pe.Temperature = plantPE.Temperature;
				}
				plant.Trigger((int)GameHashes.CropSpawned, flower);
				if (flower.TryGetComponent(out Edible edible))
					ReportManager.Instance.ReportValue(ReportManager.ReportType.
						CaloriesCreated, edible.Calories, StringFormatter.Replace(STRINGS.UI.
						ENDOFDAYREPORT.NOTES.HARVESTED, "{0}", edible.GetProperName()),
						STRINGS.UI.ENDOFDAYREPORT.NOTES.HARVESTED_CONTEXT);
			} else
				DebugUtil.LogErrorArgs(plant.gameObject,
					"Tried to spawn an invalid crop prefab:", CactusFlowerConfig.Id);
		}

		public override void OnLoad(Harmony harmony) {
			base.OnLoad(harmony);
			PUtil.InitLibrary();
			new PLocalization().Register();
			var codex = new PCodexManager();
			codex.RegisterCreatures();
			codex.RegisterPlants();
			new PPatchManager(harmony).RegisterPatchClass(typeof(RollerSnakePatches));
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods) {
			// Improve Not Enough Tags compatibility
			System.Action modifier = null;
			var obsTag = SimHashes.Obsidian.CreateTag();
			var prefabTag = SteelRollerSnakeConfig.EggId.ToTag();
			float rate = 0.05f / RollerSnakeTuning.STANDARD_CALORIES_PER_CYCLE;
			if (CREATE_DIETARY_MODIFIER_NEW != null)
				modifier = CREATE_DIETARY_MODIFIER_NEW.Invoke(SteelRollerSnakeConfig.Id,
					prefabTag, new HashSet<Tag>() { obsTag }, rate);
			else if (CREATE_DIETARY_MODIFIER != null) {
				var eggBits = new TagBits();
				eggBits.SetTag(obsTag);
				modifier = CREATE_DIETARY_MODIFIER.Invoke(SteelRollerSnakeConfig.Id, prefabTag,
					eggBits, rate);
			}
			TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(modifier);
		}

		[HarmonyPatch(typeof(CodexEntryGenerator), "GenerateCreatureEntries")]
		public class CodexEntryGenerator_GenerateCreatureEntries_Patch {
			public static void Postfix(Dictionary<string, CodexEntry> __result) {
				AddToCodex(BaseRollerSnakeConfig.SpeciesId, RollerSnakeStrings.CREATURES.
					FAMILY_PLURAL.ROLLERSNAKESPECIES, __result);
			}
		}

		[HarmonyPatch(typeof(CreatureFeederConfig), nameof(CreatureFeederConfig.
			ConfigurePost))]
		public static class CreatureFeederConfig_ConfigurePost_Patch {
			public static void Postfix(BuildingDef def) {
				if (def.BuildingComplete.TryGetComponent(out Storage storage)) {
					var filters = storage.storageFilters;
					var targetSpecies = new [] { BaseRollerSnakeConfig.SpeciesId.ToTag() };
					foreach (var collectDiet in DietManager.CollectDiets(targetSpecies))
						filters.Add(collectDiet.Key);
				}
			}
		}

		[HarmonyPatch(typeof(Crop), nameof(Crop.InformationDescriptors))]
		public class Crop_InformationDescriptors_Patch {
			public static void Postfix(Crop __instance, List<Descriptor> __result) {
				if (__instance != null && __instance.cropVal.cropId == CactusFleshConfig.Id)
					AddFlowerInfoDescriptors(__result);
			}
		}

		[HarmonyPatch(typeof(Crop), nameof(Crop.SpawnSomeFruit))]
		public class Crop_SpawnSomeFruit_Patch {
			public static void Postfix(Tag cropID, Crop __instance) {
				if (cropID == CactusFleshConfig.Id.ToTag())
					SpawnCactusFlower(__instance);
			}
		}
		
		[HarmonyPatch(typeof(Immigration), "ConfigureCarePackages")]
		public static class Immigration_ConfigureCarePackages_Patch {
			public static void Postfix(ref CarePackageInfo[] ___carePackages) {
				// Runs for both DLC and Base Game
				___carePackages = new List<CarePackageInfo>(___carePackages) {
					// Spiky Succulent Seed x1: always available
					new CarePackageInfo(CactusFruitConfig.SeedId, 1f, () => true),
					// Flower Salad x3kg: available on or after cycle 48
					new CarePackageInfo(CactusFlowerSaladConfig.Id, 3f, () => GameClock.
						Instance.GetCycle() >= 48),
					// Roller Snakelet x1: always available
					new CarePackageInfo(BabyRollerSnakeConfig.Id, 1f, () => true),
					// Roller Snake Egg x3: always available
					new CarePackageInfo(RollerSnakeConfig.EggId, 3f, () => true),
					// Aqua Bulb Sack x6: always available
					new CarePackageInfo(AquaBulbSackConfig.Id, 6f, () => true)
				}.ToArray();
			}
		}

		[HarmonyPatch(typeof(RockCrusherConfig), nameof(RockCrusherConfig.
			ConfigureBuildingTemplate))]
		public static class RockCrusherConfig_ConfigureBuildingTemplate_Patch {
			public static void Postfix() {
				// Aqua Bulb Sack x1 to Water 1000 kg
				var ingredients = new[] {
					new ComplexRecipe.RecipeElement(AquaBulbSackConfig.Id.ToTag(), 1f)
				};
				var results = new[] {
					new ComplexRecipe.RecipeElement(GameTags.Water, 1000f)
				};
				new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(RockCrusherConfig.ID,
						ingredients, results), ingredients, results) {
					time = 40f,
					nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
					description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.
						RECIPE_DESCRIPTION, RollerSnakeStrings.ITEMS.INDUSTRIAL_PRODUCTS.
						AQUABULBSACK.NAME, ElementLoader.FindElementByHash(SimHashes.Water).
						name)
				}.fabricators = new List<Tag>
				{
					RockCrusherConfig.ID.ToTag()
				};
			}
		}
	}
}
