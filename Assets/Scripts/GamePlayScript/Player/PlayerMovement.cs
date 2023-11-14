using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour, JoystickController {
	public PlayerType playerObject;
	public Light2D torchLight;
	public Light2D auraLight;
	public PlayerLife lifeScript;
	[SerializeField] Animator animator;
	string currentAnimation = "";

	void Awake() {
		GameStateManager.GameStart += SetupStats;
		GameStateManager.GameEnd += EndGame;
		GameStateManager.StartNewRoom += () => StateChangePosition();
		GameStateManager.EnterMenu += SetupStats;
		GameStateManager.EnterMenu += () => StateChangePosition(false);
	}
	public void SetupStats() {
		RevertToIntialSettings();
		lifeScript.maxHealth = playerObject.MaxHealth * 2;
		lifeScript.Health = lifeScript.maxHealth;
		lifeScript.regen = playerObject.Regeneration;
		playerSpeed = (float)playerObject.Speed;
		dashSpeed = (float)playerObject.DashPower * 1f + 10f;
		PlatformController.luck = playerObject.Luck * 3;
		maxDash = playerObject.DashStamina + 1;
		dashCooldown = (0.6f - playerObject.DashStamina * 0.03f);
		ItemsController.luck = playerObject.Luck;
		setLightStrengths();
		changeSprite();
	}
	void RevertToIntialSettings() {
		if (GameStateManager.InGame) {
			transform.Find("PlayerSprite").gameObject.GetComponent<Collider2D>().enabled = true;
			RB.gravityScale = basicG; RB.drag = 2f;
		} else {
			transform.Find("PlayerSprite").gameObject.GetComponent<Collider2D>().enabled = false;
			RB.velocity = Vector3.zero;
			RB.gravityScale = 0f;
		}
		dashAble = true; grounded = false; tempGrounded = false; dashing = false; HaltUsed = false; dashCharge = 0;
		endDashChanges();
	}
	void ResetPosition() {
		transform.position = new Vector3(0f, 1f, 0f);
	}
	void changeSprite() {
		transform.Find("PlayerSprite").gameObject.GetComponent<SpriteRenderer>().color = playerObject.color;
	}
	public void setLightStrengths() {
		torchLight.intensity = (float)playerObject.TorchIntensity * GameBuffsManager.TorchModifierMultiplier * 0.5f;
		torchLight.pointLightOuterRadius = (float)playerObject.TorchIntensity * 2f + 3f;
		torchLight.pointLightInnerAngle = (float)playerObject.TorchWidth * 3f + 30f;
		torchLight.pointLightOuterAngle = (float)playerObject.TorchWidth * 10f + 50f;
		auraLight.pointLightOuterRadius = (float)playerObject.Aura + 10f;
	}

	float XControl; Vector2 DashControl;
	[HideInInspector]
	public bool grounded = false; bool dashAble = true; bool tempGrounded = false;
	[HideInInspector] public bool dashing = false;
	int maxDash = 1;

	//initially set to 0 as the player somehow gets a charge just as the game starts.
	int _dashCharge = 0;
	public int dashCharge {
		get { return _dashCharge; }
		set { _dashCharge = value > maxDash ? maxDash : value; }
	}
	[SerializeField] LayerMask groundedMask;
	[SerializeField] float basicG = 5f, fallingG = 10f, dashTime = 0.15f;
	float dashSpeed = 15f, dashCooldown = 0.6f, playerSpeed = 8f;
	[SerializeField] Rigidbody2D RB;
	Coroutine dashingRoutine;

	void StateChangePosition(bool newroom = true) {
		if (newroom) {
			transform.position = Camera.main.transform.position = new Vector3(0f, 1f, 0f);
		} else {
			transform.position = new Vector3(0f, 1f, 0f);
		}
		RB.velocity = Vector2.zero;
		changeAnimation("Idle");
	}
	void EndGame() {
		StopAllCoroutines();
		RevertToIntialSettings();
		StartCoroutine(deathCoroutine());
	}
	IEnumerator deathCoroutine() {
		animator.speed = 0f;
		Color c = CurrentSettings.CurrentPlayerType.color;
		SpriteRenderer sr = transform.Find("PlayerSprite").gameObject.GetComponent<SpriteRenderer>();
		float tm = DeathUI.deathRoutineTime;
		float t = tm;
		while (t > 0f) {
			float r = t / tm;
			sr.color = new Color(c.r * r, c.g * r, c.b * r, 1f);
			t -= Time.unscaledDeltaTime;
			yield return null;
		}
		// changeAnimation("Dead");
		sr.color = c;
		animator.speed = 1f;
	}
	void Update() {
		if (!ControllableState()) return;
		grounded = checkGrounded();
		checkDashAdd();
		gravityScale();
		clampFallSpeed();
		stopOnGround();
		setAnimation();
		continuousWalkAndRunSounds();
	}
	void lateralMovement() {
		if (XControl == 0) { return; }
		if (!grounded) {
			float forceMultiplier = 2f;
			RB.AddForce(new Vector2(XControl * playerSpeed * forceMultiplier * GameBuffsManager.PlayerSpeedMultiplier, 0f), ForceMode2D.Force);
			RB.velocity = new Vector2(Mathf.Sign(RB.velocity.x) * Mathf.Min((playerSpeed + dashSpeed) * GameBuffsManager.PlayerSpeedMultiplier / 2f, Mathf.Abs(RB.velocity.x)), RB.velocity.y);
		} else {
			if (dashing) return;
			RB.velocity = new Vector2(XControl * playerSpeed * GameBuffsManager.PlayerSpeedMultiplier, 0f);
			if (Mathf.Abs(RB.velocity.x) < playerSpeed / 1.7f) {
				changeAnimation("Walk");
			} else {
				changeAnimation("Run");
			}
		}
	}
	void Dash() {
		if (dashCharge > 0 && DashControl.magnitude != 0 && dashAble) {
			if (dashingRoutine != null) {
				StopCoroutine(dashingRoutine);
			}
			RB.velocity = dashSpeed * DashControl * GameBuffsManager.DashRegenerationRateMultiplier;
			RB.gravityScale = 0f;
			RB.drag = 0f;
			dashCharge--;
			dashAble = false;
			dashing = true;
			DashAnimation();
			oneOffSound(PlayerAudio.audioType.dash);
			dashingRoutine = StartCoroutine(dashingEnd());
			StartCoroutine(notDashingChange());
		}
	}
	void DashAnimation() {
		float angle = DashControl.x != 0f ? (180f / Mathf.PI) * Mathf.Atan(Mathf.Abs(DashControl.y / DashControl.x)) : 90f;
		if (angle > 45f) {
			if (DashControl.y > 0) {
				changeAnimation("DashUp");
			} else {
				changeAnimation("DashDown");
			}
		} else {
			changeAnimation("DashHor");
		}
	}
	[SerializeField] LayerMask backgroundMask;
	public bool nonDroppable = false;
	void OnTriggerEnter2D(Collider2D collider) {
		if (droppingRoutine != null && (collider.gameObject.layer == LayerMask.NameToLayer("Background") || collider.gameObject.layer != LayerMask.NameToLayer("Ground"))) StopDropping();
	}
	[SerializeField] Collider2D playerColl;
	bool checkGrounded() {
		float xSize = playerColl.bounds.size.x;
		float ySize = playerColl.bounds.size.y;
		RaycastHit2D groundCastBottom = Physics2D.BoxCast((Vector2)playerColl.bounds.center - new Vector2(0f, ySize / 2f), new Vector2(xSize, 0.02f), 0f, Vector2.down, 0.01f, groundedMask);
		RaycastHit2D groundCastTop = Physics2D.BoxCast((Vector2)playerColl.bounds.center - new Vector2(0f, ySize / 2f - 0.02f), new Vector2(xSize, 0.02f), 0f, Vector2.down, 0.01f, groundedMask);
		if (RB.velocity.y <= 0f && groundCastBottom && !groundCastTop) {
			if (!grounded) oneOffSound(PlayerAudio.audioType.land);
			return true;
		} else {
			return false;
		}
	}
	Coroutine droppingRoutine = null;
	public bool HaltUsed = false;
	public float haltFactor = 0.1f;
	public void HaltOrDrop() {
		if (HaltUsed || !ControllableState() || lifeScript.panic) return;
		if (dashingRoutine != null) StopCoroutine(dashingRoutine);
		endDashChanges();
		if (grounded) {
			if (nonDroppable) return;
			droppingRoutine = StartCoroutine(dropThroughPlatform());
		} else {
			RB.velocity = haltFactor * RB.velocity;
		}
		HaltUsed = true;
	}
	IEnumerator dropThroughPlatform() {
		float initialY = transform.position.y;
		playerColl.isTrigger = true;
		while (transform.position.y > initialY - 0.5f) {
			yield return null;
		}
		StopDropping();
	}
	public void StopDropping() {
		playerColl.isTrigger = false;
		if (droppingRoutine != null) {
			StopCoroutine(droppingRoutine); droppingRoutine = null;
		}
	}
	IEnumerator dashingEnd() {
		yield return new WaitForSeconds(dashTime);
		endDashChanges();
	}
	void endDashChanges() {
		if (RB != null) RB.drag = 2f;
		dashing = false;
	}
	IEnumerator notDashingChange() {
		yield return new WaitForSeconds(dashCooldown);
		//need to show the cooldown working/some indicator that dash is ready/not
		dashAble = true;
	}
	void checkDashAdd() {
		if (grounded == false) {
			tempGrounded = false;
		}
		if (grounded && !tempGrounded && RB.velocity.y <= 0f) {
			tempGrounded = true;
			dashCharge++;
			HaltUsed = false;
		}
		if (grounded && RB.velocity.y <= 0f && dashCharge == 0) {
			if (dashCharge == 0) dashCharge++;
			if (HaltUsed == true) HaltUsed = false;
		}
	}
	void clampFallSpeed() {
		if (RB.velocity.y < 0 && RB.velocity.y < -30f) {
			RB.velocity = new Vector2(RB.velocity.x, -30f);
		}
	}
	void gravityScale() {
		if (dashing) return;
		if (RB.velocity.y < 0f) {
			RB.gravityScale = fallingG;
		} else {
			RB.gravityScale = basicG;
		}
	}
	public void InnerControl(Vector2 inputDirection, float magnitude) {
		if (!ControllableState()) return;
		if (dashing || lifeScript.panic) { XControl = 0; return; }
		float xVal = magnitude * inputDirection.x;
		if (Mathf.Abs(xVal) < 0.2f) {
			XControl = 0f; lateralMovement(); return;
		}
		XControl = Mathf.Abs(xVal) > 0.70f ? (xVal / Mathf.Abs(xVal)) * 1f : xVal;
		lateralMovement();
	}
	public void OuterControl(Vector2 inputDirection, float magnitude) {
		if (!ControllableState() || lifeScript.panic || !animator.enabled) return;
		DashControl = inputDirection;
		Dash();
	}
	public static bool ControllableState() {
		return (!GameStateManager.Paused && GameStateManager.InGame && !GameStateManager.changingRoom);
	}


	#region fear related methods
	public void panicAttack(float panicTime) {
		StartCoroutine(panicDarkness(panicTime));
	}
	public void recoveredFromPanicAttack() {
		audioScript.StopSound("player panicking", lifeScript.panicTime / 20f, true);
		torchLight.intensity = (float)playerObject.TorchIntensity * 0.5f * GameBuffsManager.TorchModifierMultiplier;
	}
	float panicMaxVol = 1f;
	IEnumerator panicDarkness(float panicTime) {
		float starttime = panicTime / 8f;
		float initialIntensity = torchLight.intensity;
		if (audioScript.CheckPlaying("player panicking")) {
			audioScript.changeVolume("player panicking", panicMaxVol, true);
		} else {
			audioScript.PlaySound("player panicking", panicTime / 2f, true, panicMaxVol);
		}
		while (starttime > 0f) {
			if (currentAnimation != "Fear") changeAnimation("Fear");
			float ratio = (panicTime / 8f - starttime) / (panicTime / 8f);
			torchLight.intensity = Mathf.Lerp(initialIntensity, 0f, ratio);
			starttime -= Time.deltaTime;
			yield return null;
		}
		torchLight.intensity = 0f;
	}
	#endregion



	#region animation related
	Coroutine _idleBored = null;
	public void changeAnimation(string name, float transitionDuration = 0f) {
		if (!animator.enabled) return;
		if (name.Substring(name.Length - Mathf.Min(3, name.Length)) != "III" && _idleBored != null) {
			stopBored();
		}
		if (name == currentAnimation) return;
		if (transitionDuration == 0f) {
			animator.Play(name);
		} else {
			animator.CrossFade(name, transitionDuration);
		}
		currentAnimation = name;
		walkAndRunSounds();
	}
	void stopOnGround() {
		if (grounded && XControl == 0 && !dashing) {
			RB.velocity = Vector3.zero;
		}
	}
	void setAnimation() {
		if (lifeScript.panic || !ControllableState()) return;
		setSpriteDirection();
		if (grounded) {
			if (RB.velocity.magnitude == 0f && _idleBored == null) {
				changeAnimation("Idle", 0.3f);
				bored();
				return;
			}
		} else {
			if (RB.velocity.y >= 0) {
				changeAnimation("Fly");
			} else {
				changeAnimation("Fall");
			}
		}
	}
	bool faceRight = true;
	[SerializeField] Transform SpriteRenderTransform;
	Vector2 rightFacingVector = new Vector3(-1f, 1f, 1f);
	Vector2 leftFacingVector = new Vector3(1f, 1f, 1f);
	void setSpriteDirection() {
		if (!animator.enabled) return;
		if (RB.velocity.x > 0f) {
			bool right = true;
			if (right == faceRight) return;
			faceRight = true;
		} else if (RB.velocity.x < 0f) {
			bool right = false;
			if (right == faceRight) return;
			faceRight = false;
		} else {
			return;
		}
		if (faceRight) {
			SpriteRenderTransform.localScale = rightFacingVector;
		} else {
			SpriteRenderTransform.localScale = leftFacingVector;
		}
	}

	IEnumerator boredIdle() {
		yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 8f));
		if (ControllableState() && !lifeScript.panic && currentAnimation == "Idle") {
			changeAnimation("IdleSneezeIII", 0.2f);
			while (!animator.GetCurrentAnimatorStateInfo(0).IsName("IdleSneezeIII") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
				yield return null;
			}
			_idleBored = null;
		}
	}
	void bored() {
		if (ControllableState() && !lifeScript.panic && currentAnimation == "Idle" && _idleBored == null) {
			_idleBored = StartCoroutine(boredIdle());
		}
	}
	void animationFix() {
		if (currentAnimation != "Idle") changeAnimation("Idle");
		currentAnimation = "Idle";
		faceRight = true;
		SpriteRenderTransform.localScale = rightFacingVector;
		if (_idleBored != null) stopBored();
	}
	void stopBored() {
		StopCoroutine(_idleBored);
		_idleBored = null;
	}

	Vector3 prevPos = Vector3.zero;
	bool wasWalking = false;
	bool wasRunning = false;
	int walkCycle = 1;
	int runCycle = 1;
	void continuousWalkAndRunSounds() {
		if (wasWalking && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= walkCycle) {
			oneOffSound(PlayerAudio.audioType.walk, true);
			walkCycle++;
			runCycle = 1;
			return;
		} else if (wasRunning && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= runCycle) {
			oneOffSound(PlayerAudio.audioType.run, true);
			runCycle++;
			walkCycle = 1;
			return;
		}
		if (!wasWalking) walkCycle = 1;
		if (!wasRunning) runCycle = 1;
	}
	void walkAndRunSounds() {
		if (currentAnimation == "Walk" && !wasWalking) {
			wasWalking = true;
			wasRunning = false;
			oneOffSound(PlayerAudio.audioType.walk, true);
		} else if (currentAnimation == "Run" && !wasRunning) {
			wasRunning = true;
			wasWalking = false;
			oneOffSound(PlayerAudio.audioType.run, true);
		} else {
			wasRunning = false;
			wasWalking = false;
		}
	}
	[SerializeField] PlayerAudio audioScript;

	bool tempStopStepSound = false;
	float blockDir = 0f;
	public void oneOffSound(PlayerAudio.audioType type, bool walkOrRun = false) {
		bool soundFootsteps = true;
		soundFootsteps = (prevPos - transform.position).magnitude >= playerSpeed * 0.1f * Time.deltaTime ? true : false;
		if (!soundFootsteps && tempStopStepSound != soundFootsteps) {
			blockDir = XControl;
		} else if (soundFootsteps && tempStopStepSound != soundFootsteps) {
			blockDir = 0f;
		}
		tempStopStepSound = soundFootsteps;
		prevPos = transform.position;
		if (Mathf.Sign(blockDir) != Mathf.Sign(XControl) && blockDir != 0f) {
			blockDir = 0f;
			soundFootsteps = true;
		}
		if (!walkOrRun) soundFootsteps = true;
		audioScript.playOneShotAudio(type, soundFootsteps);
	}
	#endregion
}
