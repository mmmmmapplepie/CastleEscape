using System.Collections;
using UnityEngine;

public class Leech : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		while (true) {
			DamageWhileColliding(false);
			LeechLife = StartCoroutine(leeching());
			if (!PreyInRange(senseRange)) {
				if (LeechLife != null) StopCoroutine(LeechLife);
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


	float period = 1f;
	Coroutine LeechLife;
	IEnumerator leeching() {
		while (true) {
			int dmg = Mathf.RoundToInt(-damage * GameBuffsManager.EnemyDamageMultiplier);
			damageAmountAndTime(dmg, period);
			yield return new WaitForSeconds(period);
		}
	}
}
