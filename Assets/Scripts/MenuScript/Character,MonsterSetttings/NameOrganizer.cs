using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NameOrganizer : MonoBehaviour {
	[SerializeField] List<Monster> monsters = new List<Monster>();
	[SerializeField] List<PlayerType> players = new List<PlayerType>();
	[SerializeField] bool InPlayerSettings = true;
	[SerializeField] GameObject cell;
	[SerializeField] Transform cellHolder;
	List<GameObject> nameCells = new List<GameObject>();
	static float ShiftTime = 0.2f;
	bool EvenCount = true;
	int above = 0;
	int centerIndex = 0;
	int below = 0;
	bool start = true;
	void setCountValues() {
		centerIndex = InPlayerSettings ? Mathf.FloorToInt(players.Count / 2) : Mathf.FloorToInt(monsters.Count / 2);
		EvenCount = InPlayerSettings ? (players.Count % 2 == 0 ? true : false) : (monsters.Count % 2 == 0 ? true : false);
		if (EvenCount) {
			above = centerIndex;
			below = centerIndex - 1;
		} else {
			above = below = centerIndex;
		}
	}
	void OnEnable() {
		setCountValues();
		InitializeNames();
	}
	void OnDisable() {
		for (int i = 0; i < cellHolder.childCount; i++) {
			Destroy(cellHolder.GetChild(i).gameObject);
			nameCells.Clear();
		}
	}
	void InitializeNames() {
		if (InPlayerSettings) {
			CreateNameCells();
			InstantShift(CurrentSettings.CurrentPlayerType.name);
		} else {
			CreateNameCells();
			InstantShift(CurrentSettings.CurrentMonster.name);
		}
		EditStats();
	}
	void CreateNameCells() {
		int cells = InPlayerSettings ? players.Count : monsters.Count;
		for (int i = 0; i < cells; i++) {
			GameObject newcell = Instantiate(cell, cellHolder);
			nameCells.Add(newcell);
			newcell.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -80f * i);
			newcell.GetComponent<TextMeshProUGUI>().text = InPlayerSettings ? players[i].name : monsters[i].name;
		}
	}
	void InstantShift(string Name) {
		int index = 0;
		if (InPlayerSettings) { index = players.FindIndex(x => x.name == Name); } else { index = monsters.FindIndex(x => x.name == Name); }
		if (index == centerIndex) {
			foreach (GameObject cell in nameCells) {
				float initialY = cell.GetComponent<RectTransform>().anchoredPosition.y;
				cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, initialY + 80f * centerIndex);
			}
			return;
		} else {
			foreach (GameObject cell in nameCells) {
				float initialY = cell.GetComponent<RectTransform>().anchoredPosition.y;
				cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, initialY + 80f * index);
			}
		}
		Stack<GameObject> tempNameHolder = new Stack<GameObject>();
		int count = nameCells.Count;
		if (index < centerIndex) {
			int itemsToMove = count - 1 - index - below;
			for (int i = 0; i < itemsToMove; i++) {
				tempNameHolder.Push(nameCells[i + below + index + 1]);
			}
			nameCells.RemoveRange(below + index + 1, itemsToMove);
			for (int i = 0; i < itemsToMove; i++) {
				GameObject cell = tempNameHolder.Pop();
				float initialY = cell.GetComponent<RectTransform>().anchoredPosition.y;
				cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, initialY + 80f * count);
				nameCells.Insert(0, cell);
			}
		} else {
			int itemsToMove = index - above;
			for (int i = itemsToMove; i > 0; i--) {
				tempNameHolder.Push(nameCells[i - 1]);
			}
			nameCells.RemoveRange(0, itemsToMove);
			for (int i = 0; i < itemsToMove; i++) {
				GameObject cell = tempNameHolder.Pop();
				float initialY = cell.GetComponent<RectTransform>().anchoredPosition.y;
				cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, initialY - 80f * count);
				nameCells.Add(cell);
			}
		}
		ChangeNameColorsDirect();
	}
	void ChangeNameColorsDirect() {
		nameCells[centerIndex].GetComponent<TextMeshProUGUI>().color = new Color32(0xFF, 0xE7, 0x2A, 0xFF);
		if (nameCells[centerIndex - 1] != null) {
			nameCells[centerIndex - 1].GetComponent<TextMeshProUGUI>().color = new Color32(0x78, 0x1E, 0x00, 0xFF);
		}
		if (nameCells[centerIndex - 2] != null) {
			nameCells[centerIndex - 2].GetComponent<TextMeshProUGUI>().color = new Color32(0x43, 0x00, 0x00, 0xFF);
		}
		if (nameCells[centerIndex + 1] != null) {
			nameCells[centerIndex + 1].GetComponent<TextMeshProUGUI>().color = new Color32(0x78, 0x1E, 0x00, 0xFF);
		}
		if (nameCells[centerIndex + 2] != null) {
			nameCells[centerIndex + 2].GetComponent<TextMeshProUGUI>().color = new Color32(0x43, 0x00, 0x00, 0xFF);
		}
	}

	#region StatsEditing
	//stat references: left null for the unused ones. I.E monster stuff left null onn player name organizer.
	void EditStats() {
		if (InPlayerSettings) {


			if (CurrentSettings.CurrentPlayerType.name == "Zaron The Zenith") {
				//enable the custom character controls
				//	the add & subtract from skillpoint
				//	the remaining skillpoints number & text
			}
			//check if it is custom character as well.
		} else {

		}
		//enable the controls if name is the custom one.
	}
	#endregion



	//use queue together with coroutines for multiple commands being given.
	[SerializeField] CurrentSettings playerMonsterSettingsScript;
	int currFillPosition = 0;
	int currCommandPosition = 0;
	int usedCommandOrders = 0;
	bool[] locked = new bool[5] { false, false, false, false, false };
	bool ExecutingCommand = false;
	public void ChangeName(bool up) {
		if (usedCommandOrders >= 5) return;
		usedCommandOrders++;
		locked[currFillPosition] = true;
		StartCoroutine(moveCommand(currFillPosition, up));
		currFillPosition = currFillPosition + 1 > 4 ? 0 : currFillPosition + 1;
	}
	IEnumerator moveCommand(int here, bool up) {
		while (locked[here]) {
			yield return null;
		}
		float time = Time.unscaledTime;
		List<Vector2> initialPos = new List<Vector2>();
		List<Vector2> finalPos = new List<Vector2>();
		foreach (GameObject cell in nameCells) {
			Vector2 anchorPos = cell.GetComponent<RectTransform>().anchoredPosition;
			initialPos.Add(anchorPos);
			finalPos.Add(up ? anchorPos + new Vector2(0f, 80f) : anchorPos - new Vector2(0f, 80f));
		}
		while (Time.unscaledTime < time + ShiftTime) {
			//ratio goes from 0f to 1f as time passes.
			float ratio = 1f - ((time + ShiftTime - Time.unscaledTime) / ShiftTime);
			for (int i = 0; i < nameCells.Count; i++) {
				nameCells[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(initialPos[i], finalPos[i], ratio);
			}
			SetColors(ratio, up);
			yield return null;
		}
		for (int i = 0; i < nameCells.Count; i++) {
			nameCells[i].GetComponent<RectTransform>().anchoredPosition = finalPos[i];
		}
		ShiftLocations(up);
		ChangeNameColorsDirect();
		EditStats();
		ExecutingCommand = false;
		usedCommandOrders--;
	}
	void SetColors(float ratio, bool up) {
		Color32 center = new Color32(0xFF, 0xE7, 0x2A, 0xFF);
		Color32 firstAway = new Color32(0x78, 0x1E, 0x00, 0xFF);
		Color32 secondAway = new Color32(0x43, 0x00, 0x00, 0xFF);
		Color32 fullOut = new Color32(0x00, 0x00, 0x00, 0xFF);
		nameCells[centerIndex].GetComponent<TextMeshProUGUI>().color = Color32.Lerp(center, firstAway, ratio);
		int directionSign = up ? 1 : -1;
		if (nameCells[centerIndex - directionSign * 1] != null) {
			nameCells[centerIndex - directionSign * 1].GetComponent<TextMeshProUGUI>().color = Color32.Lerp(firstAway, secondAway, ratio);
		}
		if (nameCells[centerIndex - directionSign * 2] != null) {
			nameCells[centerIndex - directionSign * 2].GetComponent<TextMeshProUGUI>().color = Color32.Lerp(secondAway, fullOut, ratio);
		}
		if (nameCells[centerIndex + directionSign * 1] != null) {
			nameCells[centerIndex + directionSign * 1].GetComponent<TextMeshProUGUI>().color = Color32.Lerp(firstAway, center, ratio);
		}
		if (nameCells[centerIndex + directionSign * 2] != null) {
			nameCells[centerIndex + directionSign * 2].GetComponent<TextMeshProUGUI>().color = Color32.Lerp(secondAway, firstAway, ratio);
		}
		if (nameCells[centerIndex + directionSign * 3] != null) {
			nameCells[centerIndex + directionSign * 3].GetComponent<TextMeshProUGUI>().color = Color32.Lerp(fullOut, secondAway, ratio);
		}
	}
	void ShiftLocations(bool up) {
		if (up) {
			GameObject temp = nameCells[0];
			temp.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0f, +80f * nameCells.Count);
			nameCells.RemoveAt(0);
			nameCells.Add(temp);
		} else {
			int lastindex = nameCells.Count - 1;
			GameObject temp = nameCells[lastindex];
			temp.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, +80f * nameCells.Count);
			nameCells.RemoveAt(lastindex);
			nameCells.Insert(0, temp);
		}
		if (InPlayerSettings) {
			playerMonsterSettingsScript.SetPlayerMonsterVariables(nameCells[centerIndex].GetComponent<TextMeshProUGUI>().text, null);
		} else {
			playerMonsterSettingsScript.SetPlayerMonsterVariables(null, nameCells[centerIndex].GetComponent<TextMeshProUGUI>().text);
		}
		CurrentSettings.ChangePlayerSettingsInnateAndPreview();
	}
	void Update() {
		callCommands();
	}
	void callCommands() {
		if (ExecutingCommand || !locked.Contains(true)) return;
		locked[currCommandPosition] = false;
		ExecutingCommand = true;
		currCommandPosition = currCommandPosition + 1 > 4 ? 0 : currCommandPosition + 1;
	}
}


