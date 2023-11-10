using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace HGRework.Config
{
	public class ModSettings : ModConfig
	{
		public static ModSettings Instance;

		public override ConfigScope Mode => ConfigScope.ServerSide;

		//[Header("WorldGen")]

		[Label("Minimum Pyramids")]
		[Tooltip("The minimum number of pyramids that can generate in a world.")]
		[DefaultValue(1)]
		[Range(0, 3)]
		[Increment(1)]
		[Slider]
		public int MinPyramids;

		[Label("Maximum Pyramids")]
		[Tooltip("The maximum number of pyramids that can generate in a world.")]
		[DefaultValue(3)]
		[Range(0, 3)]
		[Increment(1)]
		[Slider]
		public int MaxPyramids;

		[Label("Rework Pyramids")]
		[Tooltip("Whether or not to enable reworked pyramid generation with more rooms")]
		[DefaultValue(true)]
		public bool ReworkPyramids;

		[Label("Both Evils")]
		[Tooltip("Whether or not to enable having both corruption and crimson in a world")]
		[DefaultValue(true)]
		public bool BothEvils;

		[Label("Both Ores")]
		[Tooltip("Whether or not to enable having both ore variants in a world")]
		[DefaultValue(true)]
		public bool BothOres;
	}
}
