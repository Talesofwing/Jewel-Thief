using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Settings : Scene {
	[HideInInspector] public bool _inAnim;

	[SerializeField] AudioClip enter;

	[SerializeField] private RectTransform settingsBtn;
	[SerializeField] private RectTransform bgmBtn;
	[SerializeField] private RectTransform sfxBtn;
	[SerializeField] private RectTransform homeBtn;
	[SerializeField] private RectTransform continueBtn;
	[SerializeField] private RectTransform background;
	[SerializeField] private RectTransform canvas;
	[SerializeField] private RectTransform container;		// 容器

	[SerializeField] private Image pauseText;
	[SerializeField] private Image helpBtn;

	[SerializeField] private GearAnimationInfo gear;
	[SerializeField] private SpringAnimationInfo scene;

	[SerializeField] private Sprite offBGMSprite;
	[SerializeField] private Sprite onBGMSprite;
	[SerializeField] private Sprite offSFXSprite;
	[SerializeField] private Sprite onSFXSprite;

	private float distance;

	private Color fadeColor;
	private Color showColor;

	public void Setup () {
		Setup (canvas.sizeDelta.x, canvas.sizeDelta.y);

		showColor = pauseText.color;
		fadeColor = pauseText.color;
		fadeColor.a = 0f;

		distance = canvas.sizeDelta.x;

		Layout ();
	}

	void Layout () {
		background.sizeDelta = canvas.sizeDelta;

		pauseText.color = fadeColor;
		helpBtn.color = fadeColor;

		background.localPosition = new Vector3 (background.localPosition.x - distance, background.localPosition.y, 0f);
		sfxBtn.localPosition = new Vector3 (sfxBtn.localPosition.x - distance, sfxBtn.localPosition.y, 0f);
		bgmBtn.localPosition = new Vector3 (bgmBtn.localPosition.x - distance, bgmBtn.localPosition.y, 0f);
		homeBtn.localPosition = new Vector3 (homeBtn.localPosition.x - distance, homeBtn.localPosition.y, 0f);
		continueBtn.localPosition = new Vector3 (continueBtn.localPosition.x - distance, continueBtn.localPosition.y, 0f);

		SetAudioImage ();
	}

	// 判斷音量是否啟動而切換BGM及SFX的图片
	public void SetAudioImage () {

		if (PlayerPrefs.GetFloat ("BGMVolume") == 1f) {
			bgmBtn.GetComponent <Image> ().sprite = onBGMSprite;
		} else {
			bgmBtn.GetComponent <Image> ().sprite = offBGMSprite;
		}

		if (PlayerPrefs.GetFloat ("SFXVolume") == 1f) {
			sfxBtn.GetComponent <Image> ().sprite = onSFXSprite;
		} else {
			sfxBtn.GetComponent <Image> ().sprite = offSFXSprite;
		}

	}

	public void StartEnterAnimation () {
		StartGearAnimation (RotationDirection.left);

		StartCoroutine (BeginningEnterAnimation ());
	}
		
	IEnumerator BeginningEnterAnimation () {
		_inAnim = true;

		float lastFrameTime = Time.realtimeSinceStartup;
		float deltaTime;
		float timer = 0f;

		float speed = scene.speed;
		float duration = scene.duration;
		float springX = scene.springDistance.x;

		Vector3 endPos = new Vector3 (0f, 0f, 0f);
		Vector3 endSpringPos = new Vector3 (0f, 0f, 0f);
		// 顯示背景
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			background.localPosition = Vector3.Lerp (background.localPosition, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}
		background.localPosition = endPos;

		// 顯示幫助
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			helpBtn.color = Color.Lerp (helpBtn.color, showColor, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}
		helpBtn.color = showColor;
	
		// 顯示暫停字樣
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			pauseText.color = Color.Lerp (pauseText.color, showColor, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}
		pauseText.color = showColor;

		// 顯示SFX按鈕
		endPos = sfxBtn.localPosition;
		endPos.x += distance;
		endSpringPos = endPos;
		endSpringPos.x += springX;
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			sfxBtn.localPosition = Vector3.Lerp (sfxBtn.localPosition, endSpringPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		SoundController.PlaySFX (enter);

		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			sfxBtn.localPosition = Vector3.Lerp (sfxBtn.localPosition, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}
			
		// 顯示BGM按鈕
		endPos = bgmBtn.localPosition;
		endPos.x += distance;
		endSpringPos = endPos;
		endSpringPos.x += springX;
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			bgmBtn.localPosition = Vector3.Lerp (bgmBtn.localPosition, endSpringPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		SoundController.PlaySFX (enter);

		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			bgmBtn.localPosition = Vector3.Lerp (bgmBtn.localPosition, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}
			
		// 顯示home按鈕
		endPos = homeBtn.localPosition;
		endPos.x += distance;
		endSpringPos = endPos;
		endSpringPos.x += springX;
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer +=deltaTime * speed;
			homeBtn.localPosition = Vector3.Lerp (homeBtn.localPosition, endSpringPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		SoundController.PlaySFX (enter);

		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer +=deltaTime * speed;
			homeBtn.localPosition = Vector3.Lerp (homeBtn.localPosition, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		// 顯示continue按鈕
		endPos = continueBtn.localPosition;
		endPos.x += distance;
		endSpringPos = endPos;
		endSpringPos.x += springX;
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			continueBtn.localPosition = Vector3.Lerp (continueBtn.localPosition, endSpringPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		SoundController.PlaySFX (enter);

		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			continueBtn.localPosition = Vector3.Lerp (continueBtn.localPosition, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		// 動畫結束
		_inAnim = false;
	}

	public void StartCloseAnimation () {
		StartGearAnimation (RotationDirection.right);

		StartCoroutine (BeginningCloseAnimation ());
	}

	// 隱藏動畫
	IEnumerator BeginningCloseAnimation () {
		_inAnim = true;

		float lastFrameTime = Time.realtimeSinceStartup;
		float deltaTime;
		float timer = 0f;
	
		float speed = scene.speed;
		float duration = scene.duration;
	
		Vector3 endPos;

		endPos = continueBtn.localPosition;
		endPos.x -= distance;
		// 隱藏continue按鈕
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			continueBtn.localPosition = Vector3.Lerp (continueBtn.localPosition, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		// 隱藏home按鈕
		endPos = homeBtn.localPosition;
		endPos.x -= distance;
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer +=deltaTime * speed;
			homeBtn.localPosition = Vector3.Lerp (endPos, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}
			
		// 隱藏BGM按鈕
		endPos = bgmBtn.localPosition;
		endPos.x -= distance;
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			bgmBtn.localPosition = Vector3.Lerp (bgmBtn.localPosition, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		// 隱藏SFX按鈕
		endPos = sfxBtn.localPosition;
		endPos.x -= distance;
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			sfxBtn.localPosition = Vector3.Lerp (sfxBtn.localPosition, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		// 隱藏暫停字樣
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			pauseText.color = Color.Lerp (pauseText.color, fadeColor, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		// 隱藏幫助
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			helpBtn.GetComponent <Image> ().color = Color.Lerp (helpBtn.color, fadeColor, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		// 隱藏背景
		endPos = background.localPosition;
		endPos.x -= distance;
		timer = 0f;
		while (timer <= duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer += deltaTime * speed;
			background.localPosition = Vector3.Lerp (background.localPosition, endPos, timer);
			lastFrameTime = Time.realtimeSinceStartup;
			yield return null;
		}

		GameController.ContinueGame ();
		SceneManager.UnloadScene ("Settings");
	}

	void StartGearAnimation (RotationDirection dir) {
		StartCoroutine (GearAnimation (dir));
	}

	// 齒輪動畫
	// dir 為方向, -1 : 右, +1 : 左
	IEnumerator GearAnimation (RotationDirection dir) {
		float timer = 0f;
		float deltaTime = 0f;
		float angle = gear.angle;
		float speed = gear.speed;
		float duration = gear.duration;
		float lastFrameTime = Time.realtimeSinceStartup;

		if (dir == RotationDirection.right)
			angle *= -1;

		angle += settingsBtn.localRotation.z;

		while (timer <= gear.duration) {
			deltaTime = Time.realtimeSinceStartup - lastFrameTime;
			timer = deltaTime * speed;
			settingsBtn.localRotation = Quaternion.Euler (0f, 0f, timer / duration * angle);
			yield return null;
		}

		settingsBtn.rotation = Quaternion.Euler (0f, 0f, angle);
	}

}
