using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	#region 
	[SerializeField]
	float jumpPower, playerSpeed;




	#endregion




	float XControl;
	float YControl;
	bool dashing = false;
	bool grounded = false;
	bool tempGrounded = false;
	int dashCharge = 0;
	[SerializeField] LayerMask groundedMask;
	[SerializeField] float basicG = 5f, fallingG = 10f;
	Rigidbody2D RB;
	void Awake() {
		RB = gameObject.GetComponent<Rigidbody2D>();
	}
	void Update() {
		XControl = Input.GetAxisRaw("Horizontal");
		YControl = Input.GetAxisRaw("Vertical");
		grounded = checkGrounded();
		checkDashAdd();
		gravityScale();
		clampFallSpeed();
	}

	void FixedUpdate() {
		RB.velocity = new Vector2(XControl * playerSpeed, RB.velocity.y);
		if (dashCharge > 0 && YControl > 0 && !dashing) {
			RB.gravityScale = basicG;
			if (RB.velocity.y < 0) { RB.velocity = new Vector3(RB.velocity.x, 0f, 0f); }
			RB.AddForce(new Vector2(0, YControl * RB.mass * RB.gravityScale * jumpPower), ForceMode2D.Impulse);
			dashCharge--;
			dashing = true;
			Invoke("notDashingChange", 0.25f);
		}
	}
	bool checkGrounded() {
		Collider2D playerColl = gameObject.GetComponent<Collider2D>();
		return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.02f, groundedMask);
	}
	void notDashingChange() {
		dashing = false;
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
		if (RB.velocity.y < 0f) {
			RB.gravityScale = fallingG;
		} else {
			RB.gravityScale = basicG;
		}
	}
}
