using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace RoRMod.Content.Common
{
    internal class RoRClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Visuals")]
        [Label("Toggle Buff Icons")]
        [Tooltip("Toggles showing icons for this mod's buffs above the Player or NPCs")]
        [DefaultValue(true)]
        public bool ShowBuffIcons;
    }
}
