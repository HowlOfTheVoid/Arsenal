using System;
using System.Reflection;
using BepInEx;
using CoralBrain;
using MonoMod.RuntimeDetour;
using MoreSlugcats;
using On;
using On.CoralBrain;
using On.MoreSlugcats;
using OverseerHolograms;
using RWCustom;
using UnityEngine;

namespace Arsenal
{
	[BepInDependency("slime-cubed.slugbase", BepInDependency.DependencyFlags.HardDependency)]

	[BepInPlugin("EW.thearsenal", "EW.thearsenal", "0.0.1")]
	public class ArsenalMain : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "EW.thearsenal";
        public const string PLUGIN_NAME = "The Arsenal Slugcat";
        public const string PLUGIN_VERSION = "0.0.1";

		private void OnEnable()
        {
            On.Player.Update += PlayerUpdateHook;
        }

        void PlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
        }
    }
}
