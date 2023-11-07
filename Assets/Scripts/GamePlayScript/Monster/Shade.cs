using System.Collections;
using UnityEngine;

public class Shade : MonsterBase {
	protected override void AwakeMethod() {
		sr = GetComponent<SpriteRenderer>();
		color = sr.color;
	}
	protected override IEnumerator ChasingRoutine() {
		inviRoutine = StartCoroutine(turnInvisible());
		while (true) {
			if (!PreyInRange(senseRange)) {
				if (inviRoutine != null) StopCoroutine(inviRoutine);
				GetComponent<SpriteRenderer>().color = color;
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(0.5f)) { yield return null; continue; }
			setMovement(direction);
			yield return null;
		}
	}
	SpriteRenderer sr;
	float cloakTime = 0.5f;
	Color color;
	Coroutine inviRoutine;
	IEnumerator turnInvisible() {
		float start = cloakTime;
		while (start > 0f) {
			float ratio = start / cloakTime;
			sr.color = new Color(color.r, color.g, color.b, ratio);
			faceGlow.color = new Color(color.r, color.g, color.b, Mathf.Min(1f, ratio + 0.05f));
			start -= Time.deltaTime;
			yield return null;
		}
		sr.color = new Color(color.r, color.g, color.b, 0f);
		faceGlow.color = new Color(color.r, color.g, color.b, 0.05f);
	}
}