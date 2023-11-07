using System.Collections;
using UnityEngine;

public class Screamer : MonsterBase {
	protected override void AwakeMethod() {
		senseTime -= 1f;
	}
	protected override IEnumerator ChasingRoutine() {
		StartNewRunAway();
		while (true) {
			if (!PreyInRange(senseRange * 2f)) {
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(Random.Range(2f, senseRange)) && runaway == null) {
				runaway = StartCoroutine(runAway());
				yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
			}
			yield return null;
		}
	}
	Coroutine runaway;
	IEnumerator runAway() {
		setMovement(Quaternion.AngleAxis(Random.Range(-15f, 15f), Vector3.forward) * -direction, 2f);
		yield return new WaitForSeconds(1.5f);
		runaway = null;
	}
	protected override void damagePlayer() {
		base.damagePlayer();
		StartNewRunAway();
	}
	void StartNewRunAway() {
		if (runaway != null) {
			StopCoroutine(runaway);
		}
		runaway = StartCoroutine(runAway());
	}
}
