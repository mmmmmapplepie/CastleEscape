using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDmgBuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.EnemyDamageMultiplier *= 0.5f;
	}
	protected override void EndItemEffect() {
		GameBuffsManager.EnemyDamageMultiplier /= 0.5f;
	}
}
