using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.Audio;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class DiosBestFriend : ModItem
    {
        public const float ReviveLifePercentage = 0.4f;
        public const int ImmunityDuration = 120;
        public const int CooldownDuration = 60 * 60 * 5 / 2; // 2.5 minutes

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dio's Best Friend");
            Tooltip.SetDefault(
                "Revive upon taking lethal damage\n" +
                "This effect can only occur once every 2.5 minutes");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = Item.rare = ModContent.RarityType<Rarities.RareRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<DiosBestFriendPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class DiosBestFriendPlayer : AccessoryPlayer
    {
        private ReviveAngelWings angelWings = new ReviveAngelWings();

        public override void SetStaticDefaults()
        {
            ReviveAngelWings.LoadTexture();
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            // Revive if equipped and off cooldown
            BuffManager playerBuffs = Player.GetModPlayer<BuffablePlayer>().Buffs;
            if (AccessoryEquipped && !playerBuffs.HasBuff<ExtraLifeCooldownBuff>())
            {
                // Revive
                playSound = false;
                genGore = false;
                PlayerUtils.HealPercentage(Player, DiosBestFriend.ReviveLifePercentage);
                Player.SetImmuneTimeForAllTypes(DiosBestFriend.ImmunityDuration);

                // Buff, sound, angel effect
                playerBuffs.AddBuff(new ExtraLifeCooldownBuff(Player, DiosBestFriend.CooldownDuration));
                SoundEngine.PlaySound(RoRSound.ExtraLife.WithVolumeScale(0.5f), Player.Center);
                angelWings.Activate();

                return false;
            }

            return true;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            angelWings.Draw(Player.MountedCenter);
        }

        private class ReviveAngelWings
        {
            const byte AlphaStep = 12;
            const byte AlphaMax = 128;
            const float ScaleMax = 1f;
            const float ScaleStep = 0.05f;
            const float OffsetXLeftConstant = 48;
            const float OffsetXRightConstant = 16;
            const float OffsetXScaled = 32;
            const float OffsetYScaled = -48;

            public bool active;
            public int timeLeft;
            public byte alpha;
            public float scale;
            public static Asset<Texture2D> textureAsset;

            public static void LoadTexture()
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    textureAsset = ModContent.Request<Texture2D>("RoRMod/Content/Items/Accessories/DiosBestFriend_Wing");
                }
            }

            public void Activate()
            {
                active = true;
                timeLeft = 90;
                scale = 0;
                alpha = 0;
            }

            public void Draw(Vector2 position)
            {
                if (!active || textureAsset == null)
                {
                    return;
                }

                Texture2D texture = textureAsset.Value;

                // Set alpha blend state
                SpriteBatch spriteBatch = Main.spriteBatch;
                BlendState defaultBlendState = spriteBatch.GraphicsDevice.BlendState;
                spriteBatch.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

                // Draw
                Color color = new Color(255, 255, 255, alpha);
                float scaleRatio = scale / ScaleMax;
                float offsetX = OffsetXLeftConstant + OffsetXScaled * scaleRatio;
                float offsetY = OffsetYScaled * scaleRatio;
                Vector2 rightPosition = position - Main.screenPosition + new Vector2(OffsetXRightConstant, offsetY);
                Vector2 leftPosition = position - Main.screenPosition + new Vector2(-offsetX, offsetY);
                spriteBatch.Draw(texture, rightPosition, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, leftPosition, null, color, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0);

                // Reset blend state
                spriteBatch.GraphicsDevice.BlendState = defaultBlendState;

                if (timeLeft > 0)
                {
                    // Fade in
                    if (alpha < AlphaMax)
                    {
                        if (AlphaMax - alpha < AlphaStep)
                        {
                            alpha = AlphaMax;
                        }
                        else
                        {
                            alpha += AlphaStep;
                        }
                    }

                    // scale up
                    if (scale < ScaleMax)
                    {
                        scale += ScaleStep;
                    }

                    timeLeft--;
                }
                else
                {
                    // Fade out when timer runs out
                    if (alpha > AlphaStep)
                    {
                        alpha -= AlphaStep;
                    }
                    else
                    {
                        alpha = 0;
                        active = false;
                    }
                }
            }
        }
    }
}
