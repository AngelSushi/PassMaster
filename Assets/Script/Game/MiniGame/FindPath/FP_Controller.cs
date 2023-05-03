using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.SceneManagement;

public class FP_Controller : MiniGame {
    #region SegmentClass

    public class Segment {
        public List<GameObject> objectsSegment = new List<GameObject>();
        public int direction;
        public int xAxis;
    }

    #endregion

    #region Variables

    public List<GameObject> steps = new List<GameObject>();
    public List<GameObject> path = new List<GameObject>();
    public List<GameObject> clearPath = new List<GameObject>();
    public GameObject[] spawnPoints;
    public GameObject[] players;
    public GameObject decor;
    public GameObject winPlayer;
    public GameObject surface;
    public GameObject mainCamera;
    public int segmentLength = -1;
    public int direction = 0;
    public Material pathMaterial;
    public Material normalMat;
    public GameController gameController;
    public bool hasGenPath;

    private int lastDirection;    
    private int segmentCount;
    private GameObject beginSegment = null;
    private Dictionary<int,Segment> allSegments = new Dictionary<int,Segment>();
    private bool hasCheckedLeft;
    private bool hasAddStep;

    #endregion

    #region Unity Functions

    void Start() {
       // GameController.difficulty =2;
       
       for(int i = 0;i<98;i++) {
            steps.Add(decor.transform.GetChild(i).gameObject);
        }

        for(int i = 0;i<players.Length;i++) {
            players[i].transform.position = spawnPoints[i].transform.position;
        }

        gameController = GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>();

    }

    void Update() {

        if(!hasGenPath) {
            if(!ContainsWithTag()) {

<<<<<<< HEAD
                switch(GameController.difficulty) {
                    case GameController.Difficulty.EASY: // Facile
                        segmentLength = Random.Range(4,7);
                        break;

                    case GameController.Difficulty.MEDIUM: // Moyen
                        segmentLength = Random.Range(2,4);
                        break;

                    case GameController.Difficulty.HARD: // Difficile
=======
                switch(GameController.Instance.difficulty) {
                    case GameController.Difficulty.EASY:
                        segmentLength = Random.Range(4,7);
                        break;

                    case GameController.Difficulty.MEDIUM:
                        segmentLength = Random.Range(2,4);
                        break;

                    case GameController.Difficulty.HARD: 
>>>>>>> main
                        segmentLength = Random.Range(2,4);
                        break;        
                }

                if(path.Count == 0) { // Il n'y a encore aucun segment de fait, la direction est donc tt droite
                    direction = 1;
<<<<<<< HEAD
                    if(GameController.difficulty == GameController.Difficulty.HARD && segmentLength == 1) segmentLength = 2;
=======
                    if(GameController.Instance.difficulty == GameController.Difficulty.HARD && segmentLength == 1) segmentLength = 2;
>>>>>>> main
                }
                
                else {
                    direction = Random.Range(1,4);

                    if(lastDirection  == 1 && hasAddStep) {
                        direction = Random.Range(2,4);
                    }
                    if(lastDirection == 2 || lastDirection == 3 && hasAddStep) {
                        direction = 1;
                        hasAddStep = false;
                    }
                }

                GeneratePath(segmentLength,direction);
                lastDirection = direction;  

                foreach(GameObject obj in path) {  
                   // obj.GetComponent<MeshRenderer>().material = pathMaterial;
                    if(obj.GetComponent<BoxCollider>() == null)
                        obj.AddComponent<BoxCollider>();            
           
                    obj.GetComponent<BoxCollider>().size = new Vector3(19.54973f, obj.GetComponent<BoxCollider>().size.y,obj.GetComponent<BoxCollider>().size.z);
                    obj.GetComponent<BoxCollider>().center = new Vector3(0,0,0);
                }
            }      
            else {
                hasGenPath = true;

                for(int i = 0;i<7;i++) {
                    if(!path.Contains(decor.transform.GetChild(i).gameObject))
                        decor.transform.GetChild(i).gameObject.layer = 0;    
                }

                surface.GetComponent<NavMeshSurface>().BuildNavMesh();

                decor.transform.GetChild(110).gameObject.GetComponent<MeshRenderer>().enabled = false;
                decor.transform.GetChild(111).gameObject.GetComponent<MeshRenderer>().enabled = false;

                ClearPath();

            }
        }

    }

    public override void OnFinish() {
        
        if(!gameController.gameObject.activeSelf) 
            gameController.gameObject.SetActive(true);

       // winPlayer = player = gameController.GetPlayers()[0];

        if(winPlayer != null) { // Ne va pas fonctionner attention a refaire
            if(winPlayer.name != "User") {
                Vector3 playerPosition = new Vector3(winPlayer.transform.position.x,mainCamera.transform.position.y,winPlayer.transform.position.z);
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,playerPosition,200 * Time.deltaTime);
            }
        }

    }

    #endregion

    #region Custom Functions

    private void GeneratePath(int sLength,int dir) {     

        if(beginSegment == null) {
            if(path.Count == 0) // Aucun chemin n'a été générer
                beginSegment = decor.transform.GetChild(Random.Range(0,6)).gameObject;
            else  // Un bout de chemin a déjà été générer
                beginSegment = path[path.Count - 1];
        }

        // IL faudrait bloquer a la génération du path que si il y a une step en dessous de lui il ne peut pas aller dans cette direction

        CheckSegmentValidty(sLength,dir,beginSegment);
        path.Add(beginSegment);

        Segment seg = new Segment();
        seg.objectsSegment.Add(beginSegment);
        seg.direction = dir;

        for(int i = 0;i<sLength - 1;i++) {
            GetObjectAtDirection(direction,i,beginSegment,seg);
        }
        
        segmentCount++;

        allSegments.Add(segmentCount,seg);
        beginSegment = null;
        hasCheckedLeft = false;

    }

    private void GetObjectAtDirection(int direction,int index,GameObject beginObj,Segment instance) {

        Vector3 newPosition = Vector3.zero;

        if(direction == 1) // Front
            newPosition = beginObj.transform.localPosition + (beginObj.transform.forward * (1383 * (index+1)));
        else if(direction == 2)  //Right
            newPosition = beginObj.transform.localPosition + (beginObj.transform.up * (1251 * (index+1)));
        else if(direction == 3)  // Left
            newPosition = beginObj.transform.localPosition + (beginObj.transform.up * -1 * (1251 * (index + 1)));
        
        GameObject newObject = GetObjectByCoords(newPosition);

        if(newObject != null) {
            path.Add(newObject);
            instance.objectsSegment.Add(newObject);
            hasAddStep = true;
        }

    }
    
    private void CheckSegmentValidty(int sLength,int dir,GameObject beginObj) {
        Vector3 newPosition = Vector3.zero;

        if(dir == 1) { // Front
            newPosition = beginObj.transform.localPosition + (beginObj.transform.forward * (1383 * (sLength - 1)));
        }
        else if(dir == 2) { // Right
            newPosition = beginObj.transform.localPosition + (beginObj.transform.up * (1251 * (sLength - 1)));
        }
        else if(dir == 3) { // Left
            newPosition = beginObj.transform.localPosition + (beginObj.transform.up * -1 * (1251 * (sLength - 1)));
        }

        GameObject obj = GetObjectByCoords(newPosition);

        if(obj == null) { // le segment est plus grand que les limites

            for(int i = 0;i<sLength - 1;i++) {

                if(dir == 1)  // Front
                    newPosition = beginObj.transform.localPosition + (beginObj.transform.forward * (1383 * (i)));            
                else if(dir == 2) // Right
                    newPosition = beginObj.transform.localPosition + (beginObj.transform.up * (1251 * (i)));
                else if(dir == 3)  // Left
                    newPosition = beginObj.transform.localPosition + (beginObj.transform.up * -1 * (1251 * (i)));
                
                if(GetObjectByCoords(newPosition) == null) {
                    segmentLength = i - 1;

                    if(beginObj.tag != "LimitRightEnd" || beginObj.tag != "LimitLeftEnd") {
                        if(!hasCheckedLeft) {
                            if(direction == 2) direction = 3;
                            else if(direction == 3) direction = 2;

                            hasCheckedLeft = true;
                            CheckSegmentValidty(sLength,direction,beginObj);
                        }
                        else {
                            direction = 1;
                        }


                    }

                    break;
                }

                if(beginObj.tag == "LimitLeft" && direction == 3) {
                    this.direction = 2;
                }
                if(beginObj.tag == "LimitRight" && direction == 2) {
                    this.direction = 3;
                }
            }
        } 

        else {
            if(beginObj.tag == "LimitLeft" && direction == 3) {
                this.direction = 2;
            }
            if(beginObj.tag == "LimitRight" && direction == 2) {
                this.direction = 3;
            }
        }
    }

    private void ClearPath(/*int index*/) {
        List<GameObject> removedObj = new List<GameObject>();

        foreach(GameObject obj in path.ToArray()) {
           // GameObject obj = steps[index];

            //obj.GetComponent<MeshRenderer>().material = tm;

            if(path.Contains(obj) && IsOnEndOfSegment(obj) && GetSegmentByObj(obj).direction == 1) {
                GameObject rightObj = GetObjectByCoords(obj.transform.localPosition + (obj.transform.up *1251 ));
               // Debug.Log("obj: " + obj);
                GameObject leftObj = GetObjectByCoords(obj.transform.localPosition + (obj.transform.up *1251 * -1 ));

                // Debug.Log("rightObj: " + rightObj);
                // Debug.Log("leftObj: " + leftObj);

                if(path.Contains(rightObj)) {
                    GameObject rightFrontObj = GetObjectByCoords(rightObj.transform.localPosition + (rightObj.transform.forward * 1383));
                    // Debug.Log("frontObj: " + rightFrontObj);
                    if(rightFrontObj != null) {
                        bool hasConnection = false;

                        for(int i = 0;i<11;i++) {
                            Vector3 checkPos = rightFrontObj.transform.localPosition + (rightObj.transform.up * 1251 * i);
                            if(GetObjectByCoords(checkPos) != null && path.Contains(GetObjectByCoords(checkPos))) {
                                hasConnection = true;
                                break;
                            }
                        }

                        // Debug.Log("hasConnection: " + hasConnection);

                        if(!hasConnection ) { // Cela signifie qu'il y a un bout de path a droit mais qu'il n'a aucune autre connexion
                            for(int i = 0;i<11;i++) {
                                Vector3 removePos = rightObj.transform.localPosition + (rightObj.transform.up * 1251 * i );
                                 // Debug.Log("removeObj: " + GetObjectByCoords(removePos));
                                //  Debug.Log("check1: " + GetObjectByCoords(removePos));
                                 // Debug.Log("check2: " +path.Contains(GetObjectByCoords(removePos)));
                                
                                if(GetObjectByCoords(removePos) != null && path.Contains(GetObjectByCoords(removePos))) {
                                    removedObj.Add(GetObjectByCoords(removePos));
                                }
                            }
                        }
                    }
                }

                // ClearPath deux fois pour supprimer les grosses différences

                if(path.Contains(leftObj)) {
                    GameObject leftFrontObj = GetObjectByCoords(leftObj.transform.localPosition + (leftObj.transform.forward * 1383));
                      //Debug.Log("obj: " + obj);
                    //  Debug.Log("frontObj: " + leftFrontObj);
                    if(leftFrontObj != null) {
                        bool hasConnection = false;

                        for(int i = 0;i<11;i++) {
                            Vector3 checkPos = leftFrontObj.transform.localPosition + (leftObj.transform.up * -1 * 1251 * i);
                            if(GetObjectByCoords(checkPos) != null && path.Contains(GetObjectByCoords(checkPos))) {
                                hasConnection = true;
                                break;
                            }
                        }

                         //Debug.Log("hasConnection: " + hasConnection);

                        if(!hasConnection) { // Cela signifie qu'il y a un bout de path a gauche mais qu'il n'a aucune autre connexion
                            for(int i = 0;i<11;i++) {
                                Vector3 removePos = leftObj.transform.localPosition + (leftObj.transform.up * -1 * 1251 * i);
                               //   Debug.Log("removeObj: " + GetObjectByCoords(removePos));
                                //  Debug.Log("check1: " + GetObjectByCoords(removePos));
                                //  Debug.Log("check2: " +path.Contains(GetObjectByCoords(removePos)));

                                if(GetObjectByCoords(removePos) != null && path.Contains(GetObjectByCoords(removePos))) {
                                    removedObj.Add(GetObjectByCoords(removePos));
                                    //path.Remove(GetObjectByCoords(removePos));
                                }
                            }
                        }   
                    }
                }   
            }
        }

        foreach(GameObject o in removedObj) {
            path.Remove(o);
        }

        GameObject lastStep = null;

        foreach(GameObject obj in path.ToArray()) {
            if(obj == lastStep) {
                path.Remove(obj);
            }

            lastStep = obj;
        }
    

    }

    private bool IsOnEndOfSegment(GameObject obj) {
        foreach(Segment segment in allSegments.Values) {
            if(segment.objectsSegment[0] == obj || segment.objectsSegment[segment.objectsSegment.Count - 1] == obj) 
                return true;
        }

        return false;
    }

    private Segment GetSegmentByObj(GameObject obj) {
        foreach(Segment segment in allSegments.Values) {
            for(int i = 0;i<segment.objectsSegment.Count;i++){
                if(segment.objectsSegment[i] == obj) {
                    return segment;
                }   
            }
        }

        return null;
    }
    
    private int GetKeyByValue(Segment s) {
        foreach(int i in allSegments.Keys) {
            if(allSegments[i] == s) {
                return i;
            }
        }

        return -1;
    }

    public GameObject GetObjectByCoords(Vector3 vec) {
        foreach(GameObject obj in steps) {
            Vector3 objPos = obj.transform.localPosition;
            float distance = Vector3.Distance(objPos,vec);
            if(distance < 1) { 
                return obj;
            }
        }

        return null;
    }

    public GameObject GetObjectByWorldCoords(Vector3 vec) {
        foreach(GameObject obj in steps) {
            Vector3 objPos = obj.transform.position;
            float distance = Vector3.Distance(objPos,vec);
            if(distance < 1) { 
                return obj;
            }
        }

        return null;
    }

    private bool ContainsWithTag() {
        foreach(GameObject obj in path) {
            if(obj.tag == "EndPath") 
                return true;
            
        }

        return false;
    }

    private void ShowPath(Color color,NavMeshPath path) {
        for(int i = 0;i<path.corners.Length - 1;i++) {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
        }
    }

    public override void OnTransitionEnd() {
        
    }
    
    public override void OnSwitchCamera() {
        
    }

    public override void OnStartCinematicEnd()
    {
    }

    #endregion
}
