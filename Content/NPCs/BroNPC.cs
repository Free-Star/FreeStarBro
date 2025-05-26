using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Microsoft.Xna.Framework;
using Terraria.Utilities;
using System.Collections.Generic;
using Terraria.DataStructures;
using System.Linq;
using FreeStarBro.Content.Items;

namespace FreeStarBro.Content.NPCs
{
    [AutoloadHead]
    public class BroNPC : ModNPC
    {
        public readonly static List<Item> shopItems = new();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; // 动画帧数
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.HatOffsetY[Type] = 4;
            NPCID.Sets.NoTownNPCHappiness[Type] = true; // 禁用 NPC 幸福度

            // 影响 NPC 在图鉴中的显示方式
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers
            {
                Velocity = 2f, // 在图鉴中显示 NPC 移动速度
                Direction = -1 // -1 为左，1 为右
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // 设置为镇民 NPC
            NPC.friendly = true; // 友好 NPC
            NPC.width = 18; // 宽度
            NPC.height = 40; // 高度
            NPC.aiStyle = 7; // 镇民 AI
            NPC.damage = 10; // 攻击力
            NPC.defense = 15; // 防御
            NPC.lifeMax = 250; // 最大生命值
            NPC.HitSound = SoundID.NPCHit1; // 受击音效
            NPC.DeathSound = SoundID.NPCDeath1; // 死亡音效
            NPC.knockBackResist = 0.5f; // 击退抗性
            AnimationType = NPCID.Guide; // 动画样式
            Banner = NPC.type;
        }


        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            // 例如，当玩家拥有至少一个木头时，允许NPC生成
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && player.inventory.Any(item => item.type == ItemID.Wood && item.stack > 0))
                {
                    return true;
                }
            }
            return false;
        }

        public override void AddShops()
        {
            var shop = new NPCShop(Type, "Shop");

            // 固定出售的物品
            shop.Add(ItemID.Piano); // 钢琴
            shop.Add(ItemID.LifeCrystal); // 生命水晶
            shop.Add(ItemID.ApprenticeBait); 
            shop.Add(ItemID.JourneymanBait); 
            shop.Add(ItemID.MasterBait);
            shop.Add(ModContent.ItemType<TravelingMerchantSummon>()); // 添加旅商召唤铃
            shop.Add(ModContent.ItemType<WallBreaker>()); // 添加墙壁雷管
            shop.Add(ModContent.ItemType<MakaHead>());
            shop.Add(ModContent.ItemType<YiguHead>());
            shop.Add(ModContent.ItemType<SlimeRainSummon>());
            // shop.Add(ModContent.ItemType<ItemCollector>()); // 添加物品收集器
            
            // 随机出售物品
            foreach (var itemID in GetRandomItems(new List<int> {
                ItemID.MagicMirror,
                ItemID.SlimeStaff,
                ItemID.IronBow,
                ItemID.FlamingArrow,
                ItemID.CloudinaBottle,
            }, 3))
            {
                shop.Add(itemID); // 添加随机物品
            }



            // 注册商店到游戏
            shop.Register();
        }


        public override void OnSpawn(IEntitySource source)
        {
            AddShops(); // 初始化商店物品

            if (Main.netMode == NetmodeID.Server) // 在多人游戏中同步商店物品
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }


        public override string GetChat()
        {
            // 随机聊天内容
            WeightedRandom<string> chat = new WeightedRandom<string>();
            chat.Add("嘿！大哥，要买点东西吗？");
            chat.Add("冒险有我在才好玩！");
            chat.Add("需要帮忙解决怪物？我可不怕！");
            chat.Add("老登，爆点金币？");
            chat.Add("Man！");
            chat.Add("What Can I say！");
            chat.Add("来让我玩打野！");
            chat.Add("天地玄黄，宇宙洪荒");

            return chat; // 返回聊天内容
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28"); // "商店"按钮
            button2 = "威胁渔夫";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = "Shop";
            }
            else 
            {
                // 威胁渔夫功能
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    // 客户端发送请求到服务器
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)FreeStarBro.MessageType.SwapAnglerQuest); // 使用完整的命名空间
                    packet.Send();
                }
                else
                {
                    // 单人模式或服务器直接执行
                    Main.AnglerQuestSwap();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText("老弟成功威胁渔夫换了个新任务！", Color.Orange);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("老弟成功威胁渔夫换了个新任务！"), Color.Orange);
                        NetMessage.SendData(MessageID.AnglerQuest); // 同步新的钓鱼任务到所有客户端
                    }
                }
            }
        }

        private bool IsNpcOnscreen(Vector2 center)
        {
            // 检查玩家是否在 NPC 附近
            int w = NPC.width + 200; // 调整范围根据需要
            int h = NPC.height + 200;
            Rectangle npcScreenRect = new Rectangle((int)center.X - w / 2, (int)center.Y - h / 2, w, h);
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.active && !player.dead && player.getRect().Intersects(npcScreenRect))
                {
                    return true;
                }
            }
            return false;
        }

        private List<int> GetRandomItems(List<int> items, int count)
        {
            List<int> result = new List<int>();
            List<int> availableItems = new List<int>(items);

            for (int i = 0; i < count; i++)
            {
                if (availableItems.Count == 0)
                    break;

                int randomIndex = Main.rand.Next(availableItems.Count);
                result.Add(availableItems[randomIndex]);
                availableItems.RemoveAt(randomIndex); // 确保不会重复选择
            }
            return result;
        }

    }
}
