using HGRework.Config;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static Terraria.Utilities.NPCUtils;

namespace HGRework.HGWorldGen.Structures
{
	public class PyramidGenerator : ModSystem
	{
		private UnifiedRandom Rand => WorldGen.genRand;

		#region Setup
		// Locations of all pyramids
		private List<Point> _pyramidLocations;

		public override void Load()
		{
			// Initialize the list
			_pyramidLocations = new List<Point>();

			// Hook the vanilla pyramid generation function
			On_WorldGen.Pyramid += On_WorldGen_Pyramid;
		}

		public override void Unload()
		{
			// Unhook the vanilla pyramid generation function
			On_WorldGen.Pyramid -= On_WorldGen_Pyramid;

			// Clear the list
			_pyramidLocations.Clear();
			_pyramidLocations = null;
		}

		private bool On_WorldGen_Pyramid(On_WorldGen.orig_Pyramid orig, int i, int j)
		{
			// If we have already generated the maximum number of pyramids, return false
			if (_pyramidLocations.Count > ModSettings.Instance.MaxPyramids && ModSettings.Instance.ExtraPyramids)
				return false;

			// Otherwise, call the original function
			bool success;
			if (ModSettings.Instance.ReworkPyramids)
				success = GeneratePyramidOverride(i, j);
			else
				success = orig(i, j);

			// If the original function succeeded, add the pyramid location to the list
			if (success)
				_pyramidLocations.Add(new Point(i, j));

			return success;
		}
		#endregion

		#region Pyramid Generation
		private bool GeneratePyramidOverride(int x, int y)
		{
			var targetTile = TileID.SandstoneBrick;
			if (Main.tile[x, y].TileType == TileID.SandstoneBrick || Main.tile[x, y].WallType == WallID.PalmWood)
				return false;

			int extend = Rand.Next(9, 13);
			int height = y + Rand.Next(75 * 2, 125 * 2);//Rand.Next(75, 125);

			int width = GeneratePyramidShape(x, targetTile, y - Rand.Next(0, 7), 1, height);

			GenerateBackWall(x, y, targetTile, width, height);

			int direction = 1;
			if (Rand.NextBool(2))
				direction = -1;

			int roomHeight = Rand.Next(5, 8);
			GenerateMainEntrance(x, y, targetTile, extend, direction, roomHeight);

			var currentPos = GenerateInnerPyramid(x, y, extend, height, ref direction, roomHeight);

			GenerateExtendedCorridor(targetTile, direction, currentPos.X, currentPos.Y, roomHeight);
			return true;
		}

		private static int GeneratePyramidShape(int x, ushort targetTile, int yPos, int width, int height)
		{
			for (int i = yPos; i < height; i++)
			{
				for (int j = x - width; j < x + width - 1; j++)
				{
					Main.tile[j, i].ClearTile();
					WorldGen.PlaceTile(j, i, targetTile, forced: true);
				}

				// If we don't cap this, the pyramid could take up the entire desert.
				// Technically this makes it not a pyramid, but it probably won't be noticeable.
				if (width > 100)
					continue;

				width++;
			}

			return width;
		}

		private static void GenerateBackWall(int x, int y, ushort targetTile, int width, int height)
		{
			for (int i = x - width - 5; i <= x + width + 5; i++)
			{
				for (int j = y - 1; j <= height + 1; j++)
				{
					bool isTargetTile = true;
					for (int k = i - 1; k <= i + 1; k++)
					{
						for (int l = j - 1; l <= j + 1; l++)
						{
							if (Main.tile[k, l].TileType != targetTile)
								isTargetTile = false;
						}
					}

					if (isTargetTile)
					{
						Main.tile[i, j].WallType = WallID.SandstoneBrick;
						WorldGen.SquareWallFrame(i, j);
					}
				}
			}
		}

		private void GenerateMainEntrance(int x, int y, ushort targetTile, int extend, int direction, int roomHeight)
		{
			int num9 = x - extend * direction;
			int startY = y + extend;

			bool flag2 = true;
			while (flag2)
			{
				flag2 = false;
				bool flag3 = false;
				for (int newY = startY; newY <= startY + roomHeight; newY++)
				{
					int newX = num9;
					if (Main.tile[newX, newY - 1].TileType == TileID.Sand)
						flag3 = true;

					if (Main.tile[newX, newY].TileType == targetTile)
					{
						Main.tile[newX, newY].ClearTile();
						Main.tile[newX, newY + 1].WallType = WallID.SandstoneBrick;
						Main.tile[newX + direction, newY].WallType = WallID.SandstoneBrick;
						flag2 = true;
					}

					if (flag3)
					{
						Main.tile[newX, newY].ClearTile();
						WorldGen.PlaceTile(newX, newY, TileID.Sand, forced: true);
					}
				}

				num9 -= direction;
			}
		}

		private Point GenerateInnerPyramid(int x, int y, int extend, int height, ref int direction, int roomHeight)
		{
			int num12 = Rand.Next(20, 30);
			int startX = x - extend * direction;
			int startY = y + extend;
			bool swapDirection = true;
			bool generatedRoom = false;

			int roomCount = 0;

			int minPyramidRooms = ModSettings.Instance.MinPyramidRooms;
			int maxPyramidRooms = ModSettings.Instance.MaxPyramidRooms;

			if (minPyramidRooms > maxPyramidRooms)
				minPyramidRooms = maxPyramidRooms;

			int targetRoomCount = Rand.Next(minPyramidRooms, maxPyramidRooms + 1);

			while (true)
			{
				for (int i = startY; i <= startY + roomHeight; i++)
				{
					Main.tile[startX, i].ClearTile();
					Main.tile[startX, i].WallType = WallID.SandstoneBrick;
				}

				startX += direction;
				startY++;
				num12--;
				if (startY >= height - roomHeight * 2)
					num12 = 10;

				if (num12 <= 0)
				{
					bool success = false;
					if (!swapDirection && !generatedRoom && roomCount < targetRoomCount)
					{
						generatedRoom = true;
						success = true;
						GenerateRoom(ref direction, roomHeight, ref startX, startY);
						roomCount++;
					}

					if (swapDirection)
					{
						swapDirection = false;
						direction *= -1;
						num12 = Rand.Next(15, 20);
					}
					else if (success)
					{
						num12 = Rand.Next(10, 15);

						swapDirection = true;
						generatedRoom = false;
					}
					else
					{
						direction *= -1;
						num12 = Rand.Next(20, 40);
					}
				}

				if (startY >= height - roomHeight)
					break;
			}

			return new Point(startX, startY);
		}

		private void GenerateRoom(ref int direction, int roomHeight, ref int startX, int startY)
		{
			int someNum = Rand.Next(7, 13);
			int num18 = Rand.Next(23, 28);
			int num19 = num18;
			int initialX = startX;
			while (num18 > 0)
			{
				for (int i = 1; i <= 4; i++)
				{
					Main.tile[startX, startY - someNum + roomHeight - i].ClearTile();
					WorldGen.PlaceTile(startX, startY - someNum + roomHeight - i, TileID.SandstoneBrick, forced: true);

					Main.tile[startX, startY + roomHeight + i].ClearTile();
					WorldGen.PlaceTile(startX, startY + roomHeight + i, TileID.SandstoneBrick, forced: true);
				}

				for (int i = startY - someNum + roomHeight; i <= startY + roomHeight; i++)
				{
					if (num18 == num19 || num18 == 1)
					{
						if (i >= startY - someNum + roomHeight + 2)
						{
							Main.tile[startX, i].ClearTile();
							Main.tile[startX, i].WallType = WallID.SandstoneBrick;
						}
					}
					else if (num18 == num19 - 1 || num18 == 2 || num18 == num19 - 2 || num18 == 3)
					{
						if (i >= startY - someNum + roomHeight + 1)
						{
							Main.tile[startX, i].ClearTile();
							Main.tile[startX, i].WallType = WallID.SandstoneBrick;
						}
					}
					else
					{
						Main.tile[startX, i].ClearTile();
						Main.tile[startX, i].WallType = WallID.SandstoneBrick;
					}
				}

				num18--;
				startX += direction;
			}

			int xOffset = startX - direction;
			int left = xOffset;
			int right = initialX;
			if (xOffset > initialX)
			{
				left = initialX;
				right = xOffset;
			}

			PlaceChest(startY, left, right);

			int num26 = Rand.Next(1, 10);
			for (int num27 = 0; num27 < num26; num27++)
			{
				int i2 = Rand.Next(left, right);
				int j2 = startY + roomHeight;
				WorldGen.PlaceSmallPile(i2, j2, Rand.Next(16, 19), 1, TileID.SmallPiles);
			}

			WorldGen.PlaceTile(left + 2, startY - someNum + roomHeight + 1, 91, mute: true, forced: false, -1, Rand.Next(4, 7));
			WorldGen.PlaceTile(left + 3, startY - someNum + roomHeight, 91, mute: true, forced: false, -1, Rand.Next(4, 7));
			WorldGen.PlaceTile(right - 2, startY - someNum + roomHeight + 1, 91, mute: true, forced: false, -1, Rand.Next(4, 7));
			WorldGen.PlaceTile(right - 3, startY - someNum + roomHeight, 91, mute: true, forced: false, -1, Rand.Next(4, 7));
			for (int i = left; i <= right; i++)
			{
				WorldGen.PlacePot(i, startY + roomHeight, TileID.Pots, Rand.Next(25, 28));
			}
		}

		private void PlaceChest(int startY, int left, int right)
		{
			int itemID = Rand.Next(3);
			if (itemID == 0)
				itemID = Rand.Next(3);

			if (Main.tenthAnniversaryWorld && itemID == 0)
				itemID = 1;

			switch (itemID)
			{
				case 0:
					itemID = ItemID.PharaohsMask;
					break;
				case 1:
					itemID = ItemID.SandstorminaBottle;
					break;
				case 2:
					itemID = ItemID.FlyingCarpet;
					break;
			}

			WorldGen.AddBuriedChest((left + right) / 2, startY, itemID, notNearOtherChests: false, 1, trySlope: false, 0);
		}

		private void GenerateExtendedCorridor(ushort targetTile, int direction, int currentX, int startY, int roomHeight)
		{
			int num29 = Rand.Next(100, 200);
			int num30 = Rand.Next(500, 800);
			bool flag2 = true;
			int num12 = Rand.Next(10, 50);
			if (direction == 1)
				currentX -= roomHeight;

			int num32 = Rand.Next(5, 10);
			while (flag2)
			{
				num29--;
				num30--;
				num12--;
				for (int i = currentX - num32 - Rand.Next(0, 2); i <= currentX + roomHeight + num32 + Rand.Next(0, 2); i++)
				{
					int num34 = startY;
					if (i >= currentX && i <= currentX + roomHeight)
					{
						Main.tile[i, num34].ClearTile();
					}
					else
					{
						Main.tile[i, num34].ClearTile();
						WorldGen.PlaceTile(i, num34, targetTile, forced: true);
					}

					if (i >= currentX - 1 && i <= currentX + 1 + roomHeight)
						Main.tile[i, num34].WallType = WallID.SandstoneBrick;
				}

				startY++;
				currentX += direction;
				if (num29 <= 0)
				{
					flag2 = false;
					for (int num35 = currentX + 1; num35 <= currentX + roomHeight - 1; num35++)
					{
						if (Main.tile[num35, startY].HasTile)
							flag2 = true;
					}
				}

				if (num12 < 0)
				{
					num12 = Rand.Next(10, 50);
					direction *= -1;
				}

				if (num30 <= 0)
					flag2 = false;
			}
		}
		#endregion

		#region Extra Pyramids

		protected void GenerateExtraPyramids(int count)
		{
			const int distanceFromWorldEdge = 300;
			const int distanceFromWorldTop = 200;

			const int attemptsPerPyramid = 100000; // This is a lot, but this will ensure we get the correct number of pyramids

			int attempts = attemptsPerPyramid * count;

			Rectangle undergroundDesertLocation = GenVars.UndergroundDesertLocation;
			for (int i = 0; i < attempts; i++)
			{
				if (count <= 0)
					break;

				Point pos;

				// Generate a random position
				pos.X = Rand.Next(distanceFromWorldEdge, Main.maxTilesX - distanceFromWorldEdge);
				pos.Y = Rand.Next(distanceFromWorldTop, (int)Main.worldSurface);

				Point size;
				size.X = 100;
				size.Y = 200;

				Rectangle bounds;
				bounds.X = pos.X - size.X / 2;
				bounds.Y = pos.Y - size.Y / 2;
				bounds.Width = size.X;
				bounds.Height = size.Y;

				var tile = Framing.GetTileSafely(pos);

				// Make sure to generate only on sand
				if (!tile.HasTile || !Main.tileSand[tile.TileType])
					continue;

				// Make sure this is a valid place for the pyramid (we don't want to override anything important!)
				if (GenVars.structures?.CanPlace(bounds) == false)
					continue;

				// Prevent overlapping dungeon
				if (GenVars.dungeonSide < 0 && pos.X < GenVars.dungeonX + Main.maxTilesX * 0.15f ||
					GenVars.dungeonSide > 0 && pos.X > GenVars.dungeonX - Main.maxTilesX * 0.15f)
					continue;

				// Only generate in the underground desert
				if (!undergroundDesertLocation.Contains(pos))
					continue;

				// Move the pyramid up until there is no tile or wall (ie. the surface)
				bool validSpot = false;
				int topY = Math.Max(pos.Y - 100, 40);
				for (int j = pos.Y; j > topY; j--)
				{
					var tile2 = Framing.GetTileSafely(pos.X, j);

					if (tile2.HasTile || tile2.WallType != WallID.None)
						continue;

					// We found a valid spot!
					pos.Y = j;
					validSpot = true;
					break;
				}

				// If we didn't find a valid spot, continue
				if (!validSpot)
					continue;

				// Shift the pyramid up a bit so it's entrance doesn't get buried
				pos.Y -= 15;

				// Generate the pyramid
				if (WorldGen.Pyramid(pos.X, pos.Y))
					count--;
			}
		}

		protected void Generate(GenerationProgress progress, GameConfiguration configuration)
		{
			if (!ModSettings.Instance.ExtraPyramids)
				return;

			int minPyramids = ModSettings.Instance.MinPyramids;
			int maxPyramids = ModSettings.Instance.MaxPyramids;

			// Cap the minimum at the maximum
			if (minPyramids > maxPyramids)
				minPyramids = maxPyramids;

			// Determine the number of pyramids to generate
			int targetPyramidCount = Rand.Next(minPyramids, maxPyramids + 1);

			// Subtract the number of pyramids we have already generated
			targetPyramidCount -= _pyramidLocations.Count;

			// If we still need to generate more pyramids, do so
			if (targetPyramidCount > 0)
				GenerateExtraPyramids(targetPyramidCount);
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			// Clear the list to make sure we don't have any leftover locations
			_pyramidLocations.Clear();

			// Add our pass after the pyramids pass
			int index = tasks.FindIndex(genpass => genpass.Name.Equals("Pyramids"));
			tasks.Insert(index + 1, new PassLegacy("Extra Pyramids", (progress, configuration) =>
			{
				ModContent.GetInstance<PyramidGenerator>().Generate(progress, configuration);
			}));
		}

		#endregion
	}
}
