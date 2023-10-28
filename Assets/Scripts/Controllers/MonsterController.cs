using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {
	GameObject monsterPrefab;
	void spawnMonsters() {
		monsterPrefab = CurrentSettings.CurrentMonster.MonsterPrefab;
		spawnMonsterWave();
	}
	List<GameObject> monsterList = new List<GameObject>();
	void spawnMonsterWave() {
		clearMonsterList();
		int monsterNumber = getMonsterNumber();
		for (int i = 0; i < monsterNumber; i++) {
			spawnMonster(monsterPosition());
		}
	}
	void clearMonsterList() {
		foreach (GameObject monster in monsterList) {
			Destroy(monster);
		}
		monsterList.Clear();
	}
	int getMonsterNumber() {
		int monsterNum = 1;
		return monsterNum;
	}
	void spawnMonster(Vector3 position) {
		if (monsterPrefab == null) return;
		monsterList.Add(Instantiate(monsterPrefab, position, Quaternion.identity));
	}
	static Vector3 monsterPosition() {
		return new Vector3(Random.Range(20, 25), Random.Range(32, 30), 0f);
	}
	void Awake() {
		GameStateManager.RoomCleared += clearMonsterList;
		GameStateManager.StartNewRoom += spawnMonsters;
		GameStateManager.GameEnd += clearMonsterList;
		GameStateManager.GameStart += spawnMonsters;
	}
}
