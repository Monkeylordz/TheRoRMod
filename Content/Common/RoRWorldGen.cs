using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace RoRMod.Content.Common
{
    internal class RoRWorldGen : ModSystem
    {
        // Chest spawning consts
        private const int AttemptsPerChest = 1000;
        private const double SkyDensity = 1E-4;
        private const double SurfaceDensity = 2E-5;
        private const double UndergroundDensity1 = 8E-5;
        private const double UndergroundDensity2 = 2E-4;
        private const double UnderworldDensity = 4E-5;
        private const double ReinforcedUpgradeChance = 0.25;
        private const double GoldenUpgradeChance = 0.2;

        // static types
        private static ushort StandardChestType;
        private static ushort ReinforcedChestType;
        private static ushort GoldenChestType;
        private static int ForeignFruitType;

        private List<(int chest, ushort type)> placedChests;

        public override void PostSetupContent()
        {
            StandardChestType = (ushort)ModContent.TileType<Tiles.StandardSecurityChestTile>();
            ReinforcedChestType = (ushort)ModContent.TileType<Tiles.ReinforcedSecurityChestTile>();
            GoldenChestType = (ushort)ModContent.TileType<Tiles.GoldenSecurityChestTile>();

            ForeignFruitType = ModContent.ItemType<Items.Consumables.ForeignFruit>();
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            // Chest generation
            int chestGenIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Surface Chests"));
            if (chestGenIndex != -1)
            {
                tasks.Insert(chestGenIndex + 1, new PassLegacy("Risk of Rain Chests", GenerateChests));
            }

            // Loot generation
            tasks.Add(new PassLegacy("Risk of Rain Loot", AddChestLoot));
        }

        private void GenerateChests(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Ejecting security chests";

            placedChests = new List<(int chest, ushort type)>();
            int sky = (int)(Main.worldSurface * 0.35);
            int underworld = Main.maxTilesY - 200;
            int undergroundMidpoint = (int)((Main.rockLayer + underworld) / 2);

            // Sky
            GenerateChestsInRange(2, sky, SkyDensity);

            // Surface
            GenerateChestsInRange(sky, (int)Main.worldSurface, SurfaceDensity);

            // Underground 1
            GenerateChestsInRange((int)Main.worldSurface, undergroundMidpoint, UndergroundDensity1);

            // Underground 2
            GenerateChestsInRange(undergroundMidpoint, underworld, UndergroundDensity2);

            // Underworld
            GenerateChestsInRange(underworld, Main.maxTilesY - 2, UnderworldDensity);
        }

        private void GenerateChestsInRange(int minY, int maxY, double chestDensity)
        {
            for (int i = 0; i < Main.maxTilesX * (maxY - minY) * chestDensity; i++)
            {
                bool success = false;
                int attempts = 0;
                while (!success)
                {
                    attempts++;
                    if (attempts > AttemptsPerChest)
                    {
                        break;
                    }

                    // Randomly try to place chests
                    int x = WorldGen.genRand.Next(2, Main.maxTilesX - 2);
                    int y = WorldGen.genRand.Next(minY, maxY);
                    success = PlaceChest(x, y);
                }
            }
        }

        private bool PlaceChest(int x, int y)
        {
            ushort chestType = StandardChestType;

            // Chance to upgrade to reinforced chest
            if (WorldGen.genRand.NextDouble() < ReinforcedUpgradeChance)
            {
                chestType = ReinforcedChestType;

                // Chance to upgrade again to golden chest
                if (WorldGen.genRand.NextDouble() < GoldenUpgradeChance)
                {
                    chestType = GoldenChestType;
                }
            }

            int chest = WorldGen.PlaceChest(x, y, chestType, true, 1);
            if (chest != -1)
            {
                placedChests.Add((chest, chestType));
                return true;
            }
            return false;
        }

        private void AddChestLoot(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Adding security chest loot";

            if (placedChests == null)
            {
                return;
            }

            foreach ((int chest, ushort type) in placedChests)
            {
                if (Main.chest[chest] == null)
                {
                    continue;
                }

                if (type == StandardChestType)
                {
                    AddStandardChestLoot(Main.chest[chest]);
                }
                else if (type == ReinforcedChestType)
                {
                    AddReinforcedChestLoot(Main.chest[chest]);
                }
                else if (type == GoldenChestType)
                {
                    AddGoldenChestLoot(Main.chest[chest]);
                }
            }

            placedChests.Clear();
        }

        private void AddStandardChestLoot(Chest chest)
        {
            var itemsToAdd = new List<(int type, int stack)>();

            // 1/20/79
            AddRoRItemLoot(itemsToAdd, 0.01, 0.2, 0.79);

            // Misc items
            itemsToAdd.Add((ForeignFruitType, WorldGen.genRand.Next(1, 8)));
            itemsToAdd.Add((ItemID.Torch, WorldGen.genRand.Next(5, 35)));

            // Potions
            AddRandomPotionCommon(itemsToAdd);
            // 25% to add rare potions
            if (WorldGen.genRand.NextBool(4))
            {
                AddRandomPotionRare(itemsToAdd);
            }

            // Ammo
            AddRandomAmmo(itemsToAdd);

            // 1s - 1g
            AddRandomCoins(itemsToAdd, 100, 10000);

            AddItemsToChest(chest, itemsToAdd);
        }

        private void AddReinforcedChestLoot(Chest chest)
        {
            var itemsToAdd = new List<(int type, int stack)>();

            // 20/80/0
            AddRoRItemLoot(itemsToAdd, 0.2, 0.8);

            // 50% chance 1/20/79
            if (WorldGen.genRand.NextBool())
            {
                AddRoRItemLoot(itemsToAdd, 0.01, 0.2, 0.79);
            }

            // Misc items
            itemsToAdd.Add((ForeignFruitType, WorldGen.genRand.Next(1, 12)));
            itemsToAdd.Add((ItemID.Torch, WorldGen.genRand.Next(5, 35)));

            // Potions
            AddRandomPotionCommon(itemsToAdd);
            AddRandomPotionRare(itemsToAdd);

            // Ammo
            AddRandomAmmo(itemsToAdd);

            // 50s - 5g
            AddRandomCoins(itemsToAdd, 5000, 50000);

            AddItemsToChest(chest, itemsToAdd);
        }

        private void AddGoldenChestLoot(Chest chest)
        {
            var itemsToAdd = new List<(int type, int stack)>();

            // 100/0/0
            itemsToAdd.Add((WorldGen.genRand.NextFromCollection(ItemRaritySystem.RareItems), 1));

            // 50% chance for 20/80/0
            if (WorldGen.genRand.NextBool())
            {
                AddRoRItemLoot(itemsToAdd, 0.2, 0.8);
            }

            // Misc items
            itemsToAdd.Add((ForeignFruitType, WorldGen.genRand.Next(4, 19)));
            itemsToAdd.Add((ItemID.Torch, WorldGen.genRand.Next(5, 35)));

            // Potions
            AddRandomPotionRare(itemsToAdd);
            AddRandomPotionRare(itemsToAdd);

            // Bullets
            AddRandomAmmo(itemsToAdd);

            // 1g - 15g
            AddRandomCoins(itemsToAdd, 10000, 150000);

            AddItemsToChest(chest, itemsToAdd);
        }

        private void AddRoRItemLoot(in List<(int type, int stack)> itemsToAdd, double redChance = 0, double greenChance = 0, double whiteChance = 0)
        {
            // Use weighted random to select item
            var itemBag = new Terraria.Utilities.WeightedRandom<int>(WorldGen.genRand);

            if (redChance > 0)
            {
                int red = WorldGen.genRand.NextFromCollection(ItemRaritySystem.RareItems);
                itemBag.Add(red, redChance);
            }
            if (greenChance > 0)
            {
                int green = WorldGen.genRand.NextFromCollection(ItemRaritySystem.UncommonItems);
                itemBag.Add(green, greenChance);
            }
            if (whiteChance > 0)
            {
                int white = WorldGen.genRand.NextFromCollection(ItemRaritySystem.CommonItems);
                itemBag.Add(white, whiteChance);
            }

            if (itemBag.elements.Count > 0)
            {
                itemsToAdd.Add((itemBag.Get(), 1));
            }
        }

        private void AddRandomPotionCommon(in List<(int type, int stack)> itemsToAdd)
        {
            switch (WorldGen.genRand.Next(8))
            {
                case 0:
                    itemsToAdd.Add((ItemID.ShinePotion, WorldGen.genRand.Next(1, 9)));
                    break;
                case 1:
                    itemsToAdd.Add((ItemID.RecallPotion, WorldGen.genRand.Next(4, 11)));
                    break;
                case 2:
                    itemsToAdd.Add((ItemID.FeatherfallPotion, WorldGen.genRand.Next(1, 6)));
                    break;
                case 3:
                    itemsToAdd.Add((ItemID.HunterPotion, WorldGen.genRand.Next(1, 6)));
                    break;
                case 4:
                    itemsToAdd.Add((ItemID.SwiftnessPotion, WorldGen.genRand.Next(2, 6)));
                    break;
                case 5:
                    itemsToAdd.Add((ItemID.IronskinPotion, WorldGen.genRand.Next(2, 5)));
                    break;
                case 6:
                    itemsToAdd.Add((ItemID.RegenerationPotion, WorldGen.genRand.Next(2, 5)));
                    break;
                default:
                    break;
            }
        }

        private void AddRandomPotionRare(in List<(int type, int stack)> itemsToAdd)
        {
            switch (WorldGen.genRand.Next(9))
            {
                case 0:
                    itemsToAdd.Add((ItemID.SpelunkerPotion, WorldGen.genRand.Next(1, 5)));
                    break;
                case 1:
                    itemsToAdd.Add((ItemID.ObsidianSkinPotion, WorldGen.genRand.Next(1, 3)));
                    break;
                case 2:
                    itemsToAdd.Add((ItemID.WrathPotion, WorldGen.genRand.Next(1, 3)));
                    break;
                case 3:
                    itemsToAdd.Add((ItemID.InfernoPotion, WorldGen.genRand.Next(2, 4)));
                    break;
                case 4:
                    itemsToAdd.Add((ItemID.GravitationPotion, WorldGen.genRand.Next(1, 3)));
                    break;
                case 5:
                    itemsToAdd.Add((ItemID.LifeforcePotion, WorldGen.genRand.Next(2, 4)));
                    break;
                case 6:
                    itemsToAdd.Add((ItemID.EndurancePotion, WorldGen.genRand.Next(2, 4)));
                    break;
                case 7:
                    itemsToAdd.Add((ItemID.BattlePotion, WorldGen.genRand.Next(1, 4)));
                    break;
                default:
                    break;
            }
        }

        private void AddRandomAmmo(in List<(int type, int stack)> itemsToAdd)
        {
            switch (WorldGen.genRand.Next(5))
            {
                case 0:
                    itemsToAdd.Add((ItemID.WoodenArrow, WorldGen.genRand.Next(50, 150)));
                    break;
                case 1:
                    itemsToAdd.Add((ItemID.MusketBall, WorldGen.genRand.Next(50, 150)));
                    break;
                case 2:
                    itemsToAdd.Add((ItemID.JestersArrow, WorldGen.genRand.Next(50, 150)));
                    break;
                case 3:
                    itemsToAdd.Add((ItemID.MeteorShot, WorldGen.genRand.Next(50, 150)));
                    break;
                default:
                    break;
            }
        }


        private void AddRandomCoins(in List<(int type, int stack)> itemsToAdd, int min, int max)
        {
            int value = WorldGen.genRand.Next(min, max);
            int[] coins = Utils.CoinsSplit(value);
            if (coins[3] > 0)
            {
                itemsToAdd.Add((ItemID.PlatinumCoin, coins[3]));
            }
            if (coins[2] > 0)
            {
                itemsToAdd.Add((ItemID.GoldCoin, coins[2]));
            }
            if (coins[1] > 0)
            {
                itemsToAdd.Add((ItemID.SilverCoin, coins[1]));
            }
            if (coins[0] > 0)
            {
                itemsToAdd.Add((ItemID.CopperCoin, coins[0]));
            }
        }

        private void AddItemsToChest(Chest chest, List<(int type, int stack)> itemsToAdd)
        {
            int chestItemIndex = 0;
            foreach (var itemToAdd in itemsToAdd)
            {
                Item item = new Item();
                item.SetDefaults(itemToAdd.type);
                item.stack = itemToAdd.stack;
                chest.item[chestItemIndex] = item;
                chestItemIndex++;
                if (chestItemIndex >= 40)
                    break;
            }
        }
    }
}
