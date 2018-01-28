using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	enum State {
		INACTIVE,
		IDLE,
		ATTACK_START,
		ATTACK_ACTIVE,
		ATTACK_RECOVERY,
		BLOCKING,
		PHASING_OUT,
		PHASING_IN,
		DEAD
	}

	private const int maxCharges = 2;
	private int charges = maxCharges;
	private float rechargeProgress = 0f;
	private float rechargeDuration = 1.5f;

	public AudioClip hitSound;
	public AudioClip blockSound;
	public AudioClip attackSound;
	public AudioClip deathSound;
	public AudioClip teleportSound;
	private AudioSource audioSource;

	private const float blockDuration = 0.5f;
	private const float phaseDuration = 0.2f;
	private const float attackStartDuration = 0.05f;
	private const float attackActiveDuration = 0.25f;
	private const float attackRecoverDuration = 0.01f;

	private const float scaleVal = 2f;

	public float startX;
	public float startY;

	private Vector2 teleportDirection;

	private State state = State.INACTIVE;
	private float progress = 0f;

	private const float teleportDistance = 5f;
	private Vector2 offset = new Vector3 (1.5f, 0f, 0f);

	private Vector2 velocity = new Vector2(0f, 0f);
	private const float maxSpeed = 5f;
	private const float acceleration = 10f;
	private const float deceleration = 2f;
	private const float attackForce = 1000f;

	private Attack attack;
	private Animator animator;

	public int playerId;

	public string xAxis;
	public string yAxis;
	public string teleportAxis;
	public string attackAxis;

	public PlayerController target;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource> ();
		attack = transform.Find ("Attack").GetComponent<Attack> ();
		attack.setTarget (target);
	}
	
	// Update is called once per frame
	void Update () {

		if (state == State.INACTIVE) {
			setRotation ();
			return;
		} else if (state != State.DEAD) {
			handleCharging ();
		}

		progress += Time.deltaTime;
		float x = Input.GetAxis(xAxis);
		float y = Input.GetAxis(yAxis);
		Vector2 direction = new Vector2(x, y);	
		direction.Normalize ();

		applyDeceleration (direction);

		switch (state) {
		case State.IDLE:
			handleIdle (direction);
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

		move ();
		checkBounds ();
	
	}

	public int getCharges() {
		return charges;
	}

	private void handleCharging() {

		if (charges != maxCharges) {

			rechargeProgress += Time.deltaTime;
			if (rechargeProgress >= rechargeDuration) {
				rechargeProgress -= rechargeDuration;
				++charges;
			}

		} else {
			rechargeProgress = 0f;
		}

	}

	public void activate() {
		state = State.IDLE;
	}

	public void reset() {
		setState (State.INACTIVE);
		setTrig ("finish");
		attack.setActive (false);
		velocity = new Vector2 (0f, 0f);
		transform.position = new Vector3 (startX, startY, 0f);
		charges = maxCharges;
		rechargeProgress = 0f;
	}

	public void hit (Vector2 direction) {
		
		direction.Normalize ();
		applyForce (direction, attackForce);

		if (state == State.DEAD || state == State.BLOCKING)
			return;

		float x = Input.GetAxis(xAxis);
		float y = Input.GetAxis(yAxis);
		Vector2 inputDirection = new Vector2(x, y);	
		if (Vector2.Dot (direction, inputDirection) > 0f) {

			setState (State.BLOCKING);
			setTrig ("block");
			audioSource.PlayOneShot (blockSound, 1f);
			//animation.Play ("P1Blocking");

		} else {

			setState (State.DEAD);
			setTrig ("death");
			audioSource.PlayOneShot (deathSound, 1f);
			//animation.Play ("P1Dead");

		}

	}

	private void applyDeceleration(Vector2 direction) {

		float currentDecel = deceleration * Time.deltaTime;
		if (currentDecel > velocity.magnitude) {
			velocity = new Vector2 (0f, 0f);
		} else {
			velocity = Vector2.ClampMagnitude (velocity, velocity.magnitude - currentDecel);
		}
			

	}

	public bool isAlive() {
		return state != State.DEAD;
	}

	private bool hasProgressed(float duration) {
		return progress >= duration;
	}

	private void checkBounds() {

		float width = 8f;
		float height = 4.2f;

		Vector3 pos = transform.position;
		if (pos.x > width) {
			transform.position = new Vector3 (width, pos.y, 0f);	
			velocity.x = -velocity.x;
		} else if (pos.x < -width) {
			transform.position = new Vector3 (-width, pos.y, 0f);	
			velocity.x = -velocity.x;
		}
		if (pos.y > height) {
			transform.position = new Vector3 (pos.x, height, 0f);
			velocity.y = -velocity.y;
		} else if (pos.y < -height) {
			transform.position = new Vector3 (pos.x, -height, 0f);
			velocity.y = -velocity.y;
		}

	}

	private void setTrig(string trigger) {

		attack.setActive (false);
		animator.ResetTrigger ("attack");
		animator.ResetTrigger ("block");
		animator.ResetTrigger ("teleport");
		animator.ResetTrigger ("death");
		animator.ResetTrigger ("finish");
		animator.SetTrigger (trigger);

	}

	private void handleIdle(Vector2 direction) {


		if (Input.GetAxisRaw (teleportAxis) != 0 && charges > 0) {
			teleportDirection = new Vector2(direction.x, direction.y);
			--charges;
			setState (State.PHASING_OUT);
			setTrig ("teleport");
			audioSource.PlayOneShot (teleportSound, 1f);
			//animation.Play("P1PhaseOut");
		} else if (Input.GetAxisRaw (attackAxis) != 0) {
			setState (State.ATTACK_START);
			setTrig ("attack");
			audioSource.PlayOneShot (attackSound, 1f);
			//animation.Play ("P1Attack");
		} else {
			applyForce (direction, acceleration);
		}

		setRotation ();

	}

	private void handleBlocking() {

		if (hasProgressed(blockDuration)) {
			setState(State.IDLE);
			setTrig("finish");
			//animation.Play ("P1Idle");
		}

	}

	private void handleDead() {
		applyForce (new Vector2 (0f, -1f), 4f);
	}

	private void handlePhaseOut() {

		if (hasProgressed (phaseDuration)) {
			teleport (teleportDirection);
			setState (State.PHASING_IN);
			setTrig ("finish");
			//animation.Play("P1PhaseIn");
		}


	}

	private void handlePhaseIn() {

		if (hasProgressed (phaseDuration)) {
			setState (State.IDLE);
			setTrig("finish");
			//animation.Play ("P1Idle");
		}

		setRotation ();
	}

	private void handleAttack() {

		if (state == State.ATTACK_START) {

			if (hasProgressed (attackStartDuration)) {
				setState (State.ATTACK_ACTIVE);
				attack.setActive (true);
			}

		} else if (state == State.ATTACK_RECOVERY) {

			if (hasProgressed (attackRecoverDuration)) {
				setState (State.IDLE);
			}

		} else {

			if (hasProgressed (attackActiveDuration)) {
				setState (State.ATTACK_RECOVERY);
				attack.setActive (false);
				setTrig("finish");
				//animation.Play ("P1Idle");
			}

		}

	}

	private void setState(State newState) {
		state = newState;
		progress = 0f;
	}

	private void applyForce(Vector2 direction, float force) {
		velocity += Vector2.ClampMagnitude(direction, (force * Time.deltaTime));
		if (velocity.magnitude > maxSpeed) {
			velocity = Vector2.ClampMagnitude (velocity, maxSpeed);
		}

			
	}

	private void move() {
		transform.Translate(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0, Space.World);
	}
	
	private void teleport(Vector2 direction) {
		direction *= teleportDistance;
		transform.Translate(direction.x, direction.y, 0, Space.World);
	}

	private void setRotation() {

		Vector3 toTarget = target.getCentre() - getCentre();
		//Vector3 toTarget = target.transform.position - transform.position;
		toTarget.Normalize();

		float rotation = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
		if (rotation >= 90f || rotation <= -90f) {
			transform.localScale = new Vector3 (scaleVal, scaleVal, 1f);
			rotation += 180f;
		} else {
			transform.localScale = new Vector3 (-scaleVal, scaleVal, 1f);
		}
		transform.rotation = Quaternion.identity;
		//transform.RotateAround (getCentre (), new Vector3 (0f, 0f, 1f), rotation);
		transform.rotation = Quaternion.Euler(0f, 0f, rotation);

	}

	public Vector3 getCentre() {

		Vector3 toOrigin = Vector3.Scale (offset, transform.localScale);
		toOrigin = transform.rotation * toOrigin;
		return transform.position + toOrigin;

	}
}
