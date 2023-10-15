using System.Collections;
using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour {
	void Awake() {
		GameStateManager.GameEnd += HideMenu;
		GameStateManager.GameStart += DashRecharge;
		GameStateManager.GameStart += SetLife;
	}
	void HideMenu() {
		gameObject.SetActive(false);
		charging = false;
		StopAllCoroutines();
	}


	[SerializeField] PlayerMovement movementScript;
	[SerializeField] PlayerLife lifeScript;
	[SerializeField] TextMeshProUGUI remainingDash;
	[SerializeField] Transform healthContainer;
	[SerializeField] GameObject healthPrefab;
	int dashChargeTemp = 0;
	int healthTemp = 0;
	void Update() {
		RenderDashCharges();
		RenderHealth();
	}

	#region healthRelated
	void clearContainer() {
		foreach (Transform obj in healthContainer) {
			Destroy(obj.gameObject);
		}
	}
	void SetLife() {
		clearContainer();
		healthTemp = CurrentSettings.CurrentPlayerType.MaxHealth;
		ChangeHealth(healthTemp);
	}
	void ChangeHealth(int change) {
		if (change > 0) {
			for (int i = 0; i < change; i++) {
				Instantiate(healthPrefab, healthContainer);
			}
		} else {
			for (int i = 0; i < change; i++) {
				Destroy(healthContainer.GetChild(i).gameObject);
			}
		}
	}
	void RenderHealth() {
		int change = CalculateLifeChange();
		if (change == 0) return;
		ChangeHealth(change);
	}
	int CalculateLifeChange() {
		int realHealth = lifeScript.Health;
		if (healthTemp != realHealth) {
			int change = realHealth - healthTemp;
			healthTemp = realHealth;
			return change;
		} else {
			return 0;
		}
	}

	#endregion


	#region dashChargeRelated
	void RenderDashCharges() {
		if (dashChargeTemp != movementScript.dashCharge) {
			dashChargeTemp = movementScript.dashCharge;
			remainingDash.text = dashChargeTemp.ToString();
		}
	}
	void DashRecharge() {
		StartCoroutine(DashRechargeRoutine());
	}
	int chargeMax;
	float regenTime = 1f;
	Vector2 sliderSize = Vector2.zero;
	bool charging = false;
	[SerializeField] RectTransform background;
	[SerializeField] RectTransform chargeTimerSlider;
	IEnumerator DashRechargeRoutine() {
		Coroutine chargingRoutine = null;
		sliderSize.x = background.rect.width;
		sliderSize.y = background.rect.height;
		chargeMax = CurrentSettings.CurrentPlayerType.DashStamina + 1;
		regenTime = -(CurrentSettings.CurrentPlayerType.DashRegeneration * 1.5f / 9f) + (2f + 1.5f / 9f);
		while (true) {
			if (movementScript.dashCharge == chargeMax) {
				charging = false;
				chargeTimerSlider.sizeDelta = sliderSize;
				if (chargingRoutine != null) { StopCoroutine(chargingRoutine); }
				yield return null;
				continue;
			}
			if (charging) {
				yield return null;
				continue;
			}
			if (movementScript.dashCharge < chargeMax) {
				charging = true;
				chargingRoutine = StartCoroutine(DashCharge());
				yield return null;
			}
		}
	}
	IEnumerator DashCharge() {
		float startTime = Time.time;
		while (Time.time < startTime + regenTime) {
			float ratio = (Time.time - startTime) / regenTime;
			chargeTimerSlider.sizeDelta = new Vector2(ratio * sliderSize.x, sliderSize.y);
			yield return null;
		}
		movementScript.dashCharge++;
		charging = false;
	}
	#endregion





}
