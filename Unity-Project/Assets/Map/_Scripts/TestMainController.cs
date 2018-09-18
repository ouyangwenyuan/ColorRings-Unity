using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TestMainController : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	private void OnGUI () {
		if (GUILayout.Button ("backMap")) {
		int curLevel =	PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL,1);
			PlayerPrefs.SetInt (CommonConst.PrefKeys.CURRENT_LEVEL, curLevel+1);
			SceneManager.LoadScene ("MapScene");
		}
	}
}