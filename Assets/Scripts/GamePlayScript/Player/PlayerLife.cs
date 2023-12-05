using System;
using System.Collections;
using UnityEngine;

public class PlayerLife : MonoBehaviour {
	public PlayerMovement MovementScript;
	float _fear = 0f;
	public float panicTime = 0.5f;
	public float Fear {
		get { return _fear; }
		set { _fear = Mathf.Clamp(value, 0f, 100f); }
	}
	Coroutine RisingFearRoutine;
	Coroutine PanicRoutine;
	float aura = 0f;
	public bool panic = false;
	public bool panicImminent = false;
	public int maxHealth = 1;
	public int regen = 0;
	int _health = 3;
	public int Health {
		get { return _health; }
		set {
			_health = (value) < 0 ? 0 : (value > maxHealth ? maxHealth : value);
		}
	}

	[SerializeField] SpriteRenderer playerSprite;
	Color playerColor;
	void Awake() {
		GameStateManager.GameEnd += EndGame;
		GameStateManager.GameEnd += endPanic;
		GameStateManager.EnterMenu += EndGame;
		GameStateManager.GameStart += StartGame;
		GameStateManager.StartNewRoom += NewRoom;
		GameStateManager.RoomCleared += RoomClear;

	}
	void endPanic() {
		if (panic) {
			panicEnd?.Invoke();
		}
	}
	void EndGame() {
		StopAllCoroutines();

	}
	void StartGame() {
		hitRecoveryRoutineHolder = null;
		playerColor = CurrentSettings.CurrentPlayerType.color;
		StartFear();
	}

	void NewRoom() {
		changeHealth(regen);
		ChangeFear(-(aura * 2f + 30f));
		SetPEOnOrOff(true);
	}
	void RoomClear() {
		SetPEOnOrOff(false);
	}
	void SetPEOnOrOff(bool on) {
		foreach (Transform t in transform.Find("PEHolder")) {
			if (on) {
				t.gameObject.GetComponent<ParticleSystem>().Play();
			} else {
				t.gameObject.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
		}
	}


	#region healthRelated
	Coroutine hitRecoveryRoutineHolder = null;
	public bool changeHealth(int amount, float recoveryTime = 2f) {
		if (hitRecoveryRoutineHolder != null || !GameStateManager.InGame) return false;
		Health += amount;
		if (amount < 0) {
			MovementScript.LandAndDmg(MovementScript.DmgPE, Vector3.zero);
			MovementScript.oneOffSound(PlayerAudio.audioType.dmg);
			hitRecoveryRoutineHolder = StartCoroutine(hitRecoveryRoutine(recoveryTime));
			checkDeath();
		}
		return true;
	}
	IEnumerator hitRecoveryRoutine(float recoveryTime) {
		float t = recoveryTime;
		while (t > 0f) {
			playerSprite.color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.4f * Mathf.Sin(t * recoveryTime * 2f * Mathf.PI) + 0.6f);
			t -= Time.deltaTime;
			yield return null;
		}
		playerSprite.color = new Color(playerColor.r, playerColor.g, playerColor.b, 1f);
		hitRecoveryRoutineHolder = null;
	}
	void Regen() {
		Health += regen;
	}
	void checkDeath() {
		if (Health == 0) {
			GameStateManager.Defeat();
			//make a event that is called when this happens.
		}
	}
	#endregion


	void StartFear() {
		Fear = 0;
		panic = false;
		aura = (float)CurrentSettings.CurrentPlayerType.Aura;
		RisingFearRoutine = StartCoroutine(FearRaiseRoutine());
	}
	public void ChangeFear(float value) {
		if (panic || panicImminent) return;
		Fear += value;
	}
	IEnumerator FearRaiseRoutine() {
		while (true) {
			if (panic || GameStateManager.Paused || panicImminent) { yield return null; continue; }
			if (Fear < 100f) Fear += (2f + 3f * (1f / aura)) * Time.deltaTime;
			CheckPanickAttack();
			yield return null;
		}
	}
	void CheckPanickAttack() {
		if (Fear >= 100f) { PanicRoutine = StartCoroutine(Panicking()); }
	}
	public static event Action panicStart, panicEnd;
	IEnumerator Panicking() {
		panicImminent = true;
		while (!MovementScript.grounded || MovementScript.dashing) {
			yield return null;
		}
		panicStart?.Invoke();
		panic = true;
		panicImminent = false;
		MovementScript.StopDropping();
		gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		MovementScript.panicAttack(panicTime);
		MovementScript.changeAnimation("Fear", 0.1f);
		yield return new WaitForSeconds(panicTime);
		MovementScript.changeAnimation("Idle", 0.1f);
		MovementScript.recoveredFromPanicAttack();
		panicEnd?.Invoke();
		panic = false;
		Fear = 0f;
	}
}
