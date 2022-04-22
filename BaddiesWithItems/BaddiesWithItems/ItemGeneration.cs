using RoR2;
using RoR2.CharacterAI;
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
            typeof(ItemTierCatalog)
        })]
        private static void Init()
        {
            Run.onServerGameOver += onServerGameOver;
            PlayerCharacterMasterController.onPlayerAdded += onPlayerAdded;
            PlayerCharacterMasterController.onPlayerRemoved += onPlayerRemoved;
        }

        private static void onServerGameOver(Run arg1, GameEndingDef arg2)
        {
            _cachedTotalItemCount = 0;
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

        private static int maxItemsToGenerate;

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
                CharacterMaster characterMaster = PlayerCharacterMasterController.instances[i].master;
                if (characterMaster)
                {
                    for (int y = 0; y < characterMaster.inventory.itemAcquisitionOrder.Count; y++)
                    {
                        ItemIndex itemIndex = characterMaster.inventory.itemAcquisitionOrder[y];
                        if (!ItemCatalog.GetItemDef(itemIndex).hidden)
                        {
                            n += characterMaster.inventory.GetItemCount(itemIndex);
                        }
                    }
                }
            }
            //_cachedTotalItemCount += Util.GetItemCountForTeam(TeamIndex.Player, itemIndex, true, true);
            return n;
        }

        private static ItemDef EvaluateItem()
        {
            WeightedSelection<ItemDef> weightedSelection = new WeightedSelection<ItemDef>(8);
            for (int i = 0; i < EnemiesWithItems.AvailableItemTierDefs.Length; i++)
            {
                ItemDef itemDef = Run.instance.treasureRng.NextElementUniform<ItemDef>((from itemDefValue in PickupLists.finalItemDefList where itemDefValue != null && itemDefValue.tier == AvailableItemTierDefs[i].tier select itemDefValue).ToList());
                weightedSelection.AddChoice(itemDef, ItemTierWeights[i]);
            }

            return weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
        }

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
            maxItemsToGenerate = 0;

            if (InheritItems.Value || (inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0 || inventory.GetItemCount(DLC1Content.Items.GummyCloneIdentifier) > 0)) // inheritance or umbrae, which inherit from players.
            {
#if DEBUG
                Debug.Log("Going to generate items from an inventory. " + inventory + " Master: " + masterToCopyFrom);
#endif
                CleanInventoryDependingOnConfigAndCopyFromMaster(inventory, masterToCopyFrom);
                return;
            }
            if (GenerateItems.Value) // Using generator instead
            {
#if DEBUG
                Debug.Log("Going to use the Item Generator for inventory: " + inventory);
#endif
                if (PickupLists.finalItemDefList == null || PickupLists.finalItemDefList.Length <= 0)
                {
#if DEBUG
                    Debug.LogError("Cannot generate items as finalItemDefList is empty or null.");
#endif
                    return;
                }

                // If scaling is true, then use the total items of all players in lobby. ORIGINAL BEHAVIOR
                maxItemsToGenerate = (int)Math.Pow(Run.instance.stageClearCount + 1, 2) + _cachedTotalItemCount;
                if (!Scaling.Value)  // More balanced behavior, using the average of all players
                    maxItemsToGenerate = (int)Math.Pow(Run.instance.stageClearCount + 1, 2) + (_cachedTotalItemCount / _cachedPlayerCount);

                float destruction = ConfigToFloat(MaxItemsToGenerateMultiplier.Value);
                if (destruction != 1)
                {
#if DEBUG
                    Debug.Log("Modifying maximum amount of items to generate by " + destruction + " original amount: " + maxItemsToGenerate + " target amount: " + Mathf.CeilToInt(maxItemsToGenerate * destruction));
#endif
                    maxItemsToGenerate = Mathf.CeilToInt(maxItemsToGenerate * destruction);
                }

                int currentFailedAttempts = 0;
                int currentItemsGenerated = 0;
                while (currentItemsGenerated < maxItemsToGenerate && currentFailedAttempts <= maxFailedAttempts)
                {
                    ItemDef evaluation = EvaluateItem();
                    if (evaluation == null || evaluation.itemIndex == ItemIndex.None || Run.instance.IsItemExpansionLocked(evaluation.itemIndex))
                    {
                        currentFailedAttempts++;
                        continue;
                    }

                    int currentGenCap = 0;
                    int currentItemTierCap = 0;
                    for (int i = 0; i < AvailableItemTierDefs.Length; i++)
                    {
                        currentItemTierCap = ItemTierCaps[i];
                        if (inventory.GetTotalItemCountOfTier(AvailableItemTierDefs[i].tier) > currentItemTierCap && currentItemTierCap > 0)
                        {
#if DEBUG
                            Debug.LogError("Generation failed due to item tier limitation: " + currentItemTierCap + " items of tier " + AvailableItemTierDefs[i].tier);
#endif
                            currentFailedAttempts++;
                            continue;
                        }
                    }

                    int amountToGive = UnityEngine.Random.Range(0, Mathf.Max((maxItemsToGenerate - currentItemsGenerated), 1));
                    //int amountToGive = UnityEngine.Random.Range(0, maxItemsToGenerate);
                    float configItemMultiplier = ConfigToFloat(ItemMultiplier.Value);
                    if (configItemMultiplier != 1f && configItemMultiplier != 0f)
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
#if DEBUG
                        Debug.LogWarning("The limiter is on, and we found a item that is meant to be limited: " + evaluation + " amountToGive: " + amountToGive + " currentGenCap: " + currentGenCap + " current stage: " + (Run.instance.stageClearCount + 1));
#endif
                    }
                    if (amountToGive > currentGenCap && currentGenCap > 0)
                        amountToGive = currentGenCap;

                    if (FixItemMultiplierCaps.Value && amountToGive > maxItemsToGenerate - currentItemsGenerated)
                    {
                        amountToGive = maxItemsToGenerate - currentItemsGenerated;
                    }

                    inventory.GiveItem(evaluation, amountToGive);
                    currentItemsGenerated += amountToGive;
#if DEBUG
                    Debug.Log("A single gen cycle complete, currentItemsGenerated: " + currentItemsGenerated + " amountToGive: " + amountToGive + " currentGenCap: " + currentGenCap + " ItemGenerated: " + evaluation);
#endif
                };
#if DEBUG
                Debug.Log("Gen cycles complete. MaxItemsToGenerate: " + maxItemsToGenerate + " AmountOfItemsAdded: " + currentItemsGenerated + " FailedRerolls: " + currentFailedAttempts);
#endif
            }

            if (EnemiesWithItems.EquipItems.Value && inventory.currentEquipmentIndex == EquipmentIndex.None)
            {
                if (PickupLists.finalEquipmentDefs == null || PickupLists.finalEquipmentDefs.Length <= 0)
                {
#if DEBUG
                    Debug.LogError("Cannot generate equips as finalEquipmentDefs is empty or null.");
#endif
                    return;
                }
                EquipmentDef equipmentDef = EvaluateEquipment();
                if (equipmentDef != null)
                {
                    if (equipmentDef.equipmentIndex != EquipmentIndex.None && !Run.instance.IsEquipmentExpansionLocked(equipmentDef.equipmentIndex))
                    {
                        inventory.ResetItem(RoR2Content.Items.AutoCastEquipment);
                        inventory.GiveItem(RoR2Content.Items.AutoCastEquipment);
                        inventory.SetEquipmentIndex(equipmentDef.equipmentIndex);
                    }
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

        public static void ResetAllItemsInInventory(Inventory inventory)
        {
            foreach (ItemIndex item in inventory.itemAcquisitionOrder)
            {
                inventory.ResetItem(item);
            }
        }

        public static void CleanInventoryDependingOnConfigAndCopyFromMaster(Inventory inventory, CharacterMaster master)
        {
            int doppelCount = inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger);
            int gummyCount = inventory.GetItemCount(DLC1Content.Items.GummyCloneIdentifier);
            bool alreadyInheriting = gummyCount > 0 || doppelCount > 0;

            if (master == null || master.inventory == null)
            {
#if DEBUG
                Debug.LogError("Cannot copy inventory from master as master is " + master + " and its inventory is " + master.inventory);
#endif
                return;
            }

            if (!alreadyInheriting)
            {
                foreach (ItemIndex item in inventory.itemAcquisitionOrder)
                {
                    foreach (ItemTierDef itemTier in AvailableItemTierDefs)
                    {
                        if (ItemCatalog.GetItemDef(item).tier == itemTier.tier)
                        {
                            inventory.ResetItem(item);
                        }
                    }
                }

                inventory.CopyItemsFrom(master.inventory);
            }

            BlacklistAndMultiplyAndLimitInventory(inventory, alreadyInheriting && UmbraeBlacklistLimitOperation.Value);

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

            if (alreadyInheriting) //In case they lose the items at some point
            {
                if (inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) != doppelCount)
                {
                    inventory.ResetItem(RoR2Content.Items.InvadingDoppelganger);
                    inventory.GiveItem(RoR2Content.Items.InvadingDoppelganger, doppelCount);
                }
                if (inventory.GetItemCount(DLC1Content.Items.GummyCloneIdentifier) != gummyCount)
                {
                    inventory.ResetItem(DLC1Content.Items.GummyCloneIdentifier);
                    inventory.GiveItem(DLC1Content.Items.GummyCloneIdentifier, gummyCount);
                }
            }
        }

        public static void BlacklistAndMultiplyAndLimitInventory(Inventory inventory, bool alreadyInheriting = false)
        {
            float itemMultiplier = ConfigToFloat(ItemMultiplier.Value);
            ItemDef[] itemDefsScheduledForDeletion = new ItemDef[0];
            foreach (ItemIndex item in inventory.itemAcquisitionOrder)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(item);
                if (ItemBlackList.Contains(itemDef))
                {
                    if (InheranceBlacklist.Value)
                    {
                        HG.ArrayUtils.ArrayAppend<ItemDef>(ref itemDefsScheduledForDeletion, itemDef);
                    }
                    continue;
                }
                foreach (var itemTierDef in AvailableItemTierDefs)
                {
                    if (itemDef.tier == itemTierDef.tier)
                    {
                        int count = inventory.GetItemCount(itemDef);
                        count = itemMultiplier != 1 && !alreadyInheriting ? count : (int)Math.Ceiling(count * itemMultiplier);
                        //inventory.ResetItem(itemDef); We cannot reset item, that would modify the itemAcquisitionOrder collection.
                        inventory.RemoveItem(itemDef, inventory.GetItemCount(itemDef) - 1); //Leave it at one stack
                        if (LimitedItemsDictionary.ContainsKey(itemDef) && Limiter.Value)
                        {
                            count = ((from kvp in LimitedItemsDictionary where kvp.Key == itemDef select kvp.Value)).FirstOrDefault();
                            if (count <= 0)
                                count = Run.instance.stageClearCount + 1;
#if DEBUG
                            Debug.LogWarning("Inherance: The limiter is on, and we found a item that is meant to be limited: " + itemDef + " amountToGive: " + count + " current stage: " + Run.instance.stageClearCount + 1);
#endif
                        }
                        inventory.GiveItem(itemDef, count - 1);
                    }
                }
            }
            foreach (ItemDef delet in itemDefsScheduledForDeletion)
            {
                inventory.ResetItem(delet);
#if DEBUG
                Debug.LogWarning("Inherance: deleted " + delet + " as it was in the blacklist.");
#endif
            }
        }

        //Doppelganger fuckery because it seems that they do not get their targets assigned until one frame later
        //Personally, this is very dirty, the whole deal of resetting the whole inventory and just redo what the game does.
        private static IEnumerator GetTargetOfDoppelganger(Inventory inventoryToCopyTo)
        {
            yield return new WaitForEndOfFrame();

            //Get the original master, wooo null checking galore
            BaseAI baseai = inventoryToCopyTo.gameObject.GetComponent<BaseAI>();
            if (baseai == null || baseai.currentEnemy.characterBody == null || baseai.currentEnemy.characterBody.master == null)
            {
#if DEBUG
                Debug.LogError("Setting up inventory for a doppelganger failed: BaseAI is " + baseai + " CharacterBody of the current enemy is " + baseai.currentEnemy.characterBody + " and master of that body is " + baseai.currentEnemy.characterBody.master);
#endif
                yield return null;
            }

            if (baseai.currentEnemy.characterBody.master.playerCharacterMasterController)
            {
                ResetAllItemsInInventory(inventoryToCopyTo);
                inventoryToCopyTo.CopyItemsFrom(baseai.currentEnemy.characterBody.inventory);
                if (UmbraeBlacklistLimitOperation.Value)
                {
                    BlacklistAndMultiplyAndLimitInventory(inventoryToCopyTo);
                }
#if DEBUG
                Debug.Log("Successfully set umbra inventory with " + baseai.currentEnemy.characterBody.master.playerCharacterMasterController.GetDisplayName() + " as base. UmbraeBlacklistMultiplyLimitOperation: " + UmbraeBlacklistLimitOperation.Value);
#endif
                yield return null;
            }
#if DEBUG
            Debug.Log("Setting up inventory for a doppelganger failed: turns out that the master isn't a player, aborting");
#endif
        }

        private const int maxFailedAttempts = 5;

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
            bobTheBuilder.Append("Max Items To Generate: " + maxItemsToGenerate);
            Debug.Log(bobTheBuilder.ToString());
        }
    }
}