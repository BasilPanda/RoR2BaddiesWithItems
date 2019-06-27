using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace BaddiesWithItems
{
    public static class Hooks
    {
        private static System.Random rand = new System.Random();

        // Enemies w/ items hook
        public static void baddiesItems()
        {
            On.RoR2.CombatDirector.AttemptSpawnOnTarget += (orig, self, spawnTarget) =>
            {
                if (self.GetFieldValue<DirectorCard>("currentMonsterCard") == null)
                {
                    self.InvokeMethod<DirectorCard>("PrepareNewMonsterWave", self.GetPropertyValue<WeightedSelection<DirectorCard>>("monsterCards").Evaluate(self.GetFieldValue<Xoroshiro128Plus>("rng").nextNormalizedFloat));
                }
                if (!spawnTarget)
                {
                    return false;
                }
                if ((float)self.GetFieldValue<int>("spawnCountInCurrentWave") >= CombatDirector.maximumNumberToSpawnBeforeSkipping)
                {
                    return false;
                }
                int cost = self.GetFieldValue<DirectorCard>("currentMonsterCard").cost;
                int num = self.GetFieldValue<DirectorCard>("currentMonsterCard").cost;
                int num2 = self.GetFieldValue<DirectorCard>("currentMonsterCard").cost;
                CombatDirector.EliteTierDef eliteTierDef = self.GetFieldValue<CombatDirector.EliteTierDef>("currentActiveEliteTier");
                EliteIndex eliteIndex = self.GetFieldValue<EliteIndex>("currentActiveEliteIndex");
                num2 = (int)((float)num * self.GetFieldValue<CombatDirector.EliteTierDef>("currentActiveEliteTier").costMultiplier);
                if ((float)num2 <= self.monsterCredit)
                {
                    num = num2;
                    eliteTierDef = self.GetFieldValue<CombatDirector.EliteTierDef>("currentActiveEliteTier");
                    eliteIndex = self.GetFieldValue<EliteIndex>("currentActiveEliteIndex");
                }
                else
                {
                    eliteTierDef = new CombatDirector.EliteTierDef
                    {
                        costMultiplier = 1f,
                        damageBoostCoefficient = 1f,
                        healthBoostCoefficient = 1f,
                        eliteTypes = new EliteIndex[]
                        {
                            EliteIndex.None
                        }
                    };
                    eliteIndex = EliteIndex.None;
                }
                if (!self.GetFieldValue<DirectorCard>("currentMonsterCard").CardIsValid())
                {
                    return false;
                }
                if (self.monsterCredit < (float)num)
                {
                    return false;
                }
                if (self.skipSpawnIfTooCheap && (float)num2 * CombatDirector.maximumNumberToSpawnBeforeSkipping < self.monsterCredit)
                {
                    if (self.GetPropertyValue<int>("mostExpensiveMonsterCostInDeck") > num)
                    {
                        return false;
                    }
                }
                SpawnCard spawnCard = self.GetFieldValue<DirectorCard>("currentMonsterCard").spawnCard;
                DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    spawnOnTarget = spawnTarget.transform,
                    preventOverhead = self.GetFieldValue<DirectorCard>("currentMonsterCard").preventOverhead
                };
                DirectorCore.GetMonsterSpawnDistance(self.GetFieldValue<DirectorCard>("currentMonsterCard").spawnDistance, out directorPlacementRule.minDistance, out directorPlacementRule.maxDistance);
                directorPlacementRule.minDistance *= self.spawnDistanceMultiplier;
                directorPlacementRule.maxDistance *= self.spawnDistanceMultiplier;
                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, directorPlacementRule, self.GetFieldValue<Xoroshiro128Plus>("rng"));
                directorSpawnRequest.ignoreTeamMemberLimit = true;
                directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Monster);
                GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                if (!gameObject)
                {
                    Debug.LogFormat("Spawn card {0} failed to spawn. Aborting cost procedures.", new object[]
                    {
            spawnCard
                    });
                    return false;
                }
                self.monsterCredit -= (float)num;
                self.SetFieldValue("spawnCountInCurrentWave", self.GetFieldValue<int>("spawnCountInCurrentWave") + 1);
                CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
                GameObject bodyObject = component.GetBodyObject();
                if (!self.combatSquad && self.combatSquadPrefab)
                {
                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(self.combatSquadPrefab, self.dropPosition ? self.dropPosition.position : self.transform.position, Quaternion.identity);
                    self.SetPropertyValue("combatSquad", gameObject2.GetComponent<CombatSquad>());
                    NetworkServer.Spawn(gameObject2);
                    Action<CombatSquad> action = self.GetFieldValue<Action<CombatSquad>>("onCombatSquadAddedServer");
                    if (action != null)
                    {
                        action(self.combatSquad);
                    }
                }
                if (self.combatSquad)
                {
                    self.combatSquad.AddMember(component);
                }
                float num3 = eliteTierDef.healthBoostCoefficient;
                float damageBoostCoefficient = eliteTierDef.damageBoostCoefficient;
                EliteDef eliteDef = EliteCatalog.GetEliteDef(eliteIndex);
                EquipmentIndex equipmentIndex = (eliteDef != null) ? eliteDef.eliteEquipmentIndex : EquipmentIndex.None;
                if (equipmentIndex != EquipmentIndex.None)
                {
                    component.inventory.SetEquipmentIndex(equipmentIndex);
                }
                if (self.combatSquad)
                {
                    int livingPlayerCount = Run.instance.livingPlayerCount;
                    num3 *= Mathf.Pow((float)livingPlayerCount, 1f);
                }

                // this is it
                if (Run.instance.stageClearCount >= EnemiesWithItems.StageReq.Value)
                {
                    CharacterMaster player = PlayerCharacterMasterController.instances[rand.Next(0, Run.instance.livingPlayerCount)].master;
                    component.inventory.CopyItemsFrom(player.inventory);
                    EnemiesWithItems.checkConfig(component.inventory, player);
                }

                component.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num3 - 1f) * 10f));
                component.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((damageBoostCoefficient - 1f) * 10f));
                DeathRewards component2 = bodyObject.GetComponent<DeathRewards>();
                if (component2)
                {
                    component2.expReward = (uint)((float)num * self.expRewardCoefficient * Run.instance.compensatedDifficultyCoefficient);
                    component2.goldReward = (uint)((float)num * self.expRewardCoefficient * 2f * Run.instance.compensatedDifficultyCoefficient);
                }
                if (self.spawnEffectPrefab && NetworkServer.active)
                {
                    Vector3 origin = gameObject.transform.position;
                    CharacterBody component3 = bodyObject.GetComponent<CharacterBody>();
                    if (component3)
                    {
                        origin = component3.corePosition;
                    }
                    EffectManager.instance.SpawnEffect(self.spawnEffectPrefab, new EffectData
                    {
                        origin = origin
                    }, true);
                }
                return true;
            };

        }

        // Enemies drop hook -- needs fixing
        public static void enemiesDrop()
        {
            On.RoR2.DeathRewards.OnKilledServer += (orig, self, damageInfo) =>
            {
                orig(self, damageInfo);
                TeamIndex team = TeamComponent.GetObjectTeam(self.gameObject);
                if (EnemiesWithItems.DropItems.Value && Util.CheckRoll(EnemiesWithItems.ConfigToFloat(EnemiesWithItems.DropChance.Value), 0f, null) && team.Equals(TeamIndex.Monster))
                {
                    CharacterBody enemy = self.GetFieldValue<CharacterBody>("characterbody");
                    Chat.AddMessage("1");
                    Inventory inventory = enemy.master.inventory;
                    List<PickupIndex> tier1Inventory = new List<PickupIndex>();
                    List<PickupIndex> tier2Inventory = new List<PickupIndex>();
                    List<PickupIndex> tier3Inventory = new List<PickupIndex>();
                    List<PickupIndex> lunarTierInventory = new List<PickupIndex>();
                    Chat.AddMessage("2");
                    foreach (ItemIndex item in ItemCatalog.allItems)
                    {
                        if (EnemiesWithItems.Tier1Items.Value && ItemCatalog.tier1ItemList.Contains(item) && inventory.GetItemCount(item) > 0)
                        {
                            tier1Inventory.Add(new PickupIndex(item));
                        }
                        else if (EnemiesWithItems.Tier2Items.Value && ItemCatalog.tier2ItemList.Contains(item) && inventory.GetItemCount(item) > 0)
                        {
                            tier2Inventory.Add(new PickupIndex(item));
                        }
                        else if (EnemiesWithItems.Tier3Items.Value && ItemCatalog.tier3ItemList.Contains(item) && inventory.GetItemCount(item) > 0)
                        {
                            tier3Inventory.Add(new PickupIndex(item));
                        }
                        else if (EnemiesWithItems.LunarItems.Value && ItemCatalog.lunarItemList.Contains(item) && inventory.GetItemCount(item) > 0)
                        {
                            lunarTierInventory.Add(new PickupIndex(item));
                        }
                    }
                    Chat.AddMessage("3");
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
                    Chat.AddMessage("4");
                    List<PickupIndex> list = weightedSelection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
                    PickupDropletController.CreatePickupDroplet(list[Run.instance.treasureRng.RangeInt(0, list.Count)], self.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + self.transform.forward * 2f);
                }
            };

        }
    }
}
