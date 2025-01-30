using System.Collections.Generic;

namespace NotEnoughGadgets.PatchHelpers
{
	internal class IgnoredItems
	{
		public static readonly HashSet<string> ignoredItems = new HashSet<string>
		{
			"shovel",
			"lighter",
			"RemoteController",
			"Key",
			"Stopwatch",
			"GoldenShovel",
			"KeyCaboose",
			"ExpertShovel",
			"Boombox",
			"Cassette_Album01",
			"Cassette_Album02",
			"Cassette_Album03",
			"Cassette_Album04",
			"Cassette_Album05",
			"Cassette_Album06",
			"Cassette_Album07",
			"Cassette_Album08",
			"Cassette_Album09",
			"Cassette_Album10",
			"Cassette_Album11",
			"Cassette_Album12",
			"Cassette_Album13",
			"Cassette_Album14",
			"Cassette_Album15",
			"Cassette_Album16",
			"Cassette_Playlist01",
			"Cassette_Playlist02",
			"Cassette_Playlist03",
			"Cassette_Playlist04",
			"Cassette_Playlist05",
			"Cassette_Playlist06",
			"Cassette_Playlist07",
			"Cassette_Playlist08",
			"Cassette_Playlist09",
			"Cassette_Playlist10",
			"KeyDE6Slug",
			"KeyDM1U",
		};

		public static bool IgnoreItem(string prefabName)
		{
			if (ignoredItems.Contains(prefabName))
			{
				Main.DebugLog($"Skipping item with prefab name: {prefabName}");
				return true;
			}

			if (prefabName.Contains("custom_item_mod"))
			{
				if (Main.settings.ignoreCustomItems)
				{
					Main.DebugLog($"Skipping custom item: {prefabName}");
					return true;
				}
			}

			if (prefabName.Contains("SM_ItemSpec"))
			{
				if (Main.settings.ignoreSkinManager)
				{
					Main.DebugLog($"Skipping SM paint can: {prefabName}");
					return true;
				}
			}

			return false;
		}
	}
}
