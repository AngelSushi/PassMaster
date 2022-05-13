using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ButtonController : MonoBehaviour {

    public int menu;

    public GameObject[] buttons = new GameObject[3];
    public GameObject shopParent;
    public GameObject shopParentList;
    public GameObject minigame;

    public List<ShopMenu> shopList = new List<ShopMenu>();
    public List<Material> stickers = new List<Material>();

    private string menuName;

    public GameObject ball;
    public GameObject kart;

    private bool hasChangeBarValue;

    public static bool relaunch;
    public GameObject relaunchObj;
    public AudioSource start;

    private bool hasShowMenu;

    void Update() {
        if(hasShowMenu) 
            ManageMenu();
    }

    public void OnClickButton(int button) {
        switch(menu) {
            case 0: // Premier Menu
                if(button == 0)  {// A cliquer sur le bouton jeu de plateau
                    menu = 1;
                    menuName = "JDP";
                }
            
                if(button == 1) { // A cliquer sur le bouton Mini-Jeux
                    menu = 1;
                    menuName = "MJ";
                }

                if(button == 2)  // A cliquer sur le bouton Boutique
                    menu = 2;
                
                break;

            case 1: // Menu Difficulté
            
                GameController.difficulty = button == 0 ? GameController.Difficulty.EASY : button == 1 ? GameController.Difficulty.MEDIUM : button == 2 ? GameController.Difficulty.HARD : GameController.Difficulty.EASY;

                if(menuName == "JDP") 
                    menu = 3;
                   // SceneManager.LoadScene("Main",LoadSceneMode.Single);

                if(menuName == "MJ") {
                    foreach(GameObject btn in buttons) 
                        btn.SetActive(false);
                        
                    minigame.SetActive(true);
                }

                break;

            case 3:  // Choix des personnages
                SceneManager.LoadScene("Main",LoadSceneMode.Single);
                break;    
   
        }
    }
    public void ShopHover(int index) {
        shopParent.transform.GetChild(4).GetChild(0).GetChild(index).GetChild(0).gameObject.SetActive(true);
        shopParent.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = shopList[index].description;

        if(index == 0 || index == 1 || index == 2 || index == 3) { // Ballon
            kart.SetActive(false);
            ball.SetActive(true);
        }

        else { // Kart
            ball.SetActive(false);
            kart.SetActive(true);
            Material[] mats = {kart.GetComponent<MeshRenderer>().materials[0],kart.GetComponent<MeshRenderer>().materials[1],stickers[index-4]};

            kart.GetComponent<MeshRenderer>().materials = mats;
        }
    }

    public void ShopHoverQuit(int index) {
        ball.SetActive(false);
        kart.SetActive(false);

        shopParent.transform.GetChild(4).GetChild(0).GetChild(index).GetChild(0).gameObject.SetActive(false);
        shopParent.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "";
    }

    private void ManageMenu() {
        if(menu == 0) {
            shopParent.transform.parent.GetChild(0).gameObject.SetActive(true);
            shopParent.transform.parent.GetChild(1).gameObject.SetActive(true);
            shopParent.transform.parent.GetChild(8).gameObject.SetActive(true);

            shopParent.transform.parent.GetChild(7).gameObject.SetActive(false);
            if(hasChangeBarValue) hasChangeBarValue = false;

            foreach(GameObject button in buttons) 
                button.SetActive(true);

            buttons[0].transform.GetChild(0).gameObject.GetComponent<Text>().text = "Plateau";
            buttons[1].transform.GetChild(0).gameObject.GetComponent<Text>().text = "Mini-Jeux";
            buttons[2].transform.GetChild(0).gameObject.GetComponent<Text>().text = "Boutique";

            if(relaunch) 
                relaunchObj.SetActive(true);
        }

        if(menu == 1) {
            shopParent.transform.parent.GetChild(7).gameObject.SetActive(true);
            if(hasChangeBarValue) hasChangeBarValue = false;

            buttons[0].transform.GetChild(0).gameObject.GetComponent<Text>().text = "Facile";
            buttons[1].transform.GetChild(0).gameObject.GetComponent<Text>().text = "Moyen";
            buttons[2].transform.GetChild(0).gameObject.GetComponent<Text>().text = "Difficile";
        }

        if(menu == 2) {
            shopParent.transform.parent.GetChild(7).gameObject.SetActive(true);
            
            foreach(GameObject button in buttons) 
                button.SetActive(false);

            shopParent.SetActive(true);

            if(!hasChangeBarValue) {
                shopParent.transform.GetChild(5).GetComponent<Scrollbar>().value = 1;
                hasChangeBarValue = true;
            }

            shopParent.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = "" + MoneyController.couronnes;
            shopParent.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text = "" + MoneyController.coins;

            for(int i = 0;i<shopParent.transform.GetChild(4).GetChild(0).childCount;i++) {
                if(MoneyController.couronnes < shopList[i].couronnes || MoneyController.coins < shopList[i].coins) 
                    shopParent.transform.GetChild(4).GetChild(0).GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
        }

        if(menu == 3) {
            foreach(GameObject btn in buttons) 
                btn.SetActive(false);
            
            shopParent.transform.parent.GetChild(10).gameObject.SetActive(true);
            
        }  
    }

    public void LaunchMiniGame(int minigame) {
        switch(minigame) {
            case 0: // Archery
                StartCoroutine(LoadMiniGame("Archery"));
                break;

            case 1: // findPath
                StartCoroutine(LoadMiniGame("FindPath"));
                break;

            case 2: // KeyBall
                StartCoroutine(LoadMiniGame("KeyBall"));
                break;

            case 3: // HideAndSeek
                StartCoroutine(LoadMiniGame("HideAndSeek"));
                break;
        }
    }

    private IEnumerator LoadMiniGame(string sceneName) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);

        while(!operation.isDone) {
            yield return null;
        }
        
        switch(sceneName) {
            case "FindPath":
                GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<FP_Controller>().runSinceMenu = true;
                break;

            case "Archery":
                GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<ArcheryController>().runSinceMenu = true;
                break;

            case "KeyBall":
                GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<KBController>().runSinceMenu = true;
                break;
                
            case "HideAndSeek":
                GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<HSController>().runSinceMenu = true;
                break;
        }

        
        ChangeStateScene("MainMenu",false);
            
    }
    public void BackButton() {
        switch(menu) {
            case 1:
                minigame.SetActive(false);
                menu = 0;
                break;
            case 2:
                shopParent.SetActive(false);
                menu = 0;
                break;
            case 3:
                minigame.SetActive(false);
                menu = 0;
                break;        
        }
    }

    public void ChangeStateScene(string sceneName,bool state) {
        GameObject[] objects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();

        foreach(GameObject obj in objects) {       
            obj.SetActive(state);
        }
    }

    public void OnShowMenu(InputAction.CallbackContext e) {
        if(e.started && !hasShowMenu) {
            hasShowMenu = true;
            start.Play();
            shopParent.transform.parent.parent.GetChild(1).gameObject.SetActive(false);
            shopParent.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
        }   
    }

    public void RelaunchGame() {
        GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().ChangeStateScene("Main",true);
        SceneManager.UnloadSceneAsync("MainMenu");
    }
}
