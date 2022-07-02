using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Tiles
{
    internal class StandardSecurityChestTile : AbstractSecurityChest
    {
        protected override string ChestName => "Standard Security Chest";

        protected override int ChestItemID => ModContent.ItemType<Items.Placeables.StandardSecurityChest>();

        protected override int UnlockValue => Item.buyPrice(0, 1);

        protected override string UnlockLabelTexturePath => "RoRMod/Content/Tiles/StandardSecurityChestTile_UnlockLabel";

        protected override SoundStyle UnlockSound => RoRSound.ChestUnlockStandard;

        protected override int DustId => DustID.Iron;

        protected override Color MapNameColor => new Color(0x6a, 0x7d, 0x84);

        protected override Color MapNameLockedColor => new Color(0x6c, 0x6c, 0x6c);

       
    }
}
