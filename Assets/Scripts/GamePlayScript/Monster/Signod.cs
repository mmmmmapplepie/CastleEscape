using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signod : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		damageAmountAndTime(Mathf.Min(1, GameBuffsManager.EnemyDamageMultiplier), 0f);
		while (true) {
			if (!PreyInRange(senseRange * 1.5f)) {
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(1.5f)) { yield return null; continue; }
			setMovement(direction);
			yield return null;
		}
	}
}
