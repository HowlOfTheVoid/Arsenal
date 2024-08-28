namespace Arsenal
{
	internal class ArsenalDefaultColors
	{
		public static void Init()
		{
			//Sets Default Color of Arsenal
			On.PlayerGraphics.DefaultBodyPartColorHex += ArsenalDefaultColor;
			On.PlayerGraphics.DefaultSlugcatColor += ArsenalSlugcatColor;

		}

		//Set Arsenal's Default General Color
		private static Color ArsenalSlugcatColor(On.PlayerGraphics.orig_DefaultSlugcatColor orig, SlugcatStats.Name slugcatID)
		{
			if (slugcatID == ArsenalMain.ArsenalName)
			{
				return new Color(0.1f, 0.4f, 0.1f);
			}
			else
			{
				return orig(slugcatID);
			}
		}
		//Set Arsenal's Default Body Parts Color
		private static List<string> ArsenalDefaultColor(On.PlayerGraphics.orig_DefaultBodyPartColorHex orig, SlugcatStats.Name slugcatID)
		{
			if (slugcatID == ArsenalMain.ArsenalName)
			{
				List<string> list = new List<string>
				{
					Custom.colorToHex(ArsenalSlugcatColor(PlayerGraphics.DefaultSlugcatColor, slugcatID)),
					"101010"
				};
				return list;
			}
			else
			{
				return orig(slugcatID);
			}
		}
	}
}
