using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using TreeEditor;

public class TilePositionSizeRandomizer : MonoBehaviour {
	public Vector2 TileAnchorCenter;
	[SerializeField] Tilemap tilemap;
	[SerializeField] TileBase tileBase;
	public int luck = 0;

	//buff prefabs
	[SerializeField] List<GameObject> buffPrefabs, debuffPrefabs = new List<GameObject>();

	int currentSize = 1;
	void Start() {
		randomizeTile();
	}

	public void randomizeTile() {
		randomizeSize();
		randomizePosition();
		randomizeBuffAndDebuff();
	}
	void randomizeSize() {
		tilemap.ClearAllTiles();
		currentSize = Random.Range(1, 5);
		for (int i = 0; i < currentSize; i++) {
			tilemap.SetTile(new Vector3Int(i, 0, 0), tileBase);
		}
	}
	void randomizePosition() {
		// position will be calibrated based on the bottom left of the allowable positions
		float xDisplacement = Random.Range(0f, 5f - (float)currentSize);
		float yDisplacement = Random.Range(0, 5f);
		transform.position = new Vector3(xDisplacement + TileAnchorCenter.x - 2.5f, yDisplacement + TileAnchorCenter.y - 2f);
	}
	//buff/debuff chances are out of hundred. pick random from -50 to 50. range from -50 to debuff gives debuff and 50 down to buffchance gives buff. affected by Luck stat.
	void randomizeBuffAndDebuff() {
		int itemchanceluck = luck / 3;
		int itemChance = Random.Range(1, 101);
		if (itemChance > itemchanceluck) return;

		int buffChance = 15;
		buffChance = buffChance - luck;
		int buff_debuffInt = Random.Range(-50, 50);
		GameObject bonus = null;
		if (buff_debuffInt >= buffChance) {
			// 	bonus = buffPrefabs[Random.Range(0, buffPrefabs.Count)];
		} else {
			// 	bonus = debuffPrefabs[Random.Range(0, debuffPrefabs.Count)];
		}
		setBonus(bonus);
	}
	void setBonus(GameObject prefab) {
		if (prefab == null) return;
		Vector3 pos = new Vector3(Random.Range(-2f, 2f) + TileAnchorCenter.x, transform.position.y + 1f + Random.Range(0f, (TileAnchorCenter.y + 4f) - (transform.position.y + 1f)));
		Instantiate(prefab, pos, Quaternion.identity, transform);
	}
}
