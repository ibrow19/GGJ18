using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour {

	public PlayerController player;
	public SpriteRenderer charge1;
	public SpriteRenderer charge2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		int count = player.getCharges ();
		switch (count) {
		case 2:
			charge1.enabled = true;
			charge2.enabled = true;
			break;
		case 1:
			charge1.enabled = true;
			charge2.enabled = false;
			break;
		default:
			charge1.enabled = false;
			charge2.enabled = false;
			break;
		}

	}
}
