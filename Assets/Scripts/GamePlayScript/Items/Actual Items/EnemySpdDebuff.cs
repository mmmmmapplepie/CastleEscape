using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpdDebuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.EnemySpeedMultiplier *= 1.5f;
	}
	protected override void EndItemEffect() {
		GameBuffsManager.EnemySpeedMultiplier /= 1.5f;
	}
}
