namespace Arsenal
{
	internal class ArsenalSkills
	{

		public static void Init()
		{
			/*
             *  TODO LIST:
             *      - Insta-Spear Walls
             *      - Move Tutorial Dialogues
            */

			// Weaker Spear Throws
			On.Player.ThrownSpear += ArsenalThrowSpear;

			// Producing the Spears and Throwing them from a crawl position
			On.Player.GrabUpdate += ArsenalGrabUpdate;
		}

		// Like Spearmaster, Arsenal has to use GrabUpdate to decide when to pull out spears. This is added to the end of GrabUpdate, so 
		//    we have to do a lot of similar things to it.
		private static void ArsenalGrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
		{

			//Total Spear Progression is set to Arsenal's current spear charge. That way, we know if it's already
			//  part-way though.
			float spearProg = self.GetCat().ArsSpearCharge;
			// Debug.Log("Spear Progression: " + spearProg);

			// Invoke origin. Game can't run if it doesn't run its own stuff too-
			orig(self, eu);
			if (
				// Cat type is Arsenal
				self.slugcatStats.name == ArsenalMain.ArsenalName &&
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

				if (spearProg < 0.1f)
				{
					self.GetCat().setSpearCharge(Mathf.Lerp(spearProg, 0.11f, 0.1f));
				}
				else
				{
					if (!self.GetCat().PlayingSound)
					{
						self.GetCat().setSoundPlaying(true);
						self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Chain_Break, 0f, 0.6f, 1f + UnityEngine.Random.value * 0.5f);
					}
					self.GetCat().setSpearCharge(Mathf.Lerp(spearProg, 1f, 0.05f));
				}

				// Makes head shake if spear is more than halfway ready
				if (spearProg > 0.2f)
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
					self.room.PlaySound(SoundID.Slugcat_Pick_Up_Spear, 0f, 1.5f, 0.5f + UnityEngine.Random.value * 1.5f);
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
					int handUsed = self.FreeHand();
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


					// Speedy Ejection code (Only should trigger if Arsenal is laying down when producing a spear!
					if (self.bodyMode == Player.BodyModeIndex.Crawl && self.FoodInStomach > 0)
					{
						// Throw the Spear with the Strength of Hunter, then Return to Normal
						self.slugcatStats.name = SlugcatStats.Name.Red;
						self.slugcatStats.throwingSkill = 3;
						self.ThrowObject(handUsed, eu);
						self.slugcatStats.name = ArsenalMain.ArsenalName;
						self.slugcatStats.throwingSkill = 0;
						self.SubtractFood(1);
					}

				}
			}
			// If the input button for pickup is released, push the spear back down.
			else if (self.slugcatStats.name == ArsenalMain.ArsenalName && !self.input[0].pckp && spearProg > 0f)
			{
				self.GetCat().setSpearCharge(Mathf.Lerp(spearProg, 0f, 0.07f));

				// If the progress is less than 5%, just set to 0. Reset SoundPlaying so it'll play again!
				if(spearProg < 0.05f)
				{
					self.GetCat().setSpearCharge(0f);
					self.GetCat().setSoundPlaying(false);
				}
			}
		}

		// Throws like a baby now!
		private static void ArsenalThrowSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
		{
			orig(self, spear);
			if (self.slugcatStats.name == ArsenalMain.ArsenalName)
			{
				spear.throwModeFrames = 3;
			}
		}
	}
}
