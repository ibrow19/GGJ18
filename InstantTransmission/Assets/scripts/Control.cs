using UnityEngine;
using UnityEngine.SceneManagement;

public class Control : MonoBehaviour
{
	public void Update()
	{
		if (Input.GetAxisRaw ("P1Attack") != 0 || Input.GetAxisRaw ("P2Attack") != 0 ||
		    Input.GetAxisRaw ("P1Teleport") != 0 || Input.GetAxisRaw ("P2Teleport") != 0) {
			SceneManager.LoadScene ("main");
		}
	}
}
