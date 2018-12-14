using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]    // every component must have a CircleCollider2D component
public class AIController : Controller {

    private CharacterState AiState;         // Check track of state
    private string aiState;                 // get our state into string form
    private CircleCollider2D circle;        // get our circle collider
    private Rigidbody2D rb;                 // get our rigidbody
    private Pawn hitTarget;                 // get the target we hit
    private int lives;                      // get our lives
    private float attDamage;                // get our damage that we give
    private bool isGrounded;                // check if on the ground
    private GameObject target;              // get our target that we are fighting
    private float jumpCount;                // get how many time can jump
    private float cnt, cntD;                // timer values (count, countdown)
    private float hitDirection;             // get our the direction we hit the target
    private float jumpTimer;                // set a timer for jumps
    private float direction;                // get the direction in switch we will move

    // Ai State Varaibles
    private enum Options { Idle, GoTO, Attack }     // different states that will go through to decide our actions
    private Options option;                         // get our action states
    private string opt;                             // set our action states into a string
    private bool detect, canSee;                    // checking values to change our action states
    private bool attacking, shoot;                  // checking values to change what our action states can do

	// Use this for initialization
	void Start () {
        cnt = 0.8f;                             // set timer
        jumpTimer = 1f;                         // set timer
        rb = GetComponent<Rigidbody2D>();       // set our rigidbody
        lives = GameManager.instance.AILives;   // set our lives
        ChangeOptions(Options.Idle);            // initilize our state
        SetUpCircleCollider();                  // set up our circle collider
    }

	// Update is called once per frame
	void Update () {

        // if have a pawn check if on ground otherwise find our pawn and set how many times can jump
        if (pawn != null) {
            isGrounded = pawn.IsGrounded();
        } else {
            pawn = gameObject.GetComponentInChildren<Pawn>();
            jumpCount = pawn.jumps;
        }

        // if ground gravity off otherwise gravity on
        if (isGrounded) {
            rb.gravityScale = 0;
            if (rb.velocity.y < 0) {
                rb.velocity = Vector2.zero; // reset downward velocity if hit the ground going down
            }
            jumpCount = pawn.jumps;
        } else {
            rb.gravityScale = 1;

            if (rb.velocity.y < -20f) {
                rb.velocity = new Vector2(rb.velocity.x, -20f);     // max fall speed
            }
        }


        // if have a target then we can start detecting it and changing our states
        if (target != null) {
            detect = true;
            canSee = CanSee();
        } else {
            detect = false;
            canSee = false;
        }
        FSM();                  // run our state changes

        // if attack set a timer to not just spawn attack
        if (attacking) {
            AttackTimer();
        }

        // if can jump then jump
        if (jumpCount > 0) {
            RandomJump();
        }

        // if have a pawn change state and direction
        if (pawn != null) {
            aiState = ChangeAIState();
            pawn.ChangeAnimationState(aiState);
            GetHitDirection();
            pawn.MoveDirection(direction);
        }
    }

    // AI Finite State Machine
    void FSM() {
        if (opt == "Idle") {
            // Do Nothing

            // if detect GoTo
            if (detect) {
                ChangeOptions(Options.GoTO);
            }
            // if see Attack
            if (canSee) {
                ChangeOptions(Options.Attack);
            }

        } else if (opt == "GoTO") {
            // go to target
            if (target != null) {
                GoToTarget();
            }

            // if see Attack
            if (canSee) {
                ChangeOptions(Options.Attack);
            }
            // if cant detect idle
            if (!detect) {
                ChangeOptions(Options.Idle);
            }
        } else if (opt == "Attack") {
            // Possible attack target
            if (target != null) {
                GoToTarget();
            }
            if (!attacking) {
                cntD = cnt;
                Attack();
            }

            // if cant detect Idle
            if (!detect) {
                ChangeOptions(Options.Idle);
            }
            // cant see GoTO
            if (!canSee) {
                ChangeOptions(Options.GoTO);
            }
        }
    }

    void ChangeOptions(Options state) {
        // Changes action states
        option = state;
        opt = option.ToString();
    }

    string ChangeAIState() {
        // changes animation states (same as player controller)
        if (direction == 0) {
            AiState = CharacterState.Idle;
        }

        if (direction != 0) {
            AiState = CharacterState.Run;
        }

        if (attacking) {
            AiState = CharacterState.Attack;

            if (shoot) {
                AiState = CharacterState.Shoot;
                if (pawn.GetComponent<Ninja>()) {
                    AiState = CharacterState.Throw;
                }
            }
        }

        if (!isGrounded) {
            AiState = CharacterState.Jump;

            if (attacking) {
                AiState = CharacterState.JumpAtt;

                if (shoot) {
                    AiState = CharacterState.Shoot;
                    if (pawn.GetComponent<Ninja>()) {
                        AiState = CharacterState.Throw;
                    }
                }
            }
        }

        return AiState.ToString();
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.transform.parent != null) {
            GameObject initial = collision.gameObject.transform.parent.gameObject;
            if (initial.transform.tag == "Character") {
                target = initial.transform.parent.gameObject; // get our target if we can find and detect it
            }
        }

        // Attack logic (same as player controller)
        if (attacking) {
            string hitTarget = collision.gameObject.name;
            attDamage = pawn.Attack();
            if (collision.gameObject.transform.parent != null) {
                if (collision.gameObject.transform.parent.GetComponent<Pawn>() != null) {
                    this.hitTarget = collision.gameObject.transform.parent.GetComponent<Pawn>();
                }
            }
            if (hitTarget == "Head" || hitTarget == "Body" || hitTarget == "Legs") {
                if (this.hitTarget != null) {
                    GameManager.instance.playerDamageTaken += attDamage;
                    this.hitTarget.TakeDamage(attDamage, hitDirection, hitTarget);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        // reset values (same as playercontroller)
        if (other.gameObject.tag == "Boundary") {
            lives -= 1;
            GameManager.instance.AILives = lives;
            pawn.damagePercentage = 0;
            GameManager.instance.AIDamageTaken = 0;
            rb.velocity = Vector2.zero;
            if (lives < 0 || GameManager.instance.playerLives < 0) {
                Destroy(gameObject);
            } else {
                transform.position = new Vector3(0, 30, 0);
            }
        }
    }

    void SetUpCircleCollider() {
        // set up circle coller that detect player
        circle = GetComponent<CircleCollider2D>();
        circle.radius = 50;
        circle.isTrigger = true;
    }

    void GoToTarget() {
        // follows target x-axis movement
        Vector3 goToTarget = target.transform.position - transform.position;
        transform.right = new Vector3(goToTarget.x, 0, 0);
        GetDirection(goToTarget);
        pawn.MoveDirection(direction);
    }

    void GetDirection(Vector3 vector) {
        // don't stop exactly on our target
        if (vector.x > 2.9f) {
            direction = .5f;
        } else if (vector.x < -2.9f) {
            direction = -.5f;
        } else {
            direction = 0;
        }
    }

    void Attack() {
        // Possible to attack
        int num = Random.Range(1, 3);
        if (pawn.GetComponent<Knight>()) {  // less options if a knight
            num = 1;
        }

        if (num == 1) {     // Melee Attack
            bool inRange = InRange();
            if (inRange) {
                attacking = true;
            }
        }
        if (num == 2) { // Range Attack
            pawn.Shoot();
            attacking = true;
            shoot = true;
        }
    }

    bool InRange() {
        // Find if the target is close enought to melee attack
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 3f);
        if (hit.collider != null) {
            if (hit.collider.transform.parent != null) {
                if (hit.collider.transform.parent.gameObject.tag == "Character") {
                    return true;
                }
            }
        }

        return false;
    }

    bool CanSee() {
        // if can see the target
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 20f); // Orgin, look diretion, distance can see
        if (hit.collider != null) {
            if (hit.collider.transform.parent != null) {
                if (hit.collider.transform.parent.gameObject.tag == "Character") {
                    return true;
                }
            }
        }

        return false;
    }

    void GetHitDirection() {
        //(same as Player controller)
        if (direction > 0) {
            hitDirection = 1;
        }

        if (direction < 0) {
            hitDirection = -1;
        }
    }

    void AttackTimer() {
        // (Attack time same as Player controller)
        if (isGrounded) {
            direction = 0;
        }
        cntD -= Time.deltaTime;
        if (cntD < 0) {
            attacking = false;
            shoot = false;
        }
    }

    void RandomJump() {
        // Our character will random jump after a timer
        jumpTimer -= Time.deltaTime;
        if (jumpTimer < 0) {
            int num = Random.Range(1, 11);
            Jump(num);                       // send a random num to randomly jump
        }
    }

    void Jump(int num) {
        // if our random num is even then jump
        if (num % 2 == 0) {
            pawn.Jump(1);
            jumpCount -= 1;
        }

        jumpTimer = 1f; // reset timer
    }
}
