using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Pawn {

    public GameObject bullet, muzzle;       // Create variables for our bullet instantiation

    private GameObject bullets;             // Create a variable to create a gameobject if not already created

    public override void ChangeAnimationState(string state) {
        Animator anim = GetComponent<Animator>();   // get our animator

        // if state is slide or shoot or jumpAtt or jumpShoot or runShoot
        if (state == "Slide" || state == "Shoot" || state == "JumpAtt" || state == "JumpShoot" || state == "RunShoot") {
            anim.Play(state);                   // set animation state
        } else {                                // otherwise
            base.ChangeAnimationState(state);   // lose at base state
        }
    }

    public override bool IsGrounded() {
        // Robot is grounded 
        Transform feet = transform.GetChild(5);                                     // getting our feet gameobject transform
        RaycastHit2D hit = Physics2D.Raycast(feet.position, Vector2.down, .5f);     // looking down from our feet transform to see if collider with anything
        if (hit.collider != null) {                                                 // if collider with something
            if (hit.collider.tag == "Ground" || hit.collider.tag == "Platform") {   // check if it is tagged with ground or platform
                if (hit.collider.tag == "Platform" && Input.GetKey(KeyCode.S)) {    // if tagged with platform and hold S
                    return false;                                                   // return false
                }                                                                   // otherwise
                return true;                                                        // return true
            }
        }

        return base.IsGrounded();   // if didnt return anything check the base and return false
    }

    public override void Shoot() {
        GameObject shotLocation = null;             // set a gameobject intially to null 
        if (GameObject.Find("Bullets") == null) {   // if there is no Bullets gameobject
            bullets = new GameObject("Bullets");    // create a Bullets gameobject
        } else {                                    // otherwise
            bullets = GameObject.Find("Bullets");   // set to Bullets gameobject
        }

        // check for all child transforms
        foreach (Transform child in transform) {
            if (child.name == "Shoot") {            // if one is name Shoot
                shotLocation = child.gameObject;    // set our shoot location
            }
        }

        // First set our muzzle in a gameobject and parent it to our gameobject then destory after 0.4 seconds
        // Second set our bullect in a gameobject and parent to our gameobject
        GameObject clone = Instantiate(muzzle, shotLocation.transform.position, shotLocation.transform.rotation);
        clone.transform.parent = bullets.transform;
        Destroy(clone, 0.4f);
        clone = Instantiate(bullet, shotLocation.transform.position, shotLocation.transform.rotation);
        clone.transform.parent = bullets.transform;
    }

}
