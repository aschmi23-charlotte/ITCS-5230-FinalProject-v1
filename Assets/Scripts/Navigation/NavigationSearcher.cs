using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using NUnit.Framework.Constraints;
using JetBrains.Annotations;

// Pathfinding Grid for A*
// We'll use Manhattan pathing and distance, since the game world is grid-based.

public class NavigationSearcher {
    public NavigationController controller {get; private set;} 
    public int maxDepth = 0;
    public Heuristic heuristic = Heuristic.LinearDistance;
    public Vector2Int startPosition {get; private set;} = Vector2Int.zero;
    public Vector2Int endPosition {get; private set;} = Vector2Int.zero;
    public BoundsInt bounds {get; private set;}
    public NavigationPathNode[,] nodes {get; private set;}

    public enum Heuristic {
        LinearDistance,
        ManhattanDistance,
    }

    protected bool unsolvable = false;
    protected FastPriorityQueue<NavigationPathNode> searchQueue = null;
    protected bool searching = false;


    public NavigationSearcher(NavigationController p_controller, int p_maxDepth = 0, Heuristic p_heuristic = Heuristic.LinearDistance) {
        controller = p_controller;
        maxDepth = p_maxDepth;
        heuristic = p_heuristic;

        bounds = controller.NavMap.Map.cellBounds;
        nodes = new NavigationPathNode[bounds.size.x, bounds.size.y];

        foreach (Vector3Int pos in bounds.allPositionsWithin) {
            nodes[pos.x - bounds.min.x, pos.y - bounds.min.y] = new NavigationPathNode(this, (Vector2Int)pos);
        }

        // We're never gonna queue more nodes than the total number, right?
        // Probably overkill, but at least I know it works.
        searchQueue = null;
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

    public void SetSearchTargets(Vector2Int startPos, Vector2Int endPos) {
        startPosition = startPos;
        endPosition = endPos;
    }

    public void InitSearch() {
        searchQueue = new FastPriorityQueue<NavigationPathNode>(nodes.Length);

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

                searchQueue.Enqueue(neighbor.node, neighbor.node.CalculatePriority(heuristic));
            }
        }

        return false;
    }

    public bool IsSolutionFound() {
        return GetEndNode().IsDiscovered();
    }
    
    public void ResetSolution() {
        foreach (Vector3Int pos in bounds.allPositionsWithin) {
            GetNodeAt(pos).ResetSolution();
        }
    }

    public bool SearchUntilComplete() {
        InitSearch();
        UpdateSearch();
        return IsSolutionFound();
    }

    public void StartSearchCoroutine(int stepsPerFrame = 300) {
        controller.StartCoroutine(SearchCoroutine(stepsPerFrame));
    }

    IEnumerator<object> SearchCoroutine(int stepsPerFrame = 300) {
        searching = true;
        InitSearch();
        while(!UpdateSearch(stepsPerFrame)) {
            yield return null;
        }
        searching = false;
    }
}