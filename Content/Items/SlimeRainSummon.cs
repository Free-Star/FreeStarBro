using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FreeStarBro.Content.Items
{
	public class SlimeRainSummon : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.UseSound = SoundID.Item44;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(silver: 50);
			Item.consumable = false; // 可设置为 true 表示一次性物品
		}

		public override bool CanUseItem(Player player)
		{
			// 若当前已在史莱姆雨中，则不能再次使用
			return !Main.slimeRain;
		}

		public override bool? UseItem(Player player)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) // 单人或服务器
			{
				Main.StartSlimeRain(true); // 立即触发史莱姆雨
				Main.NewText("天空开始下起史莱姆雨！", 50, 255, 130);
			}

			// 添加混沌减益（持续 5 秒 = 300 帧）
			player.AddBuff(BuffID.ChaosState, 300);

			return true;
		}
	}
}
