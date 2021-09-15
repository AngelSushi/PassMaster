using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameController : MonoBehaviour {

    private bool hasClick;

    // Animation de saut quand le joueur gagne - Optionnel - Fait sur KeyBall juste a copier
    // Faire rotation du bot dans FindPath - Fait
    // Fair en sorte qu'on puisse pas ouvrir le menu pause dans le bouton caméra  - Fait
    // FindPath erreur lorsque l'on gagne   et full bug a revoir  - Fait
    //menu shop - mettre le bouton accéder a droite - fait a vérifier
    //menu pause - appelé options - Fait juste faire le bouton quitter avec le bouton reprendre sur le menu principal
    // revoir ui main - Fait
    // Probleme d'index lors de la sortie du menu caméra sur l'ui principal - Fait
    // Vérifier si archery marche avec difficulté facile et moyenne - Fait 
    // Pb au tour 2 avec les tours qui se décremente - Fait

    
    // Problème sur l'hud du classement - A vérifier pb de rank en plus- 

    // Pour après faire un podium pour la fin de chaque mini jeu 
    // Pb position caméra tour 2 - En cours
    // Parfois quand ca va a gauche ca va en diagonale - FindPath
    // IA Archery bug en facile parfois bot ne tire plus du tout - Régler problème entre vitesse de la cible et vitesse de la balle a modifier

    
    // pb placement texte dialogue en build
    // pb placement icon reward en build


    public void RunMiniGame(bool training) {
        StartCoroutine(LoadScene(training));
    }

    private IEnumerator LoadScene(bool training) {        
        if(!hasClick) {
            AsyncOperation operation = SceneManager.LoadSceneAsync(GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().actualMiniGame.minigameName,LoadSceneMode.Additive);
            hasClick = true;

            while(!operation.isDone) {
                yield return null;
            }

            SceneManager.UnloadSceneAsync("MiniGameLabel");
            
            if(training) {
                switch(GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().actualMiniGame.minigameName) {
                    case "FindPath":
                        GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<FP_Controller>().isTraining = true;
                        break;

                    case "Archery":
                        GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<ArcheryController>().isTraining = true;
                        break;

                    case "KeyBall":
                        GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<KBController>().isTraining = true;
                        break;

                    case "HideAndSeek":
                        GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<HSController>().isTraining = true;
                        break;
                }
            }

            
        }

    }
}
