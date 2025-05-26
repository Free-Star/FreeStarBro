using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System;

namespace FreeStarBro.Content.Items
{
    public class WallBreaker : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(silver: 10);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool? UseItem(Player player)
        {
            // 获取鼠标位置
            Vector2 position = Main.MouseWorld;
            int tileX = (int)(position.X / 16f);
            int tileY = (int)(position.Y / 16f);

            // 检查是否在世界范围内
            if (tileX < 0 || tileX >= Main.maxTilesX || tileY < 0 || tileY >= Main.maxTilesY)
                return false;

            // 创建爆炸效果范围
            int radius = 10;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    int targetX = tileX + i;
                    int targetY = tileY + j;

                    // 检查是否在圆形范围内
                    if (Math.Sqrt(i * i + j * j) <= radius)
                    {
                        if (targetX >= 0 && targetX < Main.maxTilesX && targetY >= 0 && targetY < Main.maxTilesY)
                        {
                            // 只破坏墙壁
                            if (Main.tile[targetX, targetY].WallType != 0)
                            {
                                WorldGen.KillWall(targetX, targetY);
                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                {
                                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, targetX, targetY);
                                }
                            }
                        }
                    }
                }
            }

            // 播放爆炸音效
            SoundEngine.PlaySound(SoundID.Item14, position);

            // 增加粒子效果数量以匹配更大的范围
            for (int i = 0; i < 50; i++)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-4, 4));
                Dust.NewDust(position, 22, 22, DustID.Smoke, velocity.X, velocity.Y);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bomb, 1)
                .AddIngredient(ItemID.Gel, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
} 