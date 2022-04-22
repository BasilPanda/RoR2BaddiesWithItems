# Enemies With Items
## By Basil & Anreol
A mod allows enemies to be spawned with items for a more difficult experience!
This mod differs from the Artifact of Evolution (give all monsters the same items every stage artifact) as it scales harder and it makes every enemy have different items.
Can be played with the Artifact of Evolution for guaranteed items on every monster.

**Survivors of the Void update!**

**3.0.0 has a new config file, make sure to delete the old one.**
**Full control over item tiers! Check config!**

Want the enemies to inherit player items? Possible with the InheritItems setting!

By default, enemies will only start spawning with items **after the 5th stage (first loop).**

Please contact Anreol#8231 on Discord for any issues or interests!

Basil#7379 will *probably* be back when the the first DLC, Survivors of the Void, comes out.

## Installation
1. Install [BepInEx Mod Pack](https://thunderstore.io/package/bbepis/BepInExPack/)
2. Download the latest EnemiesWithItems.dll [here](https://thunderstore.io/package/BasilPanda/EnemiesWithItems/).
3. Move EnemiesWithItems.dll to your \BepInEx\plugins folder

## Configuration

1. To find the config file, first start up the game with EnemiesWithItems.dll in your \BepInEx\plugins folder already!
2. Then go to \BepInEx\config and open com.Basil.EnemiesWithItems.cfg

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
**v3.0.5**
- Recompiled for 1.2.3.1
	- Thats literally everything, other than that enemies will now always generate at least 1 item each roll so generation rolls don't get wasted.
	- And added some other failsafe.
	
**v3.0.4**
- Config features
	- Added a config for inheriting enemies to get items that exist in the blacklist get removed.
	- Added a config for umbras or gummy clones to get their items blacklisted and limited.
	- Added a config for umbras or gummy clones to get their WHOLE inventory multiplied.
	- Added a config to multiply the maximum amount of items a enemy can generate in a whole gen cycle.
	- Added a config so items that exceed the maximum amount of items in a whole gen cycle in a single gen cycle thanks to the item multiplier get capped properly.
	
- Fixes
	- Fixed inherance not working due to how items were being removed.

**v3.0.3**
- Updated for Survivors of the Void
	- Item availability now gets recalculated automatically on every run start.
	- Items are now filtered depending on enabled expansions.
	- Enemies will no longer generate items if there's no living players or if the game is already over. (Yeah there was a out of range issue because of this)

**v3.0.2**

- Added a ton of debug logs (Only in a debug build)
- Total item count now only counts visible items
- ewi_midRunData now reports the correct number of max items to generate
- Fixed cached item count carrying over to the next run
- **Important note:** If you are using this mod alongside [AI Blacklist](https://thunderstore.io/package/Moffein/AI_Blacklist/) and [BossEquipmentFix](https://thunderstore.io/package/Groove_Salad/BossEquipmentFix/) and your game keeps freezing after a while, disable BossEquipmentFix.
	- It is currently unknown why this incompatibility happens

**v3.0.1**

- Nullchecked some things that should've been nullchecked but weren't.
- Fixed an out of range issue.

**v3.0.0**

- Refactored the entire mod (Made by Anreol#8231, if you have any issues, contact him!)
	- Performance should've been increased as it is no longer querying some values over and over again.
	- Blacklists now get applied to a different list at bootup, so it will no longer waste item rolls on something that it will later remove.
		- If you accidently blacklist something like BoostHP or Ghost, it should not be removed from elite enemies. This applies to any other item added by things other than this mod.
		- Item generation now has a cap on failed generated attempts as a failsafe to don't freeze the game in case the user has a very restrictive config.
		- Enemies should now roll the items that the user wants, as rolls aren't wasted on things that get later removed.
		- No more Heretics!
		
	- Hardcoded lists are gone.
		- They are now the default values of the blacklists in the config file.
	
	- Config file has been redone, you'll have to delete your old one.
	- Config file no longer accepts item indexes or equipment indexes as a valid entry in blacklists.
	- Added a config for elite equipment to get banned from generating by default, no need for the user to add them themselves.
	- Added a config to automatically blacklist items like scraps and Halcyon Seed.
	- Added a config for AI Blacklisted items to get banned from generating by default.
		- Disable at your own conditions! The default item blacklist no longer includes AI blacklisted items.

	- Added configs for item tier generations, check them out!
		- Enemies should now be able to generate Boss Tier items.
	
	- Enemies longer multiply their Gestures if generating items at random, and they *should* always have one single Gesture at all times if they generated a equipment.
	- Enemies only generate equipment if they dont have one already.
	- If copying the equipment from a player, it will no longer get a fuel array in case the equipment was blacklisted.
	- Added a ton of debug commands
		- ewi_reloadconfig | Reloads the config file, along with everything else, server only.
		- ewi_dumpItemBlackList | Dumps the currently loaded item blacklist to console.
		- ewi_dumpEquipBlackList | Dumps the currently loaded equipment blacklist to console.
		- ewi_dumpLimiterBlackList | Dumps the currently loaded item limiter dictionary to console.
		- ewi_dumpAllItemTierData | Dumps all the currently loaded arrays related to ItemTiers to console.
		- ewi_midRunData | Shows data specific to the run to console. Only usable in a run.
		- ewi_dumpItemPool | Dumps the currently loaded item pool to console, which enemies will generate items from.
		- ewi_dumpEquipPool | Dumps the currently loaded equipment pool to console, which enemies will generate equipment from.
	
	- Dropped pointless dependency on R2API
	- Bunch other things that I probably forgot about
		
**v2.0.4**

- Changed alternative scaling option behavior to average item count instead of highest.

**v2.0.3**

- Fixed CustomBlacklist not working properly. Thanks to GG#2610 for reporting it!
- Added an alternative scaling option for people who play with bigger lobbies than 4.

**v2.0.2**

- Changed config text for blacklisting items to use codenames instead of ids.
- Removed hard blacklisting of Visions and Strides of Heresy. Thanks to Cinder#5504 for reporting it!
- Enemies in void field should now also have their intended items ontop of generated items. Thanks to NightLorde#6949 for reporting it!

**v2.0.1**

- Updated for R2API v3.0.30

**v2.0.0**

- Updated for the 1 Year Anniversary Update.
- Changed default StageReq to 6 (after first loop). Should not generate if player is in first loop and going straight to the moon.
- Code refactoring due to ItemIndex/EquipmentIndex changes.
- Enemies should now properly be able to generate items on the first stage if StageReq is set to 0 or 1.
- Custom blacklist configs now accept item code names!

**v1.2.12**

- Updated for R2API v2.5.14

**v1.2.11**

- Updated for v1.0 of the game!
- Updated for R2API v2.5.7

**v1.2.10**

- Fixed health and damage not scaling with players. Thanks to bang#4745 for reporting it!

**v1.2.9**

- Updated for R2API v2.4.10

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
