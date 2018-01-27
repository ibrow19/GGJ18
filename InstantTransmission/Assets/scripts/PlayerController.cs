using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	enum State {
		IDLE,
		ATTACK_START,
		ATTACK_ACTIVE,
		ATTACK_RECOVERY,
		BLOCKING,
		PHASING_OUT,
		PHASING_IN,
		DEAD
	}

	private const float blockDuration = 1f;
	private const float PhaseDuration = 1f;
	private const float attackStartDuration = 0.2f;
	private const float attackActiveDuration = 0.2f;
	private const float attackRecoverDuration = 0.2f;

	private Vector2 teleportDirection;

	private State state = State.IDLE;
	private float progress = 0f;

	private const float velocity = 3f;
	private const float teleportDistance = 1f;
	private Vector2 offset = new Vector3 (0f, 1f, 0f);

	private Attack attack;
	private Animator animator;

	public string xAxis;
	public string yAxis;
	public string teleportAxis;
	public string attackAxis;

	public PlayerController target;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		attack = transform.Find ("Attack").GetComponent<Attack> ();
		attack.setTarget (target);
	}
	
	// Update is called once per frame
	void Update () {

		progress += Time.deltaTime;

		switch (state) {
		case State.IDLE:
			handleIdle ();
			break;
		case State.BLOCKING:
			handleBlocking ();
			break;
		case State.DEAD:
			handleDead ();
			break;
		case State.PHASING_IN:
			handlePhaseIn ();
			break;
		case State.PHASING_OUT:
			handlePhaseOut ();
			break;	
		default:
			handleAttack ();
			break;
		}
	
	}

	private void handleIdle() {

		float x = Input.GetAxis(xAxis);
		float y = Input.GetAxis(yAxis);

		Vector2 direction = new Vector2(x, y);	
		direction.Normalize ();

		if (Input.GetAxisRaw (teleportAxis) != 0) {
			setState (State.PHASING_OUT);
			teleportDirection = direction;
			animator.SetTrigger ("teleport");
		} else if (Input.GetAxisRaw (attackAxis) != 0) {
			setState (State.ATTACK_START);
			animator.SetTrigger ("attack");
		} else {
			move (direction);
		}

		setRotation ();

	}

	private void handleBlocking() {

	}

	private void handleDead() {

	}

	private void handlePhaseOut() {

	}

	private void handlePhaseIn() {

	}

	private void handleAttack() {

	}

	private void setState(State newState) {
		state = newState;
		progress = 0f;
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
