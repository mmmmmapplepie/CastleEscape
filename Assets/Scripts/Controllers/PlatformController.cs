using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
	public static int luck = 0;
	[SerializeField] GameObject platformPrefab, doorPrefab;

	[SerializeField]
	float gridWidth = 5f, gridHeight = 8f;
	[SerializeField]
	int gridNumX = 5, gridNumY = 6;
	List<TilePositionSizeRandomizer> tileScriptList = new List<TilePositionSizeRandomizer>();
	public void makePlatforms() {
		for (int i = 0; i < gridNumY; i++) {
			float yPos = i * gridHeight - 17f;
			makeRows(-29f, gridNumX, gridWidth, yPos);
			makeRows(4f, gridNumX, gridWidth, yPos);
		}
		changeAllTiles();
	}

	//will make platforms using the input position as the leftmost position for gridNum of times with the gridWidth
	void makeRows(float startXPos, int gridNum, float width, float inputY) {
		for (int i = 0; i < gridNum; i++) {
			GameObject newPlatform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity, transform);
			TilePositionSizeRandomizer tileScript = newPlatform.GetComponent<TilePositionSizeRandomizer>();
			tileScript.TileAnchorCenter = new Vector2((i * width) + (width / 2f) + startXPos, inputY);
			tileScriptList.Add(tileScript);
		}
	}
	public void changeAllTiles() {
		foreach (TilePositionSizeRandomizer tileScript in tileScriptList) {
			tileScript.randomizeTile();
		}
		placeDoor();
	}
	GameObject door = null;
	void placeDoor() {
		if (door != null) Destroy(door);
		float x = Random.Range(8f, 27f);
		x = Random.Range(0, 2) == 0 ? x : -x;
		float y = Random.Range(0, 2) == 0 ? Random.Range(8f, 35f) : Random.Range(-18f, -8f);
		door = Instantiate(doorPrefab, new Vector3(x, y, 0f), Quaternion.identity);
	}
	public void destroyPlatforms() {
		foreach (TilePositionSizeRandomizer tile in tileScriptList) {
			Destroy(tile.gameObject);
		}
		tileScriptList.Clear();
		if (door != null) Destroy(door);
	}

	void Awake() {
		GameStateManager.StartNewRoom += changeAllTiles;
		GameStateManager.EnterMenu += destroyPlatforms;
		GameStateManager.GameStart += makePlatforms;
	}
}
