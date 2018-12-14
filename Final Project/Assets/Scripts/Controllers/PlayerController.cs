using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller {

    private CharacterState playerState;     // Create a varaible to hold our state
    private float direction;                // Create a varaible to store the direction
    private Rigidbody2D rb;                 // Create a varaible to store our Rigibody component
    private int lives;                      // Creat a variable to store our lives

    private bool attacking;         // Create a variable to check if attacking
    private float cnt, cntD;        // Create variable for timer (count, countdown)
    private float attDamage;        // Create a variable for our damage
    private Pawn target;            // Create a variable to get a target
    private int hitDirection;       // Create a variable to get our direction that we hit
    private bool isGrounded;        // Create a variable to check if on the ground
    private int jumpCount;          // Create a variable to limit our jumps
    private bool shoot;             // Create a variable to check if we are shooting


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();           // Get our rigibdody into our variable
        lives = GameManager.instance.playerLives;   // Get our lives
        cnt = 0.35f;                                // set our timer
    }
	
	// Update is called once per frame
	void Update () {
        // Set our direction
        direction = Input.GetAxis("Horizontal");

        if (pawn != null) {                                     // if have a pawn set
            isGrounded = pawn.IsGrounded();                     // check if on the ground
        } else {                                                // otherwise
            pawn = gameObject.GetComponentInChildren<Pawn>();   // find our pawn
        }


        // if ground set turn off gravity else turn it on
        if (isGrounded) {
            rb.gravityScale = 0;
            if (rb.velocity.y < 0) {        // if going down
                rb.velocity = Vector2.zero; // don't go down
            }
            jumpCount = pawn.jumps; // set our jumps
        } else {
            rb.gravityScale = 1;

            if (rb.velocity.y < -20f) {
                rb.velocity = new Vector2(rb.velocity.x, -20f); // max fall speed
            }
        }

        if (jumpCount > 0) {    // if can jump
            Jump();             // Jump
        }

        // set our attack if press mouse button 1 or 2
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) {
            attacking = true;
            cntD = cnt;         // set timer so cant spawn the attack buttons

            if (Input.GetKeyDown(KeyCode.Mouse1) && !pawn.GetComponent<Knight>()) { // as long as this is not a knight we can shoot
                pawn.Shoot();
                shoot = true;
            }

            if (pawn.GetComponent<Knight>()) {      // TODO later
                // do a charge Attack when made
            }
        }

        if (attacking) {    // if we are attack
            AttackTimer();  // run timer to attack again after
        }

        // as long as there is a pawn we can change our state and move
        if (pawn != null) {
            playerState = ChangeState();
            pawn.ChangeAnimationState(playerState.ToString());
            GetHitDirection();
            pawn.MoveDirection(direction);
        }
    }

    // Change our character state
    CharacterState ChangeState() {
        if (direction == 0) {                               // idle if not moving
            playerState = CharacterState.Idle;
        }

        if (direction != 0) {                               // run is moving
            playerState = CharacterState.Run;
        }

        if (attacking) {                                    // attack if attacking
            playerState = CharacterState.Attack;

            if (shoot) {                                    // shoot if attacking with shoot
                playerState = CharacterState.Shoot;
                if (pawn.GetComponent<Ninja>()) {           // or throw if ninja
                    playerState = CharacterState.Throw;
                }
            }
        }

        if (!isGrounded) {                                  // jump if not grounded
            playerState = CharacterState.Jump;

            if (attacking) {                                // same attack things just if not grounded
                playerState = CharacterState.JumpAtt;
                if (pawn.GetComponent<Adverturer>()) {
                    playerState = CharacterState.Attack;
                }

                if (shoot) {
                    playerState = CharacterState.Shoot;
                    if (pawn.GetComponent<Ninja>()) {
                        playerState = CharacterState.Throw;
                    }
                }
            }
        }

        return playerState;                                 // return our state
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (attacking) {                                    // if attack find what we are attack and do damage to it if it has a pawn component attach to it
            string hitTarget = collision.gameObject.name;
            attDamage = pawn.Attack();

            if (collision.gameObject.transform.parent != null) {
                if (collision.gameObject.transform.parent.GetComponent<Pawn>() != null) {
                    target = collision.gameObject.transform.parent.GetComponent<Pawn>();    // set the target we are going to hit after we found it
                }
            }

            // depending on where we hit the target will have different action happen to the target
            if (hitTarget == "Head" || hitTarget == "Body" || hitTarget == "Legs") {
                if (target != null) {
                    GameManager.instance.AIDamageTaken += attDamage;
                    target.TakeDamage(attDamage, hitDirection, hitTarget);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        // Dies and reset values
        if (other.gameObject.tag == "Boundary") {
            lives -= 1;
            GameManager.instance.playerLives = lives;
            pawn.damagePercentage = 0;
            GameManager.instance.playerDamageTaken = 0;
            rb.velocity = Vector2.zero;
            if (lives < 0 || GameManager.instance.playerLives < 0) {
                Destroy(gameObject);
            } else {
                transform.position = new Vector3(0, 30, 0);
            }
        }
    }

    void GetHitDirection() {
        // finds our the direction at with we hit
        if (direction > 0) {
            hitDirection = 1;
        }

        if (direction < 0) {
            hitDirection = -1;
        }
    }

    void Jump() {
        // does our jump and shows animation
        if (Input.GetKeyDown(KeyCode.Space)) {  // Jump Up
            pawn.Jump(1);
            jumpCount -= 1;
            pawn.ChangeAnimationState("Idle");
            pawn.ChangeAnimationState("Jump");
        }
    }

    void AttackTimer() {
        // time inbetween attacks
        if (pawn.IsGrounded()) {
            direction = 0;
        }
        cntD -= Time.deltaTime;
        if (cntD < 0) {
            attacking = false;
            shoot = false;
        }
    }
}
