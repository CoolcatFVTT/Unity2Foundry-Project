using UnityEngine;
using SimpleJSON;
using System;

public class Wall : MonoBehaviour
{
	public WallType m_wallType = WallType.Wall;
	public DoorState m_doorState = DoorState.Closed;
	public float m_projectionDepth = 0.0f;
	public float m_projectionWidth = 0.0f;
	public Vector3 m_pointA = new Vector3(-0.5f, 0.0f, 0.0f);
	public Vector3 m_pointB = new Vector3( 0.5f, 0.0f, 0.0f);

	[NonSerialized]	public Vector3 m_pointAWorld;
	[NonSerialized] public Vector3 m_pointBWorld;


	public WallTypeData Data { get { return WallTypeData.Get(m_wallType); } }


	private void Awake()
	{
		Recalculate();
	}

	public void Recalculate()
	{
		m_pointAWorld = transform.TransformPoint(m_pointA);
		m_pointBWorld = transform.TransformPoint(m_pointB);		
	}

	public void RecalculateFull(ref ProjectionData data)
	{
		Recalculate();

		if (Data.NeedsProjection && m_projectionDepth > 0.0f)
		{
			Vector3 center = (m_pointA + m_pointB) * 0.5f;
			Vector3 depth = Vector3.down * m_projectionDepth;
			float halfWidth = m_projectionWidth * 0.5f;
			Vector3 limitA = center + (m_pointA - center).normalized * halfWidth;
			Vector3 limitB = center + (m_pointB - center).normalized * halfWidth;
			data.m_limitAWorld = transform.TransformPoint(limitA);
			data.m_limitBWorld = transform.TransformPoint(limitB);
			data.m_projectAWorld = transform.TransformPoint(limitA + depth);
			data.m_projectBWorld = transform.TransformPoint(limitB + depth);
		}
		else
		{
			data.m_projectAWorld = data.m_limitAWorld = m_pointAWorld;
			data.m_projectBWorld = data.m_limitBWorld = m_pointBWorld;
		}
	}	

	private void OnDrawGizmos()
	{
		Gizmos.color = Data.Color;
		DrawGizmo();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.LerpUnclamped(Data.Color, Color.white, 0.5f);
		DrawGizmo();
	}

	private void DrawGizmo()
	{
		ProjectionData pd = new ProjectionData();
		RecalculateFull(ref pd);

		Gizmos.DrawSphere(m_pointAWorld, 0.02f);
		Gizmos.DrawSphere(m_pointBWorld, 0.02f);
		Gizmos.DrawSphere(transform.position, 0.05f);
		Gizmos.DrawLine(m_pointAWorld, m_pointBWorld);

		if (Data.NeedsProjection && m_projectionDepth > 0.0f)
		{
			Gizmos.DrawSphere(pd.m_projectAWorld, 0.02f);
			Gizmos.DrawSphere(pd.m_projectBWorld, 0.02f);
			Gizmos.DrawLine(m_pointAWorld, pd.m_projectAWorld);
			Gizmos.DrawLine(m_pointBWorld, pd.m_projectBWorld);
			Gizmos.DrawLine(pd.m_projectAWorld, pd.m_projectBWorld);

			Gizmos.DrawSphere(pd.m_limitAWorld, 0.02f);
			Gizmos.DrawSphere(pd.m_limitBWorld, 0.02f);
		}
	}

	public JSONObject ExportJSON(Camera camera, Vector3 coordOffset)
	{
		JSONObject json = new JSONObject();
		json["_id"] = GetRandomID();

		Vector3 pointAScreen = Utils.Project(m_pointAWorld, camera, coordOffset);
		Vector3 pointBScreen = Utils.Project(m_pointBWorld, camera, coordOffset);
		json["c"] = Utils.ToJSON(pointAScreen, pointBScreen);

		json["light"].AsInt = (int)Data.LightRestriction;
		json["move"].AsInt = (int)Data.MovementRestriction;
		json["sight"].AsInt = (int)Data.SightRestriction;
		json["sound"].AsInt = (int)Data.SoundRestriction;
		json["dir"].AsInt = (int)Data.WallDirection;
		json["door"].AsInt = (int)Data.DoorType;
		json["ds"].AsInt = Data.NeedsDoorState ? (int)m_doorState : (int)DoorState.Closed;

		JSONObject flags = new JSONObject();

		Vector3 wallDirScreen = pointBScreen - pointAScreen;
		float wallDirLength2 = wallDirScreen.sqrMagnitude;
		if (m_projectionDepth > 0.0f && wallDirLength2 > 0.0001f)
		{
			ProjectionData pd = new ProjectionData();
			RecalculateFull(ref pd);

			Vector3 projectAScreen = Utils.Project(pd.m_projectAWorld, camera, coordOffset);
			Vector3 projectBScreen = Utils.Project(pd.m_projectBWorld, camera, coordOffset);
			Vector3 limitAScreen = Utils.Project(pd.m_limitAWorld, camera, coordOffset);
			Vector3 limitBScreen = Utils.Project(pd.m_limitBWorld, camera, coordOffset);
			float limitA = Mathf.Clamp01(Vector3.Dot(wallDirScreen, limitAScreen - pointAScreen) / wallDirLength2);
			float limitB = Mathf.Clamp01(Vector3.Dot(wallDirScreen, limitBScreen - pointAScreen) / wallDirLength2);

			JSONObject unity2foundry = new JSONObject();
			unity2foundry["projectA"] = Utils.ToJSON(projectAScreen);
			unity2foundry["projectB"] = Utils.ToJSON(projectBScreen);
			unity2foundry["limitA"] = Utils.ToJSON(limitA);
			unity2foundry["limitB"] = Utils.ToJSON(limitB);
			flags["unity2foundry"] = unity2foundry;
		}
		else
		{
			flags.Inline = true;
		}
		json["flags"] = flags;

		return json;
	}

	public string GetRandomID()
	{
		int id = gameObject.GetInstanceID();
		return Utils.GetRandomID(id);
	}

	public struct ProjectionData
	{
		public Vector3 m_projectAWorld;
		public Vector3 m_projectBWorld;
		public Vector3 m_limitAWorld;
		public Vector3 m_limitBWorld;
	}
}
