# MajorItemsByAreaTracker

MajorItemsByAreaTracker is a randomizer connection that adds a semi-spoiler randomizer game mode with an in-game tracker.

## How does the format work?

When you first load into a new save, a log file is generated called `MajorItemCountByAreaLog.txt`. This log tells you how
many major items you will find in each *map area*, and which items are considered major. By default, all skills and required TE items
are included. The goal of the format is to collect all the required items and defeat the Radiance.

## Mod Settings

In the mod menu, you can control when the UI is visible - always, while the game is paused, or never.

## Connection Settings

In the "All Major Items" connection submenu, you can configure the settings for a new rando save. If enabled, this will affect the hash, 
so you can verify your settings are the same as everyone else you're playing with - using this mod is preferred for races rather than
using HKSpoilerViewer. To play with default settings, you only need to enable the connection. Optionally, you can include unique keys 
(e.g. Love Key and Elegant Key) and charm notches to add additional items to the tracker/objective.

## Custom Item Support

Connections may integrate with this mod using [ConnectionMetadataInjector tags](https://github.com/BadMagic100/ConnectionMetadataInjector#tag-spec).
This mod requests the following properties:

| Property | Type | Parent Object | Description | Default Handling |
| -------- | ---- | ------------- | ----------- | ---------------- |
| `IsMajorItem` | `bool` | `AbstractItem` | Whether the item should be considered a major item. | Checks [various criteria](https://github.com/BadMagic100/MajorItemByAreaTracker/blob/df2d6883bcedec9ec0b37730835f37beca423c18/SemiSpoilerLogger/MajorItemHandler.cs#L27). In lieu of setting this property, you may also fulfil one of the other criteria (e.g. putting a custom item in the "Skills" PoolGroup). |
| `MajorItemName` | `string` | `AbstractItem` | The name of the item as it should appear in the log. | The AbstractItem's name property with underscores replaced by spaces |