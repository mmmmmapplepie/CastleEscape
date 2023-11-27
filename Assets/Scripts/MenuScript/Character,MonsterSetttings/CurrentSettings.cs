using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentSettings : MonoBehaviour {
	[SerializeField] AudioPlayer UIAudio;
	[SerializeField] TextMeshProUGUI highscore;
	public GameObject Player;
	static GameObject player;
	[SerializeField] List<PlayerType> PlayerTypes = new List<PlayerType>();
	[SerializeField] List<Monster> Monsters = new List<Monster>();
	public static int[] CustomPlayerStats;
	public static PlayerType CurrentPlayerType;
	public static Monster CurrentMonster;
	public static string CurrentSettingName;

	CameraFollow cameraScript;
	void Awake() {
		cameraScript = Camera.main.GetComponent<CameraFollow>();
		player = Player;
		loadSavedSettings();
		gameObject.SetActive(false);
	}
	public void loadSavedSettings() {
		LoadCustomPlayerSettings();
		if (PlayerPrefs.HasKey("SetPlayer") && PlayerPrefs.HasKey("SetMonster")) {
			string playerset = PlayerPrefs.GetString("SetPlayer");
			string monsterset = PlayerPrefs.GetString("SetMonster");
			CurrentSettingName = playerset + monsterset;
			CurrentPlayerType = PlayerTypes.Find(x => x.name == playerset);
			CurrentMonster = Monsters.Find(x => x.name == monsterset);
		} else {
			string playerset = "Ore The Ordinary";
			string monsterset = "Castle Monster";
			CurrentSettingName = playerset + monsterset;
			CurrentPlayerType = PlayerTypes.Find(x => x.name == playerset);
			CurrentMonster = Monsters.Find(x => x.name == monsterset);
			PlayerPrefs.SetString("SetPlayer", playerset);
			PlayerPrefs.SetString("SetMonster", monsterset);
		}
		if (PlayerPrefs.HasKey(CurrentSettingName)) {
			highscore.text = PlayerPrefs.GetString(CurrentSettingName);
		}
		ChangePlayerSettingsInnateAndPreview();
		HighScore.ShowHighScore();
	}



	#region Load/Set Custom Player Settings
	public static List<string> stats = new List<string>() { "MaxHealth", "Regeneration", "DashPower", "DashRegeneration", "DashStamina", "Luck", "Speed", "TorchIntensity", "TorchWidth", "Aura" };
	void LoadCustomPlayerSettings() {
		CustomPlayerStats = new int[10];
		for (int i = 0; i < stats.Count; i++) {
			if (PlayerPrefs.HasKey(stats[i] + "CustomPlayer")) {
				CustomPlayerStats[i] = PlayerPrefs.GetInt(stats[i] + "CustomPlayer");
			} else {
				CustomPlayerStats[i] = 1;
				PlayerPrefs.SetInt(stats[i] + "CustomPlayer", 1);
			}
		}
		SetCustomPlayerSettings();
	}
	public void SetCustomPlayerSettings() {
		PlayerType customPlayer = PlayerTypes.Find(x => x.name == "Zaron The Zenith");
		for (int i = 0; i < 10; i++) {
			customPlayer.SetStat(stats[i], CustomPlayerStats[i]);
		}
	}
	#endregion


	public static void ChangePlayerSettingsInnateAndPreview() {
		player.GetComponent<PlayerMovement>().playerObject = CurrentPlayerType;
		player.GetComponent<PlayerMovement>().SetupStats();
		CurrentSettingName = CurrentPlayerType.name + CurrentMonster.name;
		HighScore.ShowHighScore();
	}
	public void SetPlayerMonsterVariables(string playerName, string monsterName) {
		if (playerName != null) {
			CurrentPlayerType = PlayerTypes.Find(x => x.name == playerName);
		}
		if (monsterName != null) {
			CurrentMonster = Monsters.Find(x => x.name == monsterName);
		}
	}














	bool InCharacterSetup = true;
	[SerializeField] GameObject CharacterSetup, MonsterSetup;
	[SerializeField] TextMeshProUGUI SetupToggleBtnTxt;
	public void MonsterCharacterToggle() {
		UIAudio.PlaySound("Click");
		if (InCharacterSetup) {
			InCharacterSetup = false;
			CharacterSetup.SetActive(false);
			MonsterSetup.SetActive(true);
			SetupToggleBtnTxt.text = "Pick\nCharacter";
		} else {
			InCharacterSetup = true;
			CharacterSetup.SetActive(true);
			MonsterSetup.SetActive(false);
			SetupToggleBtnTxt.text = "Pick\nMonster";
		}
	}

	[SerializeField] GameObject GameMenu;
	public void SaveSettings() {
		UIAudio.PlaySound("Click");
		for (int i = 0; i < stats.Count; i++) {
			PlayerPrefs.SetInt(stats[i] + "CustomPlayer", CustomPlayerStats[i]);
		}
		PlayerPrefs.SetString("SetPlayer", CurrentPlayerType.name);
		PlayerPrefs.SetString("SetMonster", CurrentMonster.name);
		cameraScript.moveCamera(new Vector3(0f, 0.5f, -10f));
		gameObject.SetActive(false);
		GameMenu.SetActive(true);
	}
}
