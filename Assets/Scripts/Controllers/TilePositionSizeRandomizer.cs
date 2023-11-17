using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilePositionSizeRandomizer : MonoBehaviour {
	public Vector2 TileAnchorCenter;
	[SerializeField] Tilemap tilemap;
	[SerializeField] TileBase tileBase;

	//buff prefabs
	[SerializeField] List<GameObject> buffPrefabs, debuffPrefabs = new List<GameObject>();

	int currentSize = 1;
	void Start() {
		randomizeTile();
	}

	public void randomizeTile() {
		randomizeSize();
		randomizePosition();
		BuffAndDebuff();
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
	void BuffAndDebuff() {
		ItemsController.randomizeBuffAndDebuff(TileAnchorCenter);
	}



}
