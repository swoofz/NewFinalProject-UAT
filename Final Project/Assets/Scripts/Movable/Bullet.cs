using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float bulletSpeed;   // Create a variable to set the bullet speed
    public float damage;        // Create a variable to set how much damage a bullet does


	// Update is called once per frame
	void Update () {
        transform.position += transform.right * bulletSpeed * Time.deltaTime;   // when made move the bullet in it right direction
	}

    void OnTriggerEnter2D(Collider2D other) {
        string hitTarget = other.gameObject.name;       // setting what we hit in a variable for easy acess

        // looking to see if hit a something with a pawn component
        if (other.gameObject.transform.parent != null) {
            if (other.gameObject.transform.parent.GetComponent<Pawn>() != null) {
                Pawn target = other.gameObject.transform.parent.GetComponent<Pawn>();   // if found set as target

                if (hitTarget == "Head" || hitTarget == "Body" || hitTarget == "Legs") { // check what part of our target was hit
                    if (target != null) {
                        target.TakeDamage(damage, transform.right.x, hitTarget);        // and take damage

                        // updating values in gameManager for the Fight UI numbers
                        if (target.transform.parent.tag == "Player") {
                            GameManager.instance.playerDamageTaken += damage;
                        }
                        if (target.transform.parent.tag == "AI") {
                            GameManager.instance.AIDamageTaken += damage;
                        }

                    }
                    Destroy(gameObject);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        // making sure our bullets don't get destory when exit our AI collider
        if (other.gameObject.tag != "AI") { 
            Destroy(gameObject);
        }
    }
}
