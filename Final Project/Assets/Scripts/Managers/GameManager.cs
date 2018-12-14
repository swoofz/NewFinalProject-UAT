using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum Characters { Knight, FemaleNinja, Robot, Adverturer }     // Create an enum for character can be

    public static GameManager instance;     // Create a singleton Game Manager

    [Header("Lives")]
    public int playerLives;     // Create a variable for player lives
    public int AILives;         // Create a variable for AI lives
    private int livesP, livesAI;

    [Header("End Scenes")]
    public string winScreen;    // Create a variable to show the win screen if win
    public string loseScreen;   // Create a variable to show the lose screen if lose

    [HideInInspector] public List<Transform> startLocation;             // Create a list for start locations when player spawns for the first time
    [HideInInspector] public float playerDamageTaken, AIDamageTaken;    // Create variables for player and ai damage that they have taken
    [HideInInspector] public int randomNum = 1000;                      // Create a variable to only use a random number once

    private string player1, ai;         // Create variable to store player's and ai's character of choice
    private Characters AI;              // Create a variable to switch ai characters
    private bool gameOver;              // Create a variable to kept track if the game is over
    private CreateCharacter create;     // Create a variable to get our create character script to create characters

    // On wake just make sure this is the only gameobject script in the scene 
    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Use this for initialization
    void Start() {
        create = GetComponent<CreateCharacter>();   // get our create character component
        livesP = playerLives;                       // get our intial lives for the player
        livesAI = AILives;                          // get our intial lives for the AI
    }

    // Update is called once per frame
    void Update() {

        if (playerLives < 0 || AILives < 0) {   // is player or AI has no more lives
            gameOver = true;                    // game is over
        }

        if (gameOver) {
            // Show the game over Scene
            if (playerLives < 0) {
                // Show Win Screne
                ChangeScenes(loseScreen);
            }
            if (AILives < 0) {
                // Show the Lose Screne
                ChangeScenes(winScreen);
            }

            ResetGame();
        }

	}

    void ResetGame() {
        // Reset our game Values
        playerLives = livesP;
        AILives = livesAI;
        playerDamageTaken = 0;
        AIDamageTaken = 0;
        randomNum = 1000;
        gameOver = false;
    }

    void PickAI() {
        // Pick a random character
        int randomNum = Random.Range(0, 4);

        AI = ( Characters )randomNum;
        ai = AI.ToString();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Don't this on the next scene

        // Create AI and Player
        instance.PickAI();
        instance.create.Create(instance.player1, true, "Player");
        instance.create.Create(instance.ai, false, "AI");
    }

    // Public scene functions
    public void ChangeScenes(string scene) {
        SceneManager.LoadScene(scene);  // load scene by name
    }

    public void LoadArena(string scene) {
        SceneManager.LoadScene(scene);              // load scene by name
        SceneManager.sceneLoaded += OnSceneLoaded;  // do actions if the scene is loaded
    }

    public void ToggleScreen(GameObject screen) {
        screen.SetActive(!screen.activeSelf);       // toggle a screen
    }

    public void QuitGame() {
        Application.Quit(); // Close the application
    }

    public void PickCharacter(string character) {
        instance.player1 = character;   // pick a character
    }

    public void IfPlayer(GameObject moveForwardButton) {
        if (instance.player1 != null) {         // if have a character
            moveForwardButton.SetActive(true);  // active a button to go to the next scene
        }
    }
}
