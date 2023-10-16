using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class PlayerMovement : MonoBehaviour, JoystickController {
	public PlayerType playerObject;
	public Light2D torchLight;
	public Light2D auraLight;
	public PlayerLife lifeScript;


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
	bool dashAble = true; bool grounded = false; bool tempGrounded = false; bool dashing = false;
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
	Rigidbody2D RB;
	Coroutine dashingRoutine;



	void Awake() {
		RB = gameObject.GetComponent<Rigidbody2D>();
		GameStateManager.GameStart += SetupStats;
		GameStateManager.GameEnd += SetupStats;
	}
	void Update() {
		if (!ControllableState()) return;
		grounded = checkGrounded();
		checkDashAdd();
		gravityScale();
		clampFallSpeed();
	}
	void lateralMovement() {
		if (playerSpeed < Mathf.Abs(RB.velocity.x)) {
			float forceMultiplier = 1f;
			if ((RB.velocity.x > 0 && XControl < 0) || (RB.velocity.x < 0 && XControl > 0)) forceMultiplier = 3f;
			RB.AddForce(new Vector2(XControl * playerSpeed * forceMultiplier, 0f), ForceMode2D.Force);
		} else {
			RB.velocity = new Vector2(XControl * playerSpeed, RB.velocity.y);
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
			dashingRoutine = StartCoroutine(dashingEnd());
			StartCoroutine(notDashingChange());
		}
	}
	bool checkGrounded() {
		Collider2D playerColl = gameObject.GetComponent<Collider2D>();
		return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.02f, groundedMask);
	}
	bool HaltUsed = false;
	public void endDash() {
		if (HaltUsed || !ControllableState()) return;
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
		if (!ControllableState()) return;
		if (dashing) return;
		if (Mathf.Abs(inputDirection.x * magnitude) < 0.3f) { XControl = 0f; lateralMovement(); return; }
		float x = inputDirection.x;
		XControl = Mathf.Abs(x) > 0.5f ? (x / Mathf.Abs(x)) * 1f : (x) / 0.5f;
		lateralMovement();
	}
	public void OuterControl(Vector2 inputDirection, float magnitude) {
		if (!ControllableState()) return;
		DashControl = inputDirection;
		Dash();
	}
	bool ControllableState() {
		return (!GameStateManager.Paused && GameStateManager.InGame);
	}
}
