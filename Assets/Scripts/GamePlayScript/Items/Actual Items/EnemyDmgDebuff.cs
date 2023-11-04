using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDmgDebuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.EnemyDamageMultiplier *= 2f;
	}
	protected override void EndItemEffect() {
		GameBuffsManager.EnemyDamageMultiplier /= 2f;
	}
}
