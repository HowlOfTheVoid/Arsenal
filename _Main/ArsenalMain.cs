using Arsenal.CreatureAIs;
using Arsenal.Expedition;
using Expedition;
using IL.Menu;
using IL.MoreSlugcats;
using Menu;
using Mono.Cecil;
using MonoMod;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Arsenal
{

	[BepInPlugin(MOD_ID, MOD_ID, PLUGIN_VERSION)]
	public class ArsenalMain : BaseUnityPlugin
	{
		private const string MOD_ID = "EW.thearsenal";
		public const string PLUGIN_GUID = "EW.thearsenal";
		public const string PLUGIN_NAME = "The Arsenal Slugcat";
		public const string PLUGIN_VERSION = "0.0.1";

		public bool IsInit;
		public static readonly SlugcatStats.Name ArsenalName = new SlugcatStats.Name("Arsenal", true);

		private void OnEnable()
		{
			On.Player.ctor += PlayerCTORHook;

			On.RainWorld.OnModsInit += WrapInit.Wrapper(LoadResources);

			ArsenalSelectMenu.Init();
			ArsenalStats.Init();
			ArsenalSkills.Init();
			ArsenalExpeditionSetup.Init();
			ArsenalDefaultColors.Init();
			ArsenalBeginnings.Init();

			ArsenalScavAI arsenalScavAI = new ArsenalScavAI();
			arsenalScavAI.Init();

		}

		private void LoadResources(RainWorld rainWorld)
		{
			// Futile.atlasManager.LoadImage("");
			try
			{
				if (IsInit) return;


				On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
				On.GameSession.ctor += GameSessionOnctor;
				// cleanup

				// MachineConnector.SetRegisteredOI("NCR.theunbound", UnbOptions);
				IsInit = true;
			}
			catch (Exception e)
			{
				Logger.LogError(e);
				throw;
			}

		}

		private void RainWorldGameOnShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
		{
			orig(self);
			ClearMemory();
		}
		private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
		{
			orig(self, game);
			ClearMemory();
		}
		private void ClearMemory()
		{
			// clear collections here

		}

		// Checks if the player is Arsenal, and sets "isArsenal" to the player's booleans if so. This helps set up a bool for Arsenal's abilities later.
		private void PlayerCTORHook(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
		{
			// Invoke origin. Game can't run if it doesn't run its own stuff too-
			orig(self, abstractCreature, world);

			// Check if slugcat's name is Arsenal, sets class bool if so.
			if (self.slugcatStats.name == ArsenalName)
			{
				self.GetCat().IsArsenal = true;
			}
		}
		/*
		private void ArsenalFriendlyScavs(On.ScavengersWorldAI.orig_Update orig, ScavengersWorldAI self)
		{
			orig(self);


			if (arsenalSquadCooldown <= 0)
			{
				int l = 0;
				// Debug.Log("Going for Arsenal's special Scav AI!");

			CHECK_WHILE_MARKER:
				while (l < self.world.game.Players.Count)
				{

					float scavLove = self.world.game.session.creatureCommunities.LikeOfPlayer(CreatureCommunities.CommunityID.Scavengers, self.world.RegionNumber, l);
					// Debug.Log("Scav Love for player " + l + ": " + scavLove);
					int protectSquadCount = 0;
					if (scavLove > 0.5f)
					{
						for (int m = 0; m < self.outPosts.Count; m++)
						{
							protectSquadCount += this.protectorSquads[l];
						}
						for (int n = 0; n < self.traders.Count; n++)
						{
							if (self.traders[n].transgressedByPlayer)
							{
								protectSquadCount--;
							}
						}
					}
					if (
						self.world.game.Players[l].Room.shelter || 
						self.world.game.Players[l].Room.gate ||
						(self.world.game.IsStorySession &&
						self.world.game.session.characterStats.name.value == "EWarsenal" &&
						(self.world.game.timeInRegionThisCycle < 4800 ||
						scavLove < 0.0f)))
					{
						resetArsenalSquadCooldown(scavLove, self);
						l++;
						goto CHECK_WHILE_MARKER;
					}
					// Debug.Log("Squads on Player: " + self.playerAssignedSquads.Count);
					// Debug.Log("Average Squads that should be on Player: " + (protectSquadCount + (int)(scavLove * 2f) + 1));
					if (self.playerAssignedSquads.Count <= protectSquadCount + (int)(scavLove * 2f) &&
						self.world.game.IsStorySession &&
						self.world.game.session.characterStats.name.value == "EWarsenal")
					{
						// Debug.Log("Checking for Free Scavs");
						ScavengerAbstractAI saai = self.scavengers[UnityEngine.Random.Range(0, self.scavengers.Count)];
						if (saai.squad != null && !saai.squad.HasAMission)
						{
							// Debug.Log("Squad with no Mission Found!");
							self.playerAssignedSquads.Add(saai.squad);
							saai.squad.targetCreature = self.world.game.Players[l];
							saai.squad.missionType = ((scavLove > 0f) ?
								ScavengerAbstractAI.ScavengerSquad.MissionID.ProtectCreature :
								ScavengerAbstractAI.ScavengerSquad.MissionID.GuardOutpost);
							resetArsenalSquadCooldown(scavLove, self);
							Debug.Log("-------A PROTECTION SQUAD IS LOOKING FOR PLAYER: " + saai.squad.missionType.ToString());
						}
					}
					else
					{
						resetArsenalSquadCooldown(scavLove, self);
					}
					l++;
					goto CHECK_WHILE_MARKER;
				}
			}
			else
			{
				arsenalSquadCooldown--;
			}
		}

		private void resetArsenalSquadCooldown(float like, ScavengersWorldAI self)
		{
			if(self.world.game.IsStorySession &&
				self.world.game.session.characterStats.name.value == "EWarsenal")
			{
				if(self.world.region == null)
				{
					arsenalSquadCooldown = ((like >= 0f) ? UnityEngine.Random.Range(800, 1300) : UnityEngine.Random.Range(5000, 8000));
					return;
				}
				arsenalSquadCooldown = ((like >= 0f) ? UnityEngine.Random.Range
					(self.world.region.regionParams.scavengerDelayRepeatMin,
					self.world.region.regionParams.scavengerDelayRepeatMax) :
					UnityEngine.Random.Range
					(self.world.region.regionParams.scavengerDelayInitialMin,
					self.world.region.regionParams.scavengerDelayInitialMax));
			}
		}

		private void ArsenalFriendlyOutpost(On.ScavengersWorldAI.Outpost.orig_ctor orig, ScavengersWorldAI.Outpost self, ScavengersWorldAI worldAI, int room)
		{
			orig(self, worldAI, room);
			if (this.protectorSquads == null)
			{
				this.protectorSquads = new int[4];
			}
		} 
		*/
	}
}
