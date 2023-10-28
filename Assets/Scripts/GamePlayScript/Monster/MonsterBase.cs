using System.Collections;
using UnityEngine;
public class MonsterBase : MonoBehaviour {
	public float senseRange = 2f;
	[SerializeField] Monster monsterStats;
	[SerializeField] SpriteRenderer faceGlow;
	public Rigidbody2D RB;
	[SerializeField] Animator ani;
	protected float moveSpeed = 5f;
	protected float damage = 1f;
	float senseTime = 5f;
	float losePlayerRangeMultiplier = 1f;
	protected Transform player = null;
	Vector3 baseScale;
	void Awake() {
		player = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
		baseScale = transform.localScale;
		moveSpeed = monsterStats.Speed * moveSpeed;
		senseRange = senseRange * monsterStats.Senses;
		senseTime = senseTime / monsterStats.Hunger;
		losePlayerRangeMultiplier = monsterStats.Hunger / 2f + 0.5f;
		GetComponent<SpriteRenderer>().color = faceGlow.color = monsterStats.Color;
		damage = monsterStats.Damage * 2f;
	}

	protected Coroutine SensingRoutineHolder = null;
	protected Coroutine MovingRoutineHolder = null;
	protected Coroutine ChasingRoutineHolder = null;
	void Update() {
		checkPrey();
		naturalStroll();
		spriteDirection();
	}
	//check if it senses player in range.
	void checkPrey() {
		//if in range start coroutine for the wait.
		if (ChasingRoutineHolder != null || SensingRoutineHolder != null) return;
		if (PreyInRange(senseRange)) {
			SensingRoutineHolder = StartCoroutine(SensingRoutine());
		}
	}
	protected bool PreyInRange(float range) {
		return (player.position - transform.position).magnitude <= range ? true : false;
	}
	void naturalStroll() {
		if (ChasingRoutineHolder != null || MovingRoutineHolder != null) return;
		MovingRoutineHolder = StartCoroutine(MovingRoutine());
	}
	IEnumerator MovingRoutine() {
		yield return new WaitForSeconds(Random.Range(0, 5f) / (moveSpeed * GameBuffsManager.EnemySpeedMultiplier));
		RB.velocity = (new Vector3(NZR(), NZR(), 0f)).normalized * moveSpeed * GameBuffsManager.EnemySpeedMultiplier;
		ani.speed = 2f * GameBuffsManager.EnemySpeedMultiplier;
		// yield return new WaitForSeconds(Random.Range(0f, 5f) / (moveSpeed * GameBuffsManager.EnemySpeedMultiplier));
		yield return new WaitForSeconds(Random.Range(0f, 5f));
		RB.velocity = Vector3.zero;
		ani.speed = 1f;
		MovingRoutineHolder = null;
	}

	//NZR non zero random.
	float NZR() {
		float v = 0;
		while (v == 0) {
			v = Random.Range(-1f, 1f);
		}
		return v;
	}
	IEnumerator SensingRoutine() {
		float remainingTime = senseTime;
		while (remainingTime > 0f) {
			if (!PreyInRange(senseRange)) {
				SensingRoutineHolder = null;
				yield break;
			}
			remainingTime -= Time.deltaTime * GameBuffsManager.EnemySpeedMultiplier;
			yield return null;
		}
		ani.Play("Chase");
		faceGlow.color = new Color(1f, 1f, 1f, 1f);
		ChasingRoutineHolder = StartCoroutine(ChasingRoutine());
		StopCoroutine(calmingRoutine);
		calmingRoutine = null;
		StopCoroutine(SensingRoutineHolder);
		SensingRoutineHolder = null;
		StopCoroutine(MovingRoutineHolder);
		MovingRoutineHolder = null;
	}


	//this method may be different for each monster. should be overridden.
	protected virtual IEnumerator ChasingRoutine() {
		yield return null;
		StopChase();
	}
	protected Vector3 direction;
	bool facingLeft = true;
	float maxRotation = 20f;
	float rotationReturnRate = 1f;
	void spriteDirection() {
		if (RB.velocity.x > 0) {
			transform.localScale = new Vector3(-baseScale.x, baseScale.y, baseScale.z);
			if (facingLeft) {
				SetRotation(-RZ());
				facingLeft = false;
			}
		} else if (RB.velocity.x < 0) {
			transform.localScale = new Vector3(baseScale.x, baseScale.y, baseScale.z);
			if (!facingLeft) {
				SetRotation(-RZ());
				facingLeft = true;
			}
		}
		if (ChasingRoutineHolder == null) {
			float CR = RZ();
			CR = CR > 0 ? CR - Time.deltaTime * rotationReturnRate : CR + Time.deltaTime * rotationReturnRate;
			SetRotation(CR);
		}
	}
	protected void setMovement(Vector3 moveDir, float multiplier = 1f) {
		RB.velocity = moveDir.normalized * multiplier * moveSpeed * GameBuffsManager.EnemySpeedMultiplier;
		spriteDirection();
		float angle = moveDir.x == 0 ? (moveDir.y == 0 ? RZ() : (facingLeft == true ? -maxRotation : maxRotation)) : Mathf.Atan(moveDir.y / moveDir.x);
		SetRotation(angle);
	}

	//get current z rotation
	float RZ() {
		return transform.rotation.z;
	}
	protected void SetRotation(float angle) {
		transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}

	Coroutine calmingRoutine = null;
	protected void StopChase() {
		StopCoroutine(SensingRoutineHolder);
		SensingRoutineHolder = null;
		StopCoroutine(ChasingRoutineHolder);
		ChasingRoutineHolder = null;
		StopCoroutine(MovingRoutineHolder);
		MovingRoutineHolder = null;
		calmingRoutine = StartCoroutine(calmingDown());
	}
	public float calmTime = 0.5f;
	IEnumerator calmingDown() {
		ani.Play("Idle");
		float time = calmTime;
		while (time > 0f) {
			faceGlow.color = new Color(1f, 1f, 1f, time / calmTime);
			time -= Time.deltaTime;
			yield return null;
		}
		faceGlow.color = new Color(1f, 1f, 1f, 0f);
	}
	void OnTriggerEnter2D(Collider2D coll) {
		checkWall(coll);
		if (coll.gameObject.tag != "Player") return;
		damagePlayer();
	}
	void checkWall(Collider2D coll) {
		if (coll.gameObject.tag == "bGround") {
			//need additional check so that it doesnt repeat glitching when stuck to wall back and forth.
			Vector2 normal = ((Vector2)transform.position - coll.ClosestPoint(transform.position)).normalized;
			float dot = Vector2.Dot(normal, RB.velocity);
			if (dot > 0f) return;
			Vector2 final = RB.velocity - 2f * dot * normal;
			RB.velocity = final;
			spriteDirection();
		}
	}
	void OnTriggerStay2D(Collider2D coll) {
		checkWall(coll);
		if (coll.gameObject.tag != "Player") return;
		damagePlayer();
	}
	protected virtual void damagePlayer() {
		player.root.gameObject.GetComponent<PlayerLife>().changeHealth((int)(damage * GameBuffsManager.EnemyDamageMultiplier));
	}
}
