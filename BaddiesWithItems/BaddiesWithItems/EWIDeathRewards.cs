using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace BaddiesWithItems
{
    internal class EWIDeathRewards : MonoBehaviour, IOnKilledServerReceiver
    {
        public void OnKilledServer(DamageReport damageReport)
        {
            if (Util.CheckRoll(EnemiesWithItems.ConfigToFloat(EnemiesWithItems.DropChance.Value), 0f, null) && damageReport.victimBody.master.teamIndex == TeamIndex.Monster)
            {
                Inventory inventory = damageReport.victimBody.master.inventory;
                List<PickupIndex> tier1Inventory = new List<PickupIndex>();
                List<PickupIndex> tier2Inventory = new List<PickupIndex>();
                List<PickupIndex> tier3Inventory = new List<PickupIndex>();
                List<PickupIndex> lunarTierInventory = new List<PickupIndex>();
                foreach (ItemIndex item in ItemCatalog.allItems)
                {
                    if (inventory.GetItemCount(item) <= 0)
                        continue;
                    if (EnemiesWithItems.Tier1Items.Value && ItemCatalog.tier1ItemList.Contains(item))
                    {
                        tier1Inventory.Add(PickupCatalog.FindPickupIndex(item));
                    }
                    else if (EnemiesWithItems.Tier2Items.Value && ItemCatalog.tier2ItemList.Contains(item))
                    {
                        tier2Inventory.Add(PickupCatalog.FindPickupIndex(item));
                    }
                    else if (EnemiesWithItems.Tier3Items.Value && ItemCatalog.tier3ItemList.Contains(item))
                    {
                        tier3Inventory.Add(PickupCatalog.FindPickupIndex(item));
                    }
                    else if (EnemiesWithItems.LunarItems.Value && ItemCatalog.lunarItemList.Contains(item))
                    {
                        lunarTierInventory.Add(PickupCatalog.FindPickupIndex(item));
                    }
                }
                WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
                if (EnemiesWithItems.Tier1Items.Value)
                {
                    weightedSelection.AddChoice(tier1Inventory, 0.9f);
                }
                if (EnemiesWithItems.Tier2Items.Value)
                {
                    weightedSelection.AddChoice(tier2Inventory, 0.1f);
                }
                if (EnemiesWithItems.Tier3Items.Value)
                {
                    weightedSelection.AddChoice(tier3Inventory, 0.05f);
                }
                if (EnemiesWithItems.LunarItems.Value)
                {
                    weightedSelection.AddChoice(lunarTierInventory, 0.01f);
                }
                List<PickupIndex> list = weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
                if (list.Count == 0)
                {
                    return;
                }
                PickupDropletController.CreatePickupDroplet(list[Run.instance.treasureRng.RangeInt(0, list.Count)], damageReport.victimBody.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + damageReport.victimBody.transform.forward * 2f);
            }
        }
    }
}