using STRINGS;
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable InconsistentNaming

namespace RollerSnake {
	internal static class RollerSnakeStrings {
		public static class CLUSTER_NAMES {
			public static class TETRAMENT_CLUSTER {
				public static LocString NAME = "Tetrament Cluster";
				public static LocString DESCRIPTION = "A hot, dry cluster of visitable planetoids made from the remains of an anomalous planet.";
			}

			public static class VANILLA_TETRAMENT_CLUSTER {
				public static LocString NAME = "Tetrament Classic Cluster";
				public static LocString DESCRIPTION = "A cluster of planets with a larger desert planetoid to start on, replicating the layout of the vanilla game's Tetrament asteroid.";
			}
		}

		public static class CODEX {
			public static class AQUABULB {
				public static LocString TITLE = "Aqua Bulb";
				public static LocString SUBTITLE = "Aqueous Plant";

				public static class BODY {
					public static LocString CONTAINER1 = "The Aqua Bulb stores large amounts of water in its bulb. Its thick protective membrane prevents critters from eating the plant for its liquid reserves.";
				}
			}

			public static class CACTUSFRUIT {
				public static LocString TITLE = "Spiky Succulent";
				public static LocString SUBTITLE = "Edible Plant";

				public static class BODY {
					public static LocString CONTAINER1 = "The Spiky Succulent is a curious plant. Able to survive without water for long periods of drought, it only truly thrives and fruits when given ample irrigation.\n\nProducing both an edible flesh and flower, cultivating the Spiky Succulent provides a more varied diet for Duplicants than other edible plants.";
				}
			}

			public static class ROLLERSNAKE {
				public static LocString TITLE = "Roller Snake";
				public static LocString SUBTITLE = "Domesticable Critter";
				public static LocString SPECIES_TITLE = "Roller Snakes";
				public static LocString SPECIES_SUBTITLE = "Critter Species";

				public static class BODY {
					public static LocString CONTAINER1 = "The reptilian species of Roller Snakes moves in the most peculiar way. Coiling up upon itself and rolling along in a loop is its preferred means of transportation.\n\nDespite their dry native environment, Roller Snakes do not drown in liquids.";
					public static LocString CONTAINER2 = "The rattle on the tail of the Roller Snake, while effective at warning and deterring predators in the wild, is entirely decorative in captivity and can be shorn off without any damage to the animal.\n\nBecause the metal in a Roller Snake's diet is accumulated in its rattle, their rattles are highly sought after for metallurgy.";
				}
			}

			public static class ROLLERSNAKESTEEL {
				public static LocString TITLE = "Tough Roller Snake";
				public static LocString SUBTITLE = "Critter Morph";

				public static class BODY {
					public static LocString CONTAINER1 = "<smallcaps>Pictured: \"Tough\" Roller Snake variant</smallcaps>";
					public static LocString CONTAINER2 = "Roller Snakes are known to be quite kind to their caretakers and do not bite or otherwise retaliate in response to grooming or shearing.";
				}
			}

			public static class STORY_TRAITS {
				public static class CRITTER_MANIPULATOR {
					public static class SPECIES_ENTRIES {
						public static LocString SNAKE = "There's a snake in my boot! Review data for more information.";
					}

					public static class SPECIES_ENTRIES_EXPANDED {
						public static LocString ROLLERSNAKESPECIES = "When this creature tried to roll out of the building, the scanner nearly became caught in an infinite loop.";
					}
				}
			}
		}

		public static class CREATURES {
			public static class FAMILY {
				public static LocString ROLLERSNAKESPECIES = UI.FormatAsLink("Roller Snake", RollerSnakeConfig.Id.ToUpperInvariant());
			}

			public static class FAMILY_PLURAL {
				public static LocString ROLLERSNAKESPECIES = "Roller Snakes";
			}

			public static class SPECIES {
				public static class AQUABULB {
					public static LocString NAME = UI.FormatAsLink("Aqua Bulb", AquaBulbConfig.Id.ToUpperInvariant());
					public static LocString DESC = UI.FormatAsLink("Aqua Bulbs", AquaBulbConfig.Id.ToUpperInvariant()) +
						" are incapable of propagating but can be harvested for a single, " +
						ELEMENTS.WATER.NAME + "-filled sack. The " +
						BUILDINGS.PREFABS.ROCKCRUSHER.NAME +
						" can be used to extract the water within it.";
				}

				public static class CACTUSFRUIT {
					public static LocString NAME = UI.FormatAsLink("Spiky Succulent", CactusFruitConfig.Id.ToUpperInvariant());
					public static LocString DESC = "Spiky to the touch, the " + NAME +
						" produces both an edible " +
						UI.FormatAsLink("Flesh", CactusFleshConfig.Id.ToUpperInvariant()) +
						" and " +
						UI.FormatAsLink("Flower", CactusFlowerConfig.Id.ToUpperInvariant()) +
						".";
				}

				public static class SEEDS {
					public static class CACTUSFRUITSEED {
						public static LocString NAME = UI.FormatAsLink("Spiky Succulent Seed", CactusFruitConfig.Id.ToUpperInvariant());
						public static LocString DESC = "The beginnings of a " +
							CACTUSFRUIT.NAME + ". Just add " + ELEMENTS.WATER.NAME + ".";
						public static LocString DOMESTICATEDDESC = "This cactus " +
							UI.FormatAsLink("Plant", "PLANTS") +
							" survives well in the harsh, dry desert. To thrive and flower, it needs copious amounts of " +
							ELEMENTS.WATER.NAME + ".";
					}
				}

				public static class ROLLERSNAKE {
					public static LocString NAME = UI.FormatAsLink("Roller Snake", RollerSnakeConfig.Id.ToUpperInvariant());
					public static LocString DESC = "A peculiar critter that moves by winding into a loop and rolling.";
					public static LocString EGG_NAME = UI.FormatAsLink("Roller Snakelet Egg", RollerSnakeConfig.Id.ToUpperInvariant());

					public static class BABY {
						public static LocString NAME = UI.FormatAsLink("Roller Snakelet", RollerSnakeConfig.Id.ToUpperInvariant());
						public static LocString DESC = "The young of a " +
							ROLLERSNAKE.NAME +
							". Smaller and has trouble rolling up into an actual loop.";
					}

					public static class VARIANT_STEEL {
						public static LocString NAME = UI.FormatAsLink("Tough Roller Snake", SteelRollerSnakeConfig.Id.ToUpperInvariant());
						public static LocString DESC = "A peculiar critter that moves by winding into a loop and rolling.";
						public static LocString EGG_NAME = UI.FormatAsLink("Tough Roller Snakelet Egg", SteelRollerSnakeConfig.Id.ToUpperInvariant());

						public static class BABY {
							public static LocString NAME = UI.FormatAsLink("Tough Roller Snakelet", SteelRollerSnakeConfig.Id.ToUpperInvariant());
							public static LocString DESC = "The young of a " +
								VARIANT_STEEL.NAME +
								". Smaller and has trouble rolling up into an actual loop.";
						}
					}
				}
			}
		}

		public static class ITEMS {
			public static class INDUSTRIAL_PRODUCTS {
				public static class AQUABULBSACK {
					public static LocString NAME = UI.FormatAsLink("Aqua Bulb Sack", AquaBulbSackConfig.Id.ToUpperInvariant());
					public static LocString DESC = "The bulbous, water-filled membrane of a " +
						CREATURES.SPECIES.AQUABULB.NAME +
						". The water within can be used by Duplicants once the membrane is ruptured. The " +
						BUILDINGS.PREFABS.ROCKCRUSHER.NAME +
						" can be used to extract the water within.";
				}
			}

			public static class FOOD {
				public static class CACTUSFLESH {
					public static LocString NAME = UI.FormatAsLink("Cactus Flesh", CactusFleshConfig.Id.ToUpperInvariant());
					public static LocString DESC = "The barely edible flesh of a " +
						CREATURES.SPECIES.CACTUSFRUIT.NAME + ".";
				}

				public static class CACTUSFLESHGRILLED {
					public static LocString NAME = UI.FormatAsLink("Grilled Cactus Flesh", CactusFleshGrilledConfig.Id.ToUpperInvariant());
					public static LocString DESC = "The grilled flesh of a " +
						CREATURES.SPECIES.CACTUSFRUIT.NAME +
						".\n\nGrilling renders the prickly bits mostly harmless.";
					public static LocString RECIPEDESC = "Tasty grilled " +
						CACTUSFLESH.NAME + ".";
				}

				public static class CACTUSFLOWER {
					public static LocString NAME = UI.FormatAsLink("Cactus Flower", CactusFlowerConfig.Id.ToUpperInvariant());
					public static LocString DESC = "The edible and sugary red flower of a " +
						CREATURES.SPECIES.CACTUSFRUIT.NAME + ".";
				}

				public static class CACTUSFLOWERSALAD {
					public static LocString NAME = UI.FormatAsLink("Flower Salad", CactusFlowerSaladConfig.Id.ToUpperInvariant());
					public static LocString DESC = "Delectable " + CACTUSFLOWER.NAME +
						" mixed with crunchy " + STRINGS.ITEMS.FOOD.LETTUCE.NAME +
						".\n\nThe blend of cool and sweet makes one's taste buds delight.";
					public static LocString RECIPEDESC = "Mixed and seared " + NAME + ".";
				}
			}
		}

		public static class SUBWORLDS {
			public static class TESTDESERT {
				public static LocString NAME = "Desert";

				public static LocString DESC = "Warm and dry, this biome contains sparse " +
					UI.FormatAsLink("plant life", "PLANTS") + " and " +
					UI.FormatAsLink("critters", "CREATURES") +
					" adapted to its climate. With some adaptation, these resources can be used to sustain a growing colony.";

				public static LocString UTILITY =
					ELEMENTS.SAND.NAME + " and " + ELEMENTS.SANDSTONE.NAME +
					" dominate this biome, with small pockets of " +
					UI.FormatAsLink("Gas", "GAS") + " mixed in. Deposits of " +
					ELEMENTS.IRONORE.NAME + " and " + ELEMENTS.FOSSIL.NAME +
					" will be useful for industry, and scans suggest the presence of " +
					ELEMENTS.CRUDEOIL.NAME + " and " + ELEMENTS.CARBON.NAME +
					" for power.\n\nThough there are few sources of " +
					ELEMENTS.WATER.NAME + " or " + ELEMENTS.OXYGEN.NAME +
					" here, there is nothing inherently dangerous here to harm my Duplicants. It should be safe enough to explore.";
			}
		}

		public static class WORLDS {
			public static class TETRAMENT {
				public static LocString NAME = "Tetrament";
				public static LocString DESCRIPTION = "A location with moderately hot temperatures, Tetrament is home to large expanses of hot, dry desert in addition to its lush forests, swamps, and jungles.\n\n<smallcaps>Tetrament is an ideal location for Duplicant life. Its environment is quite different from other locations but is not any more challenging.</smallcaps>\n\n";
			}

			public static class TETRAMENT_WARP {
				public static LocString NAME = "Dry Radioactive Forest Asteroid";
				public static LocString DESCRIPTION = "A large-ish forested asteroid with a frozen radioactive core.\n\n<smallcaps>While Dry Radioactive Forest Asteroids are largely foresty, they also contain a great deal of rust and salt.</smallcaps>";
			}
		}
	}
}
