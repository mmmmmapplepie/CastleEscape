using System.Collections;
using UnityEngine;
public class Husher : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		hushRoutine = StartCoroutine(TurnOfftorch());
		while (true) {
			if (!PreyInRange(senseRange)) {
				returnTorchToNormal();
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(2f)) { yield return null; continue; }
			setMovement(direction);
			yield return null;
		}
	}
	float turnoffTime = 0.2f;
	float offWaitTime = 2f;
	Coroutine hushRoutine;
	static bool fullyOff = false;
	IEnumerator TurnOfftorch() {
		float time = turnoffTime;
		while (time > 0f) {
			TorchMovement.SetTorchIntensity(time / turnoffTime);
			time -= Time.deltaTime;
			yield return null;
		}
		if (fullyOff != true) fullyOff = true;
		yield return new WaitForSeconds(offWaitTime);

		TorchMovement.SetTorchIntensity(1f);
	}
	void returnTorchToNormal() {
		if (hushRoutine != null) StopCoroutine(hushRoutine);
		TorchMovement.SetTorchIntensity(1f);

	}
	void OnDestroy() {
		returnTorchToNormal();
	}
}
