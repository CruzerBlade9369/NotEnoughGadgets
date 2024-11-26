using System;
using System.Linq;

using UnityModManagerNet;

using UnityEngine;

namespace NotEnoughGadgets
{
	public class Settings : UnityModManager.ModSettings, IDrawable
	{
		public readonly string? version = Main.mod?.Info.Version;
		ConfigHandler.LimitsConfig? config = ConfigHandler.LoadConfig();

		[Draw("Enable logging")]
		public bool isLoggingEnabled =
#if DEBUG
			true;
#else
            false;
#endif

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

		public void OnChange() { }

		public void DrawGUI(UnityModManager.ModEntry modEntry)
		{
			this.Draw(modEntry);
			DrawConfigs();
		}

		private void DrawConfigs()
		{
			GUILayout.BeginVertical(GUILayout.MinWidth(400), GUILayout.ExpandWidth(false));
			GUILayout.Space(4);

			if (config is null)
			{
				GUILayout.Label("Config does not exist yet! Load into a save to generate config.");
				Main.DebugLog("Attempting to reload config.");
				config = ConfigHandler.LoadConfig();
			}
			else
			{
				GUILayout.Label("Save reload is required to apply changes on settings below.");
				GUILayout.Label("Set item limits (Min = 1, Max = 100)");

				foreach (var entry in config.itemInitialAmounts.ToList())
				{
					GUILayout.BeginHorizontal();

					GUILayout.Label(entry.Key, GUILayout.Width(200));

					string newLimit = GUILayout.TextField(entry.Value.ToString());
					if (int.TryParse(newLimit, out int parsedLimit))
					{
						parsedLimit = Mathf.Clamp(parsedLimit, 1, 100);
						config.itemInitialAmounts[entry.Key] = parsedLimit;
					}
					else
					{
						GUILayout.Label("Invalid value");
					}

					GUILayout.EndHorizontal();
				}
			}

			/*for (int i = 0; i < config.itemInitialAmounts.Count(); i++)
			{
				var entry = config.itemInitialAmounts.ElementAt(i);

				GUILayout.BeginHorizontal();

				GUILayout.Label(entry.Key, GUILayout.Width(200));

				string newLimit = GUILayout.TextField(entry.Value.ToString());
				if (int.TryParse(newLimit,out int parsedLimit))
				{
					parsedLimit = Mathf.Clamp(parsedLimit, 1, 100);
					config.itemInitialAmounts[entry.Key] = parsedLimit;
				}
				else
				{
					GUILayout.Label("Invalid value");
				}

				GUILayout.EndHorizontal();
			}*/

			GUILayout.EndVertical();
		}
	}
}
