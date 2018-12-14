using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLocations : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        foreach (Transform child in transform) {
            // Send the transform of each child to the gameManager
            GameManager.instance.startLocation.Add(child);
        }
	}
}
