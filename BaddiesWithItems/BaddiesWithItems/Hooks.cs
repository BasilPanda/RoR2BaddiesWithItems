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
                if (spawnTarget)
                {
                    if (self.GetFieldValue<DirectorCard>("currentMonsterCard") == null)
                    {
                        self.SetFieldValue("currentMonsterCard", self.GetPropertyValue<WeightedSelection<DirectorCard>>("monsterCards").Evaluate(self.GetFieldValue<Xoroshiro128Plus>("rng").nextNormalizedFloat));
                        self.lastAttemptedMonsterCard = self.GetFieldValue<DirectorCard>("currentMonsterCard");
                        self.SetFieldValue("currentActiveEliteIndex", self.GetFieldValue<Xoroshiro128Plus>("rng").NextElementUniform<EliteIndex>(EliteCatalog.eliteList));
                    }
                    int num = 0;
                    for (int i = 0; i < self.GetPropertyValue<WeightedSelection<DirectorCard>>("monsterCards").Count; i++)
                    {
                        DirectorCard value = self.GetPropertyValue<WeightedSelection<DirectorCard>>("monsterCards").GetChoice(i).value;
                        int num2 = value.cost;
                        if (!(value.spawnCard as CharacterSpawnCard).noElites)
                        {
                            num2 = (int)((float)num2 * CombatDirector.eliteMultiplierCost);
                        }
                        num = Mathf.Max(num, num2);
                    }
                    int num3 = self.GetFieldValue<DirectorCard>("currentMonsterCard").cost;
                    bool flag = !(self.GetFieldValue<DirectorCard>("currentMonsterCard").spawnCard as CharacterSpawnCard).noElites && self.monsterCredit >= (float)num3 * CombatDirector.eliteMultiplierCost;
                    if (flag)
                    {
                        num3 = (int)((float)num3 * CombatDirector.eliteMultiplierCost);
                    }
                    bool flag2 = self.GetFieldValue<DirectorCard>("currentMonsterCard").CardIsValid();
                    bool flag3 = self.monsterCredit >= (float)num3;
                    bool flag4 = !self.skipSpawnIfTooCheap || self.monsterCredit <= (float)num3 * CombatDirector.maximumNumberToSpawnBeforeSkipping;
                    bool flag5 = num3 >= num;
                    if (flag2 && flag3 && (flag4 || flag5))
                    {
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
                        GameObject gameObject = DirectorCore.instance.TrySpawnObject(spawnCard, directorPlacementRule, self.GetFieldValue<Xoroshiro128Plus>("rng"));

                        if (gameObject)
                        {
                            float num4 = 1f;
                            float num5 = 1f;
                            CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
                            GameObject bodyObject = component.GetBodyObject();
                            if (self.isBoss)
                            {
                                if (!self.bossGroup)
                                {
                                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BossGroup"));
                                    NetworkServer.Spawn(gameObject2);
                                    self.SetPropertyValue("bossGroup", gameObject2.GetComponent<BossGroup>());
                                    self.bossGroup.dropPosition = self.dropPosition;
                                }
                                self.bossGroup.AddMember(component);
                            }
                            if (flag)
                            {
                                num4 = 4.7f;
                                num5 = 2f;
                                component.inventory.SetEquipmentIndex(EliteCatalog.GetEliteDef(self.GetFieldValue<EliteIndex>("currentActiveEliteIndex")).eliteEquipmentIndex);
                            }
                            self.monsterCredit -= (float)num3;
                            if (self.isBoss)
                            {
                                int livingPlayerCount = Run.instance.livingPlayerCount;
                                num4 *= Mathf.Pow((float)livingPlayerCount, 1f);
                            }

                            // this is it
                            if (Run.instance.stageClearCount >= EnemiesWithItems.StageReq.Value)
                            {
                                CharacterMaster player = PlayerCharacterMasterController.instances[rand.Next(0, Run.instance.livingPlayerCount)].master;
                                component.inventory.CopyItemsFrom(player.inventory);
                                EnemiesWithItems.checkConfig(component.inventory, player);
                            }

                            component.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num4 - 1f) * 10f));
                            component.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num5 - 1f) * 10f));
                            DeathRewards component2 = bodyObject.GetComponent<DeathRewards>();
                            if (component2)
                            {
                                component2.expReward = (uint)((float)num3 * self.expRewardCoefficient * Run.instance.compensatedDifficultyCoefficient);
                                component2.goldReward = (uint)((float)num3 * self.expRewardCoefficient * 2f * Run.instance.compensatedDifficultyCoefficient);
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
                        }
                    }
                }
                return false;
            };

        }

        // Enemies drop hook
        public static void enemiesDrop()
        {
            On.RoR2.DeathRewards.OnKilled += (orig, self, damageInfo) =>
            {
                orig(self, damageInfo);
                if (EnemiesWithItems.DropItems.Value && Util.CheckRoll(EnemiesWithItems.ConfigToFloat(EnemiesWithItems.DropChance.Value), 0f, null))
                {
                    CharacterBody enemy = self.GetFieldValue<CharacterBody>("characterbody");
                    Inventory inventory = enemy.master.inventory;
                    List<PickupIndex> tier1Inventory = new List<PickupIndex>();
                    List<PickupIndex> tier2Inventory = new List<PickupIndex>();
                    List<PickupIndex> tier3Inventory = new List<PickupIndex>();
                    List<PickupIndex> lunarTierInventory = new List<PickupIndex>();

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
                    PickupDropletController.CreatePickupDroplet(list[Run.instance.treasureRng.RangeInt(0, list.Count)], self.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + self.transform.forward * 2f);
                }
            };

        }
    }
}
