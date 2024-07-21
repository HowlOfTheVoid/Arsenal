namespace Arsenal
{
	public static class ArsenalCWT
	{
		public class ArsenalCat
		{
			// Makes sure the player is of the class "Arsenal". Set in CTOR
			public bool IsArsenal;

			// Whether or not a sound is already being played for pulling a spear out. 
			public bool PlayingSound;

			public bool CanInstaPole;
			public int ArsInstaPoleCooldown;
			public float ArsSpearCharge;
			public ArsenalCat()
			{
				CanInstaPole = false;
				ArsInstaPoleCooldown = 0;
				ArsSpearCharge = 0f;
			}

			public void setSpearCharge(float p)
			{
				this.ArsSpearCharge = Mathf.Clamp(p, 0f, 1f);
			}

			public void setSoundPlaying(bool b)
			{
				this.PlayingSound = b;
			}

		}

		private static readonly ConditionalWeakTable<Player, ArsenalCat> Arsenal = new ConditionalWeakTable<Player, ArsenalCat>();
		public static ArsenalCat GetCat(this Player player) => Arsenal.GetValue(player, _ => new ArsenalCat());
	}
}
