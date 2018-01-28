using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	enum State {
		SETUP,
		FIGHT,
		IN_GAME,
		WINNER,
		REMATCH,
	}

	private Sprite fightSprite;
	private Sprite p1Sprite;
	private Sprite p2Sprite;
	private Sprite rematchSprite;

	private const float stateTime = 2f;

	private SpriteRenderer srenderer;

	private float progress = 0f;
	private State state = State.SETUP;

	public PlayerController p1;
	public PlayerController p2;

	// Use this for initialization
	void Start () {

		fightSprite = Resources.Load <Sprite> ("fight"); 
		p1Sprite = Resources.Load <Sprite> ("player1wins"); 
		p2Sprite = Resources.Load <Sprite> ("player2wins"); 
		rematchSprite = Resources.Load <Sprite> ("rematch"); 
		srenderer = GetComponent<SpriteRenderer> ();
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
			case State.WINNER:
				handleWinner ();
				break;
			default:
				handleRematch ();
				break;
			}

	}

	private void handleSetup() {
		if (progress > stateTime) {
			setState (State.FIGHT);
			srenderer.enabled = true;
			srenderer.sprite = fightSprite;
		}
	}

	private void handleFight() {
		if (progress > stateTime) {
			setState (State.IN_GAME);
			srenderer.enabled = false;
		}
	}

	private void handleInGame() {
		if (gameOver () && progress > stateTime) {
			setState (State.WINNER);
			srenderer.enabled = true;
			if (p1.isAlive ()) {
				srenderer.sprite = p1Sprite;
			} else {
				srenderer.sprite = p2Sprite;
			}
		} else if (!gameOver()) {
			progress = 0f;
		}
	}

	private void handleWinner() {
		if (progress > stateTime) {
			setState (State.REMATCH);
			srenderer.sprite = rematchSprite;
		}
	}

	private void handleRematch() {

		if (Input.GetAxisRaw ("P1Attack") != 0 || Input.GetAxisRaw ("P2Attack") != 0 ||
		    Input.GetAxisRaw ("P1Teleport") != 0 || Input.GetAxisRaw ("P2Teleport") != 0) {
			setState (State.SETUP);
			srenderer.enabled = false;
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
