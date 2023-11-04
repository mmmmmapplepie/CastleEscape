using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpdBuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.EnemySpeedMultiplier /= 2f;
	}
	protected override void EndItemEffect() {
		GameBuffsManager.EnemySpeedMultiplier *= 2f;
	}
}
