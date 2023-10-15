using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {
	static GameObject monsterPrefab;
	public static float monsterDamage, monsterSpeed;
	public static void spawnInitialMonsters() {
		monsterPrefab = CurrentSettings.CurrentMonster.MonsterPrefab;
		spawnMonsterWave();
	}
	static List<GameObject> monsterList = new List<GameObject>();
	static public void spawnMonsterWave() {
		clearMonsterList();
		int monsterNumber = getMonsterNumber();
		for (int i = 0; i < monsterNumber; i++) {
			spawnMonster(monsterPosition());
		}
	}
	static public void clearMonsterList() {
		foreach (GameObject monster in monsterList) {
			Destroy(monster);
		}
		monsterList.Clear();
	}
	static int getMonsterNumber() {
		int monsterNum = 1;
		return monsterNum;
	}
	static void spawnMonster(Vector3 position) {
		if (monsterPrefab == null) return;
		monsterList.Add(Instantiate(monsterPrefab, position, Quaternion.identity));
	}
	static Vector3 monsterPosition() {
		return new Vector3(Random.Range(0, 1), Random.Range(0, 1), 0f);
	}
}
