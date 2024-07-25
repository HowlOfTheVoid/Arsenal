using Expedition;
using IL.Menu;
using IL.MoreSlugcats;
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

		public int[] protectorSquads;
		public int arsenalSquadCooldown = 0;

		private void OnEnable()
		{
			//TODO: Further tweaks to being Arsenal
			On.Player.ctor += PlayerCTORHook;

			//TODO: Producing the Spears
			On.Player.GrabUpdate += ArsenalGrabUpdate;


			/*
             *  TODO LIST:
             *      - Insta-Spear Walls
             *      - Move Tutorial Dialogues
             *      - Spear from mouth? Maybe hunger cost
             *      - Friendly Scav Squads
            */
			// Weaker Spear Throws
			On.Player.ThrownSpear += ArsenalThrowSpear;
			// Friendly Protector Squads
			On.ScavengersWorldAI.Outpost.ctor += ArsenalFriendlyOutpost;
			On.ScavengersWorldAI.Update += ArsenalFriendlyScavs;

			// Arsenal Stats and Initialization for Menu
			On.SlugcatStats.Name.Init += ArsenalAddName;
			On.SlugcatStats.getSlugcatStoryRegions += ArsenalStoryRegions;
			On.SlugcatStats.SlugcatFoodMeter += ArsenalFoodMeter;
			On.SlugcatStats.ctor += ArsenalStatsCtor;
			On.SlugcatStats.NourishmentOfObjectEaten += ArsenalNourishment;
			On.SlugcatStats.AutoGrabBatflys += ArsenalGrabsBatflys;
			On.SlugcatStats.SlugcatStartingKarma += ArsenalStartingKarma;

			// Arsenal Character Select Menu Stuff
			On.Menu.SlugcatSelectMenu.SetSlugcatColorOrder += AddArsenalToOrder;
			On.Menu.SlugcatSelectMenu.SlugcatPage.AddImage += ArsenalMenuImage;
			On.Menu.SlugcatSelectMenu.SlugcatPageNewGame.ctor += ArsenalNewGamePage;
			On.Menu.SlugcatSelectMenu.SlugcatPageContinue.ctor += ArsenalContinuePage;

			// Arsenal Expedition Stuff
			On.Menu.CharacterSelectPage.UpdateSelectedSlugcat += ArsenalSlugcatMenuDesc;
			On.Menu.CharacterSelectPage.GetSlugcatPortrait += ArsenalPortrait;
		}

		// SlugcatStats functions and overrides
		private static readonly SlugcatStats.Name ArsenalName = new SlugcatStats.Name("Arsenal", true);
		private void ArsenalAddName(On.SlugcatStats.Name.orig_Init orig)
		{
			orig();
			ExtEnum<SlugcatStats.Name>.values.AddEntry(ArsenalName.value);
			Expedition.ExpeditionGame.unlockedExpeditionSlugcats.Add(ArsenalName);
			Expedition.ExpeditionGame.playableCharacters.Add(ArsenalName);
		}
		private IntVector2 ArsenalFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
		{
			if(slugcat == ArsenalName)
			{
				return new IntVector2(10, 8);
			}
			return orig(slugcat);
		}
		private void ArsenalStatsCtor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
		{
			orig(self, slugcat, malnourished);
			if(slugcat == ArsenalName)
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
		private int ArsenalNourishment(On.SlugcatStats.orig_NourishmentOfObjectEaten orig, SlugcatStats.Name slugcat, IPlayerEdible eatenObject)
		{
			if(
				// If we are considering Arsenal as the eater
				slugcat == ArsenalName &&
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
		private bool ArsenalGrabsBatflys(On.SlugcatStats.orig_AutoGrabBatflys orig, SlugcatStats.Name slugcatNum)
		{
			if(slugcatNum == ArsenalName)
			{
				return true;
			}
			return orig(slugcatNum);
		}
		private int ArsenalStartingKarma(On.SlugcatStats.orig_SlugcatStartingKarma orig, SlugcatStats.Name slugcatName)
		{
			if(slugcatName == ArsenalName)
			{
				return 2;
			}
			return orig(slugcatName);
		}
		private string[] ArsenalStoryRegions(On.SlugcatStats.orig_getSlugcatStoryRegions orig, SlugcatStats.Name i)
		{
			if (i == ArsenalName)
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
		


		// Arsenal Character Select Stuff
		private void AddArsenalToOrder(On.Menu.SlugcatSelectMenu.orig_SetSlugcatColorOrder orig, Menu.SlugcatSelectMenu self)
		{
			orig(self);
			self.slugcatColorOrder.Add(ArsenalName);
		}
		private void ArsenalMenuImage(On.Menu.SlugcatSelectMenu.SlugcatPage.orig_AddImage orig, Menu.SlugcatSelectMenu.SlugcatPage self, bool ascended)
		{
			if (self.slugcatNumber == ArsenalName)
			{
				if (ascended)
				{
					// SET SCENE ID TO ARSENAL AS GHOST
				}
				else
				{
					// SET SCENE ID TO ARSENAL NORMALLY
				}
/*				self.sceneOffset = new Vector2(10f, 75f);
				self.slugcatDepth = 3f;
				self.markOffset = new Vector2(0f, 0f);
				self.glowOffset = new Vector2(0f, -10f);

				self.sceneOffset.x = self.sceneOffset.x - (1366f - self.menu.manager.rainWorld.options.ScreenSize.x) / 2f;
				self.slugcatImage = new Menu.InteractiveMenuScene(self.menu, self, Menu.MenuScene.SceneID.Slugcat_White); // <-- CHANGE THIS TO SCENE_ID LATER!!!!!
				self.subObjects.Add(self.slugcatImage);
				if (self.HasMark)
				{
					self.markSquare = new FSprite("pixel", true);
					self.markSquare.scale = 14f;
					self.markSquare.color = Color.Lerp(self.effectColor, Color.white, 0.7f);
					self.Container.AddChild(self.markSquare);
					self.markGlow = new FSprite("Futile_White", true);
					self.markGlow.shader = self.menu.manager.rainWorld.Shaders["FlatLight"];
					self.markGlow.color = self.effectColor;
					self.Container.AddChild(self.markGlow);   
				} */
			}
			else
			{
				orig(self, ascended);
			}
		}
		private void ArsenalNewGamePage(On.Menu.SlugcatSelectMenu.SlugcatPageNewGame.orig_ctor orig, Menu.SlugcatSelectMenu.SlugcatPageNewGame self, Menu.Menu menu, Menu.MenuObject owner, int pageIndex, SlugcatStats.Name slugcatNumber)
		{
			orig(self, menu, owner, pageIndex, slugcatNumber);
			Menu.SlugcatSelectMenu slugcatSelectMenu = menu as Menu.SlugcatSelectMenu;
			string name, description;
			if(slugcatNumber == ArsenalName)
			{
				name = menu.Translate("THE ARSENAL");
				description = menu.Translate("A crafty creature with the ability to produce spears, but not the strength to throw them.<LINE>Making friends may be your only means to survival...");
				description = Custom.ReplaceLineDelimeters(description);
				int num = description.Count((char f) => f == '\n');
				float num2 = 0f;
				if (num > 1)
				{
					num2 = 30f;
				}
				self.difficultyLabel = new Menu.MenuLabel(menu, self, name, new Vector2(-1000f, self.imagePos.y - 249f + num2), new Vector2(200f, 30f), true);
				self.difficultyLabel.label.alignment = FLabelAlignment.Center;
				self.subObjects.Add(self.difficultyLabel);
				self.infoLabel = new Menu.MenuLabel(menu, self, description, new Vector2(-1000f, self.imagePos.y - 249f - 60f + num2 / 2f), new Vector2(400f, 60f), true);
				self.infoLabel.label.alignment = FLabelAlignment.Center;
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
		}
		private void ArsenalContinuePage(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_ctor orig, Menu.SlugcatSelectMenu.SlugcatPageContinue self, Menu.Menu menu, Menu.MenuObject owner, int pageIndex, SlugcatStats.Name slugcatNumber)
		{
			orig(self, menu, owner, pageIndex, slugcatNumber);
			if (slugcatNumber == ArsenalName)
			{
				(self as Menu.SlugcatSelectMenu.SlugcatPage).AddImage(self.saveGameData.ascended);
			}
		}


		// Arsenal Expedition Updates
		private void ArsenalSlugcatMenuDesc(On.Menu.CharacterSelectPage.orig_UpdateSelectedSlugcat orig, Menu.CharacterSelectPage self, int num)
		{
			orig(self, num);
			SlugcatStats.Name name = ExpeditionGame.playableCharacters[num];
			ExpeditionData.slugcatPlayer = name;
			if(name == ArsenalName)
			{
				self.slugcatName.text = self.menu.Translate("THE ARSENAL");
				self.slugcatDescription.text = self.menu.Translate("Though it may lack the strength to do so alone,<LINE> with allies alongside them, the Arsenal sets out once more.").Replace("<LINE>", Environment.NewLine);
				ChallengeOrganizer.filterChallengeTypes = new List<string>();
				self.waitForSaveData = true;
			}
		}
		private Menu.MenuIllustration ArsenalPortrait(On.Menu.CharacterSelectPage.orig_GetSlugcatPortrait orig, Menu.CharacterSelectPage self, SlugcatStats.Name slugcat, Vector2 pos)
		{
			if(slugcat == ArsenalName)
			{
				return new Menu.MenuIllustration(self.menu, self, "illustrations", "multiplayerportrait41-arsenal", pos, true, true);
			}
			return orig(self, slugcat, pos);
		}



		// Checks if the player is Arsenal, and sets "isArsenal" to the player's booleans if so. This helps set up a bool for Arsenal's abilities later.
		private void PlayerCTORHook(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
		{
			// Invoke origin. Game can't run if it doesn't run its own stuff too-
			orig(self, abstractCreature, world);

			// Check if slugcat's name is Arsenal, sets class bool if so.
			if (self.slugcatStats.name.value == "EWarsenal")
			{
				self.GetCat().IsArsenal = true;
			}
		}

		// Like Spearmaster, Arsenal has to use GrabUpdate to decide when to pull out spears. This is added to the end of GrabUpdate, so 
		//    we have to do a lot of similar things to it.
		private void ArsenalGrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
		{
			// Invoke origin. Game can't run if it doesn't run its own stuff too-
			orig(self, eu);
			if (
				// Cat type is Arsenal
				self.GetCat().IsArsenal &&
				// Cat is not jumping or throwing
				(!self.input[0].jmp &&
				!self.input[0].thrw) &&
				// Cat IS holding Pickup
				self.input[0].pckp &&
				// Cat has both hands open
				(self.grasps[0] == null && self.grasps[1] == null) &&
				// Nothing in the stomach
				self.objectInStomach == null)
			{

				//Total Spear Progression is set to Arsenal's current spear charge. That way, we know if it's already
				//  part-way though.
				float spearProg = self.GetCat().ArsSpearCharge;
				if (spearProg < 0.1f)
				{
					self.GetCat().setSpearCharge(Mathf.Lerp(spearProg, 0.11f, 0.1f));
				}
				else
				{
					if (!self.GetCat().PlayingSound)
					{
						self.GetCat().setSoundPlaying(true);
						self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.SM_Spear_Pull, 0f, 1f, 1f + UnityEngine.Random.value * 0.5f);
					}
					self.GetCat().setSpearCharge(Mathf.Lerp(spearProg, 1f, 0.05f));
				}

				// Makes head shake if spear is more than halfway ready
				if (spearProg > 0.6f)
				{
					(self.graphicsModule as PlayerGraphics).head.vel += Custom.RNV() * ((spearProg - 0.6f) / 0.4f) * 2f;
				}

				//Once 95% is reached, just sets it to complete.
				if (spearProg > 0.95f)
				{
					self.GetCat().setSpearCharge(1f);
				}

				// If complete, retrieve the spear.
				if (spearProg == 1f)
				{
					// Play snap sound, return "setSoundPlaying" to false for next spear pull.
					self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.SM_Spear_Grab, 0f, 1f, 0.5f + UnityEngine.Random.value * 1.5f);
					self.GetCat().setSoundPlaying(false);

					// Create drips and sparks around the player's head, like spearmaster's tail
					Vector2 pos = (self.graphicsModule as PlayerGraphics).head.pos;
					for (int j = 0; j < 4; j++)
					{
						Vector2 a = Custom.DirVec(pos, self.bodyChunks[1].pos);
						self.room.AddObject(new WaterDrip(pos + Custom.RNV() * UnityEngine.Random.value * 1.5f, Custom.RNV() * 3f * UnityEngine.Random.value + a * Mathf.Lerp(2f, 6f, UnityEngine.Random.value), false));
					}
					for (int k = 0; k < 5; k++)
					{
						Vector2 a2 = Custom.RNV();
						self.room.AddObject(new Spark(pos + a2 * UnityEngine.Random.value * 40f, a2 * Mathf.Lerp(4f, 30f, UnityEngine.Random.value), Color.white, null, 4, 18));
					}

					// Reset Spear Progress.
					self.GetCat().setSpearCharge(0f);

					//Instantiate spear, then put it in your hand, if one is open to take it.
					AbstractSpear abstractSpear = new AbstractSpear(self.room.world, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID(), false);
					abstractSpear.pos = self.abstractCreature.pos;
					abstractSpear.RealizeInRoom();
					Vector2 vector = self.bodyChunks[0].pos;
					Vector2 a3 = Custom.DirVec(self.bodyChunks[1].pos, self.bodyChunks[0].pos);
					if (Mathf.Abs(self.bodyChunks[0].pos.y - self.bodyChunks[1].pos.y) > Mathf.Abs(self.bodyChunks[0].pos.x - self.bodyChunks[1].pos.x) && self.bodyChunks[0].pos.y > self.bodyChunks[1].pos.y)
					{
						vector += Custom.DirVec(self.bodyChunks[1].pos, self.bodyChunks[0].pos) * 5f;
						a3 *= -1f;
						a3.x += 0.4f * (float)self.flipDirection;
						a3.Normalize();
					}
					abstractSpear.realizedObject.firstChunk.HardSetPosition(vector);
					abstractSpear.realizedObject.firstChunk.vel = Vector2.ClampMagnitude((a3 * 2f + Custom.RNV() * UnityEngine.Random.value) / abstractSpear.realizedObject.firstChunk.mass, 6f);
					if (self.FreeHand() > -1)
					{
						self.SlugcatGrab(abstractSpear.realizedObject, self.FreeHand());
					}

					// Sets spear colors by Jolly Custom Color
					if (abstractSpear.type == AbstractPhysicalObject.AbstractObjectType.Spear)
					{
						(abstractSpear.realizedObject as Spear).Spear_makeNeedle(1, true);
						if ((self.graphicsModule as PlayerGraphics).useJollyColor)
						{
							(abstractSpear.realizedObject as Spear).jollyCustomColor = new Color?(PlayerGraphics.JollyColor(self.playerState.playerNumber, 2));
						}
					}
				}
			}
		}

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

		private void ArsenalThrowSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
		{
			orig(self, spear);
			if (self.GetCat().IsArsenal)
			{
				spear.throwModeFrames = 3;
			}
		}
	
	}
}
