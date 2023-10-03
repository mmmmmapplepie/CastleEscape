using System.Collections;
using UnityEngine;
public class PlayerMovement : MonoBehaviour, JoystickController {
	float XControl; Vector2 DashControl;
	bool dashAble = false; bool grounded = false; bool tempGrounded = false; bool dashing = false;
	int dashCharge = 0;
	[SerializeField] LayerMask groundedMask;
	[SerializeField] float basicG = 5f, fallingG = 10f, dashCooldown = 0.5f, dashSpeed = 20f, dashTime = 0.2f, playerSpeed = 8f;
	Rigidbody2D RB;
	Coroutine dashingRoutine;
	void Awake() {
		RB = gameObject.GetComponent<Rigidbody2D>();
	}
	void Update() {
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
			// print(XControl);
			RB.velocity = new Vector2(XControl * playerSpeed, RB.velocity.y);
		}
	}
	void Dash() {
		if (dashCharge > 0 && DashControl.magnitude != 0 && !dashAble) {
			if (dashingRoutine != null) {
				StopCoroutine(dashingRoutine);
			}
			checkDashIntoGround();
			RB.gravityScale = 0f;
			RB.velocity = Vector2.zero;
			RB.velocity = dashSpeed * DashControl;
			RB.drag = 0f;
			dashCharge--;
			dashAble = true;
			dashing = true;
			dashingRoutine = StartCoroutine(dashingEnd());
			StartCoroutine(notDashingChange());
		}
	}
	void checkDashIntoGround() {
		if (DashControl.y <= 0f && grounded == true) {
			dashCharge++;
		}
	}
	bool checkGrounded() {
		Collider2D playerColl = gameObject.GetComponent<Collider2D>();
		return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.02f, groundedMask);
	}
	IEnumerator dashingEnd() {
		yield return new WaitForSeconds(dashTime);
		RB.drag = 2f;
		dashing = false;
	}
	IEnumerator notDashingChange() {
		yield return new WaitForSeconds(dashCooldown);
		dashAble = false;
	}
	void checkDashAdd() {
		if (grounded == false) {
			tempGrounded = false;
		}
		if (grounded == true && tempGrounded == false && RB.velocity.y <= 0f) {
			tempGrounded = true;
			dashCharge++;
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
		if (dashing) return;
		if (Mathf.Abs(inputDirection.x * magnitude) < 0.3f) { XControl = 0f; lateralMovement(); return; }
		float x = inputDirection.x;
		XControl = Mathf.Abs(x) > 0.5f ? (x / Mathf.Abs(x)) * 1f : (x) / 0.5f;
		lateralMovement();
	}
	public void OuterControl(Vector2 inputDirection, float magnitude) {
		DashControl = inputDirection;
		Dash();
	}

}
