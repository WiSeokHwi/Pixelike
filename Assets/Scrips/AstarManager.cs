using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AstarManager : MonoBehaviour
{
    public static AstarManager instance;

    private void Awake()
    {
        instance = this;
    }
    public List<Node> GeneratePath(Node startNode, Node endNode)
    {
        List<Node> openSet = new List<Node>();
        Node[] nodes = FindObjectsByType<Node>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (Node n in nodes)
        {
            n.gScore = float.MaxValue;
        }

        startNode.gScore = 0;
        startNode.hScore = Vector2.Distance(startNode.transform.position, endNode.transform.position);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            int lowestF = default;

            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }
            Node currentNode = openSet[lowestF];
            openSet.RemoveAt(lowestF);

            if (currentNode == endNode)
            {
                List<Node> path = new List<Node>();

                path.Insert(0, endNode);
                
                while (currentNode != startNode)
                {
                
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }
                path.Reverse();
                return path;

            }
            foreach (Node connection in currentNode.connections)
            {
                float gScore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, connection.transform.position);
                if (gScore < connection.gScore)
                {
                    connection.cameFrom = currentNode;
                    connection.gScore = gScore;
                    connection.hScore = Vector2.Distance(connection.transform.position, endNode.transform.position);
                    if (!openSet.Contains(connection))
                    {
                        openSet.Add(connection);
                    }
                }
            }
        }
        return null;
    }
}
