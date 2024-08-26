using BepInEx.Logging;
using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arsenal
{
	public class ArsenalOptions : OptionInterface
	{
		private readonly ManualLogSource Logger;

		public readonly Configurable<bool> EveryoneHasFriendly;

		private UIelement[] UIArrArsenalOptions;
		private UIelement[] UIArrArsenalCredits;

		public ArsenalOptions(ArsenalMain arsenalInstance, ManualLogSource loggerSource)
		{
			Logger = loggerSource;
			
			EveryoneHasFriendly = config.Bind("EveryoneHasFriendly", false);

			ArseDebug.Log("Arsenal Options Initialized!!!");
		}

		public override void Initialize()
		{
			var opTab = new OpTab(this, "Options");
			var credTab = new OpTab(this, "Credits");
			this.Tabs = new[]
			{
				opTab,
				credTab
			};
			UIArrArsenalOptions = new UIelement[]
			{
				new OpLabel(10f, 550f, "Options", true),

				// First Row Options (y = 520f)

				new OpLabel(10f, 520f, "Everyone gets Friendly Scav Squads!"),
				new OpCheckBox(EveryoneHasFriendly, new Vector2(230f, 490f))

			};
			UIArrArsenalCredits = new UIelement[]
			{
				new OpLabel(10f, 550f, "A Word from the Modder", true),

				// First Paragraph (y = 520f)
				new OpLabel(10f, 520f, "I'd like to thank my best friend, NeonCityRain, for being the catalyst for this mod- Honestly if he didn't have"),
				new OpLabel(10f, 510f, "the passion to get into coding and modding Rainworld, I doubt I would either. He helped me from making "),
				new OpLabel(10f, 500f, "the code to sparking ideas, even in cases that I was reaching beyond whatever he did with his "),
				new OpLabel(10f, 490f, "Unbound mod (Or various other mods, lol). Big thanks to you, bro~!"),

				// Second Paragraph (y = 460f)
				new OpLabel(10f, 460f, "And a thanks as well to Arty, Ymir, Moss, Bur, Pix, Tanz, Verd, Anaur, KJ, Forgotten and Sherwood, because"),
				new OpLabel(10f, 450f, "without friends, I probably wouldn't have ended up as passionate about video games as I am now- Because"),
				new OpLabel(10f, 440f, "it's so much more fun with friends. Thank you all <3"),

				// Third Paragraph (y = 410f)
				new OpLabel(10f, 410f, "And to anyone looking to get into modding, I'd suggest finding someone close to you to do it with. The"),
				new OpLabel(10f, 400f, "community can be a chaotic place, and while there's the chance that you do find people out there that vibe"),
				new OpLabel(10f, 390f, "with you, the inherent chaos of the community can pull the rug out from beneath your feet. Find someone "),
				new OpLabel(10f, 380f, "you feel you can rely on as an anchor to encourage you when others might ignore or diss you, and you'll "),
				new OpLabel(10f, 370f, "feel a lot more encouraged to do amazing things.")

			};
			opTab.AddItems(UIArrArsenalOptions);
			credTab.AddItems(UIArrArsenalCredits);
		}
	}
}
