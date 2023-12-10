using System.Collections;
using UnityEngine;
public class MonsterBase : MonoBehaviour {
	[HideInInspector] public float senseRange = 1f;
	[SerializeField] protected Monster monsterStats;
	[SerializeField] protected SpriteRenderer faceGlow;
	public Rigidbody2D RB;
	[SerializeField] Animator ani;
	protected float moveSpeed = 1f;
	protected float damage = 1f;
	protected float senseTime = 2.5f;
	protected Transform player = null;
	Vector3 baseScale;
	Color hue;
	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, senseRange);
	}
	void Awake() {
		player = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
		baseScale = transform.localScale;
		moveSpeed = monsterStats.Speed * 1.25f;
		senseRange = monsterStats.Senses + 7f;
		senseTime = 0.5f * (6f - monsterStats.Hunger) - 0.25f;
		hue = monsterStats.Color;
		GetComponent<SpriteRenderer>().color = hue;
		monsterAudio = GetComponent<MonsterAudio>();
		ChangeFaceLight(0f);
		if (monsterStats.Damage < 2) {
			damage = monsterStats.Damage;
		} else {
			damage = monsterStats.Damage * 2f;
		}
		AwakeMethod();
	}
	protected virtual void AwakeMethod() { }
	protected Coroutine SensingRoutineHolder = null;
	protected Coroutine MovingRoutineHolder = null;
	protected Coroutine ChasingRoutineHolder = null;
	Coroutine calmingRoutine = null;
	void stopRoutine(ref Coroutine routine) {
		if (routine == null) return;
		if (routine == MovingRoutineHolder) ani.speed = 1f;
		StopCoroutine(routine);
		routine = null;
	}
	void Update() {
		checkPrey();
		naturalStroll();
		spriteDirection();
		UpdateMethod();
	}
	protected virtual void UpdateMethod() { }
	void checkPrey() {
		if (ChasingRoutineHolder != null || SensingRoutineHolder != null) return;
		if (PreyInRange(senseRange)) {
			SensingRoutineHolder = StartCoroutine(SensingRoutine());
		}
	}
	void ChangeFaceLight(float alpha) {
		faceGlow.color = new Color(hue.r, hue.g, hue.b, alpha);
	}


	public bool PreyInRange(float range) {
		if (!GameStateManager.InGame) return false;
		return (player.position - transform.position).magnitude <= range ? true : false;
	}
	void naturalStroll() {
		if (ChasingRoutineHolder != null || MovingRoutineHolder != null) return;
		MovingRoutineHolder = StartCoroutine(MovingRoutine());
	}
	IEnumerator MovingRoutine() {
		monsterAudio.PlayIdleSound();
		yield return new WaitForSeconds(Random.Range(0, 5f) / (moveSpeed * GameBuffsManager.EnemySpeedMultiplier));
		RB.velocity = (new Vector3(NZR(), NZR(), 0f)).normalized * moveSpeed * GameBuffsManager.EnemySpeedMultiplier;
		ani.speed = 2f * GameBuffsManager.EnemySpeedMultiplier;
		yield return new WaitForSeconds(Random.Range(0f, 5f));
		RB.velocity = Vector3.zero;
		ani.speed = 1f;
		monsterAudio.PlayIdleSound();
		stopRoutine(ref MovingRoutineHolder);
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
		float remainingTime = Random.Range(senseTime, senseTime * 1.5f);
		while (remainingTime > 0f) {
			if (!PreyInRange(senseRange)) {
				SensingRoutineHolder = null;
				yield break;
			}
			remainingTime -= Time.deltaTime * GameBuffsManager.EnemySpeedMultiplier;
			yield return null;
		}
		ChangeFaceLight(1f);
		ChasingRoutineHolder = StartCoroutine(ChasingRoutine());
		monsterAudio.PlayChaseSound();
		ani.Play("Chase");
		stopRoutine(ref calmingRoutine);
		stopRoutine(ref MovingRoutineHolder);
		stopRoutine(ref SensingRoutineHolder);
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
		float angle = moveDir.x == 0 ? (moveDir.y == 0 ? RZ() : (moveDir.y > 0 ? (facingLeft == true ? -maxRotation : maxRotation) : (facingLeft == true ? maxRotation : -maxRotation))) : 180f * Mathf.Atan(moveDir.y / moveDir.x) / Mathf.PI;
		angle = Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), maxRotation);
		SetRotation(angle);
	}

	float RZ() {
		return transform.rotation.z;
	}
	protected void SetRotation(float angle) {
		transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}

	protected void StopChase() {
		stopRoutine(ref SensingRoutineHolder);
		stopRoutine(ref ChasingRoutineHolder);
		stopRoutine(ref MovingRoutineHolder);
		calmingRoutine = StartCoroutine(calmingDown());
	}
	[HideInInspector] public float calmTime = 0.5f;
	IEnumerator calmingDown() {
		ani.Play("Idle");
		float time = calmTime;
		float initialIntensity = faceGlow.color.a;
		while (time > 0f) {
			ChangeFaceLight(initialIntensity * time / calmTime);
			time -= Time.deltaTime;
			yield return null;
		}
		ChangeFaceLight(0f);
	}
	protected bool InOuterArea = false;
	void OnTriggerEnter2D(Collider2D coll) {
		if (triggerEnter && coll.gameObject.tag == "bGround") { enterOuterWall(coll); InOuterArea = true; }
		if (coll.gameObject.tag == "Player" && damagePlayerOnCollision) damagePlayer();
	}
	void OnTriggerExit2D(Collider2D coll) {
		if (triggerEnter && coll.gameObject.tag == "bGround") { enterOuterWall(coll); InOuterArea = false; }
	}
	void enterOuterWall(Collider2D coll) {
		HitOuterWall();
		reflectFromWall(coll);
	}
	void reflectFromWall(Collider2D coll) {
		Vector2 normal = ((Vector2)transform.position - coll.ClosestPoint(transform.position)).normalized;
		if (normal == Vector2.zero) normal = -transform.position.normalized;
		float dot = Vector2.Dot(normal, RB.velocity);
		float dotNormal = RB.velocity.magnitude > 0f ? dot / RB.velocity.magnitude : 1f;
		if (dot >= 0f) {
			if (!InOuterArea || dotNormal > 0.87f) return;
			setMovement(-transform.position);
			return;
		}
		Vector2 final = RB.velocity - 2f * dot * normal;
		RB.velocity = final;
		spriteDirection();
	}
	void OnTriggerStay2D(Collider2D coll) {
		if (triggerStay && coll.gameObject.tag == "bGround") { reflectFromWall(coll); }
		if (coll.gameObject.tag == "Player" && damagePlayerOnCollision) damagePlayer();
		OnTriggerMethod2D(coll);
	}

	protected virtual void OnTriggerMethod2D(Collider2D coll) {
	}
	protected virtual void damagePlayer() {
		if (GameStateManager.changingRoom) return;
		int dmg = Mathf.Min(-1, Mathf.RoundToInt(-damage * GameBuffsManager.EnemyDamageMultiplier));
		damageAmountAndTime(dmg);
	}
	protected virtual void damageAmountAndTime(float dmg, float time = 2f) {
		player.root.gameObject.GetComponent<PlayerLife>().changeHealth((int)dmg, time);
	}
	bool damagePlayerOnCollision = true;
	protected bool triggerEnter = true;
	protected bool triggerStay = true;
	protected void DamageWhileColliding(bool dmgOnCollision = true) {
		damagePlayerOnCollision = dmgOnCollision;
	}
	protected virtual void HitOuterWall() { }









	MonsterAudio monsterAudio;

}
