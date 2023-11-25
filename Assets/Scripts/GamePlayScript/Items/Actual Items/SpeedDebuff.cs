using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDebuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.PlayerSpeedMultiplier /= 2f;
	}
	protected override void EndItemEffect() {
		GameBuffsManager.PlayerSpeedMultiplier *= 2f;
	}
}
