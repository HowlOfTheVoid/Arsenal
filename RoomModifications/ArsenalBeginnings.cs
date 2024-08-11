using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arsenal
{
	internal class ArsenalBeginnings
	{
		public static void Init()
		{
			On.SaveState.setDenPosition += SetArsenalsDenPosition;
		}

		public static void SetArsenalsDenPosition(On.SaveState.orig_setDenPosition orig, SaveState self)
		{
			// Debug.Log("Started Arsenal Den Position");
			if(self.saveStateNumber == ArsenalMain.ArsenalName || self.saveStateNumber.value == "Arsenal")
			{
				// Debug.Log("Confirmed Arsenal");
				if(WorldLoader.FindRoomFile("SU_S01", false, ".txt") != null)
				{
					// Debug.Log("Found Start Room!");
					self.denPosition = "SU_S01";
					return;
				}
			}
			else
			{
				orig(self);
			}
		}
	}
}
