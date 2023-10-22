using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore;

public class PlayerMovement : MonoBehaviour, JoystickController {
	public PlayerType playerObject;
	public Light2D torchLight;
	public Light2D auraLight;
	public PlayerLife lifeScript;
	[SerializeField] Animator animator;
	string currentAnimation = "";

	public void SetupStats() {
		RevertToIntialSettings();
		lifeScript.maxHealth = playerObject.MaxHealth * 2;
		lifeScript.Health = lifeScript.maxHealth;
		lifeScript.regen = playerObject.Regeneration;
		playerSpeed = (float)playerObject.Speed;
		dashSpeed = (float)playerObject.DashPower * 3f + 10f;
		PlatformController.luck = playerObject.Luck * 3;
		maxDash = playerObject.DashStamina + 1;
		dashCooldown = (0.6f - playerObject.DashStamina * 0.03f);
		setLightStrengths();
		changeSprite();
		animationFix();
	}
	void RevertToIntialSettings() {
		dashAble = true; grounded = false; tempGrounded = false; dashing = false; HaltUsed = false; dashCharge = 0;
		if (RB != null) { RB.gravityScale = basicG; RB.drag = 2f; }
		endDashChanges();
		transform.position = new Vector3(0f, 0.5f, 0f);
	}
	void changeSprite() {
		transform.Find("PlayerSprite").gameObject.GetComponent<SpriteRenderer>().color = playerObject.color;
	}
	void setLightStrengths() {
		torchLight.intensity = (float)playerObject.TorchIntensity * 0.5f;
		torchLight.pointLightOuterRadius = (float)playerObject.TorchIntensity * 2f + 3f;
		torchLight.pointLightInnerAngle = (float)playerObject.TorchWidth * 3f + 30f;
		torchLight.pointLightOuterAngle = (float)playerObject.TorchWidth * 10f + 50f;
		auraLight.pointLightOuterRadius = (float)playerObject.Aura + 10f;
	}

	float XControl; Vector2 DashControl;
	bool dashAble = true; public bool grounded = false; bool tempGrounded = false; public bool dashing = false;
	int maxDash = 1;

	//initially set to 0 as the player somehow gets a charge just as the game starts.
	int _dashCharge = 0;
	public int dashCharge {
		get { return _dashCharge; }
		set { _dashCharge = value > maxDash ? maxDash : value; }
	}
	[SerializeField] LayerMask groundedMask;
	[SerializeField] float basicG = 5f, fallingG = 10f, dashTime = 0.15f;
	float dashSpeed = 20f, dashCooldown = 0.6f, playerSpeed = 8f;
	[SerializeField] Rigidbody2D RB;
	Coroutine dashingRoutine;

	Animation ani = null;

	void Awake() {
		GameStateManager.GameStart += SetupStats;
		GameStateManager.GameEnd += SetupStats;
		GameStateManager.GameEnd += EndGame;
	}
	void EndGame() {
		StopAllCoroutines();
	}
	void Update() {
		if (!ControllableState()) return;
		grounded = checkGrounded();
		checkDashAdd();
		gravityScale();
		clampFallSpeed();
		setAnimation();
	}
	void lateralMovement() {
		if (XControl == 0) { RB.velocity = new Vector2(0f, RB.velocity.y); return; }
		if (!grounded) {
			float forceMultiplier = 1f;
			if ((RB.velocity.x > 0 && XControl < 0) || (RB.velocity.x < 0 && XControl > 0)) forceMultiplier = 3f;
			RB.AddForce(new Vector2(XControl * playerSpeed * forceMultiplier, 0f), ForceMode2D.Force);
		} else {
			RB.velocity = new Vector2(XControl * playerSpeed, 0f);
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
			RB.gravityScale = 0f;
			RB.velocity = Vector2.zero;
			RB.velocity = dashSpeed * DashControl;
			RB.drag = 0f;
			dashCharge--;
			dashAble = false;
			dashing = true;
			DashAnimation();
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
	bool checkGrounded() {
		Collider2D playerColl = gameObject.GetComponent<Collider2D>();
		return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.02f, groundedMask);
	}
	bool HaltUsed = false;
	public void HaltOrDrop() {
		if (HaltUsed || !ControllableState() || lifeScript.panic) return;
		HaltUsed = true;
		RB.velocity = Vector2.zero;
		if (dashingRoutine != null) StopCoroutine(dashingRoutine);
		endDashChanges();
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
			dashCharge++;
			HaltUsed = false;
		}
	}
	void clampFallSpeed() {
		if (RB.velocity.y < 0 && RB.velocity.y < -30f) {
			RB.velocity = new Vector3(RB.velocity.x, -30f, 0f);
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
		if (!ControllableState() || dashing || lifeScript.panic) return;
		if (Mathf.Abs(inputDirection.x * magnitude) < 0.2f) { XControl = 0f; lateralMovement(); return; }
		float x = inputDirection.x * magnitude;
		XControl = Mathf.Abs(x) > 0.70f ? (x / Mathf.Abs(x)) * 1f : x;
		lateralMovement();
	}
	public void OuterControl(Vector2 inputDirection, float magnitude) {
		if (!ControllableState() || lifeScript.panic) return;
		DashControl = inputDirection;
		Dash();
	}
	bool ControllableState() {
		return (!GameStateManager.Paused && GameStateManager.InGame);
	}


	#region fear related methods
	public void panicAttack(float panicTime) {
		StartCoroutine(panicDarkness(panicTime));
	}
	public void recoveredFromPanicAttack() {
		torchLight.intensity = (float)playerObject.TorchIntensity * 0.5f;
	}
	IEnumerator panicDarkness(float panicTime) {
		float starttime = Time.time;
		float initialIntensity = torchLight.intensity;
		while (Time.time < panicTime / 4f + starttime) {
			float ratio = (Time.time - starttime) / (panicTime / 4f);
			torchLight.intensity = Mathf.Lerp(initialIntensity, 0f, ratio);
			yield return null;
		}
		torchLight.intensity = 0f;
	}
	#endregion



	#region animation related
	Coroutine _idleBored = null;
	public void changeAnimation(string name, float transitionDuration = 0f) {
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
	}
	void setAnimation() {
		if (lifeScript.panic || !ControllableState()) return;
		setSpriteDirection();
		if (RB.velocity.magnitude == 0 && grounded && _idleBored == null) {
			if (XControl == 0) RB.velocity = Vector2.zero;
			changeAnimation("Idle", 0.3f);
			bored();
			return;
		}
		if (!grounded) {
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
		changeAnimation("Idle");
		currentAnimation = "Idle";
		faceRight = true;
		SpriteRenderTransform.localScale = rightFacingVector;
		if (_idleBored != null) stopBored();
	}
	void stopBored() {
		StopCoroutine(_idleBored);
		_idleBored = null;
	}

	#endregion
}
