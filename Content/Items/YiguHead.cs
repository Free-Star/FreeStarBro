using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FreeStarBro.Content.Items
{
	// This tells tModLoader to look for a texture called MinionBossMask_Head, which is the texture on the player
	// and then registers this item to be accepted in head equip slots
       [AutoloadEquip(EquipType.Head)]
       public class YiguHead : ModItem
       {
               public override void SetStaticDefaults()
               {
                       // 让头饰在各种动作中都正常显示
                       ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
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