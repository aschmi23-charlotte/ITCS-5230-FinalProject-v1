using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System;
using Unity.Behavior;

// Pathfinding Grid for A*
// We'll use Manhattan pathing and distance, since the game world is grid-based.

[System.Serializable]
public class NavigationSearcher {
    public enum Heuristic {
        LinearDistance,
        ManhattanDistance,
    }

    [SerializeField] public Heuristic heuristic = Heuristic.LinearDistance;
    [SerializeField] public int maxDepth = 0;
    [field: SerializeField] public Vector2Int startPosition {get; private set;} = Vector2Int.zero;
    [field: SerializeField] public Vector2Int endPosition {get; private set;} = Vector2Int.zero;
    
    [field: NonSerialized] public NavigationController controller {get; private set;} 
    [field: NonSerialized] public BoundsInt bounds {get; private set;}
    [field: NonSerialized] public NavigationPathNode[,] nodes {get; private set;}

    protected bool unsolvable = false;
    protected bool searching = false;
    protected FastPriorityQueue<NavigationPathNode> searchQueue = null;
    protected LinkedList<NavigationPathNode> discoveredNodes = null;

    public NavigationSearcher() {}
    public NavigationSearcher(NavigationController p_controller, int p_maxDepth = 0, Heuristic p_heuristic = Heuristic.LinearDistance) {

        maxDepth = p_maxDepth;
        heuristic = p_heuristic;

        Initialize(p_controller);
    }

    public void Initialize(NavigationController p_controller) {
        controller = p_controller;
        
        bounds = controller.NavMap.Map.cellBounds;
        nodes = new NavigationPathNode[bounds.size.x, bounds.size.y];

        foreach (Vector3Int pos in bounds.allPositionsWithin) {
            nodes[pos.x - bounds.min.x, pos.y - bounds.min.y] = new NavigationPathNode(this, (Vector2Int)pos);
        }
    }

    public NavigationPathNode GetNodeAtWorldPos(Vector2 pos) {
        return GetNodeAtWorldPos((Vector3)pos);
    }

    public NavigationPathNode GetNodeAtWorldPos(Vector3 pos) {
        return GetNodeAt(controller.NavMap.Map.WorldToCell(pos));
    }

    public NavigationPathNode GetNodeAt(Vector2Int pos) {
        return GetNodeAt((Vector3Int)pos);
    }
    public NavigationPathNode GetNodeAt(Vector3Int pos) {
        return nodes[pos.x - bounds.min.x, pos.y - bounds.min.y];
    }

    public NavigationPathNode GetStartNode() {
        return GetNodeAt(startPosition);
    }

    public NavigationPathNode GetEndNode() {
        return GetNodeAt(startPosition);
    }

    public void SetCellSearchTargets(Vector2Int startPos, Vector2Int endPos) {
        startPosition = startPos;
        endPosition = endPos;
    }

    public void SetSearchTargets(Vector2 startPos, Vector2 endPos) {
        SetCellSearchTargets((Vector2Int)controller.NavMap.Map.WorldToCell(startPos), (Vector2Int)controller.NavMap.Map.WorldToCell(endPos));
    }

    public void StartSearch() {
        searchQueue = new FastPriorityQueue<NavigationPathNode>(nodes.Length);
        discoveredNodes = new LinkedList<NavigationPathNode>();

        NavigationPathNode startNode = GetStartNode();

        // Priming the start node:
        startNode.Discover(0, 0, NavigationHelpers.Direction.None);
        searchQueue.Enqueue(startNode, startNode.CalculatePriority(heuristic));
    }


    // Return true to indicate the search is finished, false to indicate we need to continue later.
    public bool UpdateSearch(int steps = 0) {
        // If steps is negative or 0, then run until completion.
        for (int i = 0; steps <= 0 || i < steps; i++) {
            // If the queue is empty, then no solution is possible.
            if (searchQueue.Count == 0) {
                unsolvable = true;
                return true;
            }

            NavigationPathNode node = searchQueue.Dequeue();
            if (node.IsEnd()) {
                node.ReportSolution();
                return true;
            }

            // If we're limiting depth, cut off nodes that get too deep.
            if (maxDepth > 0 && node.depth >= maxDepth) {
                continue;
            }

            foreach (NavigationPathNode.Neighbor neighbor in node.GetValidNeighbors()) {
                if (neighbor.node.IsDiscovered()) {
                    continue;
                }

                neighbor.node.Discover(
                    node.depth + 1,
                    node.searchPathCost + neighbor.node.GetNavTile().BaseCost,
                    NavigationHelpers.GetOppositeDirection(neighbor.direction)
                );

                discoveredNodes.AddLast(neighbor.node);

                if (neighbor.node.IsObstructed()) {
                    continue;
                }

                searchQueue.Enqueue(neighbor.node, neighbor.node.CalculatePriority(heuristic));
            }
        }

        return false;
    }

    public bool IsSolutionFound() {
        return GetEndNode().IsDiscovered();
    }
    
    // Prep for a new search.
    public void Reset() {
        unsolvable = false;
        searching = false;
        if (discoveredNodes != null) {
            foreach (NavigationPathNode node in discoveredNodes) {
                node.ResetSolution();
            }
            discoveredNodes.Clear();   
        }   

        if (searchQueue != null) {
            searchQueue.Clear();
        }
    }

    public bool SearchUntilComplete() {
        StartSearch();
        UpdateSearch();
        return IsSolutionFound();
    }

    public void StartSearchCoroutine(int stepsPerFrame = 300) {
        controller.StartCoroutine(SearchCoroutine(stepsPerFrame));
    }

    IEnumerator<object> SearchCoroutine(int stepsPerFrame = 300) {
        searching = true;
        StartSearch();
        while(!UpdateSearch(stepsPerFrame)) {
            yield return null;
        }
        searching = false;
    }

    public List<NavigationPathNode> GetSolutionAsList() {
        List<NavigationPathNode> retVal = new List<NavigationPathNode>(GetEndNode().depth);

        NavigationPathNode node = GetStartNode();
        while (node != null) {
            retVal.Add(node);
            node = node.GetSolvedChildNode();
        }

        return retVal;
    }
}