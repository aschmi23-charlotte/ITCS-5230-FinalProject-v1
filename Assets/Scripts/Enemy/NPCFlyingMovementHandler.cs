using System.Collections.Generic;
using UnityEngine;

public class NPCFlyingMovementHandler : FlyingMovementHandler {
    
    [Header("NPC Movement")]
    public Vector2 moveInput;
    public NavigationSearcher navigation;

    public bool runSearch = false;

    protected List<NavigationPathNode> solutionNodes = null;
    public int solutionIndex = 0;

    void Start() {
        navigation.Initialize(NavigationController.Instance);

    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (runSearch) {
            runSearch = false;
            navigation.Reset();
            navigation.SetSearchTargets(transform.position, PlayerBrain.Instance.transform.position);
            navigation.SearchUntilComplete();
            Debug.LogFormat("SolutionFound = {0}", navigation.IsSolutionFound());
            if (navigation.IsSolutionFound()) {
                solutionNodes = navigation.GetSolutionAsList();
                solutionIndex = 0;
            }
        }

        if (solutionNodes != null) {
            NavigationPathNode currentNode = navigation.GetNodeAtWorldPos(transform.position);
            
            if (currentNode.IsPartOfSolution() && solutionIndex < solutionNodes.Count - 1) {
                solutionIndex = currentNode.depth + 1;
            }

            NavigationPathNode targetNode = solutionNodes[solutionIndex];
            ProcessMovementDirection((targetNode.GetWorldPosition() - (Vector2)transform.position).normalized);


            if (targetNode.IsWithinNode(transform.position)) {
                if (solutionIndex == solutionNodes.Count - 1) {
                    solutionNodes = null;
                    solutionIndex = 0;
                } else {
                    solutionIndex++;
                }
            }
        } else {
            ProcessMovementDirection(Vector2.zero);
        }

        // ProcessMovementDirection(moveInput);
    }

    void OnDrawGizmos() {
        if (solutionNodes != null) {
            Gizmos.color = Color.rebeccaPurple;
            

            foreach (NavigationPathNode node in solutionNodes) {
                Gizmos.DrawSphere(node.GetWorldPosition(), 0.2f);
            }
        }
    }
}