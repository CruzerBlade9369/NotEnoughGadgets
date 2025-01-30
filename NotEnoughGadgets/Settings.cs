using System;
using System.Linq;
using System.Collections.Generic;

using UnityModManagerNet;

using UnityEngine;
using DV.Shops;

using NotEnoughGadgets.Shared;
using NotEnoughGadgets.PatchHelpers;

namespace NotEnoughGadgets
{
	public class Settings : UnityModManager.ModSettings, IDrawable
	{
		public readonly string? version = Main.mod?.Info.Version;
		GlobalShopController? gsc;

		public static Dictionary<string, int> itemsWithHigherAppliedLimit = new Dictionary<string, int>();

		[Draw("Enable logging")]
		public bool isLoggingEnabled =
#if DEBUG
			true;
#else
            false;
#endif

		[Draw("REAL no limit (use with caution, you can't delete items you already bought)")]
		public bool realNoLimit = false;

		[Draw("Ignore Custom Item Mod items")]
		public bool ignoreCustomItems = false;

		[Draw("Ignore Skin Manager custom paint cans")]
		public bool ignoreSkinManager = false;

		public override void Save(UnityModManager.ModEntry entry)
		{
			Save(this, entry);
			ConfigHandler.SaveConfig();
			Main.DebugLog("Config updated and saved to " + ConfigHandler.configFilePath);
		}

		public void OnChange()
		{

		}

		public void OpenGUI(UnityModManager.ModEntry modEntry)
		{
			ConfigHandler.LoadConfig();
			gsc = GlobalShopController.Instance;
		}

		public void DrawGUI(UnityModManager.ModEntry modEntry)
		{
			this.Draw(modEntry);
			DrawConfigs();
		}

		private void DrawConfigs()
		{
			int maxLimit = realNoLimit ? Int32.MaxValue : 100;
			int minLimit = 1;

			GUILayout.BeginVertical(GUILayout.MinWidth(800), GUILayout.ExpandWidth(false));
			GUILayout.Space(4);

			if (ConfigHandler.itemInitialAmounts == null || ConfigHandler.itemInitialAmounts.Count <= 0)
			{
				GUILayout.Label("Config does not exist yet! Load into a save to generate config.");
			}
			else
			{
				GUILayout.Label("Save reload is required to apply changes on settings below.");
				GUILayout.Label($"Set item limits. Applied lower limit depends on how many of a specific item you've already bought, and upper limit is {maxLimit}");

				foreach (var entry in ConfigHandler.itemInitialAmounts.ToList())
				{
					GUILayout.BeginHorizontal();

					if (GUILayout.Button(new GUIContent("x", "Remove this limit"), GUILayout.Width(30f)))
					{
						ConfigHandler.itemInitialAmounts.Remove(entry.Key);
						continue;
					}

					if (itemsWithHigherAppliedLimit.TryGetValue(entry.Key, out int value) && gsc != null)
					{
						using (new GUIColorScope(Color.red))
						{
							GUILayout.Label(new GUIContent(entry.Key, "Invalid limit") , GUILayout.MaxWidth(200));
						}
					}
					else
					{
						GUILayout.Label(entry.Key, GUILayout.MaxWidth(200));
					}

					string newLimit = GUILayout.TextField(entry.Value.ToString(), GUILayout.Width(100));
					if (int.TryParse(newLimit, out int parsedLimit))
					{
						parsedLimit = Mathf.Clamp(parsedLimit, minLimit, maxLimit);
						ConfigHandler.itemInitialAmounts[entry.Key] = parsedLimit;
					}
					else
					{
						GUILayout.Label("ERROR: Invalid value");
					}

					GUILayout.EndHorizontal();
				}
			}

			GUILayout.EndVertical();
		}
	}
}
