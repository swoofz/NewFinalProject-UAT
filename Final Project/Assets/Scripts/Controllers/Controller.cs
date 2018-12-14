using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour {
    // Create an enum for character states
    public enum CharacterState { Idle, Run, Attack, Slide, Jump, Shoot, JumpAtt, Throw }  

    public Pawn pawn;       // Create a variable to hold this controller pawn
}
