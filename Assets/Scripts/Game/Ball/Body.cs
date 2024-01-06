using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {

	public Animator animator;
	public CircleCollider2D col;

	void OnTriggerEnter2D (Collider2D other) {

		if (! LevelController.running)
			return;

		if (other.CompareTag ("Treasure") || other.CompareTag ("LoseArea")) {
			LevelController.Instance.ImpactLose ();
		}

	}

}
