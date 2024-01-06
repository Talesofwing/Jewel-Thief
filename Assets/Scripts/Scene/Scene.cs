using UnityEngine;

public abstract class Scene : MonoBehaviour{

	// 開發時大小
	[SerializeField] protected float devH;
	[SerializeField] protected float devW;

	// 比例
	protected float scalingH;
	protected float scalingW;

	// 實際
	protected float screenH;
	protected float screenW;

	public void Setup (float w, float h) {
		screenH = h;
		screenW = w;

		CaculateScaling ();
	}

	private void CaculateScaling () {
		scalingH = screenH / devH;
		scalingW = screenW / devW;
	}

}
