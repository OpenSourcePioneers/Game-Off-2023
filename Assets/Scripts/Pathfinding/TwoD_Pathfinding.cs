using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TwoD_Pathfinding : MonoBehaviour
{
	[HideInInspector]
	private TwoD_Node startNode, targetNode;
	private TwoD_Grid grid;

	PathManager pathManager;
	void Awake()
	{
		grid = GetComponent<TwoD_Grid>();
		pathManager = GetComponent<PathManager>();
	}

    public void StartPathfinding(Vector3 start, Vector3 end)
    {
		StartCoroutine(FindPath(start, end));
    }

	private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
	{
		bool pathFound = false;
		startNode = TwoD_Grid.NodeFromWorldPosition(startPos);
		targetNode = TwoD_Grid.NodeFromWorldPosition(targetPos);
		List<TwoD_Node> openSet = new List<TwoD_Node>();
		HashSet<TwoD_Node> closedSet = new HashSet<TwoD_Node>();

		if(startNode != targetNode || startNode.walkable && targetNode.walkable)
			openSet.Add(startNode);

		while(openSet.Count > 0)
		{
			TwoD_Node node = openSet[0];
			for(int i = 1; i < openSet.Count; i++)
			{
				if(openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}
			openSet.Remove(node);
			closedSet.Add(node);
			if(node == targetNode)
			{
				pathFound = true;
				break;
			}
			foreach(TwoD_Node neighbour in grid.GetNeighbours(node))
			{
				if(!neighbour.walkable || closedSet.Contains(neighbour))
				{
					continue;
				}
				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if(newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;
					if(!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
		yield return null;
		Vector3[] wayPoints = new Vector3[0];
		if(pathFound)
			wayPoints = RetracePath(startNode, targetNode);

		pathManager.FinishedFinding(wayPoints, pathFound);
	}

	private List<Vector3> SimplifyPath(List<TwoD_Node> path)
	{
		List<Vector3> points = new List<Vector3>();
		Vector3 oldDir = Vector3.zero;
		for(int i = 1; i < path.Count; i++)
		{
			Vector3 newDir = path[i-1].nodePosition - path[i].nodePosition;
			if(newDir! != oldDir)
				points.Add(path[i].nodePosition);
			oldDir = newDir;
		}
		return points;
	}

	private Vector3[] RetracePath(TwoD_Node startNode, TwoD_Node endNode)
	{
		List<TwoD_Node> path = new List<TwoD_Node>();
		TwoD_Node currentNode = endNode;

		while(currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		List<Vector3> wayPoints = SimplifyPath(path);
		wayPoints.Reverse();
		return wayPoints.ToArray();
	}
	private int GetDistance(TwoD_Node nodeA, TwoD_Node nodeB)
	{
		//Heuristic function
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if(dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}
}