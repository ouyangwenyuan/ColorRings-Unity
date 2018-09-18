using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BackHome : MonoBehaviour {

	private void OnGUI () {
		if (GUILayout.Button ("back")) {
			SceneManager.LoadScene(0);
		}
	}
}