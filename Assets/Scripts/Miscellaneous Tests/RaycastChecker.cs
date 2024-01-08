using System.Collections.Generic;
using UnityEngine;

public class RaycastChecker : MonoBehaviour {
	void Awake() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(0, -1f), 1f);
		if (hit.collider != null) { print(hit.collider.name); }
	}
}
