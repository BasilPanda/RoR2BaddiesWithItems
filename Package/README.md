# Enemies With Items
## By Basil & Anreol
Original package can be found [Here](https://thunderstore.io/package/BasilPanda/EnemiesWithItems/)
A mod allows enemies to be spawned with items for a more difficult experience!
This mod differs from the Artifact of Evolution (give all monsters the same items every stage artifact) as it scales harder and it makes every enemy have different items.
Can be played with the Artifact of Evolution for guaranteed items on every monster.

Want the enemies to inherit player items? Possible with the InheritItems setting!

By default, enemies will only start spawning with items **after the 5th stage (first loop).**

Please contact Anreol#8231 on Discord for any issues or interests!

## Installation
1. Install [BepInEx Mod Pack](https://thunderstore.io/package/bbepis/BepInExPack/)
2. Download the latest EnemiesWithItems.dll [here](https://thunderstore.io/package/Anreol/EnemiesWithItems/).
3. Move EnemiesWithItems.dll to your \BepInEx\plugins folder

## Configuration

1. To find the config file, first start up the game with EnemiesWithItems.dll in your \BepInEx\plugins folder already!
2. Then go to \BepInEx\config and open com.Anreol.EnemiesWithItems.cfg

**Please check the config file and edit accordingly for your personal experience!**

## FAQ

- Q: I'm having heavy frame drops/lagging with ItemMulitplier set to 5 or greater! How do I fix this?
	- A: You can't really fix this issue. Enemies with more items are more demanding in computer resources. You can only alleviate it by getting better computer parts. 

- Q: How does the item drop work for enemies?
	- A: As of 3.0.0, the item drop chance is actually the chance of the enemy being able to drop anything. On death it calculates the chance of dropping an item of its inventory, and that is the item's tier's generation chance multiplied by 5.

- Q: How does InheritItems work?
	- A: It will randomly choose a player to inherit items from. Copied items will not be affected by any of the mod's blacklists, except for the equipment blacklist.

- Q: How do you calculate the item gen cap for item generation?
	- A: As of 3.0.0, if not inheriting from a player, it goes as follows:
		- Selects a random item that isn't blacklisted, is in an available item tier, based off the item tier's generation chance.
		- If the monster has too many items of the same tier, generate a different item.
		- Amount is a random number from 0 to the current stage plus the player's total item count or the player's average item count, depending on the config.
		- Amount is multiplied by ItemMulitplier specified in the config.
		- Amount is limited by the Limiter if its enabled.
			- Amount is set to number of stages cleared if the limiter list had the item's entry with zero.

- Q: I want to see what the enemies have. How can I?
	- A: While it is possible, having a good solution for this (so not using chat or console logs) is rather difficult to come up.

## Changelog
**v3.0.7**
- Player item inherance is now a float instead of a true / false activation.
- If inherance blacklist is enabled, it will now check twice for violating items.
	- This should have no performance hit / barely any performance hit, as its replacing one of the game's filter functions with a custom one.

- Fixed a issue regarding the usage of a base game's method which led to unintentional behavior whenever inherance was enabled.

**v3.0.6**
- Changed the mod's GUID to my username as I'm (Anreol) the one maintaining the mod right now
	- This means config file might get regenerated, just copy and paste the old stuff into the new file

- Added an additional null check to something that should never be null because when I was playing by myself it somehow threw off a few errors?
- Removed every entry in this changelog previous to the available version in this package post

**v3.0.5**
- Recompiled for 1.2.3.1
	- Thats literally everything, other than that enemies will now always generate at least 1 item each roll so generation rolls don't get wasted.
- And added some other failsafe.


## Other Basil Mods
These mods are other mods that Basil made, they might be kinda old, and not work anymore!
[DronesInheritItems](https://thunderstore.io/package/BasilPanda/DronesInheritItems/)
[RemoveAllyCap](https://thunderstore.io/package/BasilPanda/RemoveAllyCap/)
