using HGRework.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.Main.CurrentFrameFlags;

namespace HGRework.HGWorldGen
{
	public class DrunkSeedStuff : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			if (ModSettings.Instance.BothEvils)
			{
				// Generate both corruption and crimson
				ForceDrunkPass("Corruption", tasks);

				// Handle altars and shadow orbs
				ForceDrunkPass("Tile Cleanup", tasks);
			}

			if (ModSettings.Instance.BothOres)
			{
				// Generate both ores
				ForceDrunkPass("Shinies", tasks);
			}
		}

		private void ForceDrunkPass(string passName, List<GenPass> tasks)
		{
			int passIndex = tasks.FindIndex(genpass => genpass.Name.Equals(passName));
			tasks.Insert(passIndex, new PassLegacy("Enabling Drunk Seed", (p, c) => WorldGen.drunkWorldGen = true));
			tasks.Insert(passIndex + 2, new PassLegacy("Disabling Drunk Seed", (p, c) => WorldGen.drunkWorldGen = false));
		}
	}
}
