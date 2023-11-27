using UnityEngine;

public class Guide : MonoBehaviour {
	[SerializeField] GameObject guidePanel;

	public void OpenGuide() {
		guidePanel.SetActive(true);
	}
	public void CloseGuide() {
		guidePanel.SetActive(false);
	}
}
