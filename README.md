# Enemies With Items
## By Basil
A mod allows enemies to be spawned with items for a more difficult experience!

**Now scales exponentially depending on the total number of items from players!**

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
**I highly recommend deleting the config file if you previously installed this mod so it can be updated with the most recent config layout.**


### Default Config Settings

[General Settings]

* Sets the multiplier for items to be inherited/generated.
ItemMultiplier = 1

* Sets the minimum stage to be cleared before having enemies inherit/generate items.
StageReq = 4

* Toggles hard blacklisted items to be inherited/generated.
BlacklistItems = false

* Toggles certain items to be capped.
Limiter = true

* Toggles Tier 1 (white) items to be inherited/generated.
Tier1Items = true

* Toggles Tier 2 (green) items to be inherited/generated.
Tier2Items = true

* Toggles Tier 3 (red) items to be inherited/generated.
Tier3Items = true

* Toggles Lunar (blue) items to be inherited/generated.
LunarItems = true

* Toggles Use items to be inherited/generated.
EquipItems = false

* Toggles hard blacklisted Use items to be inherited/generated. MOST BLACKLISTED USE ITEMS ARE UNDODGEABLE.
EquipBlacklist = false

* Enter items ids separated by a comma and a space to blacklist certain items. ex) 41, 23, 17
[Item IDs](https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names)
CustomBlacklist = 

* Enter equipment ids separated by a comma and a space to blacklist certain equipments. ex) 1, 14, 13 
CustomEquipBlacklist = 

[Generator Settings]

* Toggles item generation for enemies.
GenerateItems = true

* The multiplicative max item cap for generating Tier 1 (white) items.
Tier1GenCap = 4

* The multiplicative max item cap for generating Tier 2 (green) items.
Tier2GenCap = 2

* The multiplicative max item cap for generating Tier 3 (red) items.
Tier3GenCap = 1

* The multiplicative max item cap for generating Lunar (blue) items.
LunarGenCap = 1

* The percent chance for generating a Tier 1 (white) item.
Tier1GenChance = 40

* The percent chance for generating a Tier 2 (green) item.
Tier2GenChance = 20

* The percent chance for generating a Tier 3 (red) item.
Tier3GenChance = 1

* The percent chance for generating a Lunar (blue) item.
LunarGenChance = 0.5

* The percent chance for generating a Use item.
EquipGenChance = 10

[Inherit Settings]

* Toggles enemies to randomly inherit items from a random player. Overrides Generator Settings.
InheritItems = false

## Hard Blacklisted Items & Equips

Bustling Fungus
N'kuhana's Opinion
Sticky Bomb
Stun Grenade
Unstable Tesla Coil

Disposable Missile Launcher
Eccentric Vase
Helfire Tincture
Preon Accumulator
Primordial Cube
Radar Scanner
Royal Capacitor
The Back-up

## FAQ

Q: How does the item drop work for enemies?

A: It will randomly choose an item from the pool of items that the enemy has when it dies. White items are the most common to drop and so on.

Q: How does InheritItems work?

A: It will randomly choose a player to inherit items from.

Q: How do you calculate the item gen cap for item generation?

A: Currently the way it is done is randomly selecting a value from 0 to the current stage * item gen cap, inclusively, for every item.

Q: What items are being limited?

A: The following items are limited and subject to change based on user complaints:

Brilliant Behemoth - Capped at 2 - 5.5m Radius
Cautious Slug - Capped at 30 - 4600% Passive Health Regen
Chronobauble - Capped at 1 - 60% slow for 1 second
Fuel Cell - Capped at 3 - ~30% Reduced Equipment CD (guesstimate)
Gasoline - Capped at 2 - 16m Range & Burn for 225% Monster Base Damage
Gesture of the Drowned - Capped at 1 - 50% Reduced Equipment CD
Tougher Times - Capped at 7 - ~51% Dodge Chance
Tri-tip Dagger - Capped at 3 - 45% Bleed Chance

## Changelog

**v1.2.3**

- Added custom equipment blacklist setting

**v1.2.2**

- Updated R2API dependency string

**v1.2.1**

- Hard blacklisted The Back-up & Eccentric Vase

**v1.2.0**

- Updated mod for the Scorched Acres patch!
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