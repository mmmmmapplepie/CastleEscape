using System.Collections;
using UnityEngine;

public class Leech : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		DamageWhileColliding(false);
		LeechLife = StartCoroutine(leeching());
		while (true) {
			if (!PreyInRange(senseRange)) {
				if (LeechLife != null) { print("stopleech"); StopCoroutine(LeechLife); }
				DamageWhileColliding(true);
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(0.5f)) { yield return null; continue; }
			setMovement(direction);
			yield return null;
		}
	}
	float period = 1.5f;
	Coroutine LeechLife = null;
	IEnumerator leeching() {
		while (true) {
			int dmg = Mathf.RoundToInt(-damage * GameBuffsManager.EnemyDamageMultiplier);
			damageAmountAndTime(dmg, period);
			yield return new WaitForSeconds(period);
		}
	}
}
