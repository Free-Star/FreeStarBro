using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FreeStarBro.Content.Items
{
	// This tells tModLoader to look for a texture called MinionBossMask_Head, which is the texture on the player
	// and then registers this item to be accepted in head equip slots
	[AutoloadEquip(EquipType.Head)]
	public class MakaHead : ModItem
	{
		public override void SetStaticDefaults()
		{
			// 强制在所有动作状态下都使用这张贴图
			ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = true;
			ArmorIDs.Head.Sets.UseAltFaceHeadDraw[Item.headSlot] = false;
		}

		public override void SetDefaults() {
			Item.width = 32;
			Item.height = 32;

			// Common values for every boss mask
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 75);
			Item.vanity = true;
			Item.maxStack = 1;
		}
	}
}