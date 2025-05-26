using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.Chat;
using Terraria.DataStructures;
using FreeStarBro.Content.Tiles;

namespace FreeStarBro
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class FreeStarBro : Mod
	{
		public enum MessageType : byte
		{
			SwapAnglerQuest,
			ToggleCollector
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();
			switch (msgType)
			{
				case MessageType.SwapAnglerQuest:
					if (Main.netMode == NetmodeID.Server)
					{
						Main.AnglerQuestSwap();
						ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("老弟成功威胁渔夫换了个新任务！"), Color.Orange);
						NetMessage.SendData(MessageID.AnglerQuest);
					}
					break;

				case MessageType.ToggleCollector:
					int x = reader.ReadInt32();
					int y = reader.ReadInt32();
					bool newState = reader.ReadBoolean();

					Main.NewText($"收到ToggleCollector消息: ({x}, {y}), newState={newState}, NetMode={Main.netMode}", Color.Yellow);

					if (Main.netMode == NetmodeID.Server)
					{
						int entityID = ModContent.GetInstance<ItemCollectorEntity>().Find(x, y);
						Main.NewText($"服务器找到的entityID: {entityID}", Color.Yellow);
						
						if (entityID != -1)
						{
							var entity = (ItemCollectorEntity)TileEntity.ByID[entityID];
							if (entity != null)
							{
								entity.SetActive(newState);
								
								ModPacket packet = GetPacket();
								packet.Write((byte)MessageType.ToggleCollector);
								packet.Write(x);
								packet.Write(y);
								packet.Write(newState);
								packet.Send(-1, whoAmI);
								Main.NewText($"服务器已广播状态: {newState}", Color.Yellow);
							}
						}
					}
					else if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						int entityID = ModContent.GetInstance<ItemCollectorEntity>().Find(x, y);
						Main.NewText($"客户端找到的entityID: {entityID}", Color.Yellow);
						
						if (entityID != -1)
						{
							var entity = (ItemCollectorEntity)TileEntity.ByID[entityID];
							if (entity != null)
							{
								entity.SetActive(newState);
								Main.NewText($"客户端已更新状态", Color.Yellow);
							}
						}
					}
					break;
			}
		}

		public override void Load()
		{
			// TileEntity会自动注册，不需要手动注册
		}
	}
}
