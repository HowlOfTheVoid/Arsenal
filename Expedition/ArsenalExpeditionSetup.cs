using Expedition;
using System.Linq;

namespace Arsenal
{
	internal class ArsenalExpeditionSetup
	{
		public static void Init()
		{

			// Arsenal Expedition Stuff
			On.Menu.CharacterSelectPage.UpdateSelectedSlugcat += ArsenalSlugcatMenuDesc;
			On.Menu.CharacterSelectPage.GetSlugcatPortrait += ArsenalPortrait;
			On.Expedition.ExpeditionData.GetPlayableCharacters += ArsenalPlayable;
			On.Expedition.ExpeditionProgression.CheckUnlocked += IsArsenalUnlocked;
			On.Menu.CharacterSelectPage.ctor += AddArsenalSelectOption;
		}

		private static void AddArsenalSelectOption(On.Menu.CharacterSelectPage.orig_ctor orig, Menu.CharacterSelectPage self, Menu.Menu menu, Menu.MenuObject owner, Vector2 pos)
		{
			orig(self, menu, owner, pos);
			if(!ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "slime-cubed.slugbase"))
			{
				int index = ExpeditionGame.playableCharacters.IndexOf(ArsenalMain.ArsenalName);
				bool greyedOut = !ExpeditionGame.unlockedExpeditionSlugcats.Contains(ExpeditionGame.playableCharacters[index]);

				self.slugcatButtons[index] = new Menu.SelectOneButton(menu, self, "", "SLUG-" + index.ToString(), 
																      new Vector2(360f, 295f), new Vector2(94f, 94f), 
																	  self.slugcatButtons, index);
				self.subObjects.Add(self.slugcatButtons[index]);

				self.slugcatButtons[index].buttonBehav.greyedOut = greyedOut;
				self.slugcatPortraits.Add(self.GetSlugcatPortrait(ExpeditionGame.playableCharacters[index], self.slugcatButtons[index].pos + new Vector2(5f, 5f)));
				self.slugcatPortraits[index].sprite.SetAnchor(0f, 0f);
				self.subObjects.Add(self.slugcatPortraits[index]);
			}
		}

		// TODO: OPTIONAL, make a condition by which Arsenal may be unlocked, then change from true to possibly false-
		private static bool IsArsenalUnlocked(On.Expedition.ExpeditionProgression.orig_CheckUnlocked orig, ProcessManager manager, SlugcatStats.Name slugcat)
		{
			if (slugcat == ArsenalMain.ArsenalName)
			{
				return true;
			}
			else
			{
				return orig(manager, slugcat);
			}
		}

		private static List<SlugcatStats.Name> ArsenalPlayable(On.Expedition.ExpeditionData.orig_GetPlayableCharacters orig)
		{
			List<SlugcatStats.Name> list = orig();
			list.Add(ArsenalMain.ArsenalName);
			return list;
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

				// TODO: BACKGROUND CHANGE
				
				self.waitForSaveData = true;
			}
		}
		private static Menu.MenuIllustration ArsenalPortrait(On.Menu.CharacterSelectPage.orig_GetSlugcatPortrait orig, Menu.CharacterSelectPage self, SlugcatStats.Name slugcat, Vector2 pos)
		{
			if (slugcat == ArsenalMain.ArsenalName)
			{
				return new Menu.MenuIllustration(self.menu, self, "illustrations", "multiplayerportraitarsenal1", pos, true, true);
			}
			return orig(self, slugcat, pos);
		}

	}
}
