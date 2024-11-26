using HarmonyLib;

using DV.Shops;

using UnityEngine;

using NotEnoughGadgets;
using NotEnoughGadgets.Patches;

[HarmonyPatch(typeof(GlobalShopController))]
[HarmonyPatch("InitializeShopData")]
class CustomInitialAmountsPatch
{
	static void Postfix(GlobalShopController __instance)
	{
		if (__instance.shopItemsData == null) return;

		ConfigHandler.LimitsConfig config = ConfigHandler.LoadOrCreateConfig();
		bool configUpdated = false;

		Main.DebugLog("Intercepting InitializeShopData to customize initial amounts.");

		for (int i = 0; i < __instance.shopItemsData.Count; i++)
		{
			ShopItemData shopItem = __instance.shopItemsData[i];
			if (shopItem == null || shopItem.item == null) continue;

			string prefabName = shopItem.item.ItemPrefabName;

			// skip if item is ignored
			if (IgnoredItems.ignoredItems.Contains(prefabName))
			{
				Main.DebugLog($"Skipping item with prefab name: {prefabName}");
				continue;
			}

			// check if item exists in the config
			if (config.itemInitialAmounts.TryGetValue(prefabName, out int customInitialAmount))
			{
				shopItem.amount = customInitialAmount;
			}
			else
			{
				// add missing item to config with default value if item doesn't exist
				config.itemInitialAmounts[prefabName] = shopItem.amount;
				configUpdated = true;
			}

			// update the initialAmounts list in GlobalShopController
			if (i < __instance.initialAmounts.Count)
			{
				__instance.initialAmounts[i] = shopItem.amount;
			}
			else
			{
				__instance.initialAmounts.Add(shopItem.amount);
			}
		}

		// save updated config if changes were made
		if (configUpdated)
		{
			ConfigHandler.SaveConfig(config);
			Debug.Log("Config updated with new items and saved to " + ConfigHandler.configFilePath);
		}
	}
}
