using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameOrganizer : MonoBehaviour {
	[SerializeField] List<Monster> monsters = new List<Monster>();
	[SerializeField] List<PlayerType> players = new List<PlayerType>();
	[SerializeField] bool InPlayerSettings = true;
	[SerializeField] GameObject cell;
	[SerializeField] Transform cellHolder;
	List<GameObject> nameCells = new List<GameObject>();
	static float ShiftTime = 0.25f;
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
			int tempCount = tempNameHolder.Count;
			for (int i = 0; i < tempCount; i++) {
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
			int tempCount = tempNameHolder.Count;
			for (int i = 0; i < tempCount; i++) {
				GameObject cell = tempNameHolder.Pop();
				float initialY = cell.GetComponent<RectTransform>().anchoredPosition.y;
				cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, initialY - 80f * count);
				nameCells.Insert(0, cell);
			}
		}
	}



	//stat references: left null for the unused ones. I.E monster stuff left null onn player name organizer.
	void EditStats() {
		if (InPlayerSettings) {

		} else {

		}
	}
	//use queue together with coroutines for multiple commands being given.
	Queue<Coroutine> CommandQueue = new Queue<Coroutine>();
	Queue<bool> CommandKeys = new Queue<bool>();
	bool ExecutingCommand = false;

	public void MoveUP() {

	}
	IEnumerator moveUpCommand(bool start) {
		while (start) {
			//wait until a
			//wait until at top of queue.
		}
		float time = Time.unscaledTime;
		while (Time.unscaledTime < time + ShiftTime) {
			yield return null;
		}
	}
	IEnumerator moveDownCommand(bool start) {
		while (start) {
			yield return null;
		}

	}
	public void MoveDown() {

	}
	void SetColor() {

	}
	void Update() {

	}
	public static T ReturnLastItem<T>(List<T> list) {
		if (list.Count == 0) return default;
		return list[list.Count - 1];
	}
}

// # FFE72A    -- main yellow
// #781E00       -- nearest
// #430000       --furthest
// #00000000       --outside
