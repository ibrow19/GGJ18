using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private const float velocity = 3f;
	private const float teleportDistance = 1f;
	private Vector2 offset = new Vector3 (0f, 1f, 0f);

	public string xAxis;
	public string yAxis;
	public string teleportAxis;
	public string attackAxis;

	public PlayerController target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		float x = Input.GetAxis(xAxis);
		float y = Input.GetAxis(yAxis);

		Vector2 direction = new Vector2(x, y);	
		direction.Normalize ();

		if (Input.GetAxisRaw (teleportAxis) != 0) {
			teleport (direction);
		} else {
			move (direction);
		}

		setRotation ();
	
	}

	private void move(Vector2 direction) {

		direction *= velocity * Time.deltaTime;
		transform.Translate(direction.x, direction.y, 0, Space.World);

	}
	
	private void teleport(Vector2 direction) {

		direction *= teleportDistance;
		transform.Translate(direction.x, direction.y, 0, Space.World);

	}

	private void setRotation() {

		Vector3 toTarget = target.getCentre() - getCentre();
		toTarget.Normalize();

		float rotation = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
		if (rotation >= 90f || rotation <= -90f) {
			transform.localScale = new Vector3 (1f, 1f, 1f);
			rotation += 180f;
		} else {
			transform.localScale = new Vector3 (-1f, 1f, 1f);
		}
		transform.rotation = Quaternion.Euler(0f, 0f, rotation);

	}

	public Vector3 getCentre() {

		Vector3 toOrigin = Vector3.Scale (offset, transform.localScale);
		toOrigin = transform.rotation * toOrigin;
		return transform.position + toOrigin;

	}
}
