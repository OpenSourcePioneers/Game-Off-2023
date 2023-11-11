using UnityEngine;
using System.Collections;

public class TwoD_Node
{
	public bool walkable;
	public bool spawned = false;
	public Vector3 nodePosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public TwoD_Node parent;

	public TwoD_Node(bool _walkable, Vector3 _nodePos, int _gridX, int _gridY)
	{
		walkable = _walkable;
		nodePosition = _nodePos;
		gridX = _gridX;
		gridY = _gridY;
		spawned = false;
	}

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
}