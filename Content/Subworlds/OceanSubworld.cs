using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace FreeStarBro.Content.Subworlds
{
    // 一个简单的海洋子世界，包含随机小岛、房屋和宝箱
    public class OceanSubworld : Subworld
    {
        public override int Width => 200;
        public override int Height => 600;

        public override List<GenPass> Tasks => new()
        {
            new PassLegacy("Generate", Generate)
        };

        private void Generate(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Generating Ocean";

            Main.worldSurface = 150;
            Main.rockLayer = 400;

            // 清空世界
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Main.tile[i, j].ClearEverything();
                }
            }

            // 填充水面
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = (int)Main.worldSurface; j < Main.maxTilesY; j++)
                {
                    Main.tile[i, j].LiquidAmount = 255;
                    Main.tile[i, j].LiquidType = LiquidID.Water;
                }
            }

            // 随机生成小岛
            int islandCount = WorldGen.genRand.Next(3, 6);
            for (int n = 0; n < islandCount; n++)
            {
                int centerX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                int baseY = (int)Main.worldSurface - WorldGen.genRand.Next(20);
                int width = WorldGen.genRand.Next(8, 12);

                for (int x = -width; x <= width; x++)
                {
                    int worldX = centerX + x;
                    if (worldX <= 0 || worldX >= Main.maxTilesX)
                        continue;

                    int height = (int)(Math.Sin((double)x / width * Math.PI) * 4) + 3;
                    for (int y = 0; y < height; y++)
                    {
                        int worldY = baseY - y;
                        if (worldY <= 0)
                            continue;
                        WorldGen.PlaceTile(worldX, worldY, TileID.Sand, true, true);
                    }
                }

                // 在岛上生成简易房屋和宝箱
                int houseX = centerX;
                int houseY = baseY - 2;

                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        int wx = houseX + x;
                        int wy = houseY + y;
                        if (x == -2 || x == 2 || y == 2)
                            WorldGen.PlaceTile(wx, wy, TileID.WoodBlock, true, true);
                        else if (y == -2)
                            WorldGen.PlaceTile(wx, wy, TileID.Platforms, true, true);
                    }
                }

                WorldGen.AddBuriedChest(houseX, houseY - 1, 0, true, 1);
            }
        }
    }
}
