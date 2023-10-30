using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
	bool doorTriggered = false;
	void OnTriggerEnter2D(Collider2D collider) {
		roomTrigger(collider);
	}
	void OnTriggerStay2D(Collider2D collider) {
		roomTrigger(collider);
	}
	void roomTrigger(Collider2D coll) {
		if (coll.gameObject.tag != "Player" || doorTriggered) return;
		doorTriggered = true;
		GameStateManager.ClearRoom();
	}
}
