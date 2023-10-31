using System.Collections;
using UnityEngine;

public class Ghost : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		inviRoutine = StartCoroutine(turnInvisible());
		while (true) {
			if (!PreyInRange(senseRange)) {
				if (inviRoutine != null) StopCoroutine(turnInvisible());
				gameObject.GetComponent<SpriteRenderer>().color = color;
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(0.5f)) { yield return null; continue; }
			setMovement(direction);
			yield return null;
		}
	}
	float cloakTime = 2f;
	Color color;
	Coroutine inviRoutine;
	IEnumerator turnInvisible() {
		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
		color = sr.color;
		float start = cloakTime;
		while (start > 0f) {
			float ratio = start / cloakTime;
			sr.color = new Color(color.r, color.g, color.b, ratio);
			faceGlow.color = new Color(color.r, color.g, color.b, Mathf.Min(1f, ratio + 0.1f));
			start -= Time.deltaTime;
			yield return null;
		}
	}
}
