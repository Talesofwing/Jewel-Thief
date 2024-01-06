using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class About : MonoBehaviour {

	[SerializeField] private Image authorList;				// 作者名單
	[SerializeField] private Image background;				// 背景
	[SerializeField] private AnimationInfo animationInfo;

	private Color backgroundColor;							// 背景顏色
	private Color authorListColor;							// 作者名單的顏色

	private Color backgroundFadeColor;						// 背景隱藏顏色
	private Color authorListFadeColor;					 	// 作者名單隱藏顏色

	private float exitDuration;

	private bool inAnim;

	public void Setup () {
		backgroundColor = background.color;
		backgroundFadeColor = backgroundColor;
		backgroundFadeColor.a = 0f;

		authorListColor = authorList.color;
		authorListFadeColor = authorListColor;
		authorListFadeColor.a = 0f;

		background.color = backgroundFadeColor;
		authorList.color = authorListFadeColor;
	}

	public void StartFadeInAnimation () {
		StartCoroutine (BeginningFadeInAnimation ());
	}

	// 淡入動畫
	IEnumerator BeginningFadeInAnimation () {
		inAnim = true;
		float timer = 0f;

		while (timer <= animationInfo.duration ) {

			if (! inAnim)
				yield return false;

			timer += Time.deltaTime * animationInfo.speed;
			exitDuration += Time.deltaTime * animationInfo.speed;
			background.color = Color.Lerp (backgroundFadeColor, backgroundColor, timer / animationInfo.duration);
			authorList.color = Color.Lerp (authorListFadeColor, authorListColor, timer / animationInfo.duration);
			yield return null;
		}

		inAnim = false;
	}

	public void StartFadeOutAnimation () {
		inAnim = false;
		StartCoroutine (BeginningFadeOutAnimation ());
	}

	// 淡出動畫
	IEnumerator BeginningFadeOutAnimation () {
		float timer = 0f;

		if (exitDuration > animationInfo.duration)
			exitDuration = animationInfo.duration;

		Color newBackgroundColor = background.color;
		Color newAuthorListColor = authorList.color;

		while (timer <= exitDuration) {
			timer += Time.deltaTime * animationInfo.speed;
			background.color = Color.Lerp (newBackgroundColor, backgroundFadeColor, timer / exitDuration);
			authorList.color = Color.Lerp (newAuthorListColor, authorListFadeColor, timer / exitDuration);
			yield return null;
		}

		SceneManager.UnloadScene ("About");
	}

}
