using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BackHome : MonoBehaviour {

	// private void OnGUI () {
	// 	if (GUILayout.Button ("back")) {
	// 		backHome ();
	// 	}
	// 		// if (GUILayout.Button ("backMap")) {
	// 		// 	int curLevel = PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 1);
	// 		// 	PlayerPrefs.SetInt (CommonConst.PrefKeys.CURRENT_LEVEL, curLevel + 1);
	// 		// 	SceneManager.LoadScene ("MapScene");
	// 		// }

	// }

	private void Start () {
		transform.GetComponent <Button>().onClick.AddListener(()=>{
			backHome();
		});
	}
	public void backHome () {
		SceneManager.LoadScene (0);
	}
}