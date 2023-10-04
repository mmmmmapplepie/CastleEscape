using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class PlatformController : MonoBehaviour {
	public static int luck = 0;
	[SerializeField] GameObject platformPrefab;

	[SerializeField]
	float gridWidth = 5f, gridHeight = 8f;
	[SerializeField]
	int gridNumX = 5, gridNumY = 6;
	List<TilePositionSizeRandomizer> tileScriptList = new List<TilePositionSizeRandomizer>();
	void Start() {
		makePlatforms();
	}
	void makePlatforms() {
		for (int i = 0; i < gridNumY; i++) {
			float yPos = i * gridHeight - 17f;
			makeRows(-29f, gridNumX, gridWidth, yPos);
			makeRows(4f, gridNumX, gridWidth, yPos);
		}
	}

	//will make platforms using the input position as the leftmost position for gridNum of times with the gridWidth
	void makeRows(float startXPos, int gridNum, float width, float inputY) {
		for (int i = 0; i < gridNum; i++) {
			GameObject newPlatform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity, transform);
			TilePositionSizeRandomizer tileScript = newPlatform.GetComponent<TilePositionSizeRandomizer>();
			tileScript.luck = luck;
			tileScript.TileAnchorCenter = new Vector2((i * width) + (width / 2f) + startXPos, inputY);
			tileScriptList.Add(tileScript);
			tileScript.randomizeTile();
		}
	}
	public void changeAllTiles() {
		foreach (TilePositionSizeRandomizer tileScript in tileScriptList) {
			tileScript.randomizeTile();
		}
	}
}
