using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Pawn {

    // Note:: Hard Hitter cause not much animations to work with when comes to attack

    public override void ChangeAnimationState(string state) {
        Animator anim = GetComponent<Animator>();   // get our animator

        if (state == "JumpAtt" ) {              // if state is jumpAtt
            anim.Play(state);                   // change animation state
        } else {                                // otherwise
            base.ChangeAnimationState(state);   // look at the base states
        }
    }

    public override bool IsGrounded() {
        // Knight is grounded 
        Transform feet = transform.GetChild(4);                                     // getting our feet gameobject transform
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

    public override float Attack() {
        // TODO:: Add a charge Attack after add charge attack animation
        return attackDamage;    // return our attack damage
    }
}
