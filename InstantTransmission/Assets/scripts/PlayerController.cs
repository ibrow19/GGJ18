﻿using System.Collections;
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
	private const float phaseDuration = 0.2f;
	private const float attackStartDuration = 0.05f;
	private const float attackActiveDuration = 0.2f;
	private const float attackRecoverDuration = 0.05f;

	private Vector2 teleportDirection;

	private State state = State.IDLE;
	private float progress = 0f;

	private const float velocity = 3f;
	private const float teleportDistance = 5f;
	private Vector2 offset = new Vector3 (1f, 0f, 0f);

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

	public void hit (Vector2 direction) {

		if (state == State.DEAD || state == State.BLOCKING)
			return;

		float x = Input.GetAxis(xAxis);
		float y = Input.GetAxis(yAxis);
		Vector2 inputDirection = new Vector2(x, y);	
		Debug.Log ("Attack direction: " + direction);
		Debug.Log ("Block direction: " + inputDirection);
		Debug.Log("Dot: " + Vector2.Dot (direction, inputDirection));
		if (Vector2.Dot (direction, inputDirection) > 0f) {

			setState (State.BLOCKING);
			setTrig ("block");
			//animation.Play ("P1Blocking");

		} else {

			setState (State.DEAD);
			setTrig ("death");
			//animation.Play ("P1Dead");

		}

	}

	public bool isAlive() {
		return state != State.DEAD;
	}

	private bool hasProgressed(float duration) {
		return progress >= duration;
	}

	private void setTrig(string trigger) {

		animator.ResetTrigger ("attack");
		animator.ResetTrigger ("block");
		animator.ResetTrigger ("teleport");
		animator.ResetTrigger ("death");
		animator.ResetTrigger ("finish");
		animator.SetTrigger (trigger);

	}

	private void handleIdle() {

		float x = Input.GetAxis(xAxis);
		float y = Input.GetAxis(yAxis);
		Vector2 direction = new Vector2(x, y);	
		direction.Normalize ();

		if (Input.GetAxisRaw (teleportAxis) != 0) {
			teleportDirection = new Vector2(direction.x, direction.y);
			setState (State.PHASING_OUT);
			setTrig ("teleport");
			//animation.Play("P1PhaseOut");
		} else if (Input.GetAxisRaw (attackAxis) != 0) {
			setState (State.ATTACK_START);
			setTrig ("attack");
			//animation.Play ("P1Attack");
		} else {
			move (direction);
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
		transform.Translate(0f, -velocity * Time.deltaTime, 0, Space.World);
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
		//Vector3 toTarget = target.transform.position - transform.position;
		toTarget.Normalize();

		float rotation = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
		if (rotation >= 90f || rotation <= -90f) {
			transform.localScale = new Vector3 (1f, 1f, 1f);
			rotation += 180f;
		} else {
			transform.localScale = new Vector3 (-1f, 1f, 1f);
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
