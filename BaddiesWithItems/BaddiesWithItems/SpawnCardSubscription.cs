using RoR2;
using System;
using UnityEngine;

namespace BaddiesWithItems
{
    public static class SpawnCardSubscription
    {
        // Enemies w/ items
        [SystemInitializer(new Type[]
        {   typeof(BodyCatalog)
        })]
        public static void BaddiesItems()
        {
            SpawnCard.onSpawnedServerGlobal += SpawnResultItemAdder;
            SpawnCard.onSpawnedServerGlobal += ItemDropperComponentAdder;
        }

        public static bool CanStartGivingItems(TeamIndex? teamIndexOfBodyToGive = null)
        {
            if (Run.instance.stageClearCount + 1 < EnemiesWithItems.StageReq.Value)
            {
#if DEBUG
                Debug.Log("Cannot give items yet because the current stage doesnt fulfill the stage requirement");
#endif
                return false;
            }
            
            if (EnemiesWithItems.StageReq.Value == 6 && SceneCatalog.mostRecentSceneDef.isFinalStage && Run.instance.loopClearCount == 0)
            {
#if DEBUG
                Debug.Log("Won't give enemies items because stage requirement is stage 6 and we are in the final stage.");
#endif
                return false;
            }
            
            if (teamIndexOfBodyToGive != null)
            {
#if DEBUG
                Debug.Log("teamIndexOfBodyToGive isn't null: " + teamIndexOfBodyToGive);
#endif
                if (!TeamManager.IsTeamEnemy(teamIndexOfBodyToGive.GetValueOrDefault(), TeamIndex.Player))
                {
#if DEBUG
                    Debug.Log(teamIndexOfBodyToGive + " wasn't an enemy to the players.");
#endif
                    return false;
                }
            }

            return true;
        }

        private static void ItemDropperComponentAdder(SpawnCard.SpawnResult obj)
        {
            if (!obj.success) //First check, chances are that if it wasnt successful theres not going to be any way to get the master
                return;
            CharacterMaster spawnResultMaster = obj.spawnedInstance ? obj.spawnedInstance.GetComponent<CharacterMaster>() : null;
            if (spawnResultMaster == null || !Util.CheckRoll(EnemiesWithItems.ConfigToFloat(EnemiesWithItems.DropChance.Value)))
                return;

            TeamIndex? teamIndexOverride = obj.spawnRequest.teamIndexOverride;
            if (!CanStartGivingItems(teamIndexOverride))
                return;

            UnityEngine.GameObject gameObject = spawnResultMaster.GetBodyObject();
            if (gameObject)
                gameObject.AddComponent<EWIDeathRewards>();
        }

        public static void SpawnResultItemAdder(SpawnCard.SpawnResult spawnResult)
        {
            CharacterMaster spawnResultMaster = spawnResult.spawnedInstance ? spawnResult.spawnedInstance.GetComponent<CharacterMaster>() : null;
            if (spawnResultMaster == null || !spawnResult.success || spawnResultMaster.inventory == null)
                return;

            TeamIndex? teamIndexOverride = spawnResult.spawnRequest.teamIndexOverride;
            if (!CanStartGivingItems(teamIndexOverride))
                return;
            //Xoroshiro throws off a range issue here at the beginning of the run, might have something to do with Run.instance.livingPlayerCount being zero in the very first frame.
            CharacterMaster playerToCopyFrom = PlayerCharacterMasterController.instances[RoR2.Run.instance.nextStageRng.RangeInt(0, Run.instance.livingPlayerCount)].master;
            ItemGeneration.GenerateItemsToInventory(spawnResultMaster.inventory, playerToCopyFrom);
        }
    }
}