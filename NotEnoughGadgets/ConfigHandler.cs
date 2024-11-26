using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using UnityEngine;

namespace NotEnoughGadgets
{
	internal class ConfigHandler
	{
		public static string configFilePath = Path.Combine(
		Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
		"config.json"
		);

		// config list for limits
		public class LimitsConfig
		{
			public Dictionary<string, int> itemInitialAmounts = new Dictionary<string, int>();
		}

		public static LimitsConfig? LoadConfig()
		{
			if (File.Exists(configFilePath))
			{
				try
				{
					string json = File.ReadAllText(configFilePath);
					return JsonConvert.DeserializeObject<LimitsConfig>(json) ?? new LimitsConfig();
				}
				catch
				{
					Debug.LogWarning("Config not found! Returning null.");
				}
			}

			return null;
		}

		public static LimitsConfig LoadOrCreateConfig()
		{
			if (File.Exists(configFilePath))
			{
				try
				{
					string json = File.ReadAllText(configFilePath);
					return JsonConvert.DeserializeObject<LimitsConfig>(json) ?? new LimitsConfig();
				}
				catch
				{
					Debug.LogWarning("Failed to read or parse config. Creating a new one.");
				}
			}

			LimitsConfig newConfig = new LimitsConfig();
			SaveConfig(newConfig);
			return newConfig;
		}

		// save config file
		public static void SaveConfig(LimitsConfig config)
		{
			try
			{
				string json = JsonConvert.SerializeObject(config, Formatting.Indented);
				File.WriteAllText(configFilePath, json);
			}
			catch (IOException ex)
			{
				Debug.LogError("Failed to save config: " + ex.Message);
			}
		}
	}
}
