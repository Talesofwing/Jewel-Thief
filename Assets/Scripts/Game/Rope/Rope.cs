using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour {

	// 上升動畫
	IEnumerator UpAnimation () {
		// 結束高度 = 螢幕高度
		float endY = 20f;
		float timer = 0f;
		float endTime = 1.0f;
		float speed = 0.4f;
		Vector3 startPosition = transform.position;

		while (timer <= endTime) {
			timer += Time.deltaTime * speed;
			transform.position = Vector3.Lerp (startPosition, new Vector3 (0f, endY, 0f), timer);
			yield return null;
		}

	}

	// 開始上升動畫
	public void StartUpAnimation () {
		GetComponent <Transform> ().SetParent (null);

		StartCoroutine (UpAnimation ());
	}

}
