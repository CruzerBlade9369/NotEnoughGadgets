using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using UnityEngine;

namespace NotEnoughGadgets.PatchHelpers
{
	public class ConfigHandler
	{
		public static string configFilePath = Path.Combine(
		Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
		"config.json"
		);

		// limits cache
		public static Dictionary<string, int> itemInitialAmounts = new Dictionary<string, int>();

		private static void LoadFromJson(string json)
		{
			try
			{
				var wrappedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(json);
				if (wrappedData != null && wrappedData.ContainsKey("itemInitialAmounts"))
				{
					itemInitialAmounts = wrappedData["itemInitialAmounts"];
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Failed to parse itemInitialAmounts: {ex.Message}");
			}
		}

		private static string SaveToJson()
		{
			var wrappedData = new Dictionary<string, Dictionary<string, int>>
			{
				{ "itemInitialAmounts", itemInitialAmounts }
			};
			return JsonConvert.SerializeObject(wrappedData, Formatting.Indented);
		}

		public static void LoadConfig()
		{
			if (File.Exists(configFilePath))
			{
				try
				{
					string json = File.ReadAllText(configFilePath);
					LoadFromJson(json);
				}
				catch
				{
					Debug.LogWarning("Failed to read config. itemInitialAmounts will remain empty.");
				}
			}
			else
			{
				Debug.LogWarning("Config file not found. itemInitialAmounts will remain empty.");
			}
		}

		public static void SaveConfig()
		{
			try
			{
				string json = SaveToJson();
				File.WriteAllText(configFilePath, json);
			}
			catch (IOException ex)
			{
				Debug.LogError("Failed to save config: " + ex.Message);
			}
		}
	}
}
