using Menu;
using System.Linq;

namespace Arsenal
{
	internal class ArsenalSelectMenu
	{
		public static void Init()
		{
			// Arsenal Character Select Menu Stuff
			On.Menu.SlugcatSelectMenu.SetSlugcatColorOrder += AddArsenalToOrder;
			On.Menu.SlugcatSelectMenu.SlugcatPage.AddImage += ArsenalMenuImage;
			On.Menu.SlugcatSelectMenu.SlugcatPageNewGame.ctor += ArsenalNewGamePage;
			On.Menu.SlugcatSelectMenu.SlugcatPageContinue.ctor += ArsenalContinuePage;
		}
		// Arsenal Character Select Stuff
		private static void AddArsenalToOrder(On.Menu.SlugcatSelectMenu.orig_SetSlugcatColorOrder orig, Menu.SlugcatSelectMenu self)
		{
			orig(self);
			self.slugcatColorOrder.Add(ArsenalMain.ArsenalName);
		}
		private static void ArsenalMenuImage(On.Menu.SlugcatSelectMenu.SlugcatPage.orig_AddImage orig, Menu.SlugcatSelectMenu.SlugcatPage self, bool ascended)
		{
			if (self.slugcatNumber != ArsenalMain.ArsenalName)
			{
				orig(self, ascended);
			}
			else
			{
				orig(self, ascended);
			}
		}
		private static void ArsenalNewGamePage(On.Menu.SlugcatSelectMenu.SlugcatPageNewGame.orig_ctor orig, Menu.SlugcatSelectMenu.SlugcatPageNewGame self, Menu.Menu menu, Menu.MenuObject owner, int pageIndex, SlugcatStats.Name slugcatNumber)
		{
			if (slugcatNumber != ArsenalMain.ArsenalName)
			{
				Debug.Log("Not Arsenal Name for Menu");
				orig(self, menu, owner, pageIndex, slugcatNumber);
				return;
			}
			Debug.Log("Arsenal Detected!");
			Menu.SlugcatSelectMenu slugcatSelectMenu = menu as Menu.SlugcatSelectMenu;

			self.menu = menu;
			self.owner = owner;
			self.subObjects = new List<Menu.MenuObject>();
			self.nextSelectable = new Menu.MenuObject[4];
			self.pos = new Vector2(0.33334f, 0.33334f);
			self.lastPos = self.pos;
			self.name = "Slugcat_Page_";
			self.index = pageIndex;
			self.selectables = new List<SelectableMenuObject>();
			self.mouseCursor = new Menu.MouseCursor(menu, self, new Vector2(-100f, -100f));
			self.subObjects.Add(self.mouseCursor);
			self.slugcatNumber = slugcatNumber;
			self.effectColor = PlayerGraphics.DefaultSlugcatColor(slugcatNumber);

			self.AddImage(false);

			Debug.Log(slugcatNumber);

			string text = "";
			string text2 = "";
			if (slugcatNumber == ArsenalMain.ArsenalName)
			{
				text = menu.Translate("THE ARSENAL");
				text2 = menu.Translate("A crafty creature with the ability to produce spears, but not the strength to throw them.<LINE>Making friends may be your only means to survival...");
			}
			else
			{
			}
			text2 = Custom.ReplaceLineDelimeters(text2);
			int num = text2.Count((char f) => f == '\n');
			float num2 = 0f;
			if (num > 1)
			{
				num2 = 30f;
			}
			MenuLabel difficultyLabel = new MenuLabel(menu, self, text, new Vector2(-1000f, self.imagePos.y - 249f + num2), new Vector2(200f, 30f), true, null);
			difficultyLabel.label.alignment = FLabelAlignment.Center;
			self.difficultyLabel = difficultyLabel;
			self.subObjects.Add(self.difficultyLabel);
			MenuLabel infoLabel = new MenuLabel(menu, self, text2, new Vector2(-1000f, self.imagePos.y - 249f - 60f + num2 / 2f), new Vector2(400f, 60f), true, null);
			infoLabel.label.alignment = FLabelAlignment.Center;
			self.infoLabel = infoLabel;
			self.subObjects.Add(self.infoLabel);
			if (num > 1)
			{
				self.imagePos.y = self.imagePos.y + 30f;
				self.sceneOffset.y = self.sceneOffset.y + 30f;
			}
			if (!slugcatSelectMenu.SlugcatUnlocked(slugcatNumber))
			{
				self.difficultyLabel.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey);
				self.infoLabel.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey);
				return;
			}
			self.difficultyLabel.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.MediumGrey);
			self.infoLabel.label.color = Menu.Menu.MenuRGB(Menu.Menu.MenuColors.DarkGrey);
		}
		private static void ArsenalContinuePage(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_ctor orig, Menu.SlugcatSelectMenu.SlugcatPageContinue self, Menu.Menu menu, Menu.MenuObject owner, int pageIndex, SlugcatStats.Name slugcatNumber)
		{
			orig(self, menu, owner, pageIndex, slugcatNumber);
			if (slugcatNumber == ArsenalMain.ArsenalName)
			{

			}
		}

	}

}
