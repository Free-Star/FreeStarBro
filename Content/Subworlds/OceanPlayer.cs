using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace FreeStarBro.Content.Subworlds
{
    public class OceanPlayer : ModPlayer
    {
        private bool approachMessageShown;

        public override void PostUpdate()
        {
            int tileX = (int)(Player.position.X / 16f);

            if (!SubworldSystem.IsActive<OceanSubworld>())
            {
                if (!approachMessageShown && tileX >= Main.maxTilesX - 50 && Main.myPlayer == Player.whoAmI)
                {
                    Main.NewText(Language.GetTextValue("Mods.FreeStarBro.OceanSubworld.ApproachMessage"), Color.Aqua);
                    approachMessageShown = true;
                }
                if (tileX < Main.maxTilesX - 60)
                {
                    approachMessageShown = false;
                }

                if (tileX >= Main.maxTilesX - 2 && Main.myPlayer == Player.whoAmI)
                {
                    Main.NewText(Language.GetTextValue("Mods.FreeStarBro.OceanSubworld.EnterMessage"), Color.Aqua);
                    SubworldSystem.Enter<OceanSubworld>();
                }
            }
            else
            {
                if (tileX <= 0 && Main.myPlayer == Player.whoAmI)
                {
                    Main.NewText(Language.GetTextValue("Mods.FreeStarBro.OceanSubworld.ExitMessage"), Color.Aqua);
                    SubworldSystem.Exit();
                }
            }
        }
    }
}

