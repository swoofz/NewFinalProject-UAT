using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class Pawn : MonoBehaviour {

    public float speed;         // Create a variable to controller character's speed
    public float jumpForce;     // Create a variable to controller character's jumpForce
    public float attackDamage;  // Create a variable for character attack damage
    public int jumps;       // Create a variable for how many jumps and character can do

    [HideInInspector] public float damagePercentage;    // Create a varaible for character damage over time multipled


    public virtual void MoveDirection(float direction) {
        // Move right or left
        GameObject target = transform.parent.gameObject;                                    // get the gameobject want to move
        target.transform.position += Vector3.right * direction * speed * Time.deltaTime;    // move our gameobject 
        ChangeSpriteDirection(direction);                                                   // change the direction our spite is facing based on direction going
    }

    public virtual void ChangeSpriteDirection(float direction) {
        if (direction != 0) {                                       // if have a direction
            if (direction > 0) {                                    // if it greater than zero
                transform.rotation = Quaternion.Euler(0, 0, 0);     // rotation is 0
            } else {                                                // otherwise
                transform.rotation = Quaternion.Euler(0, 180, 0);   // rotation is 180 (or flipX)
            }
        }
    }

    public virtual void Jump(float jumpDirection) {
        Rigidbody2D rb = transform.parent.GetComponent<Rigidbody2D>();  // get our parent's rigidbody
        rb.velocity = Vector3.up * jumpDirection * jumpForce;           // add an upward force
    }

    public virtual bool IsGrounded() {
        // Find what makes this true for each pawn
        return false;
    }

    public virtual void ChangeAnimationState(string state) {
        // Handle extra animations in spefic component that inheriet from Pawn
        Animator anim = GetComponent<Animator>();   // get our animator

        // if state is idle or run or jump or attack
        if (state == "Idle" || state == "Run" || state == "Jump" || state == "Attack") {
            anim.Play(state);   // change state
        }
                                // else do nothing
    }

    public virtual float Attack() {
        // Return damage

        return attackDamage;
    }

    public virtual void TakeDamage(float damage, float direction, string hitSpot) {
        // Call to take damage with increase over time of getting 
        Rigidbody2D rb = transform.parent.GetComponent<Rigidbody2D>();  // get our parent's rigidbody
        Vector2 forceDirection = Vector2.zero;                          // set an intial forceDirection
        damagePercentage += damage;                                     // add damage to our damagePercentage every time hit


        if (hitSpot == "Head") {                                                // if hit head
            forceDirection = new Vector2(0.8f * direction, -1f) * damage;       // set force direction with down force
            if (IsGrounded()) {                                                 // if on the ground
                forceDirection = new Vector2(0.8f, .2f) * direction * damage;   // set force direction with a little bit or up force
            }
        }

        if (hitSpot == "Body") {                                        // if hit body
            forceDirection = new Vector2(1f, .2f) * direction * damage; // set force mostly along the x-axis
        }
        
        if (hitSpot == "Legs") {                                            // if hit legs
            forceDirection = new Vector2(0.8f * direction, 1f) * damage;    // set mostly a upward force
        }

        rb.AddForce(forceDirection * damagePercentage); // apply force with the damage that pawn has taken

        // For now every pawn take damage the same
    }

    public virtual void Shoot() {
        // For pawn that have shoot or throw
    }

}
