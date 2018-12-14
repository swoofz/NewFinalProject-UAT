using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : Pawn {

    public GameObject kunia;    // Create a variable for our kunia prefab
    
    private GameObject bullets; // Create a variable to create a gameobject if not already created

    public override void ChangeAnimationState(string state) {
        Animator anim = GetComponent<Animator>();   // get our animator

        // if state is slide or throw or jumpatt or jumpthrow or glide
        if (state == "Slide" || state == "Throw" || state == "JumpAtt" || state == "JumpThrow" || state == "Glide") {
            anim.Play(state);                   // change state
        } else {                                // otherwise
            base.ChangeAnimationState(state);   // look at the base states
        }
    }

    public override bool IsGrounded() {
        // Ninja is grounded 
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
            if (child.name == "Kunai Location") {   // if one is name Kunai Location
                shotLocation = child.gameObject;    // set our shoot location
            }
        }

        GameObject clone = Instantiate(kunia, shotLocation.transform.position, shotLocation.transform.rotation);    // set our bullet in a gameobject
        clone.transform.parent = bullets.transform;                                                                 // child our bullet gameobject
    }

}
