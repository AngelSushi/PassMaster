using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class HSIA : CoroutineSystem {
    #region Variables
    public GameObject testedObj;
    public HSController controller;
    public NavMeshAgent agent;
    public HSController.Roles role;
    public bool isDead;
    public bool isReady;
    public bool isHidden;
    public bool isSearching;
    public bool isFinding;
    public int floor;
    public GameObject targetFurniture;
    public GameObject searchingFurniture;
    private int actualRoom;
    private int adjacentRoom;
    private int percentageRoom;
    private int percentageHidden;
    private int percentageInSameRoom;
    private int percentageChangeRoom;
    private bool hasChangeRoomCheck;
    private int timeToChangeRoom;
    private int percentageSearchingRoom;
    private int percentageChangingSearchingRoom;
    private float timeToMove;
    private int percentageToMove;
    private int percentageToRotate;
    private int lastRoom;
    private GameObject lastFurniture;
    private bool lastSearching;
    private List<GameObject> searchingFurnitures;

    private bool hasMove;
    private Vector3 furniturePos = Vector3.zero;
    public bool follow;
    public GameObject targetSeeker;

    private Vector3 potPos = Vector3.zero;

    private bool succeedFollow;
    private bool followRotation;

    private bool hasPot;
    private int percentagePotDirection;
    private GameObject collideObj;

    public bool isDizzy;
    private float dizzyTimer;
    #endregion

        // HUNTER
    // 1. Selon la difficulté, le hunter génère une salle dans l'étage ou non d'un des seekers. - Fait a tester
    // 2. Une fois dans une salle il va fouiller tout les meubles de la salle
        // ==> Lorsqu'il a fini il regenère une nouvelle salle - fait a tester
        // ==>  A chaque changement de meuble et selon la difficulté il a plus ou moins de chance de changer complètement de salle en suivant le model 1 - fait a tester
    // 3. Si il croise un seeker alors il abandonne sa salle et son meuble et suit le seeker jusqu'à ce qu'il soit cacher
        // Si il fini par l'attraper , il check si les autres sont attrapés et regènre une salle
        // Si il l'attrape pas , il tourne  a droite puis a gauche pour voir sil le trouve et si non il regènre une salle 
// S'il na pas de room le faire errer

    // Quand il random une room ca doit pas etre égale a l'ancienne room qu'il a  random - fait 
    
        // Vérifier qu'il ne peut follow qu'une personne a la fois
        // une fois que le hunter a fini de chercher s'il y a un seeker dans le meuble ou il était ca le fait sortir

    // SEEKER

        // Il va random selon la difficulté une salle ou aller. Plus la difficulté est elevé plus il prend une salle loin du hunter. Le random doit se faire dans une fonction a part comme ça on a juste a rappeller la fonction - Fait
    // Une fois qu'il est caché dans un meuble il a plus ou moins de chance de changer de meuble pendant que le hunter fouille son meuble. Selon la difficulté il génère un meuble plus ou moins loin du hunter - Fait |   15% de chance de sortir s'il est caché et que le hunter cherche et que difficulté = facile | 40% de chance de sortir s'il est caché et que le hunter cherche et que difficulté = moyen |60% de chance de sortir s'il est caché et que le hunter cherche et que difficulté = difficile;
    // Si le hunter est dans la même salle que lui et selon la difficulté il a plus ou moins de chance de changer de meuble dans la même pièce. - fait Facile = 45% | moyen = 25% | difficilee = 10% 
    // Si lors d'un quelconque chemin le seeker est dans un le chemin du hunter alors ca lui fait son comportement pour se cacher au plus vite et ensuite une fois qu'il est caché si il a réussi il va re random un meuble ou se cacher
    // Selon la difficulté le seeker peut décider de changer de meuble durant n'importe quel moment de la partie (sauf quand il se cache du hunter) - fait
    // Selon la difficulté également il a plus ou moins de chance de laisser ouverte les portes de la salle ou il passe

// Tant qu'il n'a pas trouvé de meuble , le faire "errer"

// Facile :  80% de chance de random une salle au même étage que le hunter ; 20% pas au même étage | Moyen : 50% de chance de random une salle au même étage que le hunter ; 50% de chance de random une salle pas au même étage | DIfficile : 20% de chance de random une salle au même étage que le hunter ; 80% pas au même étage
    // => Fait a tester

// Il faut prendre en compte les autres ia | Facile = Meme piece et mm meuble | Moyen = Meme piece | Difficile = Pas meme piece - Fait a tester


            // Facile : 20% de chance de random la salle d'un des seekers ou une salle adjacente
            // Moyen : 45% de chance de random la salle d'un des seekers ou une salle adjacente
            // Difficile : 60% de chance de random la salle d'un des seekers ou une salle adjacente
            // Vérifier pour les salles d'une 1 furniture --> faire une liste de tt les meubles qu'il a fouiller dans une salle, et check si elle est égale au nombre de fourniture dans la salle si c le cas ca change de salle

// Faire en sorte qu'on puisse prendre des vases et lui lancer sur la gueule plutot que le comportement

    #region Unity's Functions
    void Start() {
       // GameController.difficulty = 2;

<<<<<<< HEAD
        switch(GameController.difficulty) {
=======
        switch(GameController.Instance.difficulty) {
>>>>>>> main
            case GameController.Difficulty.EASY:
                percentageRoom = 80; 
                percentageHidden = 15;
                percentageInSameRoom = 45;
                percentageChangeRoom = 55;
                percentageSearchingRoom = 20;
                percentageChangingSearchingRoom = 40;
                percentageToMove = 35;
                percentageToRotate = 35;
                percentagePotDirection = 35;
                timeToChangeRoom = 7;
                timeToMove = 1;
                break;

            case GameController.Difficulty.MEDIUM:
                percentageRoom = 50;
                percentageHidden = 40;
                percentageInSameRoom = 25;
                percentageChangeRoom = 30;
                percentageSearchingRoom = 45;
                percentageChangingSearchingRoom = 20;
                percentageToRotate = 65;
                percentageToMove = 55;
                percentagePotDirection = 65;
                timeToChangeRoom = 15;
                timeToMove = 0.7f;
                break;

            case GameController.Difficulty.HARD: 
                percentageRoom = 20;
                percentageHidden = 60;
                percentageInSameRoom = 10;
                percentageChangeRoom = 15;
                percentageSearchingRoom = 60;
                percentageChangingSearchingRoom = 10;
                percentageToMove = 75;
                percentageToRotate = 85;
                percentagePotDirection = 80;
                timeToMove = 0.5f;
                timeToChangeRoom = 25;
                break;
        }
    }
    
    void Update() {
        if(!controller.finish && !controller.begin && !controller.randomRoles) {
            if(!isReady) {
                if(role == HSController.Roles.SEEKER) 
                    isReady = true;
                if(role == HSController.Roles.HUNTER) {
                    RunDelayed(12f,() => {
                        isReady = true;
                    });
                }
            }

            if(isReady) {
                if(isDizzy) {
                    dizzyTimer += Time.deltaTime;

                    if(dizzyTimer >= 5f) {
                        isDizzy = false;
                        dizzyTimer = 0;
                    }
                }
                else {
                    if(transform.childCount == 4) 
                        Destroy(transform.GetChild(3).gameObject);
                    if(role == HSController.Roles.SEEKER) 
                    SeekerBehaviour();
                    if(role == HSController.Roles.HUNTER) {
                        HunterBehaviour();
                    }
                }
            }
        }
    }

    #endregion


    #region Seeker's Behaviour Functions
        private void SeekerBehaviour() {
            if(!isDead) {
                if(targetFurniture == null)
                    GenerateRoom();
                else {
                    NavMeshPath path = new NavMeshPath();
                    if(!isFinding) 
                        furniturePos = targetFurniture.transform.position;
                                
                    if(GetRoomByFurniture(targetFurniture).floor == 2)
                        furniturePos.y = 614; // Si le foor est 2
                    else 
                        furniturePos.y = 577;
                    
                    if(!isFinding) {
                        agent.CalculatePath(furniturePos,path);
                        Debug.Log("status: " + path.status);
                        agent.SetPath(path);
                        ShowPath(Color.yellow,path);
                    }
                // Debug.Log("x: " + furniturePos.x);

                    if(isFinding) {

                        if(!hasMove) {
                            hasMove = true;
                            if(potPos == Vector3.zero) { // Si il n'a toujours pas trouvé de pot ou aller
                                RunDelayed(timeToMove,() => {
                                    int random = Random.Range(0,100);

                                    if(random < /*percentageToMove*/ 100) {
                                        // Lui faire aller vers le pot le plus proche
                                        potPos = GetNearestPot();
                                    }

                                    hasMove = false;
                                });
                            }
                        }

                        if(potPos != Vector3.zero && !hasPot) {
                            NavMeshPath potPath = new NavMeshPath();
                            if(floor == 2)
                                potPos.y = 614; // Si le foor est 2
                            else 
                                potPos.y = 577;

                            agent.CalculatePath(potPos,potPath);
                            agent.SetPath(potPath);
                            ShowPath(Color.green,potPath);
                        }

                        if(hasPot) {
                            int random = Random.Range(0,100);

                            Vector3 throwPot = controller.GetHunter().transform.position - transform.position;
                            if(random < /*percentagePotDirection*/ 100) {
                                int side = Random.Range(0,2);

                               /* if(side == 0)
                                    throwPot.x -= 15;
                                else
                                    throwPot.x += 15;
                                */

                                Debug.Log("throooow object");
                                throwPot.y = -5;
                                collideObj.GetComponent<PotController>().isBot = true;
                                collideObj.GetComponent<PotController>().throwObject = true;
                                collideObj.GetComponent<PotController>().thrower = transform.gameObject;
                                collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                                collideObj.GetComponent<PotController>().potDirection = throwPot;

                            }
                            potPos = Vector3.zero;
                            hasPot = false;
                            hasMove = false;
                            collideObj = null;
                        }


                        return;
                    }
                    else 
                        potPos = Vector3.zero;

                    if(isHidden) {
                        transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled= false;

                        if(controller.IsHunterBot(controller.GetHunter())) { // Le chercheur est un bot
                            if(controller.GetHunter().GetComponent<HSIA>().isSearching && controller.GetHunter().GetComponent<HSIA>().searchingFurniture == targetFurniture) 
                                HiddenBehaviour();
                            else if(controller.GetHunter().GetComponent<HSIA>().actualRoom == actualRoom) 
                                SameRoomBehaviour();            
                            else 
                                ChangeRoomBehaviour();                                          
                        }
                        else { // Le chercheur est le joueur
                            if(controller.GetHunter().GetComponent<HS_Movement>().isSearching && controller.GetHunter().GetComponent<HS_Movement>().searchingFurniture == targetFurniture)
                                HiddenBehaviour();
                            else if(controller.GetHunter().GetComponent<HS_Movement>().actualRoom == actualRoom) 
                                SameRoomBehaviour();              
                            else  
                                ChangeRoomBehaviour();            
                        }
                    
                        
                    }
                    else { // Le bot n'est plus caché dans un meuble
                        transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled= true;
                    }
                }
            }
            else {
                transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
                transform.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.gameObject.GetComponent<HSIA>().enabled = false;
                transform.position = new Vector3(0,0,0);
            }
        }

        private Vector3 GetNearestPot() {
            List<float> distancesPot = new List<float>();

            foreach(GameObject pot in controller.pots) {
                float distance = Vector3.Distance(pot.transform.position,transform.position);
                distancesPot.Add(distance);
            }

            distancesPot.Sort();

            float correctDistance = distancesPot[0];

            foreach(GameObject pot in controller.pots) {
                float distance = Vector3.Distance(pot.transform.position,transform.position);
                if(distance == correctDistance) {
                    return pot.transform.position;

                }

            }

            return Vector3.zero;
            
        }
        private void ChangeRoomBehaviour() {
            if(isHidden && !hasChangeRoomCheck) {
                hasChangeRoomCheck = true;
                RunDelayed(timeToChangeRoom,() => {
                    int random = Random.Range(0,100);

                    if(random < percentageChangeRoom) {
                        isHidden = false;
                        targetFurniture = null;
                    }

                    hasChangeRoomCheck = false;
                });
            }
        }

        private void SameRoomBehaviour() {
            int random = Random.Range(0,100);

            if(random < percentageInSameRoom) { // Le bot sort de sa cachette alors que le hunter est dans sa pièce mais ne cherche pas dans son meuble
                isHidden = false;
                targetFurniture = null;
            }
        }
        private void HiddenBehaviour() {
            int random = Random.Range(0,100);

            if(random < percentageHidden) { // Le seeker sort du meuble pendant que le hunter cherche dans son meuble
                isHidden = false;
                targetFurniture = null;
            }
        }

        private void GenerateRoom() {
            int rand = Random.Range(0,100);
            int room = -1;

            if(rand < percentageRoom) { // Il génére une salle dans le même étage que le hunter
                Debug.Log("generate in floor");
                if(GetHunterFloor() == 1) 
                    room = Random.Range(5,8);
                else if(GetHunterFloor() == 2) 
                    room = Random.Range(1,5);
            }
            else { // IL génère une salle pas dans le même étage que le hunter
                Debug.Log("don't generate in floor");
                if(GetHunterFloor() == 1)
                    room = Random.Range(1,5);
                else if(GetHunterFloor() == 2)
                    room = Random.Range(5,8);
            }

            Room targetRoom = GetRoomWithIndex(room);
            int furniture = Random.Range(0,targetRoom.furnitures.Length);
            
            if(CheckValidityOfFurnitures(targetRoom.furnitures[furniture])) {
                targetFurniture = targetRoom.furnitures[furniture];
                room = targetRoom.index;
                adjacentRoom = targetRoom.adjacentRoom;
            }
        }

        private bool CheckValidityOfFurnitures(GameObject furniture) { // Cette fonction sert a ajuster si il y a plusieurs personnes dans la meme piece et/ou meuble et son la difficulté
<<<<<<< HEAD
            if(GameController.difficulty == GameController.Difficulty.EASY)
                return true;
            if(GameController.difficulty == GameController.Difficulty.MEDIUM) {
=======
            if(GameController.Instance.difficulty == GameController.Difficulty.EASY)
                return true;
            if(GameController.Instance.difficulty == GameController.Difficulty.MEDIUM) {
>>>>>>> main
                foreach(GameObject obj in controller.GetSeekersFurniture().Values) {
                    if(obj == furniture && GetKeyByValue(obj) != transform.gameObject) 
                        return false;                 
                }

                return true;
            }
<<<<<<< HEAD
            else if(GameController.difficulty == GameController.Difficulty.HARD) {
=======
            else if(GameController.Instance.difficulty == GameController.Difficulty.HARD) {
>>>>>>> main
                foreach(GameObject obj in controller.GetSeekersFurniture().Values) {
                    if(obj == furniture && GetKeyByValue(obj) != transform.gameObject) 
                        return false;      
                    if(GetRoomByFurniture(obj).index == GetRoomByFurniture(furniture).index && GetKeyByValue(obj) != transform.gameObject) 
                        return false;                          
                }

                return true;
            }

            return false;
        }

        private int GetHunterFloor() {
            if(controller.GetHunter().GetComponent<HS_Movement>() != null) 
                return controller.GetHunter().GetComponent<HS_Movement>().floor;
            else if(controller.GetHunter().GetComponent<HSIA>() != null) 
                return controller.GetHunter().GetComponent<HSIA>().floor;
            
            return -1;
        }

        private Room GetRoomByFurniture(GameObject furniture) {
            foreach(Room room in controller.rooms) {
                foreach(GameObject obj in room.furnitures) {
                    if(obj == furniture)
                        return room;
                }
            }

            return null;
        }

        private GameObject GetKeyByValue(GameObject value) {
            foreach(GameObject obj in controller.GetSeekersFurniture().Keys) {
                if(controller.GetSeekersFurniture()[obj] == value)
                    return obj;
            }

            return null;
        }

    #endregion

    #region Hunter's Behaviour Functions
        private void HunterBehaviour() {
            if(searchingFurniture == null) {
                RandomSearchingRoomBehaviour();
            }
            else {
                if(!isSearching) {
                    if(lastSearching) // Random le pourcentage de chance de quitter une salle alors qu'elle est pas full cherché
                        ChangingSearchingRoomBehaviour();                    
                    else {
                        if(!follow) {
                            NavMeshPath path = new NavMeshPath();
                            Vector3 furniturePos = searchingFurniture.transform.position;
                            if(GetRoomByFurniture(searchingFurniture).floor == 2)
                                furniturePos.y = 614; // Si le foor est 2
                            else 
                                furniturePos.y = 577;

                            agent.CalculatePath(searchingFurniture.transform.position,path);
                            Debug.Log("hunterStatus: " + path.status);
                            Debug.Log("posY: " + furniturePos.y);
                            FollowSeeker();
                        }
                        else {
                            NavMeshPath directionPath = new NavMeshPath();
                            Vector3 direction = targetSeeker.transform.position;
                            direction.z += 11;
                            Debug.Log("enterFollowing: " + targetSeeker);

                            if(controller.isSeekerBot(targetSeeker)) {
                                if(targetSeeker.GetComponent<HSIA>().floor == 2)
                                    direction.y = 614;
                                else 
                                    direction.y = 577;
                            }
                            else {
                                if(targetSeeker.GetComponent<HS_Movement>().floor == 2)
                                    direction.y = 614;
                                else 
                                    direction.y = 577;
                            }       
                            
                            Debug.Log("direction: " + direction.y);
                            agent.CalculatePath(direction,directionPath);
                            
                            Debug.Log("statusFollowing: " + directionPath.status);
                            agent.SetPath(directionPath);
                            ShowPath(Color.green,directionPath);
                            if(!succeedFollow) {
                                succeedFollow = true;
                                int random = Random.Range(0,100);
                                //Debug.Log("rotateRandom: " + random);

                                if(random < percentageToRotate) 
                                    followRotation = true;          
                                else 
                                    followRotation = false;   

                                RunDelayed(timeToMove,() => {
                                        succeedFollow = false;
                                });
                            }

                            if(followRotation) 
                                transform.GetChild(1).LookAt(targetSeeker.transform.position);       

                        }

                        
                    }
                }

                lastSearching = isSearching;
            }
            
        }

        private void FollowSeeker() {
            Vector3 hunterPos = transform.position;
            DrawSquare(hunterPos);
            foreach(GameObject seeker in controller.GetSeekers()) {
                if(seeker != null && !follow) {
                    Vector3 seekerPos = seeker.transform.position;

                    if( (int)hunterPos.z >=  (int)seekerPos.z &&  (int)(hunterPos.z - 15) <=  (int)seekerPos.z) {
                        if(seekerPos.x >= (hunterPos.x - 15) && seekerPos.x <= (hunterPos.x + 15) ) { // Le hunter a un seeker dans son champ de vision il le suit
                            Debug.Log("is in hunter vision");
                            if(seeker.GetComponent<MeshRenderer>().enabled) { // Si le joueur n'est pas caché
                                if(controller.isSeekerBot(seeker)) {
                                    if(seeker.GetComponent<HSIA>().actualRoom == actualRoom) { 
                                        seeker.GetComponent<HSIA>().isFinding = true;
                                        targetSeeker = seeker;
                                        follow = true;
                                    }
                                }
                                else {
                                    if(seeker.GetComponent<HS_Movement>().actualRoom == actualRoom) {
                                        targetSeeker = seeker;
                                        follow = true;
                                    } 
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void DrawSquare(Vector3 hunterPos) {
            Vector3 one = new Vector3(hunterPos.x + 15,hunterPos.y,hunterPos.z);
            Vector3 two = new Vector3(hunterPos.x + 15,hunterPos.y,hunterPos.z - 15);
            Vector3 three = new Vector3(hunterPos.x - 15,hunterPos.y,hunterPos.z - 15);    
            Vector3 four = new Vector3(hunterPos.x - 15,hunterPos.y,hunterPos.z);

            Debug.DrawLine(one,two,Color.magenta);
            Debug.DrawLine(two,three,Color.magenta);
            Debug.DrawLine(four,three,Color.magenta);
            Debug.DrawLine(one,four,Color.magenta);
           
        }

        private void ChangingSearchingRoomBehaviour() {
            int random = Random.Range(0,100);
            if(!searchingFurnitures.Contains(searchingFurniture))
                searchingFurnitures.Add(searchingFurniture);

            if(random < percentageChangingSearchingRoom)
                RandomSearchingRoomBehaviour();
            else {
                if(searchingFurnitures.Count == GetRoomWithIndex(actualRoom).furnitures.Length) {
                    searchingFurnitures.Clear();
                    RandomSearchingRoomBehaviour();
                }
                else 
                    RandomFurnituresInRoom(actualRoom);             
            }
        }
        private void RandomSearchingRoomBehaviour() {
            int random = Random.Range(0,100);
            int roomIndex = -1;
            int seekerIndex = Random.Range(0,3);
            GameObject seeker = controller.GetSeekers()[seekerIndex];

            if(random < percentageSearchingRoom) { // Le hunter génère la salle d'un seeker ou son adjacente
                
                if(controller.isSeekerBot(seeker)) {
                    int randomRoom = Random.Range(0,2);
                    if(randomRoom == 0)
                        roomIndex = seeker.GetComponent<HSIA>().actualRoom;
                    else
                        roomIndex = seeker.GetComponent<HSIA>().adjacentRoom;

                    if(roomIndex != lastRoom) 
                        RandomFurnituresInRoom(roomIndex);                                     
                }
                else {
                    int randomRoom = Random.Range(0,2);
                    if(randomRoom == 0)
                        roomIndex = seeker.GetComponent<HS_Movement>().actualRoom;
                    else
                        roomIndex = seeker.GetComponent<HS_Movement>().adjacentRoom;

                    if(roomIndex != lastRoom) 
                        RandomFurnituresInRoom(roomIndex);                      
                    
                }
            }
            else { // Le hunter génère une salle autre que celle du seeker ou celle adjacente au seeker
                List<Room> notSeekerRoom = new List<Room>();

                foreach(Room room in controller.rooms) {
                    notSeekerRoom.Add(room);
                }

                DeleteSeekerRoom(seeker,notSeekerRoom);
                int roomRandom = Random.Range(1,6);
                RandomFurnituresInRoom(roomRandom,notSeekerRoom);  
            }


            lastRoom = roomIndex;
        }

        private void DeleteSeekerRoom(GameObject seeker,List<Room> notSeekerRoom) {
            if(controller.isSeekerBot(seeker)) {
                foreach(Room r in controller.rooms) {
                    if(r.index == seeker.GetComponent<HSIA>().actualRoom || r.adjacentRoom == seeker.GetComponent<HSIA>().adjacentRoom) {
                        notSeekerRoom.Remove(r);
                    }
                }
            }
            else {
                foreach(Room r in controller.rooms) {
                    if(r.index == seeker.GetComponent<HS_Movement>().actualRoom || r.adjacentRoom == seeker.GetComponent<HS_Movement>().adjacentRoom) {
                        notSeekerRoom.Remove(r);
                    }
                }
            }
        }

        private void RandomFurnituresInRoom(int room) {
            int random = Random.Range(0,GetRoomWithIndex(room).furnitures.Length);
            GameObject furniture = GetRoomWithIndex(room).furnitures[random];

            if(furniture != lastFurniture)
                searchingFurniture = furniture;

            lastFurniture = furniture;
        }

        private void RandomFurnituresInRoom(int room,List<Room> rooms) {
            int random = Random.Range(0,GetRoomWithIndex(room,rooms).furnitures.Length);
            GameObject furniture = GetRoomWithIndex(room).furnitures[random];

            if(furniture != lastFurniture)
                searchingFurniture = furniture;

            lastFurniture = furniture;
        }


    #endregion

    #region Common Functions
        private void ShowPath(Color color,NavMeshPath path) {
            for(int i = 0;i<path.corners.Length - 1;i++) {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
            }
        }

        private Room GetRoomWithIndex(int index) {
            foreach(Room room in controller.rooms) {
                if(room.index == index)
                    return room;
            }

            return null;
        }

        private Room GetRoomWithIndex(int index,List<Room> rooms) {
            foreach(Room room in rooms) {
                if(room.index == index)
                    return room;
            }

            return null;
        }
    
    #endregion

    #region Collisions Functions
        private void OnTriggerEnter(Collider hit) {
            if(hit.gameObject.tag == "Room") {
                actualRoom = hit.gameObject.GetComponent<Room>().index;
                adjacentRoom = hit.gameObject.GetComponent<Room>().adjacentRoom;
                floor = hit.gameObject.GetComponent<Room>().floor;
            }
        }

        private void OnTriggerExit(Collider hit) {
            if(hit.gameObject.tag == "Room") {
                actualRoom = 0;
                adjacentRoom = 0;
            }
        }

        private void OnCollisionEnter(Collision hit) {
            if(hit.gameObject.tag == "Pot" && isFinding) {
                hit.gameObject.transform.parent = transform;
                hit.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                hit.gameObject.transform.localPosition = new Vector3(0.008f,1.732f,-0.030f);
                hasPot = true;
                collideObj = hit.gameObject;
                transform.LookAt(controller.GetHunter().transform.position);
            }
        }

    #endregion
}
