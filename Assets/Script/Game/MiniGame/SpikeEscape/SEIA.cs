using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEIA : CoroutineSystem {

    public Rigidbody rb;
    public float jumpForce;

    public bool isJumping;
    public bool isSneaking;

    private float suceedActionPercentage;
    private GameObject lastSpike;
    private SEController controller;

    void Start() {
<<<<<<< HEAD
        switch(GameController.difficulty) {
=======
        switch(GameController.Instance.difficulty) {
>>>>>>> main
            case GameController.Difficulty.EASY:
                suceedActionPercentage = 65;
                break;

            case GameController.Difficulty.MEDIUM:
                suceedActionPercentage = 80;
                break;

            case GameController.Difficulty.HARD:
                suceedActionPercentage = 90;
                break;
        }

        controller = SEController.instance;
    }

    void Update() {
        if(!controller.begin && !controller.finish) {
            if(GetNextSpike() != null && GetNextSpike() != lastSpike) {
                Vector3 position = transform.position;
                lastSpike = GetNextSpike();
                Vector3 spikePosition = lastSpike.transform.position;
                
                int random = Random.Range(0,100);

                if(random < suceedActionPercentage) {
                    Debug.Log("enterSucceed");
                    if((int)spikePosition.y < 150) {
                        if((int)position.z - 20 >= (int)spikePosition.z && !isJumping) {
                            Jump();
                            isJumping = true;
                        }
                    }
                    else { // Sneak
                        Debug.Log("need to sneak");
                        if((int)position.z - 10 >= (int)spikePosition.z && !isSneaking && !isJumping) {
                            Debug.Log("sneak");
                            Sneak(2.82f);
                            isSneaking = true;

                            RunDelayed(0.3f,() => {
                                Sneak(5.125f);
                                isSneaking = false;
                            });
                        }
                    }
                }
            }
        }

    }

    private GameObject GetNextSpike() {
        for(int i = 0;i<30;i++) {
            foreach(GameObject spike in GameObject.FindGameObjectsWithTag("Spike")) {
                if((int)spike.transform.position.z == (int)(transform.position.z - i) && spike.transform.position.z < transform.position.z) {
                    return spike;
                }
            }
        }

        return null;
    }

    private void Jump() {
        rb.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
    }

    private void Sneak(float sneak) {
        transform.localScale = new Vector3(transform.localScale.x,sneak,transform.localScale.z);    
    }

    private void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag == "Sol") {
            if(isJumping)
                isJumping = false;
        }
    }

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Lava") {
            transform.gameObject.SetActive(false);
            if(!controller.deadPlayers.Contains(transform.gameObject))
                controller.deadPlayers.Add(transform.gameObject);
        }
    } 

}
