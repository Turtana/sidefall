using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollide : MonoBehaviour {
	public PlayerScript playerScript;

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Ground") {
			playerScript.feetOnTheGround(true);
		}
	}

	void OnCollisionExit (Collision other) {
		if (other.gameObject.tag == "Ground") {
			playerScript.feetOnTheGround(false);
		}
	}
}
