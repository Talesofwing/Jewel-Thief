using UnityEngine;

public class Hand : MonoBehaviour {

	[SerializeField] private GameObject offHand;		// 抓到寶石的手
	[SerializeField] private GameObject onHand;			// 沒抓到寶石的手

	void OnTriggerEnter2D (Collider2D other) {

		if (! LevelController.running)
			return;

		// 如果碰到嬴的區域,即嬴
		if (other.CompareTag ("Treasure")) {
			LevelController.Instance.LevelWin ();
			return;
		}

		// 如果碰到輸的區域,即輸
		if (other.CompareTag ("LoseArea")) {
			LevelController.Instance.ImpactLose ();
			return;
		}
			
	}

	// 抓取寶石
	public void CatchJewel () {
		// 顯示抓到寶石的手
		// 隱藏沒抓到寶石的手
		offHand.SetActive (true);
		onHand.SetActive (false);
	}

	// 顯示手
	public void ShowHand () {
		onHand.SetActive (true);
	}

}
