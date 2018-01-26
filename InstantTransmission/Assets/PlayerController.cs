using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private const float VELOCITY = 1f;

	public Transform mTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		Vector2 move = new Vector2(x, y);	
		move = Vector2.ClampMagnitude (move, VELOCITY * Time.deltaTime);
		transform.Translate(move.x, move.y, 0, Space.World);

		Vector3 toTarget = mTarget.transform.position - transform.position;
		toTarget.Normalize();

		float rotation = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0f, 0f, rotation);

	}
}
