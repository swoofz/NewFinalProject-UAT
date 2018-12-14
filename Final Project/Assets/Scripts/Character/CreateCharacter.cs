using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCharacter : MonoBehaviour {

    public List<GameObject> Characters;     // Create a variable to store the character that are in the game

    private GameObject player, pawn;        // Create variable to create a gameobject

    public void Create(string character, bool isPlayer, string name) {
        player = new GameObject(name);  // Create a new game object with a given name
        GetPawn(character);             // get a pawn and attach
        SetLocation();                  // Set the start location
        CharacterSetup(isPlayer);       // Set up our gameObject
    }

    void GetPawn(string character) {
        // Go through all the character and find with one we want
        for (int i = 0; i < Characters.Count; i++) {
            if (character == Characters[i].name) {
                // when find the character want store it in a variable
                pawn = Instantiate(Characters[i], player.transform.position, player.transform.rotation) as GameObject;
            }
        }
        pawn.transform.parent = player.transform;   // set our character as a child of our other gameobject
    }

    void CharacterSetup(bool isPlayer) {
        if (isPlayer) {                                 // if this is a player object
            player.tag = "Player";                      // tag it with player
            player.AddComponent<PlayerController>();    // add a player controller component
        } else {                                        // otherwise
            player.tag = "AI";                          // tag it with AI
            player.AddComponent<AIController>();        // add a AI controller component
        }
    }

    void SetLocation() {
        // Get a random number to find our object start location
        int num = Random.Range(0, GameManager.instance.startLocation.Count);
        while (num == GameManager.instance.randomNum) {                         // if that number what already use find another one
            num = Random.Range(0, GameManager.instance.startLocation.Count);
        }

        if (GameManager.instance.startLocation[num] != null) {                              // if found a location
            player.transform.position = GameManager.instance.startLocation[num].position;   // set our object to this location
            GameManager.instance.randomNum = num;                                           // set our number so can't be use again
        }
    }
}
