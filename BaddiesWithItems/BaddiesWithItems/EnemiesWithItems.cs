﻿using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace BaddiesWithItems
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Basil.EnemiesWithItems", "EnemiesWithItems", "1.2.7")]

    public class EnemiesWithItems : BaseUnityPlugin
    {

        public static ConfigWrapper<bool> GenerateItems;
        public static ConfigWrapper<string> ItemMultiplier;

        public static ConfigWrapper<int> StageReq;
        public static ConfigWrapper<bool> InheritItems;
        
        public static ConfigWrapper<string> Tier1GenCap;
        public static ConfigWrapper<string> Tier2GenCap;
        public static ConfigWrapper<string> Tier3GenCap;
        public static ConfigWrapper<string> LunarGenCap;
        public static ConfigWrapper<string> Tier1GenChance;
        public static ConfigWrapper<string> Tier2GenChance;
        public static ConfigWrapper<string> Tier3GenChance;
        public static ConfigWrapper<string> LunarGenChance;
        public static ConfigWrapper<string> EquipGenChance;

        public static ConfigWrapper<string> CustomItemBlacklist;
        public static ConfigWrapper<string> CustomEquipBlacklist;
        public static ConfigWrapper<string> CustomItemCaps;

        public static ConfigWrapper<bool> ItemsBlacklist;
        public static ConfigWrapper<bool> Limiter;
        public static ConfigWrapper<bool> Tier1Items;
        public static ConfigWrapper<bool> Tier2Items;
        public static ConfigWrapper<bool> Tier3Items;
        public static ConfigWrapper<bool> LunarItems;
        public static ConfigWrapper<bool> EquipItems;
        public static ConfigWrapper<bool> EquipBlacklist;

        public static ConfigWrapper<bool> DropItems;
        public static ConfigWrapper<string> DropChance;

        public void InitConfig()
        {
            GenerateItems = Config.Wrap(
                "Generator Settings",
                "GenerateItems",
                "Toggles item generation for enemies.",
                true);
            
            Tier1GenCap = Config.Wrap(
                "Generator Settings",
                "Tier1GenCap",
                "The multiplicative max item cap for generating Tier 1 (white) items.",
                "4");

            Tier2GenCap = Config.Wrap(
                "Generator Settings",
                "Tier2GenCap",
                "The multiplicative max item cap for generating Tier 2 (green) items.",
                "2");

            Tier3GenCap = Config.Wrap(
                "Generator Settings",
                "Tier3GenCap",
                "The multiplicative max item cap for generating Tier 3 (red) items.",
                "1");

            LunarGenCap = Config.Wrap(
                "Generator Settings",
                "LunarGenCap",
                "The multiplicative max item cap for generating Lunar (blue) items.",
                "1");

            Tier1GenChance = Config.Wrap(
                "Generator Settings",
                "Tier1GenChance",
                "The percent chance for generating a Tier 1 (white) item.",
                "40");

            Tier2GenChance = Config.Wrap(
                "Generator Settings",
                "Tier2GenChance",
                "The percent chance for generating a Tier 2 (green) item.",
                "20");

            Tier3GenChance = Config.Wrap(
                "Generator Settings",
                "Tier3GenChance",
                "The percent chance for generating a Tier 3 (red) item.",
                "1");

            LunarGenChance = Config.Wrap(
                "Generator Settings",
                "LunarGenChance",
                "The percent chance for generating a Lunar (blue) item.",
                "0.5");

            EquipGenChance = Config.Wrap(
                "Generator Settings",
                "EquipGenChance",
                "The percent chance for generating a Use item.",
                "10");

            InheritItems = Config.Wrap(
                "Inherit Settings",
                "InheritItems",
                "Toggles enemies to randomly inherit items from a random player. Overrides Generator Settings.",
                false);

            ItemMultiplier = Config.Wrap(
                "General Settings",
                "ItemMultiplier",
                "Sets the multiplier for items to be inherited/generated.",
                "1");

            StageReq = Config.Wrap(
                "General Settings",
                "StageReq",
                "Sets the minimum stage to be cleared before having enemies inherit/generate items.",
                4);

            ItemsBlacklist = Config.Wrap(
                "General Settings",
                "BlacklistItems",
                "Toggles hard blacklisted items to be inherited/generated.",
                false);

            Limiter = Config.Wrap(
                "General Settings",
                "Limiter",
                "Toggles certain items to be capped. For more information, check this mod's FAQ at the thunderstore!",
                true);
            
            DropItems = Config.Wrap(
                "General Settings",
                "DropItems",
                "Toggles items to be dropped by enemies with items.",
                false);

            DropChance = Config.Wrap(
                "General Settings",
                "DropChance",
                "Sets the percent chance that an enemy drops one of their items. Default 0.1 means average 1 in a 1000 kills will result in a drop.",
                "0.1");
            
            Tier1Items = Config.Wrap(
                "General Settings",
                "Tier1Items",
                "Toggles Tier 1 (white) items to be inherited/generated.",
                true);

            Tier2Items = Config.Wrap(
                "General Settings",
                "Tier2Items",
                "Toggles Tier 2 (green) items to be inherited/generated.",
                true);

            Tier3Items = Config.Wrap(
                "General Settings",
                "Tier3Items",
                "Toggles Tier 3 (red) items to be inherited/generated.",
                true);

            LunarItems = Config.Wrap(
                "General Settings",
                "LunarItems",
                "Toggles Lunar (blue) items to be inherited/generated.",
                true);

            EquipItems = Config.Wrap(
                "General Settings",
                "EquipItems",
                "Toggles Use items to be inherited/generated.",
                false);

            EquipBlacklist = Config.Wrap(
                "General Settings",
                "EquipBlacklist",
                "Toggles hard blacklisted Use items to be inherited/generated. MOST BLACKLISTED USE ITEMS ARE UNDODGEABLE.",
                false);

            CustomItemBlacklist = Config.Wrap(
                "General Settings",
                "CustomItemBlacklist",
                "Enter items ids separated by a comma and a space to blacklist certain items. ex) 41, 23, 17 \nItem ids: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names",
                "");

            CustomEquipBlacklist = Config.Wrap(
               "General Settings",
               "CustomEquipBlacklist",
               "Enter equipment ids separated by a comma and a space to blacklist certain equipments. ex) 1, 14, 13 \nEquip ids: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names",
               "");

            CustomItemCaps = Config.Wrap(
               "General Settings",
               "CustomItemCaps",
               "Enter item ids as X-Y separated by a comma and a space to apply caps to certain items. X is the item id and Y is the number cap. ex) 0-20, 1-5, 2-1",
               "");
        }

        public static EquipmentIndex[] EquipmentBlacklist = new EquipmentIndex[]
        {
            EquipmentIndex.Blackhole,
            EquipmentIndex.BFG,
            EquipmentIndex.Lightning,
            EquipmentIndex.Scanner,
            EquipmentIndex.CommandMissile,
            EquipmentIndex.BurnNearby,
            EquipmentIndex.DroneBackup,
            EquipmentIndex.Gateway,
            EquipmentIndex.FireBallDash
        };

        public static ItemIndex[] ItemBlacklist = new ItemIndex[]
        {
            ItemIndex.StickyBomb,
            ItemIndex.StunChanceOnHit,
            ItemIndex.NovaOnHeal,
            ItemIndex.ShockNearby,
            ItemIndex.Mushroom
        };

        public static ItemIndex[] ItemsNeverUsed = new ItemIndex[]
        {
            ItemIndex.SprintWisp,
            ItemIndex.ExecuteLowHealthElite,
            ItemIndex.TitanGoldDuringTP,            // Halcyon Seed
            ItemIndex.TreasureCache,
            ItemIndex.BossDamageBonus,
            ItemIndex.ExtraLifeConsumed,
            ItemIndex.Feather,
            ItemIndex.Firework,
            ItemIndex.SprintArmor,
            ItemIndex.JumpBoost,
            ItemIndex.GoldOnHit,
            ItemIndex.WardOnLevel,
            ItemIndex.BeetleGland,
            ItemIndex.CrippleWardOnLevel,
            ItemIndex.TPHealingNova,
            ItemIndex.LunarTrinket,                 // Beads of Fealty
            ItemIndex.LunarPrimaryReplacement,      // Visions of Heresy
            ItemIndex.BonusGoldPackOnKill           // Ghor's Tome
        };

        public static List<EquipmentIndex> allEquips = new List<EquipmentIndex>()
        {
            (EquipmentIndex)0,
            (EquipmentIndex)1,
            (EquipmentIndex)2,
            (EquipmentIndex)3,
            (EquipmentIndex)11,
            (EquipmentIndex)13,
            (EquipmentIndex)14,
            (EquipmentIndex)16,
            (EquipmentIndex)18,
            (EquipmentIndex)19,
            (EquipmentIndex)20,
            (EquipmentIndex)21,
            (EquipmentIndex)23,
            (EquipmentIndex)26,
            (EquipmentIndex)27,
            (EquipmentIndex)28,
            (EquipmentIndex)29,
            (EquipmentIndex)30,
            (EquipmentIndex)31,
            (EquipmentIndex)32,
            (EquipmentIndex)33,
        };

        public static float ConfigToFloat(string configline)
        {
            if (float.TryParse(configline, out float x))
            {
                return x;
            }
            return 1f;
        }

        public void Awake()
        {
            InitConfig();

            Hooks.baddiesItems();
            Hooks.enemiesDrop();
            Chat.AddMessage("EnemiesWithItems v1.2.7 Loaded!");
        }

        public static void checkConfig(Inventory inventory, CharacterMaster master)
        {
            if (InheritItems.Value) // inheritance
            {
                updateInventory(inventory, master);
            }
            else if (GenerateItems.Value) // Using generator instead
            {
                resetInventory(inventory);
                int scc = Run.instance.stageClearCount;

                // Get average # of items among all players.
                int totalItems = 0;
                foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
                {
                    foreach (ItemIndex index in ItemCatalog.allItems)
                    {
                        totalItems += player.master.inventory.GetItemCount(index);
                    }
                }
                int avgItems = (int)Math.Pow(scc,2) + totalItems;

                int numItems = 0;
                int amount;
                if (Tier1Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(Tier1GenChance.Value) + (scc - StageReq.Value)))
                        {
                            amount = UnityEngine.Random.Range(0, (int)(Math.Pow(scc, 2) * ConfigToFloat(Tier1GenCap.Value) + 1));
                            if (numItems + amount > avgItems)
                            {
                                amount = avgItems - numItems;
                            }
                            numItems += amount;
                            inventory.GiveItem(index, amount);
                        }
                        if (numItems >= avgItems)
                        {
                            break;
                        }
                    }
                }
                if (Tier2Items.Value && numItems < avgItems)
                {
                    foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(Tier2GenChance.Value) + (scc - StageReq.Value)))
                        {
                            amount = UnityEngine.Random.Range(0, (int)(Math.Pow(scc, 1.8) * ConfigToFloat(Tier2GenCap.Value) + 1));
                            if (numItems + amount > avgItems)
                            {
                                amount = avgItems - numItems;
                            }
                            numItems += amount;
                            inventory.GiveItem(index, amount);
                        }
                        if (numItems >= avgItems)
                        {
                            break;
                        }
                    }
                }
                if (Tier3Items.Value && numItems < avgItems)
                {
                    foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(Tier3GenChance.Value) + (scc - StageReq.Value)))
                        {
                            amount = UnityEngine.Random.Range(0, (int)(Math.Pow(scc, 1.5) * ConfigToFloat(Tier3GenCap.Value) + 1));
                            if (numItems + amount > avgItems)
                            {
                                amount = avgItems - numItems;
                            }
                            numItems += amount;
                            inventory.GiveItem(index, amount);
                        }
                        if (numItems >= avgItems)
                        {
                            break;
                        }
                    }
                }
                if (LunarItems.Value && numItems < avgItems)
                {
                    foreach (ItemIndex index in ItemCatalog.lunarItemList)
                    {
                        if(index == ItemIndex.AutoCastEquipment)
                        {
                            continue;
                        }
                        if (Util.CheckRoll(ConfigToFloat(LunarGenChance.Value) + (scc - StageReq.Value)))
                        {
                            amount = UnityEngine.Random.Range(0, (int)(Math.Pow(scc, 1.1) * ConfigToFloat(LunarGenCap.Value) + 1));
                            if (numItems + amount > avgItems)
                            {
                                amount = avgItems - numItems;
                            }
                            numItems += amount;
                            inventory.GiveItem(index, amount);
                        }
                        if (numItems >= avgItems)
                        {
                            break;
                        }
                    }
                }
                if (EquipItems.Value)
                {
                    if (Util.CheckRoll(ConfigToFloat(EquipGenChance.Value)))
                    {
                        inventory.ResetItem(ItemIndex.AutoCastEquipment);
                        inventory.GiveItem(ItemIndex.AutoCastEquipment, 1);
                        
                        while (true)
                        {
                            EquipmentIndex equipmentIndex = allEquips[Run.instance.spawnRng.RangeInt(0, allEquips.Count)];
                            
                            bool flag = true;

                            if (!EquipBlacklist.Value)
                            {
                                // Hard blacklisted
                                foreach (EquipmentIndex item in EquipmentBlacklist)
                                {
                                    if (equipmentIndex == item)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                            if (flag == true)
                            {
                                inventory.SetEquipmentIndex(equipmentIndex);
                                break;
                            }
                        }
                    }
                }
                
                multiplier(inventory);
                blacklist(inventory);

            }
            else
            {
                resetInventory(inventory);
            }
        }

        public static void resetInventory(Inventory inventory)
        {
            foreach (ItemIndex index in ItemCatalog.tier1ItemList)
            {
                inventory.ResetItem(index);
            }
            foreach (ItemIndex index in ItemCatalog.tier2ItemList)
            {
                inventory.ResetItem(index);
            }
            foreach (ItemIndex index in ItemCatalog.tier3ItemList)
            {
                inventory.ResetItem(index);
            }
            foreach (ItemIndex index in ItemCatalog.lunarItemList)
            {
                inventory.ResetItem(index);
            }
        }

        public static void updateInventory(Inventory inventory, CharacterMaster master)
        {
            if (!Tier1Items.Value)
            {
                foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                {
                    inventory.ResetItem(index);
                }
            }
            if (!Tier2Items.Value)
            {
                foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                {
                    inventory.ResetItem(index);
                }
            }
            if (!Tier3Items.Value)
            {
                foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                {
                    inventory.ResetItem(index);
                }
            }
            if (!LunarItems.Value)
            {
                foreach (ItemIndex index in ItemCatalog.lunarItemList)
                {
                    inventory.ResetItem(index);
                }
            }

            multiplier(inventory);

            if (EquipItems.Value)
            {
                inventory.ResetItem(ItemIndex.AutoCastEquipment);
                inventory.GiveItem(ItemIndex.AutoCastEquipment, 1);
                inventory.CopyEquipmentFrom(master.inventory);

                foreach (EquipmentIndex item in EquipmentBlacklist)
                {
                    if (inventory.GetEquipmentIndex() == item)
                    {
                        inventory.SetEquipmentIndex(EquipmentIndex.QuestVolatileBattery); // default to Fuel Array
                        break;
                    }
                }

            }

            blacklist(inventory);
        }

        public static void multiplier(Inventory inventory)
        {
            float itemMultiplier = ConfigToFloat(ItemMultiplier.Value);
            if (itemMultiplier != 1f)
            {
                int count = 0;
                if (Tier1Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (int)Math.Ceiling(count * itemMultiplier));
                    }
                }
                if (Tier2Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (int)Math.Ceiling(count * itemMultiplier));
                    }
                }
                if (Tier3Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (int)Math.Ceiling(count * itemMultiplier));
                    }
                }
                if (LunarItems.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.lunarItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (int)Math.Ceiling(count * itemMultiplier));
                    }
                }
            }
        }

        public static void blacklist(Inventory inventory)
        {
            foreach (ItemIndex item in ItemsNeverUsed)
            {
                inventory.ResetItem(item);
            }

            if (!ItemsBlacklist.Value)
            {
                foreach (ItemIndex item in ItemBlacklist)
                {
                    inventory.ResetItem(item);
                }
            }
            // Limiter
            if (Limiter.Value)
            {
                if (inventory.GetItemCount(ItemIndex.Bear) > 7)
                {
                    inventory.ResetItem(ItemIndex.Bear);
                    inventory.GiveItem(ItemIndex.Bear, 7);
                }
                if (inventory.GetItemCount(ItemIndex.HealWhileSafe) > 30)
                {
                    inventory.ResetItem(ItemIndex.HealWhileSafe);
                    inventory.GiveItem(ItemIndex.HealWhileSafe, 30);
                }
                if (inventory.GetItemCount(ItemIndex.EquipmentMagazine) > 3)
                {
                    inventory.ResetItem(ItemIndex.EquipmentMagazine);
                    inventory.GiveItem(ItemIndex.EquipmentMagazine, 3);
                }
                if (inventory.GetItemCount(ItemIndex.SlowOnHit) > 1)
                {
                    inventory.ResetItem(ItemIndex.SlowOnHit);
                    inventory.GiveItem(ItemIndex.SlowOnHit, 1);
                }
                if (inventory.GetItemCount(ItemIndex.Behemoth) > 2)
                {
                    inventory.ResetItem(ItemIndex.Behemoth);
                    inventory.GiveItem(ItemIndex.Behemoth, 2);
                }
                if (inventory.GetItemCount(ItemIndex.BleedOnHit) > 3)
                {
                    inventory.ResetItem(ItemIndex.BleedOnHit);
                    inventory.GiveItem(ItemIndex.BleedOnHit, 3);
                }
                if (inventory.GetItemCount(ItemIndex.IgniteOnKill) > 2)
                {
                    inventory.ResetItem(ItemIndex.IgniteOnKill);
                    inventory.GiveItem(ItemIndex.IgniteOnKill, 2);
                }
                if (inventory.GetItemCount(ItemIndex.AutoCastEquipment) > 1)
                {
                    inventory.ResetItem(ItemIndex.AutoCastEquipment);
                    inventory.GiveItem(ItemIndex.AutoCastEquipment, 1);
                }
                if (inventory.GetItemCount(ItemIndex.NearbyDamageBonus) > 3)
                {
                    inventory.ResetItem(ItemIndex.NearbyDamageBonus);
                    inventory.GiveItem(ItemIndex.NearbyDamageBonus, 3);
                }
                if(inventory.GetItemCount(ItemIndex.ShinyPearl) > Run.instance.stageClearCount)
                {
                    inventory.ResetItem(ItemIndex.ShinyPearl);
                    inventory.GiveItem(ItemIndex.ShinyPearl, Run.instance.stageClearCount);
                }
                if(inventory.GetItemCount(ItemIndex.Pearl) > Run.instance.stageClearCount)
                {
                    inventory.ResetItem(ItemIndex.Pearl);
                    inventory.GiveItem(ItemIndex.Pearl, Run.instance.stageClearCount);
                }
                if(inventory.GetItemCount(ItemIndex.Thorns) > 1)
                {
                    inventory.ResetItem(ItemIndex.Thorns);
                    inventory.GiveItem(ItemIndex.Thorns, 1);
                }
            }

            customItem(inventory);
            customEquip(inventory);
            customItemCap(inventory);
        }

        public static void customEquip(Inventory inventory)
        {
            // Custom Equip Blacklist
            string[] customEquiplist = CustomEquipBlacklist.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string equip in customEquiplist)
            {
                if (Int32.TryParse(equip, out int x))
                {
                    if (inventory.GetEquipmentIndex() == (EquipmentIndex)x)
                    {
                        inventory.SetEquipmentIndex(EquipmentIndex.None);
                    }
                }
            }
        }

        public static void customItem(Inventory inventory)
        {
            // Custom Items Blacklist
            string[] customItemlist = CustomItemBlacklist.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in customItemlist)
            {
                if (Int32.TryParse(item, out int x))
                {
                    inventory.ResetItem((ItemIndex)x);
                }
            }
        }

        public static void customItemCap(Inventory inventory)
        {
            // Custom item caps
            string[] customItemCaps = CustomItemCaps.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in customItemCaps)
            {
                string[] temp = item.Split(new[] { '-' });
                if (temp.Length == 2)
                {
                    if (Int32.TryParse(temp[0], out int itemId) && Int32.TryParse(temp[1], out int cap))
                    {
                        if (inventory.GetItemCount((ItemIndex)itemId) > cap)
                        {
                            inventory.ResetItem((ItemIndex)itemId);
                            inventory.GiveItem((ItemIndex)itemId, cap);
                        }
                    }
                }
            }
        }
    }

}
