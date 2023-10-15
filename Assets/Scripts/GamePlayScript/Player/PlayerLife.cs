using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour {
	public int maxHealth = 1;
	public int regen = 0;
	int _health = 3;
	public int Health {
		get { return _health; }
		set {
			_health = (value) < 0 ? 0 : (value > maxHealth ? maxHealth : value);
		}
	}
	public void takeDamage(int damage) {
		//effects
		Health -= damage;
	}
	void Regen() {
		Health += regen;
	}
	bool dead = false;
	void checkDeath() {
		if (Health == 0) {
			dead = true;
			//make a event that is called when this happens.
		}
	}
}
