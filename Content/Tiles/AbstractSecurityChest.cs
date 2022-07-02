using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using RoRMod.Content.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace RoRMod.Content.Tiles
{
    internal abstract class AbstractSecurityChest : ModTile
    {
		protected const int TileWidth = 2;
		protected const int TileHeight = 2;
		protected const short FrameWidth = 36;
		protected const short FrameHeight = 38;

		protected abstract string ChestName { get; }
		protected abstract int ChestItemID { get; }
		protected abstract int UnlockValue { get; }
		protected abstract string UnlockLabelTexturePath { get; }
		protected abstract SoundStyle UnlockSound { get; }
		protected abstract int DustId { get; }
		protected abstract Color MapNameColor { get; }
		protected abstract Color MapNameLockedColor { get; }

		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileSpelunker[Type] = true;
			Main.tileContainer[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;
			TileID.Sets.BasicChest[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = DustId;
			AdjTiles = new int[] { TileID.Containers };
			ChestDrop = ChestItemID;

			// Names
			ContainerName.SetDefault(ChestName);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault(ChestName);
			AddMapEntry(MapNameColor, name, MapChestName);
			name = CreateMapEntryName($"{name}_Locked");
			name.SetDefault($"Locked {ChestName}");
			AddMapEntry(MapNameLockedColor, name, MapChestName);

			// TileObjectData
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
		}

		protected static Point GetTopLeft(int i, int j)
		{
			Point topLeft = new Point(i, j);
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX % FrameWidth != 0)
			{
				topLeft.X--;
			}

			if (tile.TileFrameY != 0)
			{
				topLeft.Y--;
			}
			return topLeft;
		}

		public static string MapChestName(string name, int i, int j)
		{
			Point topLeft = GetTopLeft(i, j);

			int chest = Chest.FindChest(topLeft.X, topLeft.Y);
			if (chest < 0)
			{
				return Language.GetTextValue("LegacyChestType.0");
			}

			if (Main.chest[chest].name == "")
			{
				return name;
			}

			return name + ": " + Main.chest[chest].name;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, TileWidth * 16, TileHeight * 16, ChestDrop);
			Chest.DestroyChest(i, j);
		}

		public override ushort GetMapOption(int i, int j)
		{
			return (ushort)(Main.tile[i, j].TileFrameX / FrameWidth);
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return true;
		}

		public override bool IsLockedChest(int i, int j)
		{
			return Main.tile[i, j].TileFrameX / FrameWidth == 1;
		}

		public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
		{
			manual = true;

			// Unlock Sound
			SoundEngine.PlaySound(UnlockSound.WithVolumeScale(0.5f));

			// Shift TileFrameX and create dust
			for (int x = i; x < i + TileWidth; x++)
			{
				for (int y = j; y < j + TileHeight; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);
					if (tile.TileType == Type)
					{
						tile.TileFrameX -= FrameWidth;
						for (int k = 0; k < 4; k++)
						{
							Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.SteampunkSteam, SpeedX: Main.rand.Next(-5, 5));
						}
					}
				}
			}

			return true;
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Main.mouseRightRelease = false;
			Point topLeft = GetTopLeft(i, j);			

			player.CloseSign();
			player.SetTalkNPC(-1);
			Main.npcChatCornerItem = 0;
			Main.npcChatText = "";

			if (Main.editChest)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = string.Empty;
			}

			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
				player.editedChestName = false;
			}

			bool isLocked = Chest.IsLocked(topLeft.X, topLeft.Y);
			if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
			{
				if (topLeft.X == player.chestX && topLeft.Y == player.chestY && player.chest >= 0)
				{
					// Clicked on open chest - close chest
					player.chest = -1;
					Recipe.FindRecipes();
					SoundEngine.PlaySound(SoundID.MenuClose);
				}
				else
				{
					// Clicked on closed chest - open Chest
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, topLeft.X, topLeft.Y);
					Main.stackSplit = 600;
				}
			}
			else
			{
				if (isLocked)
				{
					// Locked: Try to unlock chest
					if (player.CanBuyItem(UnlockValue) && Chest.Unlock(topLeft.X, topLeft.Y))
					{
						// Unlock successful
						player.BuyItem(UnlockValue);
						SoundEngine.PlaySound(SoundID.Coins);
						if (Main.netMode == NetmodeID.MultiplayerClient)
						{
							NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, topLeft.X, topLeft.Y);
						}
					}
					else
                    {
						// Unlock failed
						SoundEngine.PlaySound(RoRSound.ChestUnlockFail.WithVolumeScale(0.5f));
					}
				}
				else
				{
					// Unlocked: Set chest for player
					int chest = Chest.FindChest(topLeft.X, topLeft.Y);
					if (chest >= 0)
					{
						Main.stackSplit = 600;
						if (chest == player.chest)
						{
							player.chest = -1;
							SoundEngine.PlaySound(SoundID.MenuClose);
						}
						else
						{
							SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
							// TODO (in later update): player.OpenChest(left, top, chest);
							player.chest = chest;
							Main.playerInventory = true;
							Main.recBigList = false;
							player.chestX = topLeft.X;
							player.chestY = topLeft.Y;
						}

						Recipe.FindRecipes();
					}
				}
			}

			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Point topLeft = GetTopLeft(i, j);

			int chest = Chest.FindChest(topLeft.X, topLeft.Y);
			player.cursorItemIconID = -1;
			if (chest < 0)
			{
				player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else
			{
				if (Main.tile[topLeft.X, topLeft.Y].TileFrameX / FrameWidth == 1)
				{
					// Locked: Display unlock value in gold
					player.GetModPlayer<SecurityChestPlayer>().UnlockLabelTexturePath = UnlockLabelTexturePath;
					player.cursorItemIconEnabled = false;
				}
				else
                {
					// Unlocked
					string defaultName = TileLoader.ContainerName(tile.TileType);
					player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
					if (player.cursorItemIconText == defaultName)
					{
						player.cursorItemIconText = "";
						player.cursorItemIconID = ChestItemID;
					}
					player.cursorItemIconEnabled = true;
				}
			}

			player.noThrow = 2;
			
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}
	}


	internal class SecurityChestPlayer : ModPlayer
    {
		public string UnlockLabelTexturePath
        {
			set
            {
				if (value == "" || value == null)
                {
					currentTextureAsset = null;
				}
				else if (textureAssetDictionary.ContainsKey(value))
                {
					currentTextureAsset = textureAssetDictionary[value];
				}
                else
                {
					textureAssetDictionary.Add(value, ModContent.Request<Texture2D>(value));
				}
			}
        }

		private static Dictionary<string, Asset<Texture2D>> textureAssetDictionary = new Dictionary<string, Asset<Texture2D>>();
		private Asset<Texture2D> currentTextureAsset;

        public override void ResetEffects()
        {
			currentTextureAsset = null;
		}

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
			if (currentTextureAsset != null)
            {
				Vector2 offset = new Vector2(20, 20);
				Main.EntitySpriteDraw(currentTextureAsset.Value, Main.MouseWorld + offset - Main.screenPosition, 
					null, Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
			}
        }
    }
}
