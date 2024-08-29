namespace Arsenal
{

	[BepInPlugin("EW.thearsenal", "arsenal", "0.0.1")]

	// Soft Dependency on MSC to make sure MSC Cats get added first
	[BepInDependency("MoreSlugCats", BepInDependency.DependencyFlags.SoftDependency)]
	// Soft Dependency on Slugbase to prevent mess ups in Expedition Menu generation.
	[BepInDependency("slime-cubed.slugbase", BepInDependency.DependencyFlags.SoftDependency)]

	public class ArsenalMain : BaseUnityPlugin
	{
		public const string PLUGIN_GUID = "EW.thearsenal";
		public const string PLUGIN_NAME = "The Arsenal Slugcat";
		public const string PLUGIN_VERSION = "0.0.1";

		public bool IsInit;
		public static readonly SlugcatStats.Name ArsenalName = new SlugcatStats.Name("Arsenal", true);

		private ArsenalOptions Options;

		public ArsenalMain()
		{
			try
			{
				Options = new ArsenalOptions(this, Logger);
			}
			catch(Exception ex)
			{
				ArseDebug.LogError(ex);
				throw;
			}
		}

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

				MachineConnector.SetRegisteredOI("EW.thearsenal", Options);

				On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
				On.GameSession.ctor += GameSessionOnctor;
				// cleanup

				IsInit = true;
			}
			catch (Exception e)
			{
				ArseDebug.LogError(e);
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
	}
}
