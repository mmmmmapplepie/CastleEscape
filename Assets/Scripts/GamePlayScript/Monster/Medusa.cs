using System.Collections;
using UnityEngine;

public class Medusa : MonsterBase {
	protected override void AwakeMethod() {
		realColor = player.gameObject.GetComponent<SpriteRenderer>().color;
		medusas++;
	}
	protected override IEnumerator ChasingRoutine() {
		remainingPetrifyTime += petrifyTime;
		while (true) {
			if (!PreyInRange(senseRange)) {
				StopChase();
				yield break;
			}
			if (PreyInRange(0.25f)) { setMovement(Vector3.zero); continue; }
			direction = player.position - transform.position;
			setMovement(direction);
			yield return null;
		}
	}
	protected override void UpdateMethod() {
		if (remainingPetrifyTime > 0f) {
			if (!petrified) Petrify();
			remainingPetrifyTime -= Time.deltaTime / medusas;
		} else {
			if (petrified) {
				unPetrify();
				remainingPetrifyTime = 0f;
			}
		}
	}
	bool petrified = false;
	static int medusas = 0;
	float petrifyTime = 2f;
	static float remainingPetrifyTime = 0f;
	Color realColor;
	void Petrify() {
		petrified = true;
		float greyVal = Mathf.Min((realColor.r + realColor.g + realColor.b) - 0.2f, 0f) / 3f;
		player.gameObject.GetComponent<SpriteRenderer>().color = new Color(greyVal, greyVal, greyVal, 1f);
		player.root.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
		player.root.gameObject.GetComponent<Animator>().speed = 0f;
	}
	void unPetrify() {
		petrified = false;
		player.gameObject.GetComponent<SpriteRenderer>().color = realColor;
		player.root.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
		player.root.gameObject.GetComponent<Animator>().speed = 1f;
		remainingPetrifyTime = 0f;
	}
	void OnDestroy() {
		if (petrified) unPetrify();
		medusas--;
	}
}
