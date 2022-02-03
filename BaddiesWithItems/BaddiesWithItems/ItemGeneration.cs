using RoR2;
using System;
using System.Collections;
using System.Linq;
using System.Text;
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
        }

        private static void onPlayerRemoved(PlayerCharacterMasterController obj)
        {
            if (!NetworkServer.active) return;
            instance.StartCoroutine(RebuildPlayers(obj, true));
            //Rebuild(obj, true);
        }

        private static void onPlayerAdded(PlayerCharacterMasterController obj)
        {
            if (!NetworkServer.active) return;
            instance.StartCoroutine(RebuildPlayers(obj, false));
            //Rebuild(obj, false);
        }

        private static int _cachedPlayerCount;
        private static int _cachedTotalItemCount;

        private static IEnumerator RebuildPlayers(PlayerCharacterMasterController playerExcited, bool removing)
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
            _cachedTotalItemCount = 0;
            _cachedTotalItemCount = GetPlayerItems();
        }

        private static int GetPlayerItems()
        {
            int n = 0;

            for (int i = 0; i < PlayerCharacterMasterController.instances.Count; i++)
            {
                for (int y = 0; y < PlayerCharacterMasterController.instances[i].master.inventory.itemAcquisitionOrder.Count; y++)
                {
                    n += PlayerCharacterMasterController.instances[i].master.inventory.GetItemCount(PlayerCharacterMasterController.instances[i].master.inventory.itemAcquisitionOrder[i]);
                }
            }
            //_cachedTotalItemCount += Util.GetItemCountForTeam(TeamIndex.Player, itemIndex, true, true);
            return n;
        }

        //Future proofing for item tiers defs
        private static ItemDef EvaluateItem()
        {
            WeightedSelection<ItemDef> weightedSelection = new WeightedSelection<ItemDef>(8);
            for (int i = 0; i < EnemiesWithItems.AvailableItemTiers.Length; i++)
            {
                ItemDef itemDef = Run.instance.treasureRng.NextElementUniform<ItemDef>((from itemDefValue in PickupLists.finalItemDefList where itemDefValue != null && itemDefValue.tier == AvailableItemTiers[i] select itemDefValue).ToList());
                weightedSelection.AddChoice(itemDef, ItemTierWeights[i]);
            }

            return weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
        }

        //DLC will probably have tiered equipment... probably...
        private static EquipmentDef EvaluateEquipment()
        {
            EquipmentDef tier1 = Run.instance.treasureRng.NextElementUniform<EquipmentDef>(PickupLists.finalEquipmentDefs);
            WeightedSelection<EquipmentDef> weightedSelection = new WeightedSelection<EquipmentDef>(8);
            weightedSelection.AddChoice(tier1, ConfigToFloat(EquipGenChance.Value));
            weightedSelection.AddChoice(null, 1); //Epic fail!
            return weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
        }

        public static void GenerateItemsToInventory(Inventory inventory, CharacterMaster masterToCopyFrom)
        {
            if (InheritItems.Value) // inheritance
            {
                CleanInventoryDependingOnConfigAndCopyFromMaster(inventory, masterToCopyFrom);
                return;
            }
            if (GenerateItems.Value) // Using generator instead
            {
                if (PickupLists.finalItemDefList == null || PickupLists.finalItemDefList.Length <= 0)
                {
#if DEBUG
                    Debug.LogError("Cannot generate items as finalItemDefList is empty or null.");
#endif
                    return;
                }

                int maxFailedAttempts = 5;
                int maxItemsToGenerate = 0;
                // More balanced behavior, using the average of all players
                maxItemsToGenerate = (int)Math.Pow(Run.instance.stageClearCount + 1, 2) + (_cachedTotalItemCount / _cachedPlayerCount);
                if (Scaling.Value) // If scaling is true, then use the total items of all players in lobby. ORIGINAL BEHAVIOR
                    maxItemsToGenerate = (int)Math.Pow(Run.instance.stageClearCount + 1, 2) + _cachedTotalItemCount;

                int currentFailedAttempts = 0;
                int currentItemsGenerated = 0;
                while (currentItemsGenerated <= maxItemsToGenerate && currentFailedAttempts <= maxFailedAttempts)
                {
                    ItemDef evaluation = EvaluateItem();
                    if (evaluation.itemIndex == ItemIndex.None)
                    {
                        currentFailedAttempts++;
                        continue;
                    }

                    int currentGenCap = 0;
                    int currentItemTierCap = 0;
                    for (int i = 0; i < AvailableItemTiers.Length; i++)
                    {
                        currentItemTierCap = ItemTierCaps[i];
                        if (inventory.GetTotalItemCountOfTier(AvailableItemTiers[i]) > currentItemTierCap && currentItemTierCap > 0)
                        {
                            currentFailedAttempts++;
                            continue;
                        }
                    }

                    int amountToGive = UnityEngine.Random.Range(0, maxItemsToGenerate + 1);
                    float configItemMultiplier = ConfigToFloat(ItemMultiplier.Value);
                    if (configItemMultiplier != 1)
                        amountToGive = Mathf.CeilToInt(amountToGive * configItemMultiplier);
                    if (amountToGive <= 0)
                    {
                        currentFailedAttempts++;
                        continue;
                    }

                    if (LimitedItemsDictionary.ContainsKey(evaluation) && Limiter.Value)
                    {
                        currentGenCap = ((from kvp in LimitedItemsDictionary where kvp.Key == evaluation select kvp.Value)).FirstOrDefault();
                        if (currentGenCap <= 0)
                            amountToGive = Run.instance.stageClearCount + 1;
                    }
                    else if (amountToGive > currentGenCap && currentGenCap > 0)
                        amountToGive = currentGenCap;
                    inventory.GiveItem(evaluation, amountToGive);
                    currentItemsGenerated += amountToGive;
#if DEBUG
                    Debug.Log("A single gen cycle complete, currentItemsGenerated: " + currentItemsGenerated + " amountToGive: " + amountToGive + " currentGenCap: " + currentGenCap);
#endif
                };
#if DEBUG
                Debug.Log("Max Items: " + maxItemsToGenerate + " Amount of items Added: " + currentItemsGenerated);
#endif
            }

            if (EnemiesWithItems.EquipItems.Value && inventory.currentEquipmentIndex == EquipmentIndex.None)
            {
                if (PickupLists.finalEquipmentDefs == null || PickupLists.finalEquipmentDefs.Length <= 0)
                {
#if DEBUG
                    Debug.LogError("Cannot generate equips as equipmentDefs is empty or null.");
#endif
                    return;
                }
                EquipmentDef equipmentDef = EvaluateEquipment();
                if (equipmentDef.equipmentIndex != EquipmentIndex.None)
                {
                    inventory.ResetItem(RoR2Content.Items.AutoCastEquipment);
                    inventory.GiveItem(RoR2Content.Items.AutoCastEquipment);
                    inventory.SetEquipmentIndex(equipmentDef.equipmentIndex);
                }
            }

            return;
        }

        public static void LimitItems(Inventory inventory, ItemDef item, int cap)
        {
            if (inventory.GetItemCount(item) > cap)
            {
                inventory.ResetItem(item);
                inventory.GiveItem(item, cap);
            }
        }

        public static void CleanInventory(Inventory inventory)
        {
            foreach (ItemIndex item in inventory.itemAcquisitionOrder)
            {
                inventory.ResetItem(item);
            }
        }

        public static void CleanInventoryDependingOnConfigAndCopyFromMaster(Inventory inventory, CharacterMaster master)
        {
            foreach (ItemIndex item in inventory.itemAcquisitionOrder)
            {
                foreach (ItemTier itemTier in AvailableItemTiers)
                {
                    if (ItemCatalog.GetItemDef(item).tier == itemTier)
                    {
                        inventory.ResetItem(item);
                    }
                }
            }

            inventory.CopyItemsFrom(master.inventory);
            MultiplyCurrentItems(inventory);

            if (EquipItems.Value)
            {
                inventory.ResetItem(RoR2Content.Items.AutoCastEquipment);
                inventory.GiveItem(RoR2Content.Items.AutoCastEquipment, 1);
                inventory.CopyEquipmentFrom(master.inventory);

                foreach (EquipmentDef item in EquipmentBlackList)
                {
                    if (inventory.GetEquipmentIndex() == item.equipmentIndex)
                    {
                        inventory.SetEquipmentIndex(EquipmentIndex.None);
                        break;
                    }
                }
            }
        }

        public static void MultiplyCurrentItems(Inventory inventory)
        {
            float itemMultiplier = ConfigToFloat(ItemMultiplier.Value);
            if (itemMultiplier != 1f)
            {
                foreach (ItemIndex item in inventory.itemAcquisitionOrder)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef(item);
                    if (ItemBlackList.Contains(itemDef))
                        continue;
                    foreach (var itemTier in AvailableItemTiers)
                    {
                        if (itemDef.tier == itemTier)
                        {
                            int count = inventory.GetItemCount(itemDef);
                            inventory.ResetItem(itemDef);
                            inventory.GiveItem(itemDef, (int)Math.Ceiling(count * itemMultiplier));
                        }
                    }
                }
            }
        }

        [ConCommand(commandName = "ewi_midRunData", flags = ConVarFlags.SenderMustBeServer, helpText = "Shows data specific to the run. Only usable in a run.")]
        private static void PrintMidRunData(ConCommandArgs args)
        {
            if (!Run.instance)
            {
                Debug.LogWarning("Cannot use command, you are not in a run!");
                return;
            }

            StringBuilder bobTheBuilder = new StringBuilder();
            bobTheBuilder.AppendLine("Cached total item count: " + _cachedTotalItemCount);
            bobTheBuilder.AppendLine("Cached player count: " + _cachedPlayerCount);
            if (Scaling.Value) // If scaling is true, then use the total items of all players in lobby. ORIGINAL BEHAVIOR
                bobTheBuilder.Append("Max Items To Generate: " + (int)Math.Pow(Run.instance.stageClearCount + 1, 2) + _cachedTotalItemCount);
            else // More balanced behavior, using the average of all players
                bobTheBuilder.Append("Max Items To Generate: " + (int)Math.Pow(Run.instance.stageClearCount + 1, 2) + (_cachedTotalItemCount / _cachedPlayerCount));
            Debug.Log(bobTheBuilder.ToString());
        }
    }
}