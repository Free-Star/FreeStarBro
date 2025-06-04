// using Terraria;
// using Terraria.ModLoader;
// using Terraria.WorldBuilding;
// using System.Collections.Generic;
// using System.Reflection;

// namespace FreeStarBro.patch
// {
//     public class SpiritReforgedPatch : ModSystem
//     {
//         public override void PreWorldGen()
//         {
//             // 只在超大地图中启用
//             if (Main.maxTilesX > 8400 || Main.maxTilesY > 2400)
//             {
//                 // 反射获取 WorldGen 中的 genTasks 列表
//                 FieldInfo taskListField = typeof(WorldGen).GetField("genTasks", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
//                 if (taskListField != null)
//                 {
//                     var taskList = taskListField.GetValue(null) as List<GenPass>;
//                     if (taskList != null)
//                     {
//                         taskList.RemoveAll(pass => pass.Name != null && pass.Name.Contains("Pottery"));
//                         ModContent.GetInstance<FreeStarBro>().Logger.Info("✅ Pottery structure 已在 PreWorldGen 被移除。");
//                     }
//                 }
//             }
//         }
//     }
// }
