using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Straidon : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		StartCoroutine(chargeDash());
		while (true) {
			if (!PreyInRange(senseRange * 1.2f) && !charging) {
				StopChase();
				yield break;
			}
			if (chargingtemp != charging) {
				yield return new WaitForSeconds(restTime);
				chargingtemp = charging;
				continue;
			}
			if (!charging) {
				StartCoroutine(chargeDash());
			}
			yield return null;
		}
	}


	bool chargingtemp = false;
	bool charging = false;
	float chargeTime = 2f;
	float restTime = 1f;
	IEnumerator chargeDash() {
		charging = true;
		yield return new WaitForSeconds(chargeTime);
		direction = player.position - transform.position;
		float chargetime = (direction.magnitude + 2f) / (2f * moveSpeed * GameBuffsManager.EnemySpeedMultiplier);
		setMovement(direction, 2f);
		yield return new WaitForSeconds(chargetime);
		charging = false;
	}
}
