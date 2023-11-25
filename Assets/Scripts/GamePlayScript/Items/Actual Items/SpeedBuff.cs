using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.PlayerSpeedMultiplier *= 2.5f;
	}
	protected override void EndItemEffect() {
		GameBuffsManager.PlayerSpeedMultiplier /= 2.5f;
	}
}