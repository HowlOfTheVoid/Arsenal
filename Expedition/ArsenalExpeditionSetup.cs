using Expedition;

namespace Arsenal
{
	internal class ArsenalExpeditionSetup
	{
		public static void Init()
		{

			// Arsenal Expedition Stuff
			On.Menu.CharacterSelectPage.UpdateSelectedSlugcat += ArsenalSlugcatMenuDesc;
			On.Menu.CharacterSelectPage.GetSlugcatPortrait += ArsenalPortrait;
		}


		// Arsenal Expedition Updates
		private static void ArsenalSlugcatMenuDesc(On.Menu.CharacterSelectPage.orig_UpdateSelectedSlugcat orig, Menu.CharacterSelectPage self, int num)
		{
			orig(self, num);
			SlugcatStats.Name name = ExpeditionGame.playableCharacters[num];
			ExpeditionData.slugcatPlayer = name;
			if (name == ArsenalMain.ArsenalName)
			{
				self.slugcatName.text = self.menu.Translate("THE ARSENAL");
				self.slugcatDescription.text = self.menu.Translate("Though it may lack the strength to do so alone,<LINE> with allies alongside them, the Arsenal sets out once more.").Replace("<LINE>", Environment.NewLine);
				ChallengeOrganizer.filterChallengeTypes = new List<string>();
				self.waitForSaveData = true;
			}
		}
		private static Menu.MenuIllustration ArsenalPortrait(On.Menu.CharacterSelectPage.orig_GetSlugcatPortrait orig, Menu.CharacterSelectPage self, SlugcatStats.Name slugcat, Vector2 pos)
		{
			if (slugcat == ArsenalMain.ArsenalName)
			{
				return new Menu.MenuIllustration(self.menu, self, "illustrations", "multiplayerportrait41-arsenal", pos, true, true);
			}
			return orig(self, slugcat, pos);
		}

	}
}
