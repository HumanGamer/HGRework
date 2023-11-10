using Terraria.ModLoader;

namespace HGRework
{
	public class HGRework : Mod
	{
		public static HGRework Instance { get; private set; }

		public override void Load()
		{
			Instance = this;
		}

		public override void Unload()
		{
			Instance = null;
		}
	}
}