using RoR2;
using System.Collections.Generic;
using UnityEngine;
using BaddiesWithItems;
using System.Linq;

namespace BaddiesWithItems
{
    internal class EWIDeathRewards : MonoBehaviour, IOnKilledServerReceiver
    {
        public void OnKilledServer(DamageReport damageReport)
        {
            if (!TeamManager.IsTeamEnemy(damageReport.attackerBody.master.teamIndex, damageReport.victimBody.master.teamIndex))
                return;
            Inventory inventory = damageReport.victimBody.master.inventory;

            float itemChance = 1f;
            WeightedSelection<ItemIndex> weightedSelection = new WeightedSelection<ItemIndex>(8);
            for (int i = 0; i < EnemiesWithItems.AvailableItemTierDefs.Length; i++)
            {
                itemChance = EnemiesWithItems.ItemTierWeights[i] * 5;
                ItemIndex[] itemIndices = inventory.itemAcquisitionOrder.Where(x => ItemCatalog.GetItemDef(x).tier == EnemiesWithItems.AvailableItemTierDefs[i].tier).ToArray();
                if (itemIndices.Length <= 0)
                    continue;
                weightedSelection.AddChoice(Run.instance.treasureRng.NextElementUniform<ItemIndex>(itemIndices), itemChance);
            }
            if (weightedSelection.Count <= 0)
                return; //Theres nothing to evaluate!
            ItemIndex chosenItemIndex = weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat); //will never return something bad because it cannot evaluate zeroes.
            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(chosenItemIndex), damageReport.victimBody.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + damageReport.victimBody.transform.forward * 2f);
        }
    }
}
