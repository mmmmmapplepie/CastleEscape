using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSettings : MonoBehaviour {
	[SerializeField] List<PlayerType> PlayerTypes = new List<PlayerType>();
	// [SerializeField] List<EnemyType> EnemyTypes = new List<EnemyType>();
	public static PlayerType CurrentPlayerType;
	// public static EnemyType CurrentEnemyType;
}
