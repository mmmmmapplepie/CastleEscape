using System.Collections;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour {
	void Awake() {
		GameStateManager.GameEnd += HideMenu;
		GameStateManager.GameStart += DashRecharge;
		GameStateManager.GameStart += SetLife;
		setIntialSettings();
	}
	void setIntialSettings() {
		fearSliderWidth = fearbackground.rect.width;
		fearSliderHeight = fearbackground.rect.height;
		initialSliderColor = fearSlider.gameObject.GetComponent<Image>().color;
	}
	void HideMenu() {
		charging = false;
		StopAllCoroutines();
		gameObject.SetActive(false);
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
		RenderFear();
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


	[SerializeField] RectTransform fearbackground;
	[SerializeField] RectTransform fearSlider;
	[SerializeField] Color panickColor;
	float fearSliderWidth = 0f;
	float fearSliderHeight = 0f;
	Color initialSliderColor;

	void RenderFear() {
		fearSlider.sizeDelta = new Vector2((lifeScript.Fear / 100f) * fearSliderWidth, fearSliderHeight);
		if (lifeScript.Fear == 100f) {
			fearSlider.gameObject.GetComponent<Image>().color = panickColor;
			float buzz = 3f;
			Vector2 ranPos = new Vector2(Random.Range(buzz, -buzz), Random.Range(buzz, -buzz));
			fearbackground.anchoredPosition = fearSlider.anchoredPosition = ranPos;
		} else {
			fearSlider.gameObject.GetComponent<Image>().color = initialSliderColor;
			fearbackground.anchoredPosition = fearSlider.anchoredPosition = Vector2.zero;
		}

		
	}
}
