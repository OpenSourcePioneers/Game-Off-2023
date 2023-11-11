using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TwoD_Grid : MonoBehaviour
{
	[SerializeField] public LayerMask unwalkableMask;
	[SerializeField] public Vector2 gridWorldSize;
	[SerializeField] public  float nodeRadius;
	[SerializeField] private bool showGrid = false;

	static TwoD_Grid inst;
	TwoD_Pathfinding pathfinding;
	TwoD_Node[,] grid;
	float nodeDiameter;
	int gridSizeX, gridSizeY;
	Vector3 worldBottomLeft;

	void Awake()
	{
		inst = this;
		pathfinding = GetComponent<TwoD_Pathfinding>();
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		CreateGrid();
	}
    
	private void CreateGrid()
	{
		List<TwoD_Node> unwalkable = new List<TwoD_Node>();
		grid = new TwoD_Node[gridSizeX, gridSizeY];
		worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
		//Loop through all nodes and check if is walkable
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 nodePoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !Physics.CheckSphere(nodePoint, nodeRadius - 0.1f, unwalkableMask);
				grid[x, y] = new TwoD_Node(walkable, nodePoint, x, y);
				if(!walkable)
					unwalkable.Add(grid[x, y]);
			}
		}
		List<TwoD_Node> set = new List<TwoD_Node>();
		//Configure Nodes that should be unwalkable
		foreach (var node in unwalkable)
		{
			List<TwoD_Node> diagNeighbours = GetDiagonalNeighbours(node);
			foreach (var neighbour in diagNeighbours)
			{
				if(!neighbour.walkable)
				{
					List<TwoD_Node> iNeigh = GetNeighbours(node);
					List<TwoD_Node> jNeigh = GetNeighbours(node);
					foreach (var i in iNeigh)
					{
						foreach (var j in jNeigh)
						{
							if(i == j)
								set.Add(i);
						}
					}
				}
			}
		}

		foreach (var node in set)
		{
			node.walkable = false;
		}
	}

	public List<TwoD_Node> GetNeighbours(TwoD_Node node)
	{
		List<TwoD_Node> neighbours = new List<TwoD_Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
					neighbours.Add(grid[checkX, checkY]);
			}
		}
		return neighbours;
	}

	private List<TwoD_Node> GetDiagonalNeighbours(TwoD_Node node)
	{
		List<TwoD_Node> neighbours = new List<TwoD_Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 || y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
					neighbours.Add(grid[checkX, checkY]);
			}
		}
		return neighbours;
	}

	public static TwoD_Node NodeFromWorldPosition(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x - inst.transform.position.x + inst.gridWorldSize.x / 2) / inst.gridWorldSize.x;
		float percentY = (worldPosition.z - inst.transform.position.z + inst.gridWorldSize.y / 2) / inst.gridWorldSize.y;

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((inst.gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((inst.gridSizeY - 1) * percentY);
		//Debug.Log(grid[x, y, z].nodePosition);
		return inst.grid[x, y];
	}

	void OnDrawGizmos()
	{
        if (showGrid)
        {
			Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0f, gridWorldSize.y));
			if(!Application.isPlaying)
				return;
			foreach (TwoD_Node node in grid)
			{
				if(!node.walkable)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawCube(node.nodePosition, Vector3.one * nodeDiameter);
				}
			}

			Gizmos.color = Color.blue;
			Gizmos.DrawCube(NodeFromWorldPosition(worldBottomLeft).nodePosition, Vector3.one * nodeDiameter);
		}
    }
}