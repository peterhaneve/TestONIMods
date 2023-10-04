using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.PatchManager;
using PeterHan.PLib.UI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RollerSnake {
	public sealed class RollerSnakePatches : UserMod2 {
		internal delegate System.Action CreateDietaryModifier(string id, Tag eggTag,
			HashSet<Tag> foodTags, float modifierPerCal);

		internal delegate void GenerateCreatureDescriptionContainers(GameObject creature,
			List<ContentContainer> containers);

		internal delegate void GenerateImageContainers(Sprite[] sprites,
			List<ContentContainer> containers, ContentContainer.ContentLayout layout);

		internal static readonly CreateDietaryModifier CREATE_DIETARY_MODIFIER =
			typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS).
			CreateStaticDelegate<CreateDietaryModifier>(nameof(CreateDietaryModifier),
			typeof(string), typeof(Tag), typeof(HashSet<Tag>), typeof(float));

		internal static GenerateCreatureDescriptionContainers GENERATE_DESC;
		
		internal static readonly GenerateImageContainers GENERATE_IMAGE_CONTAINERS =
			typeof(CodexEntryGenerator).CreateStaticDelegate<GenerateImageContainers>(
			nameof(GenerateImageContainers), typeof(Sprite[]), typeof(List<ContentContainer>),
			typeof(ContentContainer.ContentLayout));

		private const string TETRAMENT_ICON = "Asteroid_desert_oasis";

		private const string TEST_DESERT_BIOME_ICON = "biomeIconTestdesert";

		private static void AddCritterFluxOMatic(IList<CodexEntry> entries,
				ContentContainer critterContent) {
			int n = entries.Count;
			for (int i = 0; i < n; i++) {
				var entry = entries[i];
				int contentCount = entry.contentContainers.Count;
				if (CodexCache.FormatLinkID(entry.id) == GravitasCreatureManipulatorConfig.
						CODEX_ENTRY_ID && contentCount > 0) {
					entry.InsertContentContainer(contentCount - 1, critterContent);
					break;
				}
			}
		}

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
			var obsTag = SimHashes.Obsidian.CreateTag();
			var prefabTag = SteelRollerSnakeConfig.EggId.ToTag();
			float rate = 0.05f / RollerSnakeTuning.STANDARD_CALORIES_PER_CYCLE;
			var modifier = CREATE_DIETARY_MODIFIER?.Invoke(SteelRollerSnakeConfig.Id,
				prefabTag, new HashSet<Tag>() { obsTag }, rate);
			TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.Add(modifier);
		}

		[HarmonyPatch(typeof(CodexCache), "CollectEntries")]
		public static class CodexCache_CollectEntries_Patch {
			internal static void Postfix(string folder, IList<CodexEntry> __result) {
				// Until we get PLib 4.13 with story trait codex entries
				if (folder.ToUpper() == "STORYTRAITS") {
					var cc = new ContentContainer(new List<ICodexWidget> {
						new CodexText(Strings.Get("STRINGS.CREATURES.FAMILY_PLURAL.ROLLERSNAKESPECIES"),
							CodexTextStyle.Subtitle),
						new CodexText(Strings.Get(
							"STRINGS.CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES_EXPANDED.ROLLERSNAKESPECIES")),
						new CodexText(Strings.Get("STRINGS.CODEX.MYLOG.DIVIDER"))
					}, ContentContainer.ContentLayout.Vertical) {
						lockID = "story_trait_critter_manipulator_rollersnakespecies"
					};
					AddCritterFluxOMatic(__result, cc);
				}
			}
		}

		[HarmonyPatch]
		public class CodexEntryGenerator_GenerateCreatureEntries_Patch {
			internal static MethodBase TargetMethod() {
				// TODO Remove when versions prior to U49-574642 no longer need to be supported
				var method = typeof(CodexEntryGenerator).GetMethodSafe(
					"GenerateCreatureEntries", true, PPatchTools.AnyArguments);
				System.Type targetType;
				if (method == null) {
					targetType = PPatchTools.GetTypeSafe("CodexEntryGenerator_Creatures");
					method = targetType?.GetMethodSafe("GenerateEntries", true,
						PPatchTools.AnyArguments);
				} else
					targetType = typeof(CodexEntryGenerator);
				GENERATE_DESC = targetType?.
					CreateStaticDelegate<GenerateCreatureDescriptionContainers>(
					nameof(GenerateCreatureDescriptionContainers), typeof(GameObject),
					typeof(List<ContentContainer>));
				return method;
			}

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

		/// <summary>
		/// Applied to GravitasCreatureManipulatorConfig to use the correct description for
		/// Roller Snakes when mutated.
		/// </summary>
		[HarmonyPatch(typeof(GravitasCreatureManipulatorConfig), nameof(
			GravitasCreatureManipulatorConfig.GetDescriptionForSpeciesTag))]
		public static class GravitasCreatureManipulatorConfig_GetDescriptionForSpeciesTag_Patch {
			/// <summary>
			/// Applied after GetDescriptionForSpeciesTag runs.
			/// </summary>
			internal static void Postfix(Tag species, ref Option<string> __result) {
				if (species.Name == BaseRollerSnakeConfig.SpeciesId)
					__result = Option.Some<string>(RollerSnakeStrings.CODEX.STORY_TRAITS.
						CRITTER_MANIPULATOR.SPECIES_ENTRIES.SNAKE);
			}
		}

		/// <summary>
		/// Applied to GravitasCreatureManipulatorConfig to use the correct name for Roller
		/// Snakes when mutated.
		/// </summary>
		[HarmonyPatch(typeof(GravitasCreatureManipulatorConfig), nameof(
			GravitasCreatureManipulatorConfig.GetNameForSpeciesTag))]
		public static class GravitasCreatureManipulatorConfig_GetNameForSpeciesTag_Patch {
			/// <summary>
			/// Applied after GetNameForSpeciesTag runs.
			/// </summary>
			internal static void Postfix(Tag species, ref Option<string> __result) {
				if (species.Name == BaseRollerSnakeConfig.SpeciesId)
					__result = Option.Some<string>(RollerSnakeStrings.CREATURES.FAMILY_PLURAL.
						ROLLERSNAKESPECIES);
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
