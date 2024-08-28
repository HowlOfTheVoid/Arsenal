using Expedition;

namespace Arsenal
{
	internal class ArsenalStats
	{
		public static void Init()
		{
			// Arsenal Stats and Initialization for Menu
			On.SlugcatStats.Name.Init += ArsenalAddName;
			On.SlugcatStats.getSlugcatStoryRegions += ArsenalStoryRegions;
			On.SlugcatStats.SlugcatFoodMeter += ArsenalFoodMeter;
			On.SlugcatStats.ctor += ArsenalStatsCtor;
			On.SlugcatStats.NourishmentOfObjectEaten += ArsenalNourishment;
			On.SlugcatStats.AutoGrabBatflys += ArsenalGrabsBatflys;
			On.SlugcatStats.SlugcatStartingKarma += ArsenalStartingKarma;
		}
		private static void ArsenalAddName(On.SlugcatStats.Name.orig_Init orig)
		{
			orig();
			ExtEnum<SlugcatStats.Name>.values.AddEntry(ArsenalMain.ArsenalName.value);
			ExpeditionGame.unlockedExpeditionSlugcats.Add(ArsenalMain.ArsenalName);
			ExpeditionGame.playableCharacters.Add(ArsenalMain.ArsenalName);
		}
		private static IntVector2 ArsenalFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
		{
			if (slugcat == ArsenalMain.ArsenalName)
			{
				return new IntVector2(10, 8);
			}
			return orig(slugcat);
		}
		private static void ArsenalStatsCtor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
		{
			orig(self, slugcat, malnourished);
			if (slugcat == ArsenalMain.ArsenalName)
			{
				self.bodyWeightFac = 1.4f;
				self.generalVisibilityBonus = -0.1f;
				self.visualStealthInSneakMode = 0.6f;
				self.loudnessFac = 0.8f;
				self.lungsFac = 1f;
				self.throwingSkill = 0;
				self.poleClimbSpeedFac = 1.2f;
				self.corridorClimbSpeedFac = 1.2f;
				self.runspeedFac = 1.2f;
				if (malnourished)
				{
					self.bodyWeightFac = 1.6f;
					self.poleClimbSpeedFac = 0.8f;
					self.corridorClimbSpeedFac = 0.8f;
					self.loudnessFac = 0.4f;
				}
			}
		}
		private static int ArsenalNourishment(On.SlugcatStats.orig_NourishmentOfObjectEaten orig, SlugcatStats.Name slugcat, IPlayerEdible eatenObject)
		{
			if (
				// If we are considering Arsenal as the eater
				slugcat == ArsenalMain.ArsenalName &&
				// AND if it is a corpse of a creature
				eatenObject is Creature && (eatenObject as Creature).dead &&
				// AND The corpse is not a Fly or Baby Needleworm
				!(eatenObject is Fly || eatenObject is SmallNeedleWorm ||
				// OR If it is a Centipede, it better not be small
				(eatenObject is Centipede && (eatenObject as Centipede).Small)))
			{
				// Value of body is multiplied by 1.5.
				return 6 * eatenObject.FoodPoints;
			}
			return orig(slugcat, eatenObject);
		}
		private static bool ArsenalGrabsBatflys(On.SlugcatStats.orig_AutoGrabBatflys orig, SlugcatStats.Name slugcatNum)
		{
			if (slugcatNum == ArsenalMain.ArsenalName)
			{
				return true;
			}
			return orig(slugcatNum);
		}
		private static int ArsenalStartingKarma(On.SlugcatStats.orig_SlugcatStartingKarma orig, SlugcatStats.Name slugcatName)
		{
			if (slugcatName == ArsenalMain.ArsenalName)
			{
				return 2;
			}
			return orig(slugcatName);
		}
		private static string[] ArsenalStoryRegions(On.SlugcatStats.orig_getSlugcatStoryRegions orig, SlugcatStats.Name i)
		{
			if (i == ArsenalMain.ArsenalName)
			{
				string[] source = new string[]
				{
					"SU",
					"HI",
					"DS",
					"CC",
					"GW",
					"SH",
					"VS",
					"LM",
					"SI",
					"LF",
					"UW",
					"SS",
					"SB",
					"DM"
				};
				return source;
			}
			else
			{
				return orig(i);
			}
		}
	}
}
