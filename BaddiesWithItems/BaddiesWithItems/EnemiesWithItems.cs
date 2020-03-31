using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace BaddiesWithItems
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Basil.EnemiesWithItems", "EnemiesWithItems", "1.2.8")]

    public class EnemiesWithItems : BaseUnityPlugin
    {

        public static ConfigEntry<bool> GenerateItems;
        public static ConfigEntry<string> ItemMultiplier;

        public static ConfigEntry<int> StageReq;
        public static ConfigEntry<bool> InheritItems;
        
        public static ConfigEntry<string> Tier1GenCap;
        public static ConfigEntry<string> Tier2GenCap;
        public static ConfigEntry<string> Tier3GenCap;
        public static ConfigEntry<string> LunarGenCap;
        public static ConfigEntry<string> Tier1GenChance;
        public static ConfigEntry<string> Tier2GenChance;
        public static ConfigEntry<string> Tier3GenChance;
        public static ConfigEntry<string> LunarGenChance;
        public static ConfigEntry<string> EquipGenChance;

        public static ConfigEntry<string> CustomItemBlacklist;
        public static ConfigEntry<string> CustomEquipBlacklist;
        public static ConfigEntry<string> CustomItemCaps;

        public static ConfigEntry<bool> ItemsBlacklist;
        public static ConfigEntry<bool> Limiter;
        public static ConfigEntry<bool> Tier1Items;
        public static ConfigEntry<bool> Tier2Items;
        public static ConfigEntry<bool> Tier3Items;
        public static ConfigEntry<bool> LunarItems;
        public static ConfigEntry<bool> EquipItems;
        public static ConfigEntry<bool> EquipBlacklist;

        public static ConfigEntry<bool> DropItems;
        public static ConfigEntry<string> DropChance;

        public void InitConfig()
        {
            GenerateItems = Config.Bind(
                "Generator Settings",
                "GenerateItems",
                true,
                "Toggles item generation for enemies."
                );
            
            Tier1GenCap = Config.Bind(
                "Generator Settings",
                "Tier1GenCap",
                "4",
                "The multiplicative max item cap for generating Tier 1 (white) items."
                );

            Tier2GenCap = Config.Bind(
                "Generator Settings",
                "Tier2GenCap",
                "2",
                "The multiplicative max item cap for generating Tier 2 (green) items."
                );

            Tier3GenCap = Config.Bind(
                "Generator Settings",
                "Tier3GenCap",
                "1",
                "The multiplicative max item cap for generating Tier 3 (red) items."
                );

            LunarGenCap = Config.Bind(
                "Generator Settings",
                "LunarGenCap",
                "1",
                "The multiplicative max item cap for generating Lunar (blue) items."
                );

            Tier1GenChance = Config.Bind(
                "Generator Settings",
                "Tier1GenChance",
                "40",
                "The percent chance for generating a Tier 1 (white) item."
                );

            Tier2GenChance = Config.Bind(
                "Generator Settings",
                "Tier2GenChance",
                "20",
                "The percent chance for generating a Tier 2 (green) item."
                );

            Tier3GenChance = Config.Bind(
                "Generator Settings",
                "Tier3GenChance",
                "1",
                "The percent chance for generating a Tier 3 (red) item."
                );

            LunarGenChance = Config.Bind(
                "Generator Settings",
                "LunarGenChance",
                "0.5",
                "The percent chance for generating a Lunar (blue) item."
                );

            EquipGenChance = Config.Bind(
                "Generator Settings",
                "EquipGenChance",
                "10",
                "The percent chance for generating a Use item."
                );

            InheritItems = Config.Bind(
                "Inherit Settings",
                "InheritItems",
                false,
                "Toggles enemies to randomly inherit items from a random player. Overrides Generator Settings."
                );

            ItemMultiplier = Config.Bind(
                "General Settings",
                "ItemMultiplier",
                "1",
                "Sets the multiplier for items to be inherited/generated.");

            StageReq = Config.Bind(
                "General Settings",
                "StageReq",
                4,
                "Sets the minimum stage to be cleared before having enemies inherit/generate items."
                );

            ItemsBlacklist = Config.Bind(
                "General Settings",
                "BlacklistItems",
                false,
                "Toggles hard blacklisted items to be inherited/generated."
                );

            Limiter = Config.Bind(
                "General Settings",
                "Limiter",
                true,
                "Toggles certain items to be capped. For more information, check this mod's FAQ at the thunderstore!"
                );
            
            DropItems = Config.Bind(
                "General Settings",
                "DropItems",
                false,
                "Toggles items to be dropped by enemies with items."
                );

            DropChance = Config.Bind(
                "General Settings",
                "DropChance",
                "0.1",
                "Sets the percent chance that an enemy drops one of their items. Default 0.1 means average 1 in a 1000 kills will result in a drop."
                );
            
            Tier1Items = Config.Bind(
                "General Settings",
                "Tier1Items",
                true,
                "Toggles Tier 1 (white) items to be inherited/generated."
                );

            Tier2Items = Config.Bind(
                "General Settings",
                "Tier2Items",
                true,
                "Toggles Tier 2 (green) items to be inherited/generated."
                );

            Tier3Items = Config.Bind(
                "General Settings",
                "Tier3Items",
                true,
                "Toggles Tier 3 (red) items to be inherited/generated."
                );

            LunarItems = Config.Bind(
                "General Settings",
                "LunarItems",
                true,
                "Toggles Lunar (blue) items to be inherited/generated."
                );

            EquipItems = Config.Bind(
                "General Settings",
                "EquipItems",
                false,
                "Toggles Use items to be inherited/generated."
                );

            EquipBlacklist = Config.Bind(
                "General Settings",
                "EquipBlacklist",
                false,
                "Toggles hard blacklisted Use items to be inherited/generated. MOST BLACKLISTED USE ITEMS ARE UNDODGEABLE."
                );

            CustomItemBlacklist = Config.Bind(
                "General Settings",
                "CustomItemBlacklist",
                "",
                "Enter items ids separated by a comma and a space to blacklist certain items. ex) 41, 23, 17 \nItem ids: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
                );

            CustomEquipBlacklist = Config.Bind(
               "General Settings",
               "CustomEquipBlacklist",
               "",
               "Enter equipment ids separated by a comma and a space to blacklist certain equipments. ex) 1, 14, 13 \nEquip ids: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
               );

            CustomItemCaps = Config.Bind(
               "General Settings",
               "CustomItemCaps",
               "",
               "Enter item ids as X-Y separated by a comma and a space to apply caps to certain items. X is the item id and Y is the number cap. ex) 0-20, 1-5, 2-1"
               );
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
            Chat.AddMessage("EnemiesWithItems v1.2.8 Loaded!");
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
