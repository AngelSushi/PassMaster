using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcheryIA : MonoBehaviour {

    #region Variables

    public ArcheryController controller;
    public GameObject ammo;
    public GameObject targetTop;
    public bool hasGenArea;
    public bool chooseShifting;
    public bool shoot;
    public bool stopAmmo;
    public int speedAmmo;
    public float lastAngle;

    private TargetShifting shifting;
    private GameObject targetBottom;
    private GameObject instantiateAmmo;
    private Vector3 targetArea;
    private Vector3 direction;
    private float distance;

    public float angle;

  /** DISTANCE ZONE 
        * Zone jaune : Dmin = 0m ; Dmax = 0.2m
        * Zone rouge : Dmin = 0.2m ; Dmax = 0.4m
        * Zone bleue : Dmin = 0.4m ; Dmax = 0.6m
        * Zone noire : Dmin = 0.6m ; Dmax = 0.8m
        * Zone blanche : Dmin = 0.8m ; Dmax = 1m
    **/

    #endregion

    #region Unity's Functions

    private void Start() {    
        targetBottom = GameObject.FindGameObjectsWithTag("Target")[1]; 
        targetTop = GameObject.FindGameObjectsWithTag("Target")[0]; 
    }   

    private void Update() {

        if(!controller.begin && !controller.finish) {
            if(!chooseShifting) {
                int rand = Random.Range(0,2);
                
                Debug.Log("t: " + rand);

                if(rand == 0)
                    shifting = targetTop.GetComponent<TargetShifting>();
                else 
                    shifting = targetBottom.GetComponent<TargetShifting>();

                chooseShifting = true;                
            }
            
            if(shifting != null) {
                if(shifting.left) 
                    CalculateAngle(1.0f);
                else 
                    CalculateAngle(-1.0f);
            }
        }
    }

    #endregion

    #region IA Functions

    private void CalculateAngle(float positive) {
        if(!hasGenArea) {
            targetArea = GetTargetArea(shifting.gameObject);
            hasGenArea = true;
        }
                
        Vector3 lineOne = new Vector3(transform.position.x,transform.position.y,transform.position.z - 15) - transform.position;
        Vector3 lineTwo = targetArea - transform.position;
        targetArea = new Vector3(targetArea.x + shifting.speed * Time.deltaTime * positive,targetArea.y,targetArea.z);

        angle = Vector3.Angle(lineTwo,lineOne);

        Debug.DrawLine(transform.position,new Vector3(transform.position.x,transform.position.y,transform.position.z - 15   ),Color.magenta);
        Debug.DrawLine(transform.position,targetArea,Color.green);

        if(angle < 15.5f && lastAngle >= 15.5f && !shoot) 
            PrepareShoot();
        
        Shoot();

        lastAngle = angle;
    }

    private void PrepareShoot() {
        shoot = true;
        instantiateAmmo = Instantiate(ammo,new Vector3(transform.position.x -0.154f,transform.position.y + 0.264f,transform.position.z - 0.892f),Quaternion.Euler(0,0,0));
        instantiateAmmo.GetComponent<Ammo>().player = transform.gameObject;
        instantiateAmmo.GetComponent<Ammo>().controller = controller;
        Debug.Log("enter: " + transform.gameObject);

        direction = new Vector3(instantiateAmmo.transform.position.x,targetArea.y,targetArea.z - 100);
    }

    private void Shoot() {
        if(shoot && !stopAmmo) {          
            Debug.DrawLine(transform.position,direction,Color.red);
            Debug.DrawLine(transform.position,targetArea,Color.white);

            Debug.Log("instantiateAmmo: " + instantiateAmmo + " bot: " + transform.gameObject);
            if(instantiateAmmo != null) 
                instantiateAmmo.transform.position = Vector3.MoveTowards(instantiateAmmo.transform.position,direction,speedAmmo * Time.deltaTime);
            else
                shoot = false;
        }
    }

    private Vector3 GetTargetArea(GameObject target) {
            GameController.Difficulty difficulty = GameController.difficulty;
            int random = Random.Range(0,100);

            if(difficulty == GameController.Difficulty.EASY) { // Facile

                if(random >= 0 && random < 20)  // En dehors de la cible
                    return RandomPosition(1.05f,1.2f,target);
                else if(random >= 20 && random < 50)  // Zone blanche
                    return RandomPosition(0.8f,1f,target);
                else if(random >= 50 && random < 70)  // Zone noire
                    return RandomPosition(0.6f,0.8f,target);
                else if(random >= 70 && random < 85)  // Zone bleue
                    return RandomPosition(0.4f,0.6f,target);
                else if(random >= 85 && random < 95)  // Zone rouge
                    return RandomPosition(0.2f,0.4f,target);
                else if(random >= 95 && random < 100)  // Zone Jaune
                    return RandomPosition(0f,0.2f,target);
                
            }

            else if(difficulty == GameController.Difficulty.MEDIUM) { // Moyen
                if(random >= 0 && random < 15) // En dehors de la cible
                    return RandomPosition(1.05f,1.2f,target);
                else if(random >= 15 && random < 40) // Zone blanche
                    return RandomPosition(0.8f,1f,target);
                else if(random >= 40 && random < 65) // Zone noire
                    return RandomPosition(0.6f,0.8f,target);
                else if(random >= 65 && random < 85) // Zone bleue
                    return RandomPosition(0.4f,0.6f,target);
                else if(random >= 85 && random < 95) // Zone rouge
                    return RandomPosition(0.2f,0.4f,target);
                else if(random >= 95 && random < 100) // Zone jaune
                    return RandomPosition(0f,0.2f,target);
            }   

            else if(difficulty == GameController.Difficulty.HARD) { // Difficile

                if(random >= 0 && random < 5)  // En dehors de la cible
                    return RandomPosition(1.05f,1.2f,target);
                else if(random >= 5 && random < 15)  // Zone blanche
                    return RandomPosition(0.8f,1f,target);
                else if(random >= 15 && random < 30) // Zone noire
                    return RandomPosition(0.6f,0.8f,target);
                else if(random >= 30 && random < 55)  // Zone bleue
                    return RandomPosition(0.4f,0.6f,target);
                else if(random >= 55 && random < 80)  // Zone Rouge
                    return RandomPosition(0.2f,0.4f,target);
                else if(random >= 80 && random < 100)  // Zone Jaune
                    return RandomPosition(0f,0.2f,target);
            }   

            return Vector3.zero;   
    }

    private Vector3 RandomPosition(float dmin,float dmax,GameObject target) {
        distance = Random.Range(dmin,dmax);
        float angle = Random.Range(0,2f * Mathf.PI);
        Debug.Log("dmin: " + dmin + " dmax: " + dmax);
        float x = distance * Mathf.Cos(angle);
        float y = distance * Mathf.Sin(angle);
        float z = target.transform.position.z;

        Vector3 center = target.transform.GetChild(0).position;

        return new Vector3(center.x + x,center.y + y,z);

    }

    #endregion
}


