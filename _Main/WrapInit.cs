﻿internal class WrapInit
{
	private static bool _initialized;

	// Ensure resources are only loaded once and that failing to load them will not break other mods
	public static On.RainWorld.hook_OnModsInit Wrapper(Action<RainWorld> loadResources)
	{
		return (orig, self) =>
		{
			orig(self);

			try
			{
				if (!_initialized)
				{
					_initialized = true;
					loadResources(self);
				}
			}
			catch (Exception e)
			{
				ArseDebug.LogException(e);
			}
		};
	}
}
