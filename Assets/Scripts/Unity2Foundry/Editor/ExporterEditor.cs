using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using WebP;

[CustomEditor(typeof(Exporter))]
public class ExporterEditor : Editor
{
	private SerializedProperty serializedResolution;

	void OnEnable()
	{
		serializedResolution = serializedObject.FindProperty("m_resolution");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedResolution);
		serializedObject.ApplyModifiedProperties();

		if (GUILayout.Button("Export"))
			Export();
	}

	private void Export()
	{
		Exporter exporter = target as Exporter;
		Camera camera = exporter.GetComponent<Camera>();
		if (camera == null)
			return;

		string pngFilename = GetDefaultFilename() + ".webp";
		pngFilename = EditorUtility.SaveFilePanel("Save image as WebP", "", pngFilename, "webp");
		if (pngFilename.Length == 0)
			return;

		string jsonFilename = Path.ChangeExtension(pngFilename, ".json");

		TakeScreenshot(pngFilename);
		TakeWalls(jsonFilename);

		RenderTexture.ReleaseTemporary(camera.targetTexture);
		camera.targetTexture = null;
	}
	  
	private void TakeScreenshot(string filename)
	{
		Debug.Log("Exporting to " + filename);

		Exporter exporter = target as Exporter;
		Vector2Int resolution = exporter.m_resolution;

		Camera camera = exporter.GetComponent<Camera>();
		camera.targetTexture = GetRenderTexture(resolution);
		camera.aspect = (float)resolution.x / resolution.y;
		camera.Render();

		Texture2D texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
		RenderTexture previousRT = RenderTexture.active;
		RenderTexture.active = camera.targetTexture;
		Rect sourceRect = new Rect(0, 0, resolution.x, resolution.y);
		texture.ReadPixels(sourceRect, 0, 0, false);
		texture.Apply(false);

		RenderTexture.active = previousRT;

		{// Flip upside down.
		 // ref: https://github.com/netpyoung/unity.webp/issues/25
			Color32[] src = texture.GetPixels32();
			Color32[] dest = new Color32[src.Length];
			int w = texture.width;
			int h = texture.height;
			for (int y = 0; y < h; y++)
				Array.Copy(src, y * w, dest, (h - y - 1) * w, w);
			texture.SetPixels32(dest);
		}

		byte[] data = texture.EncodeToWebP(90, out Error result);
		if (result != Error.Success)
			Debug.LogError("Webp EncodeToWebP Error: " + result.ToString());
		else
			File.WriteAllBytes(filename, data);
	}

	private void TakeWalls(string filename)
	{
		// find walls
		Exporter exporter = target as Exporter;
		List<Wall> walls = new List<Wall>();
		foreach (GameObject go in exporter.gameObject.scene.GetRootGameObjects())
		{
			if (go.activeInHierarchy)
				walls.AddRange(go.GetComponentsInChildren<Wall>(false));
		}
		for (int i = 0; i < walls.Count; ++i)
		{
			Wall wall = walls[i];
			wall.Recalculate();
		}

		// filter simple duplicates
		for (int i = 0; i < walls.Count; ++i)
		{
			Wall current = walls[i];
			if (current == null)
				continue;
			for (int j = i + 1; j < walls.Count; ++j)
			{
				if (walls[j] != null && Utils.ApproxEqual(current, walls[j]))
					walls[j] = null;
			}
		}

		// parse existing scene
		Camera camera = exporter.GetComponent<Camera>();
		JSONObject json = null;
		if (File.Exists(filename))
		{
			try
			{
				string backupDir = Path.GetDirectoryName(filename) + "\\backup";
				Directory.CreateDirectory(backupDir);
				string backupFilename = backupDir + "\\" + Path.ChangeExtension(Path.GetFileName(filename), ".backup." + GetDefaultFilename() + ".json");
				//Debug.Log(backupFilename);
				File.Copy(filename, backupFilename, true);
				string dataRead = File.ReadAllText(filename);
				json = JSONNode.Parse(dataRead).AsObject;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return;
			}
		}
		if (json == null)
			json = new JSONObject();

		// confirm parameters
		int width = GetIntOrDefault(json, "width", camera.pixelWidth);
		int height = GetIntOrDefault(json, "height", camera.pixelHeight);
		if (width != camera.pixelWidth || height != camera.pixelHeight)
		{
			Debug.LogError("Background image dimensions in in scene file do not match.");
			return;
		}
		float padding = GetFloatOrDefault(json, "padding", 0.25f);
		float grid = GetFloatOrDefault(json, "grid", 100.0f);
		Vector3 coordOffset = new Vector3(
			Mathf.Ceil(padding * width / grid) * grid,
			Mathf.Ceil(padding * height / grid) * grid,
			0.0f);

		// export
		JSONArray array = json["walls"].AsArray;
		array.Clear();
		array.Inline = false;
		for (int i = 0; i < walls.Count; ++i)
		{
			Wall wall = walls[i];
			if (wall != null)
				array.Add(wall.ExportJSON(camera, coordOffset));
		}

		string data = json.ToString(2);
		File.WriteAllText(filename, data);
	}

	private static RenderTexture GetRenderTexture(Vector2Int resolution)
	{
		RenderTextureDescriptor descriptor = new RenderTextureDescriptor(resolution.x, resolution.y, RenderTextureFormat.ARGB32, 24);
		descriptor.msaaSamples = 8;
		descriptor.autoGenerateMips = false;
		descriptor.useMipMap = false;
		descriptor.sRGB = true;
		RenderTexture texture = RenderTexture.GetTemporary(descriptor);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		return texture;
	}

	private static string GetDefaultFilename()
	{
		return DateTime.Now.ToString("yyyyMMdd-HHmmss");
	}

	private static int GetIntOrDefault(JSONNode node, string key, int defaultValue)
	{
		if (node.HasKey(key))
			return node[key].AsInt;
		else
			return defaultValue;
	}

	private static float GetFloatOrDefault(JSONNode node, string key, float defaultValue)
	{
		if (node.HasKey(key))
			return node[key].AsFloat;
		else
			return defaultValue;
	}
}