# Enemies With Items
## By Basil
A mod allows enemies to be spawned with items for a more difficult experience!
This mod differs from the Artifact of Evolution (give all monsters the same items every stage artifact) as it scales harder and it makes every enemy have different items.
Can be played with the Artifact of Evolution for guaranteed items on every monster.

**Scales exponentially depending on the total number of items from players!**

**Drop option added! Check config!**

Want the enemies to inherit player items? Possible with the InheritItems setting!
*Will ignore generator settings.*

By default, enemies will only start spawning with items after the 4th stage.

Please contact Basil#7379 on Discord for any issues or interests!

## Installation
1. Install [BepInEx Mod Pack](https://thunderstore.io/package/bbepis/BepInExPack/)
2. Install [R2API](https://thunderstore.io/package/tristanmcpherson/R2API/)
3. Download the latest EnemiesWithItems.dll [here](https://thunderstore.io/package/BasilPanda/EnemiesWithItems/).
4. Move EnemiesWithItems.dll to your \BepInEx\plugins folder

## Configuration

1. To find the config file, first start up the game with EnemiesWithItems.dll in your \BepInEx\plugins folder already!
2. Then go to \BepInEx\config and open com.Basil.EnemiesWithItems.cfg

**Please check the config file and edit accordingly for your personal experience!**

## Hard Blacklisted Items & Equips

Bustling Fungus
Genesis Loop
N'kuhana's Opinion
Resonance Disc
Sticky Bomb
Stun Grenade
Unstable Tesla Coil

Disposable Missile Launcher
Eccentric Vase
Helfire Tincture
Preon Accumulator
Primordial Cube
Radar Scanner
Recycler
Royal Capacitor
Sawmerang
The Back-up
Volcanic Egg

## FAQ

Q: I'm having heavy frame drops/lagging with ItemMulitplier set to 5 or greater! How do I fix this?

A: You can't really fix this issue. With a higher multiplier, it makes the game more demanding in computer resources. You can only alleviate it by getting better computer parts. 

Q: How does the item drop work for enemies?

A: It will randomly choose an item from the pool of items that the enemy has when it dies. White items are the most common to drop and so on.

Q: How does InheritItems work?

A: It will randomly choose a player to inherit items from.

Q: How do you calculate the item gen cap for item generation?

A: Currently the way it is done is randomly selecting a value from 0 to the current stage * item gen cap, inclusively, for every item.

Q: I want to see what the enemies have. How can I?

A: I do not have a solution at the moment since every enemy will have their own unique inventory.

Q: What items are being limited by default?

A: Feel free to message me about item imbalance! 
The following items blow are limited and subject to change based on user complaints:

These are capped at current stage number:
Death Mark
Irradiant Pearl
Pearl
Repulsion Armor Plate

Brilliant Behemoth - Capped at 2 - 5.5m Radius
Cautious Slug - Capped at 30 - 4600% Passive Health Regen
Chronobauble - Capped at 1 - 60% slow for 1 second
Focus Crystal - Capped at 3 - 45% Increased Damage
Fuel Cell - Capped at 3 - ~30% Reduced Equipment CD (guesstimate)
Gasoline - Capped at 2 - 16m Range & Burn for 225% Monster Base Damage
Gesture of the Drowned - Capped at 1 - 50% Reduced Equipment CD
Razor Wire - Capped at 1 - 25m Radius (this will suck for melee classes)
Tougher Times - Capped at 7 - ~51% Dodge Chance
Tri-tip Dagger - Capped at 3 - 45% Bleed Chance

## Changelog

**v1.2.11**

- Updated for v1.0 of the game!
- Updated for 2.5.7 R2API.

**v1.2.10**

- Fixed health and damage not scaling with players. Thanks to bang#4745 for reporting it!

**v1.2.9**

- Updated for 2.4.10 R2API.

**v1.2.8**

- Updated mod for the Artifacts update!
- Limited Death Mark to current stage number.
- Blacklisted some more stuff, check above.

**v1.2.7**

- Updated mod for Hidden Realms update!
- Added Custom Item Caps config!
- Limited some of the new items, check above.
- Blacklisted Volanic Egg

**v1.2.6**

- Updated mod for Skills 2.0 update!

**v1.2.5**

- Updated to most recent R2API
- Changed main hook to IL
- Now compatible with Jarlyk mods! Check them out [here](https://thunderstore.io/package/Jarlyk/)! 

**v1.2.4**

- Fixed equipment generation by including lunar equips
- Changed how Gesture of the Drowned is handled in generation (runs should be harder as it will generate other lunar items more often)

**v1.2.3**

- Added custom equipment blacklist setting

**v1.2.2**

- Updated R2API dependency string

**v1.2.1**

- Hard blacklisted The Back-up & Eccentric Vase

**v1.2.0**

- Updated mod for the Scorched Acres update!
- Added random enemy item drop setting (default to false)
- Added item drop chance setting (default to 0.1%)
- Enemy item generation scaling now increases exponentially instead of linearly after StageReq
- Enemy item generation chance per tier now increases linearly per stage after StageReq
- Limited Gesture of the Drowned to at most 1 stack

**v1.1.1**

- Fixed scaling bug
- Limited Gasoline to at most 2 stacks

**v1.1.0**

- Added custom item blacklist setting
- Limited Tri-tip Dagger to at most 3 stacks
- Enemy item generation now scales by stages cleared and the average amount of player items

**v1.0.1**

- Tidied up the README

**v1.0.0**

- Enemies now spawn with or inherit items (default after 4th stage)

## My Other Mods

[DronesInheritItems](https://thunderstore.io/package/BasilPanda/DronesInheritItems/)
[RemoveAllyCap](https://thunderstore.io/package/BasilPanda/RemoveAllyCap/)