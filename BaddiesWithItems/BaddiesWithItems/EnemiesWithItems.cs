using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaddiesWithItems
{
    [BepInPlugin("com.Basil." + ModIdentifier, ModIdentifier, ModVer)]
    public class EnemiesWithItems : BaseUnityPlugin
    {
        internal const string ModIdentifier = "EnemiesWithItems";
        internal const string ModVer = "3.0.0";

        public static EnemiesWithItems instance;

        #region Config Entries

        public static ConfigEntry<bool> GenerateItems;
        public static ConfigEntry<string> ItemMultiplier;
        public static ConfigEntry<bool> Scaling;

        public static ConfigEntry<int> StageReq;
        public static ConfigEntry<bool> InheritItems;

        public static ConfigEntry<string> EquipGenChance;

        public static ConfigEntry<string> CustomItemBlacklist;
        public static ConfigEntry<string> ConfigEquipBlacklist;
        public static ConfigEntry<string> ConfigItemLimiter;
        public static ConfigEntry<string> ConfigItemTiersEnabledWeights;
        public static ConfigEntry<string> ConfigItemTiersCaps;

        public static ConfigEntry<bool> ItemsBlacklist;
        public static ConfigEntry<bool> Limiter;

        public static ConfigEntry<bool> EquipItems;
        public static ConfigEntry<bool> EquipBlacklist;
        public static ConfigEntry<bool> EquipAutoBanElite;

        public static ConfigEntry<string> DropChance;

        #endregion Config Entries

        public void InitConfigFileValues()
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

            EquipGenChance = Config.Bind(
                "Generator Settings",
                "EquipGenChance",
                "10",
                "The percent chance for generating a Use item."
                );

            InheritItems = Config.Bind(
                "Generator Settings",
                "InheritItems",
                false,
                "Toggles enemies to randomly inherit items from a random player. Overrides Generator Settings."
                );

            ItemMultiplier = Config.Bind(
                "Generator Settings",
                "ItemMultiplier",
                "1",
                "Multiplies the monster's inventory by this amount after generating items.");

            StageReq = Config.Bind(
                "General Settings",
                "StageReq",
                6,
                "Sets the stage where enemies start to inherit/generate items. If in the final stage in the first loop, it will not apply."
                );

            Limiter = Config.Bind(
                "Generator Settings",
                "Limiter",
                true,
                "Should it clean up the enemies' inventories post item generation to accomodate the item limiter blacklist."
                );

            DropChance = Config.Bind(
                "General Settings",
                "DropChance",
                "0.1",
                "Sets the percent chance that an enemy drops one of their items. Default 0.1 means average 1 in a 1000 kills will result in a drop.\nSet to zero (0) to disable dropping."
                );

            ConfigItemTiersEnabledWeights = Config.Bind(
               "General Settings",
               "ConfigItemTiersWeights",
               "Tier1-40, Tier2-20, Tier3-1, Lunar-0.5",
               "Percentage chance of items of a certain tier to be generated.\nEnter the names of item tiers as X-Y separated by a comma and a space. X as tier name & Y as percentage. ex) Tier1-40, Tier2-20.\nA item tier lacking an entry here will not be generated."
               );

            ConfigItemTiersCaps = Config.Bind(
               "General Settings",
               "ConfigItemTiersCaps",
               "Tier1-0, Tier2-0, Tier3-0, Lunar-0",
               "Max of items of a certain tier to be generated.\nEnter the names of item tiers as X-Y separated by a comma and a space. X as tier name & Y as percentage. ex) Tier1-40, Tier2-20.\nA zero (0) disables the cap. Item tiers lacking an entry here, but that exists in ConfigItemTiersEnabledWeights will default to zero."
               );

            EquipItems = Config.Bind(
                "Generator Settings",
                "EquipItems",
                false,
                "Toggles Use items to be inherited/generated."
                );

            EquipAutoBanElite = Config.Bind(
                "Generator Settings",
                "EquipAutoBanElite",
                true,
                "Should all elite equipment be automatically banned from generating for enemies. Works for unused elites (Annihilators) or modded ones (if they were added to the game properly)."
                );

            CustomItemBlacklist = Config.Bind(
                "Ban Lists",
                "CustomItemBlacklist",
                "StickyBomb, StunChanceOnHit, NovaOnHeal, ShockNearby, Mushroom, ExplodeOnDeath, LaserTurbine, ExecuteLowHealthElite, TitanGoldDuringTP, TreasureCache, BossDamageBonus, ExtraLifeConsumed, Feather, Firework, SprintArmor, JumpBoost, GoldOnHit, WardOnLevel, BeetleGland, CrippleWardOnLevel, TPHealingNova, LunarTrinket, BonusGoldPackOnKill, Squid, SprintWisp, FocusConvergence, MonstersOnShrineUse, ScrapWhite, ScrapGreen, ScrapRed, ScrapYellow, RoboBallBuddy",
                "Enter item code name separated by a comma and a space to blacklist certain items. ex) PersonalShield, Syringe\nItem names: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
                );

            ConfigEquipBlacklist = Config.Bind(
               "Ban Lists",
               "CustomEquipBlacklist",
               "Blackhole, BFG, Lightning, Scanner, CommandMissile, BurnNearby, DroneBackup, Gateway, FireBallDash, Saw, Recycle, OrbitalLaser, Enigma",
               "Enter equipment codenames separated by a comma and a space to blacklist certain equipments. ex) Saw, DroneBackup\nEquip names: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
               );

            ConfigItemLimiter = Config.Bind(
               "Ban Lists",
               "CustomItemCaps",
               "Bear-7, HealWhileSafe-30, SlowOnHit-1, Behemoth-2, BleedOnHit-3, IgniteOnKill-2, AutoCastEquipment-1, NearbyDamageBonus-3, ShinyPearl-0, Pearl-0, Thorns-1, DeathMark-0, ArmorPlate-0",
               "Enter item codenames as X-Y separated by a comma and a space to apply caps to certain items. X is the item code name and Y is the number cap. ex) PersonalShield-20, Syringe-5. A zero (0) makes the item be limited by the current number of cleared stages."
               );
        }

        #region Lists

        // These ones remain here instead of PickupLists because i believe they are part of the config.
        public static EquipmentDef[] EquipmentBlackList = new EquipmentDef[0];

        public static ItemDef[] ItemBlackList = new ItemDef[0];

        public static ItemTier[] AvailableItemTiers = new ItemTier[0];
        public static float[] ItemTierWeights = new float[0];
        public static int[] ItemTierCaps = new int[0];

        /*{
            RoR2Content.Items.StickyBomb,                   // Sticky Bomb
            RoR2Content.Items.StunChanceOnHit,              // Stun Grenade
            RoR2Content.Items.NovaOnHeal,                   // N'kuhana's Opinion
            RoR2Content.Items.ShockNearby,                  // Tesla Coil
            RoR2Content.Items.Mushroom,                     // Bustling Fungus
            RoR2Content.Items.ExplodeOnDeath,               // Genesis Loop
            RoR2Content.Items.LaserTurbine,                  // Resonance Disc

            //Items never used by enemies
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
        };*/
        public static Dictionary<ItemDef, int> LimitedItemsDictionary = new Dictionary<ItemDef, int>(); //Now already in config lol.
        /*{
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
        };*/

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

            RoR2Application.onLoad += (delegate ()
            {
                Debug.LogWarning("Loading EnemiesWithItems " + ModVer + "...");

                //Done after any system initializer, hopefully. Equipments will have elites already in.

                InitConfigFileValues();
                RebuildConfig();

                Debug.Log("EnemiesWithItems " + ModVer + " Loaded!");
            });
        }

        /*[SystemInitializer(new Type[]
        {   typeof(EliteCatalog),
        })]*/

        public static void AppendEliteEquipment() //Previously it only added vanilla elite equips
        {
            foreach (EliteDef item in EliteCatalog.eliteDefs)
            {
                HG.ArrayUtils.ArrayAppend(ref EquipmentBlackList, item.eliteEquipmentDef);
            }
        }

        public void RebuildConfig()
        {
            AvailableItemTiers = new ItemTier[0];
            ItemTierWeights = new float[0];

            ItemBlackList = new ItemDef[0];
            EquipmentBlackList = new EquipmentDef[0];
            LimitedItemsDictionary.Clear();

            ReadConfigItemTiers();
            ReadConfigItemTierLimiter();

            ReadConfigEquipmentBlacklist();
            ReadConfigItemBlacklist();
            ReadConfigItemLimiter();

            if (EquipAutoBanElite.Value)
                AppendEliteEquipment();

            PickupLists.BuildItemAndEquipmentDictionaries();

            Debug.Log("EnemiesWithItems config loaded.");
        }

        public static void ReadConfigEquipmentBlacklist()
        {
            string[] customEquiplist = ConfigEquipBlacklist.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string equipName in customEquiplist)
            {
                EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex(equipName));
                if (equipmentDef.equipmentIndex != EquipmentIndex.None)
                {
                    HG.ArrayUtils.ArrayAppend(ref EquipmentBlackList, equipmentDef);
                }
            }
        }

        public static void ReadConfigItemBlacklist()
        {
            string[] customItemlist = CustomItemBlacklist.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string itemName in customItemlist)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(itemName));
                if (itemDef.itemIndex != ItemIndex.None)
                {
                    HG.ArrayUtils.ArrayAppend(ref ItemBlackList, itemDef);
                }
            }
        }

        public static void ReadConfigItemLimiter()
        {
            string[] customItemCaps = ConfigItemLimiter.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string itemCapEntry in customItemCaps)
            {
                string[] ItemCapPair = itemCapEntry.Split(new[] { '-' });
                if (ItemCapPair.Length == 2)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(ItemCapPair[0]));
                    if (!LimitedItemsDictionary.ContainsKey(itemDef) && itemDef.itemIndex != ItemIndex.None && Int32.TryParse(ItemCapPair[1], out int cap))
                    {
                        LimitedItemsDictionary.Add(itemDef, cap);
                    }
                }
            }
        }

        public static void ReadConfigItemTiers()
        {
            string[] itemTierWeightList = ConfigItemTiersEnabledWeights.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string itemTierEntry in itemTierWeightList)
            {
                string[] itemTierWeightPair = itemTierEntry.Split(new[] { '-' });
                if (itemTierWeightPair.Length == 2)
                {
                    //TODO: Replace with a query to ItemTierDefCatalog
                    ItemTier itemTier = (ItemTier)Enum.Parse(typeof(ItemTier), itemTierWeightPair[0]);
                    if (float.TryParse(itemTierWeightPair[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float weight))
                    {
                        HG.ArrayUtils.ArrayAppend(ref AvailableItemTiers, itemTier);
                        HG.ArrayUtils.ArrayAppend(ref ItemTierWeights, weight);
                    }
                }
            }
        }

        public static void ReadConfigItemTierLimiter()
        {
            ItemTierCaps = new int[AvailableItemTiers.Length];

            string[] customItemCaps = ConfigItemTiersCaps.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string itemCapEntry in customItemCaps)
            {
                string[] ItemCapPair = itemCapEntry.Split(new[] { '-' });
                if (ItemCapPair.Length == 2)
                {
                    //TODO: Replace with a query to ItemTierDefCatalog
                    ItemTier itemTier = (ItemTier)Enum.Parse(typeof(ItemTier), ItemCapPair[0]);
                    for (int i = 0; i < AvailableItemTiers.Length; i++)
                    {
                        if (AvailableItemTiers[i] == itemTier && Int32.TryParse(ItemCapPair[1], out int cap))
                        {
                            ItemTierCaps[i] = cap;
                        }
                    }
                }
            }
        }

        [ConCommand(commandName = "ewi_reloadconfig", flags = ConVarFlags.SenderMustBeServer, helpText = "Reloads the config file, server only.")]
        private static void ReloadConfig(ConCommandArgs args)
        {
            instance.Config.Reload();
            instance.RebuildConfig();
            Debug.LogWarning("Enemies with Items config reloaded!");
        }

        [ConCommand(commandName = "ewi_dumpItemBlackList", flags = ConVarFlags.SenderMustBeServer, helpText = "Dumps the currently loaded item blacklist.")]
        private static void DumpItemBlackList(ConCommandArgs args)
        {
            Debug.Log("Item Blacklist has a length of " + ItemBlackList.Length);
            int counter = 0;
            StringBuilder stringbuilder = new StringBuilder();
            foreach (ItemDef item in ItemBlackList)
            {
                if (item.itemIndex != ItemIndex.None)
                {
                    stringbuilder.Append((item) + " | ");
                    counter++;
                }
            }
            Debug.Log(stringbuilder.ToString() + counter);
        }

        [ConCommand(commandName = "ewi_dumpEquipBlackList", flags = ConVarFlags.SenderMustBeServer, helpText = "Dumps the currently loaded equipment blacklist.")]
        private static void DumpEquipBlackList(ConCommandArgs args)
        {
            Debug.Log("Equipment Blacklist has a length of " + EquipmentBlackList.Length);
            int counter = 0;
            StringBuilder strongBuilder = new StringBuilder();
            foreach (EquipmentDef equipmentDef in EquipmentBlackList)
            {
                if (equipmentDef.equipmentIndex != EquipmentIndex.None)
                {
                    strongBuilder.Append((equipmentDef) + " | ");
                    counter++;
                }
            }
            Debug.Log(strongBuilder.ToString() + counter);
        }

        [ConCommand(commandName = "ewi_dumpLimiterBlackList", flags = ConVarFlags.SenderMustBeServer, helpText = "Dumps the currently loaded item limiter dictionary.")]
        private static void DumpLimiterBlackList(ConCommandArgs args)
        {
            Debug.Log("Item Limiter dictionary has a count of " + LimitedItemsDictionary.Count);
            int counter = 0;
            StringBuilder textConstructor = new StringBuilder();
            foreach (KeyValuePair<ItemDef, int> keyValuePair in LimitedItemsDictionary)
            {
                if (keyValuePair.Key.itemIndex != ItemIndex.None)
                {
                    textConstructor.Append((keyValuePair) + " Amnt: " + (keyValuePair.Value) + " | ");
                    counter++;
                }
            }
            Debug.Log(textConstructor.ToString() + counter);
        }

        [ConCommand(commandName = "ewi_dumpAllItemTierData", flags = ConVarFlags.SenderMustBeServer, helpText = "Dumps all the currently loaded arrays related to ItemTiers.")]
        private static void DumpAllItemTierData(ConCommandArgs args)
        {
            Debug.Log("Length of available item tiers: " + AvailableItemTiers.Length);
            Debug.Log("Length of weighted item tiers (has to be the same as previous): " + ItemTierWeights.Length);
            Debug.Log("Length of capped item tiers: " + ItemTierCaps.Length);
            int counter = 0;
            StringBuilder textConstructor = new StringBuilder();
            for (int i = 0; i < AvailableItemTiers.Length; i++)
            {
                textConstructor.Append(AvailableItemTiers[i] + " W: " + ItemTierWeights[i] + " C: " + ItemTierCaps[i] + " | ");
                counter++;
            }
            Debug.Log(textConstructor.ToString() + counter);
        }
    }
}