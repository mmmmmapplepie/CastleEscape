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
				if (charging) { chargingtemp = charging; yield return null; continue; }
				float t = restTime;
				while (t > 0f) {
					t -= Time.deltaTime;
					if (!charging) {
						if (!InOuterArea) facePlayer(0.1f);
					}
					yield return null;
				}
				chargingtemp = charging;
				continue;
			}
			if (!charging && !chargingtemp) {
				StartCoroutine(chargeDash());
			}
			yield return null;
		}
	}
	bool chargingtemp = false;
	bool charging = false;
	float chargeTime = 2f;
	float restTime = 1f;
	float chargeSpeedMultiplier = 4f;
	IEnumerator chargeDash() {
		charging = true;
		triggerStay = false;
		float remainingTime = chargeTime;
		while (remainingTime > 0f) {
			if (remainingTime > 0.2f) {
				facePlayer(0.2f);
			}
			remainingTime -= Time.deltaTime;
			yield return null;
		}
		// direction = player.position - transform.position;
		float chaseTime = (direction.magnitude + Random.Range(0.5f, 3f)) / (chargeSpeedMultiplier * moveSpeed * GameBuffsManager.EnemySpeedMultiplier);
		setMovement(direction, chargeSpeedMultiplier);
		yield return new WaitForSeconds(chaseTime);
		setMovement(direction, 0f);
		charging = false;
		triggerStay = true;
	}
	protected override void HitOuterWall() {
		if (charging) RB.velocity = Vector2.zero;
	}
	void facePlayer(float multiplier) {
		if (PreyInRange(0.2f)) {
			return;
		} else {
			direction = player.position - transform.position;
			setMovement(direction, multiplier);
		}
	}

}
