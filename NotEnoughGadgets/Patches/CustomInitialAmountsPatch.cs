using System;

using HarmonyLib;

using DV.Shops;

using UnityEngine;

using NotEnoughGadgets;
using NotEnoughGadgets.Patches;

[HarmonyPatch(typeof(GlobalShopController))]
[HarmonyPatch("Awake")]
class PreCustomLimitsPatch
{
	static void Postfix(GlobalShopController __instance)
	{
		if (__instance.shopItemsData == null) return;

		ConfigHandler.LimitsConfig config = ConfigHandler.LoadOrCreateConfig();
		bool configUpdated = false;

		foreach (ShopItemData shopItem in __instance.shopItemsData)
		{
			if (shopItem == null || shopItem.item == null) continue;

			string prefabName = shopItem.item.ItemPrefabName;

			// skip if item is ignored
			if (IgnoredItems.ignoredItems.Contains(prefabName))
			{
				Main.DebugLog($"Skipping item with prefab name: {prefabName}");
				continue;
			}

			// add missing item to config with default value if item doesn't exist
			if (!config.itemInitialAmounts.TryGetValue(prefabName, out int customAmount))
			{
				config.itemInitialAmounts[prefabName] = shopItem.initialAmount;
				configUpdated = true;
			}

			// first set both to really high amounts so we can get the number of purchased items
			shopItem.initialAmount = Int32.MaxValue;
			shopItem.allowedToHaveAmount = Int32.MaxValue;
		}

		// save updated config if changes were made
		if (configUpdated)
		{
			ConfigHandler.SaveConfig(config);
			Main.DebugLog("Config updated with new items and saved to " + ConfigHandler.configFilePath);
		}
	}
}

[HarmonyPatch(typeof(GlobalShopController))]
[HarmonyPatch(nameof(GlobalShopController.UpdateItemStocksOnGameLoad))]
class CustomLimitsPatch
{
	static void Postfix()
	{
		GlobalShopController gsc = GlobalShopController.Instance;
		if (gsc == null || gsc.shopItemsData == null)
		{
			throw new Exception($"GlobalShopController (or its shopItemsData) is null. This isn't supposed to happen.");
		}

		ConfigHandler.LimitsConfig? config = ConfigHandler.LoadConfig();
		if (config == null)
		{
			throw new Exception("For some reason, config is null. This isn't supposed to happen.");
		}

		Main.DebugLog("Intercepting InitializeShopData to customize initial amounts.");

		Settings.itemsWithHigherAppliedLimit.Clear();
		foreach (ShopItemData shopItem in gsc.shopItemsData)
		{
			if (shopItem == null || shopItem.item == null) continue;

			string prefabName = shopItem.item.ItemPrefabName;

			// skip if item is ignored
			if (IgnoredItems.ignoredItems.Contains(prefabName))
			{
				Main.DebugLog($"Skipping item with prefab name: {prefabName}");
				continue;
			}

			if (!config.itemInitialAmounts.TryGetValue(prefabName, out int customLimit))
			{
				Debug.LogError("Error when applying custom item limits");
			}
			else
			{
				int limit = customLimit;
				if (customLimit < shopItem.purchasedItems)
				{
					Debug.Log($"Set limit for {shopItem.item.ItemPrefabName} is lower than owned amount. Applying higher limit.");
					limit = shopItem.purchasedItems;
					Settings.itemsWithHigherAppliedLimit.Add(prefabName, limit);
				}

				shopItem.initialAmount = limit;
				shopItem.allowedToHaveAmount = limit;
			}
		}
	}
}
