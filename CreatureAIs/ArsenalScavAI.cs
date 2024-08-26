using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arsenal.CreatureAIs
{
	internal class ArsenalScavAI
	{
		public int[] protectorSquads;
		public int arsenalSquadCooldown = 0;

		public void Init()
		{
			// ArseDebug.Log("Arsenal Scav AI Initiated!");
			// Friendly Protector Squads
			On.ScavengersWorldAI.Outpost.ctor += ArsenalFriendlyOutpost;
			On.ScavengersWorldAI.Update += ArsenalFriendlyScavs;
		}

		private void ArsenalFriendlyScavs(On.ScavengersWorldAI.orig_Update orig, ScavengersWorldAI self)
		{
			orig(self);

			if (self.world.game.session.characterStats.name.value == "Arsenal")
			{
				if (arsenalSquadCooldown <= 0)
				{
					int l = 0;
				// ArseDebug.Log("Going for Arsenal's special Scav AI!");

				CHECK_WHILE_MARKER:
					while (l < self.world.game.Players.Count)
					{

						float scavLove = self.world.game.session.creatureCommunities.LikeOfPlayer(CreatureCommunities.CommunityID.Scavengers, self.world.RegionNumber, l);
						// ArseDebug.Log("Scav Love for player " + l + ": " + scavLove);
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
							(self.world.game.timeInRegionThisCycle < 4800 ||
							scavLove < 0.0f)))
						{
							resetArsenalSquadCooldown(scavLove, self);
							l++;
							goto CHECK_WHILE_MARKER;
						}
						ArseDebug.Log("Squads on Player: " + self.playerAssignedSquads.Count);
						ArseDebug.Log("Average Squads that should be on Player: " + (protectSquadCount + (int)(scavLove * 2f) + 1));
						if (self.playerAssignedSquads.Count <= protectSquadCount + (int)(scavLove * 2f) &&
							self.world.game.IsStorySession)
						{
							ArseDebug.Log("Checking for Free Scavs");
							ScavengerAbstractAI saai = self.scavengers[UnityEngine.Random.Range(0, self.scavengers.Count)];
							if (saai.squad != null && !saai.squad.HasAMission)
							{
								ArseDebug.Log("Squad with no Mission Found!");
								self.playerAssignedSquads.Add(saai.squad);
								saai.squad.targetCreature = self.world.game.Players[l];
								saai.squad.missionType = ((scavLove > 0f) ?
									ScavengerAbstractAI.ScavengerSquad.MissionID.ProtectCreature :
									ScavengerAbstractAI.ScavengerSquad.MissionID.GuardOutpost);
								resetArsenalSquadCooldown(scavLove, self);
								ArseDebug.Log("-------A PROTECTION SQUAD IS LOOKING FOR PLAYER: " + saai.squad.missionType.ToString());
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
					// ArseDebug.Log("Arsenal Squad Cooldown: " + arsenalSquadCooldown);
					arsenalSquadCooldown--;
				}
			}
		}

		private void resetArsenalSquadCooldown(float like, ScavengersWorldAI self)
		{
			ArseDebug.Log(self.world.game.IsStorySession);
			ArseDebug.Log(self.world.game.session.characterStats.name.value);
			if (self.world.game.IsStorySession)
			{
				if (self.world.region == null)
				{
					ArseDebug.Log("Defaulting Region");
					arsenalSquadCooldown = ((like >= 0f) ? UnityEngine.Random.Range(800, 1300) : UnityEngine.Random.Range(5000, 8000));
					return;
				}
				ArseDebug.Log("Region is: " + self.world.region.name);
				ArseDebug.Log("Delay Repeat Min: " + self.world.region.regionParams.scavengerDelayRepeatMin);
				ArseDebug.Log("Delay Repeat Max: " + self.world.region.regionParams.scavengerDelayRepeatMax);
				ArseDebug.Log("Delay Initial Min: " + self.world.region.regionParams.scavengerDelayInitialMin);
				ArseDebug.Log("Delay Initial Max: " + self.world.region.regionParams.scavengerDelayInitialMax);
				arsenalSquadCooldown = ((like >= 0f) ? UnityEngine.Random.Range
					(self.world.region.regionParams.scavengerDelayRepeatMin,
					self.world.region.regionParams.scavengerDelayRepeatMax) :
					UnityEngine.Random.Range
					(self.world.region.regionParams.scavengerDelayInitialMin,
					self.world.region.regionParams.scavengerDelayInitialMax));
			}
			ArseDebug.Log("Scav Cooldown set to: " + arsenalSquadCooldown);
		}

		private void ArsenalFriendlyOutpost(On.ScavengersWorldAI.Outpost.orig_ctor orig, ScavengersWorldAI.Outpost self, ScavengersWorldAI worldAI, int room)
		{
			orig(self, worldAI, room);
			if (this.protectorSquads == null)
			{
				this.protectorSquads = new int[4];
			}
		}
	}
}
