using RoR2;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static BaddiesWithItems.EnemiesWithItems;

namespace BaddiesWithItems
{
    internal class ItemGeneration
    {
        [SystemInitializer(new Type[]
        {   typeof(ItemCatalog),
            typeof(PickupCatalog),
        })]
        private static void Init()
        {
            PlayerCharacterMasterController.onPlayerAdded += onPlayerAdded;
            PlayerCharacterMasterController.onPlayerRemoved += onPlayerRemoved;

            //Going to break with DLC
            //TODO: OPTIMIZATION, item generation will be taken from here, instead of Generating items -> cleaning up if item is banned, why not remove banned items from here?
            foreach (var item in ItemCatalog.tier1ItemList)
            {
                HG.ArrayUtils.ArrayAppend(ref allTier1Pickups, PickupCatalog.itemIndexToPickupIndex[(int)item]);
            }
            foreach (var item in ItemCatalog.tier2ItemList)
            {
                HG.ArrayUtils.ArrayAppend(ref allTier2Pickups, PickupCatalog.itemIndexToPickupIndex[(int)item]);
            }
            foreach (var item in ItemCatalog.tier3ItemList)
            {
                HG.ArrayUtils.ArrayAppend(ref allTier3Pickups, PickupCatalog.itemIndexToPickupIndex[(int)item]);
            }
            foreach (var item in ItemCatalog.lunarItemList)
            {
                HG.ArrayUtils.ArrayAppend(ref allTierLunarPickups, PickupCatalog.itemIndexToPickupIndex[(int)item]);
            }
            foreach (var item in EquipmentCatalog.equipmentList)
            {
                HG.ArrayUtils.ArrayAppend(ref allEquipmentPickups, PickupCatalog.equipmentIndexToPickupIndex[(int)item]);
            }
        }

        private static void onPlayerRemoved(PlayerCharacterMasterController obj)
        {
            if (!NetworkServer.active) return;
            instance.StartCoroutine(Rebuild(obj, false));
            //Rebuild(obj, true);
        }

        private static void onPlayerAdded(PlayerCharacterMasterController obj)
        {
            if (!NetworkServer.active) return;
            instance.StartCoroutine(Rebuild(obj, false));
            //Rebuild(obj, false);
        }

        private static int _cachedPlayerCount;
        private static int _cachedTotalItemCount;

        //This is asking for trouble with DLC
        private static PickupIndex[] allTier1Pickups = new PickupIndex[0];
        private static PickupIndex[] allTier2Pickups = new PickupIndex[0];
        private static PickupIndex[] allTier3Pickups = new PickupIndex[0];
        private static PickupIndex[] allTierLunarPickups = new PickupIndex[0];
        private static PickupIndex[] allEquipmentPickups = new PickupIndex[0];

        private static IEnumerator Rebuild(PlayerCharacterMasterController playerExcited, bool removing)
        {
            yield return new WaitForEndOfFrame();

            _cachedPlayerCount = PlayerCharacterMasterController.instances.Count;

            if (removing)
            {
                //does this even do anything if the player no longer exists?
                playerExcited.master.inventory.onInventoryChanged -= onInventoryChanged;
            }
            else
            {
                playerExcited.master.inventory.onInventoryChanged += onInventoryChanged;
            }
        }

        //Players always get items in a slower pace to enemies spawning... this should be less expensive
        private static void onInventoryChanged()
        {
            foreach (var item in ItemCatalog.allItems)
            {
                _cachedTotalItemCount = Util.GetItemCountForTeam(TeamIndex.Player, item, false, true);
            }
        }

        private static PickupIndex EvaluatePickup(bool GenerateEquip)
        {
            PickupIndex tier1 = Run.instance.treasureRng.NextElementUniform<PickupIndex>(allTier1Pickups);
            PickupIndex tier2 = Run.instance.treasureRng.NextElementUniform<PickupIndex>(allTier2Pickups);
            PickupIndex tier3 = Run.instance.treasureRng.NextElementUniform<PickupIndex>(allTier3Pickups);
            PickupIndex tier4 = Run.instance.treasureRng.NextElementUniform<PickupIndex>(allTierLunarPickups);
            WeightedSelection<PickupIndex> weightedSelection = new WeightedSelection<PickupIndex>(8);
            weightedSelection.AddChoice(tier1, ConfigToFloat(Tier1GenChance.Value));
            weightedSelection.AddChoice(tier2, ConfigToFloat(Tier2GenChance.Value));
            weightedSelection.AddChoice(tier3, ConfigToFloat(Tier3GenChance.Value));
            weightedSelection.AddChoice(tier4, ConfigToFloat(LunarGenChance.Value));

            if (GenerateEquip)
            {
                PickupIndex equip = Run.instance.treasureRng.NextElementUniform<PickupIndex>(allEquipmentPickups);
                weightedSelection.AddChoice(equip, ConfigToFloat(EquipGenChance.Value));
            }

            return weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
        }

        public static void GenerateItemsToInventory(Inventory inventory, CharacterMaster masterToCopyFrom)
        {
            if (InheritItems.Value) // inheritance
            {
                updateInventory(inventory, masterToCopyFrom);
            }
            else if (GenerateItems.Value) // Using generator instead
            {
                //resetInventory(inventory);

                int scc = Run.instance.stageClearCount + 1;
                int maxItemsToGenerate = 0;
                int currentItemsGenerated = 0;

                if (Scaling.Value) // If scaling is true, then use the total items of all players in lobby. ORIGINAL BEHAVIOR
                    maxItemsToGenerate = (int)Math.Pow(scc, 2) + _cachedTotalItemCount;
                else // More balanced behavior, using the average of all players
                    maxItemsToGenerate = (int)Math.Pow(scc, 2) + (_cachedTotalItemCount / _cachedPlayerCount);

                do
                {
                    bool generationIsValid = false;
                    do
                    {
                        PickupDef evaluation = PickupCatalog.GetPickupDef(EvaluatePickup(inventory.currentEquipmentIndex == EquipmentIndex.None));
                        if (evaluation.itemIndex == ItemIndex.None)
                        {
                            EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(evaluation.equipmentIndex);
                            generationIsValid = true; //Let's use it as a flag.
                            if (AffixEquips.Contains(equipmentDef))
                                generationIsValid = false;
                            if (!EquipBlacklist.Value)
                                if (EquipmentBlacklist.Contains(equipmentDef)) // Hard blacklisted
                                    generationIsValid = false;

                            if (generationIsValid)
                            {
                                inventory.ResetItem(RoR2Content.Items.AutoCastEquipment);
                                inventory.GiveItem(RoR2Content.Items.AutoCastEquipment, 1);

                                inventory.SetEquipmentIndex(evaluation.equipmentIndex);
                            }
                        }
                        else
                        {
                            ItemDef itemDef = ItemCatalog.GetItemDef(evaluation.itemIndex);
                            switch (itemDef.tier)
                            {
                                case ItemTier.Tier1:
                                    if (Tier1Items.Value && inventory.GetTotalItemCountOfTier(ItemTier.Tier1) <= ConfigToFloat(Tier1GenCap.Value))
                                    {
                                        int amountToGive = UnityEngine.Random.Range(0, (int)(Math.Pow(scc, 2) * ConfigToFloat(Tier1GenCap.Value) + 1));
                                        int real = (int)Mathf.Min(amountToGive, ConfigToFloat(Tier1GenCap.Value));
                                        if (real > maxItemsToGenerate)
                                        {
                                            real = -maxItemsToGenerate;
                                        }
                                        inventory.GiveItem(itemDef, real);
                                        currentItemsGenerated += real;
                                        generationIsValid = true;
                                    }
                                    break;

                                case ItemTier.Tier2:
                                    if (Tier2Items.Value && inventory.GetTotalItemCountOfTier(ItemTier.Tier2) <= ConfigToFloat(Tier2GenCap.Value))
                                    {
                                        int amountToGive = UnityEngine.Random.Range(0, (int)(Math.Pow(scc, 2) * ConfigToFloat(Tier2GenCap.Value) + 1));
                                        int real = (int)Mathf.Min(amountToGive, ConfigToFloat(Tier2GenCap.Value));
                                        if (real > maxItemsToGenerate)
                                        {
                                            real = -maxItemsToGenerate;
                                        }
                                        inventory.GiveItem(itemDef, real);
                                        currentItemsGenerated += real;
                                        generationIsValid = true;
                                    }
                                    break;

                                case ItemTier.Tier3:
                                    if (Tier3Items.Value && inventory.GetTotalItemCountOfTier(ItemTier.Tier3) <= ConfigToFloat(Tier3GenCap.Value))
                                    {
                                        int amountToGive = UnityEngine.Random.Range(0, (int)(Math.Pow(scc, 2) * ConfigToFloat(Tier3GenCap.Value) + 1));
                                        int real = (int)Mathf.Min(amountToGive, ConfigToFloat(Tier3GenCap.Value));
                                        if (real > maxItemsToGenerate)
                                        {
                                            real = -maxItemsToGenerate;
                                        }
                                        inventory.GiveItem(itemDef, real);
                                        currentItemsGenerated += real;
                                        generationIsValid = true;
                                    }
                                    break;

                                case ItemTier.Lunar:
                                    if (LunarItems.Value && inventory.GetTotalItemCountOfTier(ItemTier.Lunar) <= ConfigToFloat(LunarGenCap.Value))
                                    {
                                        int amountToGive = UnityEngine.Random.Range(0, (int)(Math.Pow(scc, 2) * ConfigToFloat(LunarGenCap.Value) + 1));
                                        int real = (int)Mathf.Min(amountToGive, ConfigToFloat(LunarGenCap.Value));
                                        if (real > maxItemsToGenerate)
                                        {
                                            real = -maxItemsToGenerate;
                                        }
                                        inventory.GiveItem(itemDef, real);
                                        currentItemsGenerated += real;
                                        generationIsValid = true;
                                    }
                                    break;

                                case ItemTier.Boss:
                                    break;
                            }
                        }
                    } while (!generationIsValid);
                } while (currentItemsGenerated < maxItemsToGenerate);

#if DEBUG
                Debug.Log("Max Items: " + maxItemsToGenerate + " Items Added: " + currentItemsGenerated);
#endif
                MultiplyCurrentItems(inventory);
                RemoveBlacklistedItems(inventory);
            }
            else
            {
                //resetInventory(inventory);
                return;
            }
        }
    }
}