using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.iOS;
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
	float dashSpeed = 20f, dashCooldown = 0.6f, playerSpeed = 8f;
	[SerializeField] Rigidbody2D RB;
	Coroutine dashingRoutine;

	void Awake() {
		GameStateManager.GameStart += SetupStats;
		GameStateManager.GameEnd += SetupStats;
		GameStateManager.GameEnd += EndGame;
		GameStateManager.StartNewRoom += NewRoomPosition;
	}
	void NewRoomPosition() {
		transform.position = Camera.main.transform.position = new Vector3(0f, 1f, 0f);
		RB.velocity = Vector2.zero;
		changeAnimation("Idle");
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
			RB.AddForce(new Vector2(XControl * playerSpeed * forceMultiplier * GameBuffsManager.PlayerSpeedMultiplier, 0f), ForceMode2D.Force);
		} else {
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
			RB.gravityScale = 0f;
			RB.velocity = Vector2.zero;
			RB.velocity = dashSpeed * DashControl * GameBuffsManager.PlayerSpeedMultiplier;
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
	void OnTriggerEnter2D(Collider2D collider) {
		if (droppingRoutine == null || collider.gameObject.layer != groundedMask || collider.gameObject.name != "Background") return;
		StopDropping();
	}
	[SerializeField] Collider2D playerColl;
	bool checkGrounded() {
		float xSize = playerColl.bounds.size.x;
		float ySize = playerColl.bounds.size.y;
		RaycastHit2D groundCastBottom = Physics2D.BoxCast((Vector2)playerColl.bounds.center - new Vector2(0f, ySize / 2f), new Vector2(xSize, 0.02f), 0f, Vector2.down, 0.01f, groundedMask);
		RaycastHit2D groundCastTop = Physics2D.BoxCast((Vector2)playerColl.bounds.center - new Vector2(0f, ySize / 2f - 0.02f), new Vector2(xSize, 0.02f), 0f, Vector2.down, 0.01f, groundedMask);
		if (RB.velocity.y <= 0f && groundCastBottom && !groundCastTop) {
			return true;
		} else {
			return false;
		}
	}
	Coroutine droppingRoutine = null;
	public bool HaltUsed = false;
	public float haltFactor = 0.4f;
	public void HaltOrDrop() {
		if (HaltUsed || !ControllableState() || lifeScript.panic) return;
		HaltUsed = true;
		if (dashingRoutine != null) StopCoroutine(dashingRoutine);
		endDashChanges();
		if (grounded) {
			droppingRoutine = StartCoroutine(dropThroughPlatform());
		} else {
			RB.velocity = haltFactor * RB.velocity;
		}

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
		if (!ControllableState() || lifeScript.panic) return;
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
		torchLight.intensity = (float)playerObject.TorchIntensity * 0.5f * GameBuffsManager.TorchModifierMultiplier;
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
		if (grounded) {
			if (XControl == 0) RB.velocity = Vector3.zero;
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

	#endregion
}
