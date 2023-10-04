using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	[SerializeField] float maxoffset, smoothTime;

	Transform player = null;
	Coroutine currentSmooth;
	Coroutine noMovementRoutine;
	Coroutine subNoMovement;
	void Start() {
		setPlayerToFollow();
	}
	public void setPlayerToFollow() {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		if (player != null) {
			currentSmooth = StartCoroutine(cameraMove());
		}
	}
	Vector3 getPlayerPosition() {
		Vector3 pos = Vector3.zero;
		pos = player.transform.position;
		pos = new Vector3(pos.x, pos.y, -10f);
		return pos;
	}
	void LateUpdate() {
		cameraFollow();
	}
	void cameraFollow() {
		if (!GameStateManager.InGame) return;
		if (player == null) return;
		if (0.1f > Vector3.Distance(transform.position, getPlayerPosition())) return;
		if (maxoffset < Vector3.Distance(transform.position, getPlayerPosition())) {
			stopNoMovementRoutines();
			Vector3 newDir = getPlayerPosition() - transform.position;
			transform.position = getPlayerPosition() - maxoffset * newDir.normalized;
		}
		if (Vector3.Distance(transform.position, getPlayerPosition()) > maxoffset / 4f) {
			stopNoMovementRoutines();
			StopCoroutine(currentSmooth);
			currentSmooth = StartCoroutine(cameraMove());
		} else {
			noMovementRoutine = StartCoroutine(noMovementCameraMove());
		}
	}
	IEnumerator cameraMove() {
		float start = Time.unscaledTime;
		Vector3 startPos = transform.position;
		Vector3 finalPos = getPlayerPosition();
		while (Time.unscaledTime - start < smoothTime) {
			transform.position = Vector3.Lerp(startPos, finalPos, (Time.unscaledTime - start) / smoothTime);
			yield return null;
		}
	}
	IEnumerator noMovementCameraMove() {
		yield return new WaitForSecondsRealtime(0.25f);
		subNoMovement = StartCoroutine(cameraMove());
	}
	void stopNoMovementRoutines() {
		if (noMovementRoutine != null) {
			StopCoroutine(noMovementRoutine);
		}
		if (subNoMovement != null) {
			StopCoroutine(subNoMovement);
		}
	}
}
