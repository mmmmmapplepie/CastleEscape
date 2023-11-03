using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screamer : MonsterBase {
	protected override IEnumerator ChasingRoutine() {

		while (true) {
			if (!PreyInRange(senseRange * 2f)) {
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(Random.Range(0.5f, 2f))) {
				if (runaway != null) StopCoroutine(runaway);
				runaway = StartCoroutine(runAway());
			}
			yield return null;
		}
	}
	Coroutine runaway;
	IEnumerator runAway() {
		setMovement(Quaternion.AngleAxis(Random.Range(-15f, 15f), Vector3.forward) * -direction);
		yield return new WaitForSeconds(1.5f);
	}
}
