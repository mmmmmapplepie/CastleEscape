using UnityEngine;

public class Guide : MonoBehaviour {
	[SerializeField] GameObject guidePanel;

	public void OpenGuide() {
		UIStaticAccess.playClick();
		guidePanel.SetActive(true);
	}
	public void CloseGuide() {
		UIStaticAccess.playClick();
		guidePanel.SetActive(false);
	}
}
