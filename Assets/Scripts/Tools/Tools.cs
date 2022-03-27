using UnityEngine;
using UnityEditor;

public class Tools
{
	[MenuItem("Tools/Force Reserialize Assets")]
	static void ForceReserializeAssets()
	{
		bool confirm = EditorUtility.DisplayDialog(
			"Force Reserialize Assets?",
			"Are you sure you want to Force Reserialize Assets? There is no Undo!",
			"Ok", "Cancel"
		);
		if (confirm)
		{
			AssetDatabase.ForceReserializeAssets();
		}
	}
}
