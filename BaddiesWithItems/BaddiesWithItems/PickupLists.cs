using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaddiesWithItems
{
    internal class PickupLists
    {
        /// <summary>
        /// Has to be called AFTER config has been loaded.
        /// </summary>
        public static void BuildItemAndEquipmentDictionaries()
        {
            if (!ItemCatalog.availability.available || !EquipmentCatalog.availability.available)
            {
                Debug.LogWarning("Attempted to build Enemies With Items' lists before the required catalogs were available");
                return;
            }

            ItemDef[] tempItemDefList = new ItemDef[ItemCatalog.itemCount];
            (from item in ItemCatalog.allItems where !EnemiesWithItems.ItemBlackList.Contains(ItemCatalog.GetItemDef(item)) select ItemCatalog.GetItemDef(item)).ToArray().CopyTo(tempItemDefList, 0);
            EquipmentDef[] tempEquipmentDefs = new EquipmentDef[EquipmentCatalog.equipmentCount];
            (from item in EquipmentCatalog.allEquipment where !EnemiesWithItems.EquipmentBlackList.Contains(EquipmentCatalog.GetEquipmentDef(item)) select EquipmentCatalog.GetEquipmentDef(item)).ToArray().CopyTo(tempEquipmentDefs, 0);

            finalItemDefList = new ItemDef[tempItemDefList.Length];
            finalEquipmentDefs = new EquipmentDef[tempEquipmentDefs.Length];
            tempItemDefList.CopyTo(finalItemDefList, 0);
            tempEquipmentDefs.CopyTo(finalEquipmentDefs, 0);
        }

        public static ItemDef[] finalItemDefList;
        public static EquipmentDef[] finalEquipmentDefs; 

        [ConCommand(commandName = "ewi_dumpItemPool", flags = ConVarFlags.SenderMustBeServer, helpText = "Dumps the currently loaded item pool, which enemies will generate items from.")]
        private static void DumpItemPool(ConCommandArgs args)
        {
            if (finalItemDefList == null || finalItemDefList.Length <= 0)
            {
                Debug.Log("itemDefTierPair is null or empty, cannot continue.");
                return;
            }

            Debug.Log("Item Pool list has a count of " + finalItemDefList.Length);
            int realCounter = 0;
            StringBuilder brickByBrick = new StringBuilder();
            foreach (ItemDef itemDef in finalItemDefList)
            {
                if (itemDef != null)
                {
                    brickByBrick.Append((itemDef) + " | ");
                    realCounter++;
                }
            }
            Debug.Log(brickByBrick.ToString() + realCounter);
        }

        [ConCommand(commandName = "ewi_dumpEquipPool", flags = ConVarFlags.SenderMustBeServer, helpText = "Dumps the currently loaded equipment pool, which enemies will generate equipment from.")]
        private static void DumpEquipPool(ConCommandArgs args)
        {
            if (finalEquipmentDefs == null || finalEquipmentDefs.Length <= 0)
            {
                Debug.Log("equipmentDefs is null or empty, cannot continue.");
                return;
            }

            Debug.Log("Equipment Pool list has a length of " + finalEquipmentDefs.Length);
            int realCounter = 0;
            StringBuilder bobTheBuilder = new StringBuilder();
            foreach (EquipmentDef equipmentDef in finalEquipmentDefs)
            {
                if (equipmentDef != null)
                {
                    bobTheBuilder.Append(equipmentDef + " | ");
                    realCounter++;
                }
            }
            Debug.Log(bobTheBuilder.ToString() + realCounter);
        }
    }

}
