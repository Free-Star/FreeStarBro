using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using System.Linq;
using System;
using Terraria.DataStructures;
using System.IO;

namespace FreeStarBro.Content.Tiles
{
    public class ItemCollectorEntity : ModTileEntity
    {
        private const int COLLECT_RADIUS = 20;
        private const int CHEST_SEARCH_RADIUS = 15;
        private const int COLLECT_INTERVAL = 60;
        private int collectTimer = 0;
        internal bool isActive = false;
 
        public override bool IsTileValidForEntity(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.HasTile && tile.TileType == ModContent.TileType<ItemCollectorTile>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 2, 2);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
                return -1;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"尝试创建TileEntity: ({i}, {j})", Color.Yellow);
            }

            int placedEntity = Place(i, j);
            
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"创建的TileEntity ID: {placedEntity}", Color.Yellow);
            }

            return placedEntity;
        }

        public override void Update()
        {
            if (!ValidTile())
            {
                Kill(Position.X, Position.Y);
                return;
            }

            if (isActive)
            {
                collectTimer++;
                if (collectTimer >= COLLECT_INTERVAL)
                {
                    collectTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        CollectItems();
                    }
                }

                if (Main.GameUpdateCount % 20 == 0)
                {
                    Vector2 center = new Vector2(Position.X * 16 + 16, Position.Y * 16 + 16);
                    for (int i = 0; i < 360; i += 45)
                    {
                        float angle = i * (float)Math.PI / 180f;
                        Vector2 dustPos = center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * COLLECT_RADIUS * 16f;
                        Color dustColor = isActive ? Color.Green : Color.Red;
                        Dust.NewDustPerfect(dustPos, DustID.MagicMirror, Vector2.Zero, 0, dustColor, 0.5f);
                    }
                }
            }
        }

        private void CollectItems()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            List<Point> chests = new List<Point>();
            Point tilePos = Position.ToPoint();

            for (int i = -CHEST_SEARCH_RADIUS; i <= CHEST_SEARCH_RADIUS; i++)
            {
                for (int j = -CHEST_SEARCH_RADIUS; j <= CHEST_SEARCH_RADIUS; j++)
                {
                    int x = tilePos.X + i;
                    int y = tilePos.Y + j;
                    Tile tile = Main.tile[x, y];

                    if (tile.TileType == TileID.Containers || tile.TileType == TileID.Containers2)
                    {
                        chests.Add(new Point(x, y));
                    }
                }
            }

            Vector2 center = new Vector2(Position.X * 16 + 16, Position.Y * 16 + 16);
            float collectRadius = COLLECT_RADIUS * 16f;

            for (int i = 0; i < Main.maxItems; i++)
            {
                Item item = Main.item[i];
                if (item.active && !item.IsAir && Vector2.Distance(item.Center, center) <= collectRadius)
                {
                    foreach (Point chestPos in chests.OrderBy(p => 
                        Vector2.Distance(new Vector2(p.X * 16, p.Y * 16), item.Center)))
                    {
                        int chestIndex = Chest.FindChest(chestPos.X, chestPos.Y);
                        if (chestIndex >= 0)
                        {
                            Chest chest = Main.chest[chestIndex];
                            if (TryAddToChest(chest, item))
                            {
                                item.active = false;
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                                    for (int slot = 0; slot < Chest.maxItems; slot++)
                                    {
                                        if (chest.item[slot] != null && !chest.item[slot].IsAir)
                                        {
                                            NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, chestIndex, slot);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool TryAddToChest(Chest chest, Item item)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i].type == item.type && chest.item[i].stack < chest.item[i].maxStack)
                {
                    int spaceLeft = chest.item[i].maxStack - chest.item[i].stack;
                    int amountToAdd = Math.Min(spaceLeft, item.stack);
                    chest.item[i].stack += amountToAdd;
                    item.stack -= amountToAdd;
                    if (item.stack <= 0)
                        return true;
                }
            }

            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i].IsAir)
                {
                    chest.item[i] = item.Clone();
                    return true;
                }
            }

            return false;
        }

        public void ShowDebugInfo()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"收集器位置: {Position.X}, {Position.Y}", Color.Yellow);
                
                int chestCount = 0;
                for (int i = -CHEST_SEARCH_RADIUS; i <= CHEST_SEARCH_RADIUS; i++)
                {
                    for (int j = -CHEST_SEARCH_RADIUS; j <= CHEST_SEARCH_RADIUS; j++)
                    {
                        int x = Position.X + i;
                        int y = Position.Y + j;
                        Tile tile = Main.tile[x, y];
                        if (tile.TileType == TileID.Containers || tile.TileType == TileID.Containers2)
                        {
                            chestCount++;
                        }
                    }
                }
                Main.NewText($"范围内箱子数量: {chestCount}", Color.Green);

                int itemCount = 0;
                Vector2 center = new Vector2(Position.X * 16 + 16, Position.Y * 16 + 16);
                float collectRadius = COLLECT_RADIUS * 16f;
                for (int i = 0; i < Main.maxItems; i++)
                {
                    Item item = Main.item[i];
                    if (item.active && !item.IsAir && Vector2.Distance(item.Center, center) <= collectRadius)
                    {
                        itemCount++;
                    }
                }
                Main.NewText($"范围内物品数量: {itemCount}", Color.Orange);
            }
        }

        private bool ValidTile()
        {
            Tile tile = Main.tile[Position.X, Position.Y];
            return tile.HasTile && tile.TileType == ModContent.TileType<ItemCollectorTile>();
        }

        public void ToggleActive()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Main.NewText($"客户端发送切换请求: 当前状态={isActive}", Color.Yellow);
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)FreeStarBro.MessageType.ToggleCollector);
                packet.Write(Position.X);
                packet.Write(Position.Y);
                packet.Write(!isActive);
                packet.Send();
                return;
            }
            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                SetActive(!isActive);
                
                if (Main.netMode == NetmodeID.Server)
                {
                    Main.NewText($"服务器广播新状态: {isActive}", Color.Yellow);
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)FreeStarBro.MessageType.ToggleCollector);
                    packet.Write(Position.X);
                    packet.Write(Position.Y);
                    packet.Write(isActive);
                    packet.Send(-1);
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["isActive"] = isActive;
            tag["collectTimer"] = collectTimer;
        }

        public override void LoadData(TagCompound tag)
        {
            isActive = tag.GetBool("isActive");
            collectTimer = tag.GetInt("collectTimer");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(isActive);
            writer.Write(collectTimer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            isActive = reader.ReadBoolean();
            collectTimer = reader.ReadInt32();
        }

        public void SetActive(bool state)
        {
            Main.NewText($"SetActive被调用: 旧状态={isActive}, 新状态={state}", Color.Yellow);
            isActive = state;
            if (Main.netMode != NetmodeID.Server)
            {
                string message = isActive ? "收集器已启动" : "收集器已关闭";
                Color messageColor = isActive ? Color.Green : Color.Red;
                Main.NewText(message, messageColor);
            }
        }
    }
} 