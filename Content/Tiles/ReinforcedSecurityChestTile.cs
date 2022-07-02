using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Tiles
{
    internal class ReinforcedSecurityChestTile : AbstractSecurityChest
    {
        protected override string ChestName => "Reinforced Security Chest";

        protected override int ChestItemID => ModContent.ItemType<Items.Placeables.ReinforcedSecurityChest>();

        protected override int UnlockValue => Item.buyPrice(0, 5);

        protected override string UnlockLabelTexturePath => "RoRMod/Content/Tiles/ReinforcedSecurityChestTile_UnlockLabel";

        protected override SoundStyle UnlockSound => RoRSound.ChestUnlockReinforced;

        protected override int DustId => DustID.Iron;

        protected override Color MapNameColor => new Color(0x6a, 0x7d, 0x84);

        protected override Color MapNameLockedColor => new Color(0x6c, 0x6c, 0x6c);
    }
}
