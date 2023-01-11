using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class NodeAI {

    public int GCost; // The number of nodes between the start node and the current node
    public int HCost; // The number of nodes between the current node and the end node 

    public int FCost {
        get { return GCost + HCost; }
        private set { FCost = value; }
    }
    
    public bool isWalkable;
    
    public GameObject[] connections = new GameObject[2];
     
    
    public void SetupConnections(GameObject current) {
        if (current.TryGetComponent<Direction>(out Direction direction)) {
            GameObject[] directions = direction.directionsStep.Where(d => d != null).ToArray();

            connections = directions;
        }
        else {
            int index = GameController.Instance.FindIndexInParent(current.transform.parent.gameObject,current);

            if (index + 1 < current.transform.parent.childCount)
                connections[0] = current.transform.parent.GetChild(index + 1).gameObject;
            else 
                connections[0] = current.transform.parent.GetChild(index + 1 - current.transform.parent.childCount).gameObject;
        }

    }
}
