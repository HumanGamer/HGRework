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

		[DefaultValue(1)]
		[Range(0, 3)]
		[Increment(1)]
		[Slider]
		public int MinPyramids;

		[DefaultValue(3)]
		[Range(0, 3)]
		[Increment(1)]
		[Slider]
		public int MaxPyramids;

		[DefaultValue(true)]
		public bool ReworkPyramids;

		[DefaultValue(2)]
		[Range(1, 10)]
		[Increment(1)]
		[Slider]
		public int MinPyramidRooms;

		[DefaultValue(3)]
		[Range(1, 10)]
		[Increment(1)]
		[Slider]
		public int MaxPyramidRooms;

		[Header("Variants")]

		[DefaultValue(true)]
		public bool BothEvils;

		[DefaultValue(true)]
		public bool BothOres;
	}
}
