using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

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
            // 发射剑气效果
            Item.shoot = ProjectileID.LightBeam;
            Item.shootSpeed = 8f;
        }

        // 动态调整物品大小，使其在挥舞过程中实时随血量变化
        public override void ModifyItemScale(Player player, ref float scale)
        {
            float healthRatio = (float)player.statLife / player.statLifeMax2;
            float scaleMultiplier = 1.0f + (1.0f - healthRatio) * 3.0f;
            scale *= scaleMultiplier;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            float healthRatio = (float)player.statLife / player.statLifeMax2;
            float damageMultiplier = 1.0f + (1.0f - healthRatio) * 4.0f;
            damage *= damageMultiplier;
        }

        // 挥舞时产生粒子效果
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = new Vector2(hitbox.X, hitbox.Y);
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                Dust.NewDustPerfect(pos, DustID.Enchanted_Gold, velocity, 150, default, 1.2f).noGravity = true;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            float adjustedScale = Main.LocalPlayer.GetAdjustedItemScale(Item);
            return new Vector2(-Item.width * (adjustedScale - 1) / 2, -Item.height * (adjustedScale - 1) / 2);
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
