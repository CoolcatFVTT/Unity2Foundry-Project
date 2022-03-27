using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(Door))]
[CanEditMultipleObjects]
public class DoorEditor : Editor
{
	private SerializedProperty serializedDoorOpen;
	private SerializedProperty serializedMovementRange;
	private SerializedProperty serializedLabelFront;
	private SerializedProperty serializedLabelBack;

	void OnEnable()
	{
		serializedDoorOpen = serializedObject.FindProperty("m_doorOpen");
		serializedMovementRange = serializedObject.FindProperty("m_movementRange");
		serializedLabelFront = serializedObject.FindProperty("m_labelFront");
		serializedLabelBack = serializedObject.FindProperty("m_labelBack");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedDoorOpen);
		EditorGUILayout.PropertyField(serializedMovementRange);
		EditorGUILayout.PropertyField(serializedLabelFront);
		EditorGUILayout.PropertyField(serializedLabelBack);
		serializedObject.ApplyModifiedProperties();

		bool open = serializedDoorOpen.boolValue;
		float range = open ? serializedMovementRange.floatValue : 0.0f;
		Door door = target as Door;
		Transform left = door.transform.Find("DoorLeft");
		Transform right = door.transform.Find("DoorRight");
		SetDoor(left, -range, serializedLabelFront);
		SetDoor(right, range, serializedLabelBack);
	}

	private void SetDoor(Transform door, float range, SerializedProperty text)
	{
		if (door == null)
			return;

		Vector3 position = new Vector3(range, 0.0f, 0.0f);
		if (door.localPosition != position)
		{
			door.localPosition = position;
			EditorUtility.SetDirty(door);
		}

		Transform label = door.Find("Label");
		if (label != null)
		{
			string s = text.stringValue;
			bool active = !string.IsNullOrWhiteSpace(s);
			if (label.gameObject.activeSelf != active)
			{
				label.gameObject.SetActive(active);
				EditorUtility.SetDirty(label.gameObject);
			}
			TextMeshPro textMesh = label.GetComponent<TextMeshPro>();
			if (textMesh != null && textMesh.text != s)
			{
				textMesh.text = s;
				EditorUtility.SetDirty(textMesh);
			}
		}
	}
}