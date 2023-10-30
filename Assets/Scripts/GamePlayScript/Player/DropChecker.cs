using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropChecker : MonoBehaviour {
	[SerializeField] PlayerMovement movementScript;
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.layer == LayerMask.NameToLayer("BackGround")) {
			movementScript.nonDroppable = true;
		}
	}
	void OnTriggerExit2D(Collider2D collider) {
		if (collider.gameObject.layer == LayerMask.NameToLayer("BackGround")) {
			movementScript.nonDroppable = false;
		}
	}
}
