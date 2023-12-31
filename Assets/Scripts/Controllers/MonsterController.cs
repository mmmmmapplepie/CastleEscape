using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {
	GameObject monsterPrefab;
	void spawnMonsters() {
		clearMonsterList();
		monsterPrefab = CurrentSettings.CurrentMonster.MonsterPrefab;
		spawnMonsterWave();
	}
	List<GameObject> monsterList = new List<GameObject>();
	void spawnMonsterWave() {
		clearMonsterList();
		// float x = 5f;
		// spawnMonster(new Vector3(x, 0f, 0f));
		for (int i = 0; i < getMonsterNumber(); i++) {
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
		if (HighScore.CurrentScore < 20) {
			return Random.Range(1, HighScore.CurrentScore / 10 + 2);
		}
		return Random.Range(2, 5);
	}
	void spawnMonster(Vector3 position) {
		if (monsterPrefab == null) return;
		monsterList.Add(Instantiate(monsterPrefab, position, Quaternion.identity));
	}
	Vector3 monsterPosition() {
		Vector2 pos = Vector2.zero;
		while (pos.magnitude < 10f) {
			pos = new Vector2(Random.Range(-28f, 28f), Random.Range(-19f, 36f));
		}
		return new Vector3(pos.x, pos.y, 0f);
	}
	void Awake() {
		GameStateManager.StartNewRoom += spawnMonsters;
		GameStateManager.EnterMenu += clearMonsterList;
		GameStateManager.GameStart += spawnMonsters;
	}
}
