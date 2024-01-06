using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Launch : MonoBehaviour {

	[SerializeField] AnimationInfo nameAnimationInfo;
	[SerializeField] AnimationInfo logoAnimationInfo;
	[SerializeField] AnimationInfo hideAnimationInfo;

	[SerializeField] private Image logo;
	[SerializeField] private Image nameImage;

	[SerializeField] private float offsetY;
	[SerializeField] private float delayTime;

	private Color logoColor;
	private Color fadeColor;

	public void Layout () {
		logoColor = logo.color;
		fadeColor = logoColor;
		fadeColor.a = 0f;
		logo.color = fadeColor;
	}

	public void StartLaunchAnimation () {
		StartCoroutine (BeginningLaunchAnimation ());
	}

	// 動畫
	IEnumerator BeginningLaunchAnimation () {
		// logo動畫
		float timer = 0f;
		while (timer <= logoAnimationInfo.duration) {
			timer += Time.deltaTime * logoAnimationInfo.speed;
			logo.color = Color.Lerp (fadeColor, logoColor, timer / logoAnimationInfo.duration);
			yield return null;
		}
		// name動畫
		Vector3 newNamePos = nameImage.transform.localPosition;
		newNamePos.y += offsetY;
		timer = 0;
		while (timer <= nameAnimationInfo.duration) {
			timer += Time.deltaTime * nameAnimationInfo.speed;
			nameImage.transform.localPosition = Vector3.Lerp (nameImage.transform.localPosition, newNamePos, timer / nameAnimationInfo.duration);
			yield return null;
		}

		yield return new WaitForSeconds (delayTime);

		// 一起消失
		timer = 0;
		bool b = false;
		while (timer <= hideAnimationInfo.duration) {
			timer += Time.deltaTime * hideAnimationInfo.speed;
			logo.color = Color.Lerp (logo.color, fadeColor, timer / hideAnimationInfo.duration);
			nameImage.color = Color.Lerp (nameImage.color, fadeColor, timer / hideAnimationInfo.duration);

			// 當動畫執行到一半時,就開始跳轉到下一個場景
			if (timer >= hideAnimationInfo.duration / 2 && !b) {
				NextScene ();
				b = true;
			}

			yield return null;
		}



	}

	void NextScene () {
		
		if (SaveController.IsNewPlayer) {
			HelpController.Show ();
			SceneManager.LoadSceneAsync ("Menu", LoadSceneMode.Additive);
		} else {
			SceneManager.LoadSceneAsync ("Menu", LoadSceneMode.Additive);
		}

	}

}
