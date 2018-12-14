using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour {

    // Create variable to update our text in the UI
    public Text player1Lives, player1Damage, player2Lives, player2Damage;
	
	// Update is called once per frame
	void Update () {
        // Update text that is in the UI
        player1Lives.text = "Lives: " + GameManager.instance.playerLives;   // player's lives
        player1Damage.text = GameManager.instance.playerDamageTaken + "%";  // player's damage
        player2Lives.text = "Lives: " + GameManager.instance.AILives;       // AI's lives
        player2Damage.text = GameManager.instance.AIDamageTaken + "%";      // AI's damage
    }
}
