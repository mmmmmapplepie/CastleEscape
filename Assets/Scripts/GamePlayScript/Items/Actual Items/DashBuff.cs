using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.DashRegenerationRateMultiplier *= 2f;
	}
	protected override void EndItemEffect() {
		GameBuffsManager.DashRegenerationRateMultiplier /= 2f;
	}
}
