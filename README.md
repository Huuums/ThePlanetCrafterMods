# ThePlanetCrafterMods
BepInEx+Harmony mods for the Unity/Steam game The Planet Crafter

Steam: https://store.steampowered.com/app/1284190/The_Planet_Crafter/

Guide on dnSpy-based manual patches: https://steamcommunity.com/sharedfiles/filedetails/?id=2784319459

## Version <a href='https://github.com/akarnokd/ThePlanetCrafterMods/releases'><img src='https://img.shields.io/github/v/release/akarnokd/ThePlanetCrafterMods' alt='Latest GitHub Release Version'/></a>

## Supported Game Version: 0.4.0.8

# Mods

## (Cheat) Asteroid Landing Position Override

Fixes the asteroid landing position relative to the player by an offset.
This includes asteroids from rockets and random meteor showers.

Note that currently, this may fail if the landing position is determined by the game as invalid. Be in the clear open!

### Configuration

`akarnokd.theplanetcraftermods.cheatasteroidlandingposition.cfg`

```
[General]

## Relative position east-west (east is positive).
# Setting type: Int32
# Default value: 100
DeltaX = 100

## Relative position north-south (north is positive).
# Setting type: Int32
# Default value: 0
DeltaZ = 0
```

## (Cheat) Auto Consume Oxygen-Water-Food

When the Oxygen, Thirst and Health meters reach a critical level, this mod will automatically
consume an Oxygen bottle, Water bottle or any food item from the player's inventory.

Marked as cheat because it is expected the player does these manually.

## (Cheat) Photomode Hide Water

Press <kbd>Shift+F2</kbd> to toggle photomode and hide water as well.

This is marked as cheat because allows picking up objects near the water edge
where they can't be picked up normally.

## (Cheat) Highlight Nearby Resources

Press <kbd>CTRL+F</kbd> to highlight nearby resources.

### Configuration

`akarnokd.theplanetcraftermods.cheatnearbyresourceshighlight.cfg`

```
[General]

## Specifies how far to look for resources.
# Setting type: Int32
# Default value: 30
Radius = 30

## Specifies how high the resource image to stretch.
# Setting type: Int32
# Default value: 1
StretchY = 1

## List of comma-separated resource ids to look for.
# Setting type: String
# Default value: Cobalt,Silicon,Iron,ice,Magnesium,Titanium,Aluminium,Uranim,Iridium,Alloy,Zeolite,Osmium,Sulfur
ResourceSet = Cobalt,Silicon,Iron,ice,Magnesium,Titanium,Aluminium,Uranim,Iridium,Alloy,Zeolite,Osmium,Sulfur
```

## (Cheat) Inventory Capacity Override

This is a very basic override of game inventories, existing and new alike. It tries to not break certain containers
with capacity 1 or 3. Note that the game is not really setup to handle large inventories that don't fit on the screen.
This mod makes no attempts at remedying this shortcoming.

### Configuration

`akarnokd.theplanetcraftermods.cheatinventorycapacity.cfg`

```
[General]

## The overridden default inventory capacity.
# Setting type: Int32
# Default value: 250
Capacity = 250
```

## (Cheat) Machines Deposit Into Remote Containers

For this mod to work, you have to rename your containers. For example,
to make machines deposit Iron, rename your container(s) to something that includes
`*Iron`. For Uranium, rename them to `*Uranim`. Note the `*` in front of the identifiers.
Identifiers can be any case. 

You can combine multiple resources by mentioning them together: `*Iron *Cobalt`.

Typical identifiers are: 
-`Cobalt`,`Silicon`,`Iron`,`ice`,
`Magnesium`,`Titanium`,`Aluminium`,`Uranim`,
`Iridium`,`Alloy`,`Zeolite`,`Osmium`,
`Sulfur`

You can have as many containers as you like, but they will be filled in non-deterministically.
If there are no renamed containers or all renamed containers are full, the machines
will deposit the resource into their own container, as would they without this mod.

Note also that machines are slow to mine resources.


## (Cheat) Teleport to Nearest Minable

- Press <kbd>F8</kbd> to teleport to the nearest minable resource. 
- Press <kbd>Shift+F8</kbd> to teleport to the nearest minable resource and mine it instantly. 
- Press <kbd>CTRL+F8</kbd> to mine the nearest resource without moving the character. 


Note that some
resources are out of bounds and are not normally player-reachable. You may also
fall to your death so be careful on permadeath!

### Configuration

`akarnokd.theplanetcraftermods.cheatteleportnearestminable.cfg`

```
[General]

## List of comma-separated resource ids to look for.
# Setting type: String
# Default value: Cobalt,Silicon,Iron,ice,Magnesium,Titanium,Aluminium,Uranim,Iridium,Alloy,Zeolite,Osmium,Sulfur
ResourceSet = Cobalt,Silicon,Iron,ice,Magnesium,Titanium,Aluminium,Uranim,Iridium,Alloy,Zeolite,Osmium,Sulfur
```


## (Perf) Load Inventories Faster

This speeds up loading the game when there are lots of containers or (modded) containers have a lot of items.

### Configuration

None.

## (Perf) Reduce Save Size

This mods the save process and removes attributes with default values from the `WorldObject`s, reducing
the save size. These attributes are automatically restored when the game loads.

The save remains compatible with the vanilla game so it will still work without this mod (but will be
full size again).

## (UI) Customize Inventory Sort Order

Specify the order of items when clicking on the sort all button in inventories.

### Configuration

`akarnokd.theplanetcraftermods.uicustominventorysortall.cfg`

```
[General]

## List of comma-separated resource ids to look for in order.
# Setting type: String
# Default value: OxygenCapsule1,WaterBottle1,astrofood
Preference = OxygenCapsule1,WaterBottle1,astrofood
```

## (UI) Don't Close Craft Window

When crafting an item, <kbd>Right Click</kbd> to not close the crafting window.

### Configuration

None.

## (UI) Grower Grab Vegetable Only

When looking at a grown vegetable in a Grower, hold <kbd>Shift</kbd> while clicking
the vegetable itself to not take the seed, so it can immediately grow the next vegetable.

### Configuration

None.

## (UI) Hide Beacons in Photomode

When using the photomode (<kbd>F2</kbd>), this mod will hide the user placed and colored
beacons.

### Configuration

None.

## (UI) Inventory Move Multiple Items

When transferring items between the player backpack and any container,
- Press <kbd>Middle Mouse</kbd> to transfer all items of the same type (i.e., all Iron)
- Press <kbd>Shift+Middle Mouse</kbd> to transfer a small amount of items of the same type (default 5)
- Press <kbd>Ctrl+Shift+Middle Mouse</kbd> to transfer a larger amount of items of the same type (default 50)

### Configuration

`akarnokd.theplanetcraftermods.uiinventorymovemultiple.cfg`

```
[General]

## How many items to move when only a few to move.
# Setting type: Int32
# Default value: 5
MoveFewAmount = 5

## How many items to move when many to move.
# Setting type: Int32
# Default value: 50
MoveManyAmount = 50
```

## (UI) Show Container Content Info

When looking at a container before opening it, the tooltip at the bottom of the screen will show how many
items are in there, the capacity of the container and the very first item type.
(Pro tip: store different types of items in different containers)

Example: `Open Container [ 5 / 30 ] Cobalt`

### Configuration

None.

## (UI) Show Player Inventory Counts

In the bottom left part of the screen, there are some numbers showing the player's position, status and
framerate. This mod will add the current number of inventory items, the inventory capacity and
how many items can be added to it.

Example: `800,0,100:4:60   <[  5  /  30  (  -25  )]>`

### Configuration

None.

## (UI) Show Player Tooltip Item Count

When in an inventory or build screen, in the tooltip of an item, show the number of items of the same
type in the player's backpack.

Example: `Cobalt x 5`

### Configuration

None.

## (UI) Show Rocket Counts

In the Terraformation information screen (one of the large screens), show the number of rockets used for
each type of terraformation effect: oxygen, heat, pressure, biomass, next to the current growth speed.

Example: ` 2 x -----    6000.00 nPa/s`

### Configuration

None.

## (UI) Pin Recipe to Screen

On the various craft screens, use <kbd>Middle click</kbd> to pin or unpin a craftable recipe to the screen.

In the panel, the curly parenthesis indicates how many of that item is in the player's inventory.
The `< 2 >` indicates how many of the recipe can be crafted from the given inventory.

Note that pinned recipes can't be saved currently as it requires save modding.

### Configuration

`akarnokd.theplanetcraftermods.uipinrecipe.cfg`

```cs
[General]

## The size of the font used
# Setting type: Int32
# Default value: 25
FontSize = 25

## The width of the recipe panel
# Setting type: Int32
# Default value: 850
PanelWidth = 850
```

## (UI) Craft Equipment Inplace

When crafting upgrades to equimpent currently equipped, the newer equipment
will be replaced inplace. This avoids loosing backpack capacity or equipment capacity
for the duration of a traditional crafting step.

Note that the UI will indicate you are missing the equipment as an ingredient but
the crafting action will succeed if the rest of the materials are in your backpack.

:warning: Please make a backup of your save before attempting to use this mod, just in case.

### Configuration

None.

## (Lib) Support Mods with Load n Save

This mod alters the loading and saving of the game by parsing/appending custom information based on
registered callbacks. These callbacks can be registered by other plugins and thus they can use this
plugin for save-specific persistency.

Here is an example plugin that utilizes this plugin:

https://github.com/akarnokd/ThePlanetCrafterMods/blob/main/ExampleModLoadSaveSupportSoft/Plugin.cs

### Developer notes

#### Dependency setup

To add a (soft) dependency to this plugin in your own plugin, use the `BepInDependency` annotation
with the guid (`akarnokd.theplanetcraftermods.libmodloadsavesupport`) of this plugin:

```cs
[BepInPlugin(guid, "(Example) Soft Dependency on ModLoadSaveSupport", "1.0.0.0")]
[BepInDependency(libModLoadSaveSupportGuid, BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    const string libModLoadSaveSupportGuid = "akarnokd.theplanetcraftermods.libmodloadsavesupport";

    const string guid = "akarnokd.theplanetcraftermods.examplemodloadsavesupportsoft";
}
```

This way, BepInEx knows to load your plugin after this plugin.

#### Registering callbacks

This plugin uses callbacks to get what to save or notify about what has been loaded for a plugin.

First, locate this plugin via its guid:

```cs
using BepInEx.Bootstrap;

if (Chainloader.PluginInfos.TryGetValue(libModLoadSaveSupportGuid, 
        out BepInEx.PluginInfo pi))
{
}
```

If found, locate the `RegisterLoadSave` method on its instance:

```cs
// public IDisposable RegisterLoadSave(string guid, Action<string> onLoad, Func<string> onSave)

MethodInfo mi = pi.Instance.GetType().GetMethod("RegisterLoadSave",
                   new Type[] { 
                       typeof(string), 
                       typeof(Action<string>), 
                       typeof(Func<string>) 
                   }
);

```

Then, invoke it with your plugin id and the delegates to a load and a save function:

```cs
this.handle = (IDisposable)mi.Invoke(pi.Instance, new object[] { 
    guid, new Action<string>(OnLoad), new Func<string>(OnSave) 
});

//...

IDisposable handle;

void OnLoad(string content) {

}
string OnSave() {

}
```

The handle is there to remove the registration if needed.

```cs
void OnDestroy()
{
    handle?.Dispose();
    handle = null;
}
```

:warning: Please make sure the string you return does not contain the `@` or `|` characters as these are treated by the vanilla game
and this mod as separators.

#### Loading Lifecycle

This plugin will deliver the custom save data (if any) after the `SessionController.Start` of the game
returns control. This way, every vanilla game data should be initialized.

Note that there is no guaranteed order of loading data for different registered plugins.
