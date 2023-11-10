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

		[Header("Pyramids")]

		//[Label("Minimum Pyramids")]
		//[Tooltip("The minimum number of pyramids that can generate in a world. (Not guaranteed, but it will try)")]
		[DefaultValue(1)]
		[Range(0, 3)]
		[Increment(1)]
		[Slider]
		public int MinPyramids;

		//[Label("Maximum Pyramids")]
		//[Tooltip("The maximum number of pyramids that can generate in a world.")]
		[DefaultValue(3)]
		[Range(0, 3)]
		[Increment(1)]
		[Slider]
		public int MaxPyramids;

		//[Label("Rework Pyramids")]
		//[Tooltip("Whether or not to rework pyramid generation to have more rooms")]
		[DefaultValue(true)]
		public bool ReworkPyramids;

		//[Label("Minimum Pyramid Rooms")]
		//[Tooltip("The minimum number of rooms that can generate in a pyramid. (Rework Pyramids must be enabled)")]
		[DefaultValue(2)]
		[Range(1, 10)]
		[Increment(1)]
		[Slider]
		public int MinPyramidRooms;

		//[Label("Maximum Pyramid Rooms")]
		//[Tooltip("The maximum number of rooms that can generate in a pyramid. (Rework Pyramids must be enabled)")]
		[DefaultValue(3)]
		[Range(1, 10)]
		[Increment(1)]
		[Slider]
		public int MaxPyramidRooms;

		[Header("Variants")]

		//[Label("Both Evils")]
		//[Tooltip("Whether or not to enable having both corruption and crimson in a world")]
		[DefaultValue(true)]
		public bool BothEvils;

		//[Label("Both Ores")]
		//[Tooltip("Whether or not to enable having both ore variants in a world")]
		[DefaultValue(true)]
		public bool BothOres;
	}
}
