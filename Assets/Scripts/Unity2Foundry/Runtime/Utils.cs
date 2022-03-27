using SimpleJSON;
using System;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{
	public static Vector3 Project(Vector3 worldPoint, Camera camera, Vector3 coordOffset)
	{
		Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);
		screenPoint.y = camera.pixelHeight - screenPoint.y;
		screenPoint += coordOffset;
		screenPoint.z = 0.0f;
		return screenPoint;
	}

	public static bool ApproxEqual(Wall a, Wall b)
	{
		if (a.m_wallType != b.m_wallType)
			return false;
		if (!a.Data.IsMergeable)
			return false;
		return (ApproxEqual(a.m_pointAWorld, b.m_pointAWorld) && ApproxEqual(a.m_pointBWorld, b.m_pointBWorld))
			|| (ApproxEqual(a.m_pointAWorld, b.m_pointBWorld) && ApproxEqual(a.m_pointBWorld, b.m_pointAWorld));
	}

	public static bool ApproxEqual(Vector3 a, Vector3 b)
	{
		return Vector3.SqrMagnitude(a - b) < 0.0001f;
	}

	public static JSONArray ToJSON(Vector3 position)
	{
		JSONArray coords = new JSONArray { Inline = true };
		coords.Add("", ToJSON(position.x));
		coords.Add("", ToJSON(position.y));
		return coords;
	}

	public static JSONArray ToJSON(Vector3 position1, Vector3 position2)
	{
		JSONArray coords = new JSONArray { Inline = true };
		coords.Add("", ToJSON(position1.x));
		coords.Add("", ToJSON(position1.y));
		coords.Add("", ToJSON(position2.x));
		coords.Add("", ToJSON(position2.y));
		return coords;
	}

	public static JSONNumber ToJSON(float value)
	{
		double d = Math.Round(value, 4);
		return new JSONNumber(d);
	}

	public static string GetRandomID(int seed)
	{
		Random.State previousState = Random.state;
		Random.InitState(seed);
		StringBuilder b = new StringBuilder(ID_LENGTH);
		for (int i = 0; i < ID_LENGTH; ++i)
		{
			int c = Random.Range(0, CHARACTERS.Length);
			b.Append(CHARACTERS[c]);
		}
		Random.state = previousState;
		return b.ToString();
	}

	private const int ID_LENGTH = 16;
	private static readonly string CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
}
