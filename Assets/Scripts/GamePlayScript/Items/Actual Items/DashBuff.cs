using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.DashRegenerationRateMultiplier *= 1.5f;
	}
	protected override void EndItemEffect() {
		GameBuffsManager.DashRegenerationRateMultiplier /= 1.5f;
	}
}
