using System.Collections;
using UnityEngine;

public class Signod : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		damageAmountAndTime((int)((-damage / 2f) * GameBuffsManager.EnemyDamageMultiplier), 0f);
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
