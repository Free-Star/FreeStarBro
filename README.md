# FreeStarBro 模组

FreeStarBro 是一个 tModLoader 模组，提供了若干自定义道具与 NPC，扩展了泰拉瑞亚的游戏体验。

## 主要内容

### 自定义物品
- **FSword**：根据玩家血量自动放大并提升伤害的近战武器。
- **ItemCollector**：放置后能在一定范围内自动收集物品并存入附近箱子，可通过右键开关。
- **TravelingMerchantSummon**：使用后可召唤旅商到玩家位置。
- **WallBreaker**：专门用于大范围破坏墙壁的消耗品，不会破坏方块。
- **SlimeRainSummon**：立即触发史莱姆雨的道具，并为玩家施加短暂的混沌减益。
- **MakaHead / YiguHead**：两款装饰用的大头贴装备。

### 自定义方块
- **ItemCollectorTile**：与 `ItemCollector` 对应，拥有 `ItemCollectorEntity` 在后台处理自动拾取与存储逻辑，可通过网络同步开关状态。

### 自定义 NPC
- **BroNPC**：新的城镇 NPC，会出售多种物品（包括上述道具）并能通过对话威胁渔夫刷新钓鱼任务。

### 子世界
- 玩家到达主世界最右侧时会进入新的 **海洋群落** 子世界（200×600），其中随机生成小岛、房屋与宝箱。可从子世界最左端返回主世界。
- 接近世界右端时会先显示海风的文案提示，进入和离开子世界时，聊天栏也会出现提示。

### 其他
- 本模组包含简易的本地化文件 `Localization/en-US_Mods.FreeStarBro.hjson`，用于物品与 NPC 的显示文本。
- `build.txt` 与 `description.txt` 分别记录模组元信息与描述（尚待完善）。

本仓库的源代码位于 `Content` 目录下，包含所有道具、NPC 与 Tile 的实现文件，适用于 tModLoader 环境。

## 构建与运行

在开始之前，请确保已经安装好 [tModLoader](https://github.com/tModLoader/tModLoader) 与 [.NET SDK](https://dotnet.microsoft.com/download)（建议使用 6.0 及以上版本）。

开发时可将本仓库放入 `tModLoader/ModSources` 文件夹中。该目录一般位于 `Documents/My Games/Terraria/tModLoader/ModSources`（Windows）或 `~/.local/share/Terraria/tModLoader/ModSources`（Linux）。

进入项目目录后，可通过 `dotnet build` 或在 tModLoader 的 "Mod Sources" 菜单中选择 **Build + Reload** 来编译并载入模组。构建完成后，在 tModLoader 的 "Mods" 菜单中启用 FreeStarBro 即可开始游玩。
