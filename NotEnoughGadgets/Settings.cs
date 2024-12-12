using System;
using System.Linq;
using System.Collections.Generic;

using UnityModManagerNet;

using UnityEngine;
using DV.Shops;

namespace NotEnoughGadgets
{
	public class Settings : UnityModManager.ModSettings, IDrawable
	{
		public readonly string? version = Main.mod?.Info.Version;
		ConfigHandler.LimitsConfig? config = ConfigHandler.LoadConfig();
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

		public override void Save(UnityModManager.ModEntry entry)
		{
			if (config is null)
			{
				throw new Exception("Config is null!");
			}

			ConfigHandler.SaveConfig(config);
			Save(this, entry);
			Debug.Log("Config updated and saved to " + ConfigHandler.configFilePath);
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

			if (config == null)
			{
				GUILayout.Label("Config does not exist yet! Load into a save to generate config.");
				config = ConfigHandler.LoadConfig();
			}
			else
			{
				GUILayout.Label("Save reload is required to apply changes on settings below.");
				GUILayout.Label($"Set item limits. Applied lower limit depends on how many of a specific item you've already bought, and upper limit is {maxLimit}");

				foreach (var entry in config.itemInitialAmounts.ToList())
				{
					GUILayout.BeginHorizontal();

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
						config.itemInitialAmounts[entry.Key] = parsedLimit;
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
