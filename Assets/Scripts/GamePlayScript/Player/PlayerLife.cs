using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour {
	[HideInInspector]
	int _health = 3;
	public int Health {
		get { return _health; }
		set { _health = (_health - value) < 0 ? 0 : _health - value; }
	}
	void Start() {
	}
	public void takeDamage(int damage) {
		Health -= damage;
	}
}
