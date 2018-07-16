using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollide : MonoBehaviour {
	public PlayerScript playerScript;
	public GameObject player;

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Ground") {
			playerScript.feetOnTheGround();
		}
	}
}
