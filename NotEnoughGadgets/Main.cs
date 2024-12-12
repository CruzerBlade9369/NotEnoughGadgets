using System;
using System.Reflection;

using HarmonyLib;
using UnityModManagerNet;

using UnityEngine;

namespace NotEnoughGadgets
{
	public static class Main
	{
		public static bool enabled;
		public static UnityModManager.ModEntry? mod;

		public static Settings settings { get; private set; }

		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Harmony? harmony = null;

			try
			{
				try
				{
					settings = Settings.Load<Settings>(modEntry);
				}
				catch
				{
					Debug.LogWarning("Unabled to load mod settings. Using defaults instead.");
					settings = new Settings();
				}

				mod = modEntry;

				harmony = new Harmony(modEntry.Info.Id);
				harmony.PatchAll(Assembly.GetExecutingAssembly());
				DebugLog("Attempting patch.");

				modEntry.OnShowGUI = settings.OpenGUI;
				modEntry.OnGUI = settings.DrawGUI;
				modEntry.OnSaveGUI = settings.Save;
			}
			catch (Exception ex)
			{
				modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
				harmony?.UnpatchAll(modEntry.Info.Id);
				return false;
			}

			return true;
		}

		public static void DebugLog(string message)
		{
			if (settings.isLoggingEnabled)
				mod?.Logger.Log(message);
		}

		public static void SettingsConfigLoad()
		{

		}

		#region UNUSED DEBUG METHOD

		/*[HarmonyPatch(typeof(GlobalShopController))]
		[HarmonyPatch("InitializeShopData")]
		static class GetAllItems
		{
			static void Postfix(GlobalShopController __instance)
			{
				if (__instance.shopItemsData != null)
				{
					Debug.Log("Printing all shop item localization keys:");

					foreach (var shopItem in __instance.shopItemsData)
					{
						if (shopItem?.item?.localizationKeyName != null)
						{
							Debug.Log($"[NotEnoughGadgets] Item: {shopItem.item.localizationKeyName}, Prefab name: {shopItem.item.ItemPrefabName}");
						}
						else
						{
							Debug.LogWarning("Item or localizationKeyName is null.");
						}
					}
				}
				else
				{
					Debug.LogError("shopItemsData is null in GlobalShopController.");
				}
			}
		}*/

		#endregion

	}
}
