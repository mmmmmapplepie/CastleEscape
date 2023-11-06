using System.Collections;
using UnityEngine;

public class Medusa : MonsterBase {
	protected override void AwakeMethod() {
		realColor = player.gameObject.GetComponent<SpriteRenderer>().color;
		if (!hasController) { controllerScript = true; hasController = true; }
	}
	static bool hasController = false;
	bool controllerScript = false;
	protected override IEnumerator ChasingRoutine() {
		remainingPetrifyTime += petrifyTime;
		while (true) {
			if (!PreyInRange(senseRange)) {
				StopChase();
				yield break;
			}
			if (PreyInRange(0.25f)) { setMovement(Vector3.zero); yield return null; continue; }
			direction = player.position - transform.position;
			setMovement(direction);
			yield return null;
		}
	}
	protected override void UpdateMethod() {
		if (!controllerScript) return;
		if (remainingPetrifyTime != 0) print(remainingPetrifyTime);
		if (remainingPetrifyTime > 0f) {
			if (!petrified) Petrify();
			remainingPetrifyTime -= Time.deltaTime;
		} else {
			if (petrified) {
				unPetrify();
				remainingPetrifyTime = 0f;
			}
		}
	}
	bool petrified = false;
	float petrifyTime = 2.5f;
	static float remainingPetrifyTime = 0f;
	Color realColor;
	void Petrify() {
		petrified = true;
		float greyVal = Mathf.Min((realColor.r + realColor.g + realColor.b) - 0.1f, 1f) / 3f;
		player.gameObject.GetComponent<SpriteRenderer>().color = new Color(greyVal, greyVal, greyVal, 1f);
		player.root.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
		player.root.gameObject.GetComponent<Animator>().speed = 0f;
		player.root.gameObject.GetComponent<Animator>().enabled = false;
	}
	void unPetrify() {
		petrified = false;
		player.gameObject.GetComponent<SpriteRenderer>().color = realColor;
		player.root.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		player.root.gameObject.GetComponent<Animator>().speed = 1f;
		player.root.gameObject.GetComponent<Animator>().enabled = true;
		remainingPetrifyTime = 0f;
	}
	void OnDestroy() {
		if (controllerScript) hasController = false;
		if (petrified) unPetrify();
	}
}
