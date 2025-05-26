using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Terraria.ModLoader.Default;
using Terraria.Enums;

namespace FreeStarBro.Content.Tiles
{
    public class ItemCollectorTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileTable[Type] = true;
            
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(
                ModContent.GetInstance<ItemCollectorEntity>().Hook_AfterPlacement,
                -1, 0, true);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"尝试放置收集器: ({i}, {j})", Color.Yellow);
            }
            base.PlaceInWorld(i, j, item);
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"尝试右键点击: ({i}, {j})", Color.Yellow);
            }

            Point16 origin = GetTileOrigin(i, j);
            int entityID = ModContent.GetInstance<ItemCollectorEntity>().Find(origin.X, origin.Y);
            
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"找到的entityID: {entityID}", Color.Yellow);
            }
            
            if (entityID != -1)
            {
                var entity = (ItemCollectorEntity)TileEntity.ByID[entityID];
                if (entity != null)
                {
                    entity.ToggleActive();
                    return true;
                }
            }
            
            return false;
        }

        private Point16 GetTileOrigin(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;
            
            int offsetX = frameX / 18;
            int offsetY = frameY / 18;
            
            return new Point16(i - offsetX, j - offsetY);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Point16 origin = GetTileOrigin(i, j);
            int entityID = ModContent.GetInstance<ItemCollectorEntity>().Find(origin.X, origin.Y);
            if (entityID != -1)
            {
                ModContent.GetInstance<ItemCollectorEntity>().Kill(origin.X, origin.Y);
                if (TileEntity.ByID.ContainsKey(entityID))
                {
                    TileEntity.ByID[entityID] = null;
                }
            }
        }
    }
} 