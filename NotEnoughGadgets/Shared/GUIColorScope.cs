using System;
using UnityEngine;

namespace NotEnoughGadgets.Shared
{
	// https://github.com/derail-valley-modding/custom-car-loader/blob/41edb4f96414acfa3af267168696c63f65b3000e/CCL.Creator/Utility/EditorHelpers.cs#L336
	internal class GUIColorScope : IDisposable
	{
		private readonly Color _entryColor;
		private readonly Color _entryBackground;
		private readonly Color _entryContent;

		public GUIColorScope(Color? newColor = null, Color? newBackground = null, Color? newContent = null)
		{
			_entryColor = GUI.color;
			_entryBackground = GUI.backgroundColor;
			_entryContent = GUI.contentColor;

			if (newColor.HasValue) GUI.color = newColor.Value;
			if (newBackground.HasValue) GUI.backgroundColor = newBackground.Value;
			if (newContent.HasValue) GUI.contentColor = newContent.Value;
		}

		public void Dispose()
		{
			GUI.color = _entryColor;
			GUI.backgroundColor = _entryBackground;
			GUI.contentColor = _entryContent;
		}
	}
}

// footnote: took this from wiz's car changer
