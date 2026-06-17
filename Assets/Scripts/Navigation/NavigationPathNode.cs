using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Globalization;

public class NavigationPathNode : FastPriorityQueueNode {
    public class Neighbor {
        public NavigationHelpers.Direction direction;
        public NavigationPathNode node;

        public Neighbor(NavigationHelpers.Direction p_direction, NavigationPathNode p_node) {
            direction = p_direction;
            node = p_node;
        }
    }

    // The Navigation controller.
    public NavigationSearcher searcher = null;
    // The node's own position.
    public Vector2Int position = Vector2Int.zero;
    // Were we found this node from.
    public NavigationHelpers.Direction parentDirection = NavigationHelpers.Direction.None;
    // How many nodes have we gone through? Used to cut off searches after a point.
    public int depth = -1;
    // How much the search has already cost. -1 means we've not inspected this node yet.
    public int searchPathCost = -1;
    // After a search is complete, use this to recreate the forward path. None indicates this wasn't part of the solution.
    public NavigationHelpers.Direction solvedChildDirection = NavigationHelpers.Direction.None;

    public NavigationPathNode(NavigationSearcher p_searcher, Vector2Int p_position) {
        searcher = p_searcher;
        position = p_position;
    }

    public NavigationTileBase GetNavTile() {
        return searcher.controller.NavMap.Map.GetTile<NavigationTileBase>((Vector3Int)position);
    }

    public int CalculatePriority(NavigationSearcher.Heuristic heuristic, int scale = 1) {
        switch (heuristic) {
            case NavigationSearcher.Heuristic.LinearDistance:
            default:
                return (searchPathCost * scale) + (int)(Vector2Int.Distance(position, searcher.endPosition) * scale);
            case NavigationSearcher.Heuristic.ManhattanDistance:
                return (searchPathCost * scale) + (NavigationHelpers.ManhattanDistance(position, searcher.endPosition) * scale);
        }
    }

    public List<Neighbor> GetValidNeighbors() {
        // Preallocation
        List<Neighbor> retVal = new List<Neighbor>(4);

        // Some nodes may have their cost change if approached from a different direction.
        // Hence, we're not gonna worry about if neighbors are searchable for now.

        // Down
        if (position.y >= searcher.bounds.min.y) {
            retVal.Add(new Neighbor(NavigationHelpers.Direction.Down, searcher.GetNodeAt(position + Vector2Int.down )));
        }

        // Up
        if (position.y <= searcher.bounds.max.y) {
            retVal.Add(new Neighbor(NavigationHelpers.Direction.Up, searcher.GetNodeAt(position + Vector2Int.up )));
        }

        
        // Left
        if (position.x >= searcher.bounds.min.x) {
            retVal.Add(new Neighbor(NavigationHelpers.Direction.Left, searcher.GetNodeAt(position + Vector2Int.left )));
        }

        // Right
        if (position.y <= searcher.bounds.max.y) {
            retVal.Add(new Neighbor(NavigationHelpers.Direction.Right, searcher.GetNodeAt(position + Vector2Int.right )));
        }

        return retVal;
    }

    public NavigationPathNode GetParentNode() {
        if (parentDirection == NavigationHelpers.Direction.None) {
            return null;
        }

        return searcher.GetNodeAt(position + NavigationHelpers.GetDirectionAsVector(parentDirection));
    }

    public NavigationPathNode GetSolvedChildNode() {
        if (solvedChildDirection == NavigationHelpers.Direction.None) {
            return null;
        }

        return searcher.GetNodeAt(position + NavigationHelpers.GetDirectionAsVector(solvedChildDirection));
    }

    public bool IsStart() {
        return position == searcher.startPosition;
    }

    public bool IsEnd() {
        return position == searcher.startPosition;
    }

    public bool IsDiscovered() {
        return searchPathCost >= 0;
    }

    public void Discover(int p_depth, int p_searchPathCost, NavigationHelpers.Direction p_parentDirection) {
        depth = p_depth;
        searchPathCost = p_searchPathCost;
        parentDirection = p_parentDirection;
    }

    public void ReportSolution() {
        if (IsStart()) {
            // The entire solution is resolved.
            return;
        }

        NavigationPathNode pnode = GetParentNode();
        pnode.solvedChildDirection = NavigationHelpers.GetOppositeDirection(parentDirection);

        pnode.ReportSolution();
    }

    public void ResetSolution() {
        parentDirection = NavigationHelpers.Direction.None;
        searchPathCost = -1;
        solvedChildDirection = NavigationHelpers.Direction.None;
    }
}