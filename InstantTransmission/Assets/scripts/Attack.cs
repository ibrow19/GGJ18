using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

	private bool active = false;

	private PlayerController target;
	public PlayerController p;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	//void OnTriggerEnter2D(Collider2D other) {

	//	if (other.gameObject == target.gameObject && active) {
	//		Debug.Log ("Start");
	//	}

	//}

	void OnTriggerStay2D(Collider2D other) {

		if (other.gameObject == target.t.gameObject && active) {
			Vector3 direction = target.t.transform.position - p.t.transform.position;
			target.hit (new Vector2(direction.x, direction.y));
		}

	}

	public void setActive(bool active) {
		Debug.Log ("Attacking");
		this.active = active;
	}

	public void setTarget(PlayerController target) {
		this.target = target;
	}
}
