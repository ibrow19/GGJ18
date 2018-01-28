using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	enum State {
		SETUP,
		FIGHT,
		IN_GAME,
		ANNOUNCE,
		WINNER,
		REMATCH,
	}

	public Sprite fightSprite;
	public Sprite p1Sprite;
	public Sprite p2Sprite;
	public Sprite rematchSprite;

	public AudioClip fightSound;
	public AudioClip p1Sound;
	public AudioClip p2Sound;
	public AudioClip winsSound;

	private const float longTime = 2f;
	private const float shortTime = 1f;
	private const float midTime = 1.5f;

	private AudioSource audioSource;
	private SpriteRenderer srenderer;

	private float progress = 0f;
	private State state = State.SETUP;

	public PlayerController p1;
	public PlayerController p2;

	// Use this for initialization
	void Start () {

		//fightSprite = Resources.Load <Sprite> ("fight"); 
		//p1Sprite = Resources.Load <Sprite> ("player1wins"); 
		//p2Sprite = Resources.Load <Sprite> ("player2wins"); 
		//rematchSprite = Resources.Load <Sprite> ("rematch"); 
		srenderer = GetComponent<SpriteRenderer> ();
		audioSource = GetComponent<AudioSource> ();
		srenderer.enabled = false;

	}

	// Update is called once per frame
	void Update () {

		progress += Time.deltaTime;

			switch (state) {
			case State.SETUP:
				handleSetup ();
				break;
			case State.FIGHT:
				handleFight ();
				break;
			case State.IN_GAME:
				handleInGame ();
				break;
			case State.REMATCH:
				handleRematch ();
				break;
			default:
				handleWinner ();
				break;
			}

	}

	private void handleSetup() {
		if (progress > longTime) {
			setState (State.FIGHT);
			audioSource.PlayOneShot(fightSound, 1f);
			srenderer.enabled = true;
			srenderer.sprite = fightSprite;
		}
	}

	private void handleFight() {
		if (progress > shortTime) {
			setState (State.IN_GAME);
			srenderer.enabled = false;
			p1.activate ();
			p2.activate ();
		}
	}

	private void handleInGame() {
		if (gameOver()) {
			setState (State.ANNOUNCE);
			if (p1.isAlive ()) {
				audioSource.PlayOneShot (p1Sound, 1f);
			} else {
				audioSource.PlayOneShot (p2Sound, 1f);
			}
		} 
	}

	private void handleWinner() {
		if (progress > midTime) {
			if (state == State.ANNOUNCE) {
				setState (State.WINNER);
				srenderer.enabled = true;
				if (p1.isAlive ()) {
					srenderer.sprite = p1Sprite;
				} else {
					srenderer.sprite = p2Sprite;
				}
				audioSource.PlayOneShot (winsSound, 1f);
			} else {
				setState (State.REMATCH);
				srenderer.sprite = rematchSprite;
			}
		} 

	}

	private void handleRematch() {

		if (Input.GetAxisRaw ("P1Attack") != 0 || Input.GetAxisRaw ("P2Attack") != 0 ||
		    Input.GetAxisRaw ("P1Teleport") != 0 || Input.GetAxisRaw ("P2Teleport") != 0) {
			setState (State.SETUP);
			srenderer.enabled = false;
			p1.reset();
			p2.reset();
		}

	}

	private void setState(State newState) {

		state = newState;
		progress = 0f;

	}

	private bool gameOver() {
		return !(p1.isAlive () && p2.isAlive ());
	}

}
