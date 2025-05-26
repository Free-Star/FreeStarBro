using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Chat;
using Terraria.Localization;

namespace FreeStarBro.Content.Items
{
    public class TravelingMerchantSummon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 99;
            Item.value = Item.buyPrice(gold: 5); // 5金币
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            if (!NPC.AnyNPCs(NPCID.TravellingMerchant))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.TravellingMerchant);
                    
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText("旅商被召唤到了你的位置！", Color.LightGreen);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("旅商被召唤到了玩家的位置！"), Color.LightGreen);
                    }
                }
                return true;
            }
            else
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText("旅商已经在这个世界中了！", Color.Red);
                }
                return false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldCoin, 5) // 5金币
                .AddIngredient(ItemID.Lens, 1) // 镜片
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
} 