using UnityEngine;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
	public bool m_doorOpen = false;
	public float m_movementRange = 0.65f;

	[FormerlySerializedAs("LabelFront")]
	[TextArea] public string m_labelFront = "Storage\n<size=40>A</size>";
	[FormerlySerializedAs("LabelBack")]
	[TextArea] public string m_labelBack = "Storage\n<size=40>A</size>";
}
