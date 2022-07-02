using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Tiles
{
    internal class GoldenSecurityChestTile : AbstractSecurityChest
    {
        protected override string ChestName => "Golden Security Chest";

        protected override int ChestItemID => ModContent.ItemType<Items.Placeables.GoldenSecurityChest>();

        protected override int UnlockValue => Item.buyPrice(0, 15);

        protected override string UnlockLabelTexturePath => "RoRMod/Content/Tiles/GoldenSecurityChestTile_UnlockLabel";

        protected override SoundStyle UnlockSound => RoRSound.ChestUnlockGolden;

        protected override int DustId => DustID.Gold;

        protected override Color MapNameColor => new Color(0xea, 0xe8, 0xaf);

        protected override Color MapNameLockedColor => new Color(0x77, 0x75, 0x4b);
    }
}
