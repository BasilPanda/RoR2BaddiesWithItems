using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaddiesWithItems
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Basil." + ModIdentifier, "EnemiesWithItems", ModVer)]
    public class EnemiesWithItems : BaseUnityPlugin
    {
        internal const string ModIdentifier = "EnemiesWithItems";
        internal const string ModVer = "2.0.5";

        public static EnemiesWithItems instance;

        #region Config Entries

        public static ConfigEntry<bool> GenerateItems;
        public static ConfigEntry<string> ItemMultiplier;
        public static ConfigEntry<bool> Scaling;

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

        #endregion Config Entries

        public void InitConfig()
        {
            GenerateItems = Config.Bind(
                "Generator Settings",
                "GenerateItems",
                true,
                "Toggles item generation for enemies."
                );

            Scaling = Config.Bind(
                "Generator Settings",
                "ScaleAllPlayers",
                true,
                "Toggles how the mod balances potential item amount per enemy. True: Total items of all players False: Average item count from players"
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
                6,
                "Sets the stage where enemies start to inherit/generate items. If in the moon within first loop, it will not apply."
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
                "Enter item code name separated by a comma and a space to blacklist certain items. ex) PersonalShield, Syringe\nItem names: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
                );

            CustomEquipBlacklist = Config.Bind(
               "General Settings",
               "CustomEquipBlacklist",
               "",
               "Enter equipment codenames separated by a comma and a space to blacklist certain equipments. ex) Saw, DroneBackup\nEquip names: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
               );

            CustomItemCaps = Config.Bind(
               "General Settings",
               "CustomItemCaps",
               "",
               "Enter item codenames as X-Y separated by a comma and a space to apply caps to certain items. X is the item code name and Y is the number cap. ex) PersonalShield-20, Syringe-5"
               );
        }

        #region Lists

        public static EquipmentDef[] EquipmentBlacklist = new EquipmentDef[]
        {
            RoR2Content.Equipment.Blackhole,
            RoR2Content.Equipment.BFG,
            RoR2Content.Equipment.Lightning,
            RoR2Content.Equipment.Scanner,
            RoR2Content.Equipment.CommandMissile,
            RoR2Content.Equipment.BurnNearby,
            RoR2Content.Equipment.DroneBackup,
            RoR2Content.Equipment.Gateway,
            RoR2Content.Equipment.FireBallDash,
            RoR2Content.Equipment.Saw,
            RoR2Content.Equipment.Recycle,
            RoR2Content.Equipment.OrbitalLaser,     // EQUIPMENT_ORBITALLASER_NAME
            RoR2Content.Equipment.Enigma,           // EQUIPMENT_ENIGMA_NAME
        };

        public static ItemDef[] ItemBlacklist = new ItemDef[]
        {
            RoR2Content.Items.StickyBomb,                   // Sticky Bomb
            RoR2Content.Items.StunChanceOnHit,              // Stun Grenade
            RoR2Content.Items.NovaOnHeal,                   // N'kuhana's Opinion
            RoR2Content.Items.ShockNearby,                  // Tesla Coil
            RoR2Content.Items.Mushroom,                     // Bustling Fungus
            RoR2Content.Items.ExplodeOnDeath,               // Genesis Loop
            RoR2Content.Items.LaserTurbine                  // Resonance Disc
        };

        public static ItemDef[] ItemsNeverUsed = new ItemDef[]
        {
            RoR2Content.Items.ExecuteLowHealthElite,        // Old Guillotine
            RoR2Content.Items.TitanGoldDuringTP,            // Halcyon Seed
            RoR2Content.Items.TreasureCache,                // Rusted Key
            RoR2Content.Items.BossDamageBonus,              // Armor-Piercing Rounds
            RoR2Content.Items.ExtraLifeConsumed,
            RoR2Content.Items.Feather,                      // Hopoo Feather
            RoR2Content.Items.Firework,                     // Bundle of Fireworks
            RoR2Content.Items.SprintArmor,                  // Rose Buckler
            RoR2Content.Items.JumpBoost,                    // Wax Quail
            RoR2Content.Items.GoldOnHit,                    // Brittle Crown
            RoR2Content.Items.WardOnLevel,                  // Warbanner
            RoR2Content.Items.BeetleGland,                  // Queen's Gland
            RoR2Content.Items.CrippleWardOnLevel,
            RoR2Content.Items.TPHealingNova,                // Lepton Daisy
            RoR2Content.Items.LunarTrinket,                 // Beads of Fealty
            //RoR2Content.Items.LunarPrimaryReplacement,      // Visions of Heresy
            //RoR2Content.Items.LunarUtilityReplacement,      // Strides of Heresy
            RoR2Content.Items.BonusGoldPackOnKill,          // Ghor's Tome
            RoR2Content.Items.Squid,                        // Squid Polyp
            RoR2Content.Items.SprintWisp,                   // Little Disciple
            RoR2Content.Items.FocusConvergence,             // Focused Convergence
            RoR2Content.Items.MonstersOnShrineUse,          // Defiant Gouge
            RoR2Content.Items.ScrapWhite,                   // White Scrap
            RoR2Content.Items.ScrapGreen,                   // Green Scrap
            RoR2Content.Items.ScrapRed,                     // Red Scrap
            RoR2Content.Items.ScrapYellow,                  // Yellow Scrap
            // RoR2Content.Items.LunarBadLuck,                 // Purity
            RoR2Content.Items.RoboBallBuddy,                // New Minions.
        };

        public static EquipmentDef[] AffixEquips = new EquipmentDef[]
        {
            RoR2Content.Equipment.AffixBlue,
            RoR2Content.Equipment.AffixEcho,
            RoR2Content.Equipment.AffixGold,
            RoR2Content.Equipment.AffixHaunted,
            RoR2Content.Equipment.AffixLunar,
            RoR2Content.Equipment.AffixPoison,
            RoR2Content.Equipment.AffixRed,
            RoR2Content.Equipment.AffixWhite,
            RoR2Content.Equipment.AffixYellow,
        };

        public static Dictionary<ItemDef, int> LimitedItems = new Dictionary<ItemDef, int>()
        {
            { RoR2Content.Items.Bear, 7},                   // Tougher Times
            { RoR2Content.Items.HealWhileSafe, 30},         // Cautious Slug
            { RoR2Content.Items.EquipmentMagazine, 3},      // Fuel Cell
            { RoR2Content.Items.SlowOnHit, 1},              // Chronobauble
            { RoR2Content.Items.Behemoth, 2},               // Behemoth
            { RoR2Content.Items.BleedOnHit, 3},             // Tri-tip Dagger
            { RoR2Content.Items.IgniteOnKill, 2},           // Gasoline
            { RoR2Content.Items.AutoCastEquipment, 1},      // Gesture of the Drowned
            { RoR2Content.Items.NearbyDamageBonus, 3},      // Focus Crystal
            { RoR2Content.Items.ShinyPearl, -1},            // Shiny Pearl
            { RoR2Content.Items.Pearl, -1},                 // Pearl
            { RoR2Content.Items.Thorns, 1},                 // Razor Wire
            { RoR2Content.Items.DeathMark, -1},             // Death Mark
            { RoR2Content.Items.ArmorPlate, -1},            // Repulsion Armor Plate
        };

        #endregion Lists

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
            instance = this;
            InitConfig();

            Hooks.baddiesItems();
            Chat.AddMessage("EnemiesWithItems v2.0.4 Loaded!");
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

            inventory.CopyItemsFrom(master.inventory);
            MultiplyCurrentItems(inventory);

            if (EquipItems.Value)
            {
                inventory.ResetItem(RoR2Content.Items.AutoCastEquipment);
                inventory.GiveItem(RoR2Content.Items.AutoCastEquipment, 1);
                inventory.CopyEquipmentFrom(master.inventory);

                foreach (EquipmentDef item in EquipmentBlacklist)
                {
                    if (inventory.GetEquipmentIndex() == item.equipmentIndex)
                    {
                        inventory.SetEquipmentIndex(RoR2Content.Equipment.QuestVolatileBattery.equipmentIndex); // default to Fuel Array
                        break;
                    }
                }
            }

            RemoveBlacklistedItems(inventory);
        }

        //This is so so going to break with DLC
        public static void MultiplyCurrentItems(Inventory inventory)
        {
            float itemMultiplier = ConfigToFloat(ItemMultiplier.Value);
            if (itemMultiplier != 1f)
            {
                int count = 0;
                foreach (ItemIndex item in inventory.itemAcquisitionOrder)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef(item);
                    switch (itemDef.tier)
                    {
                        case ItemTier.Tier1:
                            if (Tier1Items.Value)
                            {
                                count = inventory.GetItemCount(itemDef);
                                inventory.ResetItem(itemDef);
                                inventory.GiveItem(itemDef, (int)Math.Ceiling(count * itemMultiplier));
                            }
                            break;

                        case ItemTier.Tier2:
                            if (Tier2Items.Value)
                            {
                                count = inventory.GetItemCount(itemDef);
                                inventory.ResetItem(itemDef);
                                inventory.GiveItem(itemDef, (int)Math.Ceiling(count * itemMultiplier));
                            }
                            break;

                        case ItemTier.Tier3:
                            if (Tier3Items.Value)
                            {
                                count = inventory.GetItemCount(itemDef);
                                inventory.ResetItem(itemDef);
                                inventory.GiveItem(itemDef, (int)Math.Ceiling(count * itemMultiplier));
                            }
                            break;

                        case ItemTier.Lunar:
                            if (LunarItems.Value)
                            {
                                count = inventory.GetItemCount(itemDef);
                                inventory.ResetItem(itemDef);
                                inventory.GiveItem(itemDef, (int)Math.Ceiling(count * itemMultiplier));
                            }
                            break;

                        case ItemTier.Boss:
                            break;
                    }
                }
            }
        }

        public static void RemoveBlacklistedItems(Inventory inventory)
        {
            foreach (ItemDef item in ItemsNeverUsed)
            {
                inventory.ResetItem(item);
            }

            if (!ItemsBlacklist.Value)
            {
                foreach (ItemDef item in ItemBlacklist)
                {
                    inventory.ResetItem(item);
                }
            }
            int stageClearCount = Run.instance.stageClearCount;
            // Limiter
            if (Limiter.Value)
            {
                foreach (ItemDef item in LimitedItems.Keys)
                {
                    if (LimitedItems[item] == -1)
                    {
                        LimitItems(inventory, item, stageClearCount);
                    }
                    else
                    {
                        LimitItems(inventory, item, LimitedItems[item]);
                    }
                }
            }

            ConfigItemBlacklist(inventory);
            ConfigEquipBlacklist(inventory);
            ConfigItemCaps(inventory);
        }

        public static void LimitItems(Inventory inventory, ItemDef item, int cap)
        {
            if (inventory.GetItemCount(item) > cap)
            {
                inventory.ResetItem(item);
                inventory.GiveItem(item, cap);
            }
        }

        public static void ConfigEquipBlacklist(Inventory inventory)
        {
            // Custom Equip Blacklist
            string[] customEquiplist = CustomEquipBlacklist.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string equipName in customEquiplist)
            {
                EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex(equipName));
                if (equipmentDef != null)
                {
                    if (inventory.GetEquipmentIndex() == equipmentDef.equipmentIndex)
                    {
                        inventory.SetEquipmentIndex(EquipmentIndex.None);
                    }
                }
            }
        }

        //From config file, find itemdef, remove items if entry exists
        public static void ConfigItemBlacklist(Inventory inventory)
        {
            string[] customItemlist = CustomItemBlacklist.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string itemName in customItemlist)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(itemName));
                if (itemDef != null)
                {
                    inventory.ResetItem(itemDef);
                }
            }
        }

        //From config file, find itemdef, get item cap, remove items and set to cap if over itemcap
        public static void ConfigItemCaps(Inventory inventory)
        {
            string[] customItemCaps = CustomItemCaps.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string itemCapEntry in customItemCaps)
            {
                string[] ItemCapPair = itemCapEntry.Split(new[] { '-' });
                if (ItemCapPair.Length == 2)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(ItemCapPair[0]));
                    if (itemDef != null && Int32.TryParse(ItemCapPair[1], out int cap))
                    {
                        if (inventory.GetItemCount(itemDef) > cap)
                        {
                            inventory.ResetItem(itemDef);
                            inventory.GiveItem(itemDef, cap);
                        }
                    }
                }
            }
        }

        [ConCommand(commandName = "ewi_reloadconfig", flags = ConVarFlags.SenderMustBeServer, helpText = "Reloads the config file, server only.")]
        private static void ReloadConfig(ConCommandArgs args)
        {
            Debug.LogWarning("Enemies with Items config reloaded!");
            instance.Config.Reload();
        }
    }
}