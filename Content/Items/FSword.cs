using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FreeStarBro.Content.Items
{
    public class FSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 66;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 15;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 6);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem == Item)
            {
                float healthRatio = (float)player.statLife / player.statLifeMax2;
                float scaleMultiplier = 1.0f + (1.0f - healthRatio) * 3.0f;
                Item.scale = scaleMultiplier;
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            float healthRatio = (float)player.statLife / player.statLifeMax2;
            float damageMultiplier = 1.0f + (1.0f - healthRatio) * 4.0f;
            damage *= damageMultiplier;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-Item.width * (Item.scale - 1) / 2, -Item.height * (Item.scale - 1) / 2);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
