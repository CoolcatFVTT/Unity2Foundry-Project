using UnityEngine;
using System;

public enum WallType
{
	Wall,
	Door,
	DoorSecret,
	Window,
	NarrowPassage,
	LightEffectBlocker,
	MovementBlocker
}

public class WallTypeData
{
	public MovementRestriction MovementRestriction { get; private set; } = MovementRestriction.Normal;
	public SenseRestriction LightRestriction { get; private set; } = SenseRestriction.Normal;
	public SenseRestriction SightRestriction { get; private set; } = SenseRestriction.Normal;
	public SenseRestriction SoundRestriction { get; private set; } = SenseRestriction.Normal;
	public WallDirection WallDirection { get; private set; } = WallDirection.Both;
	public DoorType DoorType { get; private set; } = DoorType.None;
	public Color Color { get; private set; } = Color.white;
	public bool NeedsDoorState { get { return DoorType != DoorType.None; } }
	public bool NeedsProjection { get; private set; } = false;
	public bool IsMergeable { get; private set; } = false;


	public static WallTypeData Get(WallType wallType)
	{
		return m_data[(int)wallType];
	}

	static WallTypeData()
	{
		int numTypes = Enum.GetNames(typeof(WallType)).Length;
		m_data = new WallTypeData[numTypes];

		m_data[(int)WallType.Wall] = new WallTypeData()
		{
			Color = new Color(1.00f, 1.00f, 0.39f),
			IsMergeable = true
		};
		m_data[(int)WallType.Door] = new WallTypeData() {
			DoorType = DoorType.Door,
			Color = new Color(0.40f, 0.40f, 0.93f),
			NeedsProjection = true
		};
		m_data[(int)WallType.DoorSecret] = new WallTypeData() {
			DoorType = DoorType.Secret,
			Color = new Color(0.65f, 0.07f, 0.83f),
			NeedsProjection = true
		};
		m_data[(int)WallType.Window] = new WallTypeData() {
			LightRestriction = SenseRestriction.None,
			SightRestriction = SenseRestriction.None,
			Color = new Color(0.51f, 0.73f, 0.05f),
			NeedsProjection = true,
			IsMergeable = true
		};
		m_data[(int)WallType.NarrowPassage] = new WallTypeData()
		{
			MovementRestriction = MovementRestriction.None,
			LightRestriction = SenseRestriction.None,
			SightRestriction = SenseRestriction.None,
			SoundRestriction = SenseRestriction.None,
			Color = new Color(0.93f, 0.05f, 0.05f),
			NeedsProjection = true,
			IsMergeable = true
		};
		m_data[(int)WallType.LightEffectBlocker] = new WallTypeData()
		{
			MovementRestriction = MovementRestriction.None,
			LightRestriction = SenseRestriction.Normal,
			SightRestriction = SenseRestriction.None,
			SoundRestriction = SenseRestriction.None,
			Color = new Color(0.93f, 0.05f, 0.05f),
			NeedsProjection = false,
			IsMergeable = true
		};
		m_data[(int)WallType.MovementBlocker] = new WallTypeData()
		{
			MovementRestriction = MovementRestriction.Normal,
			LightRestriction = SenseRestriction.None,
			SightRestriction = SenseRestriction.None,
			SoundRestriction = SenseRestriction.None,
			Color = new Color(0.93f, 0.05f, 0.05f),
			NeedsProjection = false,
			IsMergeable = true
		};
	}

	private static readonly WallTypeData[] m_data;
}
