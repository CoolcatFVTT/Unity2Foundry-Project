using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Wall))]
[CanEditMultipleObjects]
public class WallEditor : Editor
{
	private SerializedProperty m_wallType;
	private SerializedProperty m_doorState;
	private SerializedProperty m_projectionDepth;
	private SerializedProperty m_projectionWidth;
	private SerializedProperty m_pointA;
	private SerializedProperty m_pointB;

	void OnEnable()
	{
		m_wallType = serializedObject.FindProperty("m_wallType");
		m_doorState = serializedObject.FindProperty("m_doorState");
		m_projectionDepth = serializedObject.FindProperty("m_projectionDepth");
		m_projectionWidth = serializedObject.FindProperty("m_projectionWidth");
		m_pointA = serializedObject.FindProperty("m_pointA");
		m_pointB = serializedObject.FindProperty("m_pointB");
	}

	protected virtual void OnSceneGUI()
	{
		Wall wall = (Wall)target;
		wall.Recalculate();

		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.white;
		Handles.Label(VectorOffsetY(wall.m_pointAWorld, -0.4f), "A", style);
		Handles.Label(VectorOffsetY(wall.m_pointBWorld, -0.4f), "B", style);

		EditorGUI.BeginChangeCheck();		
		Vector3 pointA = Handles.PositionHandle(wall.m_pointAWorld, Quaternion.identity);
		Vector3 pointB = Handles.PositionHandle(wall.m_pointBWorld, Quaternion.identity);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(wall, "Wall Points");
			wall.m_pointA = FixVector(wall.transform.InverseTransformPoint(pointA));
			wall.m_pointB = FixVector(wall.transform.InverseTransformPoint(pointB));
			wall.Recalculate();
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		Wall wall = (Wall)target;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Wall ID");
		EditorGUILayout.SelectableLabel(wall.GetRandomID());
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.PropertyField(m_wallType);

		if (wall.Data.NeedsDoorState)
		{
			EditorGUILayout.PropertyField(m_doorState);
		}
		if (wall.Data.NeedsProjection)
		{
			EditorGUILayout.PropertyField(m_projectionDepth);
			EditorGUILayout.PropertyField(m_projectionWidth);
		}

		EditorGUILayout.PropertyField(m_pointA);
		EditorGUILayout.PropertyField(m_pointB);

		serializedObject.ApplyModifiedProperties();
	}

	private static Vector3 VectorOffsetY(Vector3 position, float scale)
	{
		return position + Vector3.up * scale * HandleUtility.GetHandleSize(position);
	}

	private static Vector3 FixVector(Vector3 value)
	{
		return new Vector3(
			(float)Math.Round(value.x, 5),
			(float)Math.Round(value.y, 5),
			(float)Math.Round(value.z, 5)
		);
	}
}