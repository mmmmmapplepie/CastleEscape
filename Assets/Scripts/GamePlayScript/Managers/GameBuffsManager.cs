using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBuffsManager : MonoBehaviour {
	//higher the value regenerate dash quicker.
	public static float DashRegenerationRateMultiplier = 1f;


	public static float EnemyDamageMultiplier = 1f;
	public static float EnemySpeedMultiplier = 1f;
	public static float PlayerSpeedMultiplier = 1f;


	//higher value means brighter and wider torch.
	public static float TorchModifierMultiplier = 1f;
	void Awake() {
		GameStateManager.GameEnd += resetMultipliers;
		GameStateManager.GameStart += resetMultipliers;
	}
	void resetMultipliers() {
		DashRegenerationRateMultiplier = 1f;
		EnemyDamageMultiplier = 1f;
		EnemySpeedMultiplier = 1f;
		PlayerSpeedMultiplier = 1f;
		TorchModifierMultiplier = 1f;
	}

}
