using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeController : MonoBehaviour {

	public GameObject[] homeCanvas;
	public GameObject playBtn;
	public GameObject settingBtn;
	public GameObject homeBtn;
	public GameObject closeSetting;
	public Transform BtnWrapper;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < BtnWrapper.childCount; i++) {
			Button btn = BtnWrapper.GetChild (i).GetComponent<Button> ();
			int x = i + 1;
			btn.onClick.AddListener (delegate () {
				switchScene (x);
			});
		}
		playBtn.GetComponent<Button> ().onClick.AddListener (delegate () {
			switchCanvas (1);
		});
		settingBtn.GetComponent<Button> ().onClick.AddListener (delegate () {
			switchCanvas (2);
		});
		homeBtn.GetComponent<Button> ().onClick.AddListener (delegate () {
			switchCanvas (0);
		});
		closeSetting.GetComponent<Button> ().onClick.AddListener (delegate () {
			switchCanvas (0);
		});
	}

	// Update is called once per frame
	void Update () {

	}

	public void showSelectGameMode () {

	}
	public void showSetting () {

	}

	public void switchCanvas (int index) {
		for (int i = 0; i < homeCanvas.Length; i++) {
			if (i == index) {
				homeCanvas[i].SetActive (true);
			} else {
				homeCanvas[i].SetActive (false);
			}
		}
	}

	public void switchScene (int index) {
		SceneManager.LoadScene (index);
		// switch (index) {
		// 	case 1:
		// 		break;
		// 	case 2:
		// 		break;
		// 	case 3:
		// 		break;
		// 	case 4:
		// 		break;
		// 	default:
		// 		break;
		// }
	}

	// private void OnGUI () {
	// 	if (GUILayout.Button ("关卡数据导入测试")) {
	// 		SceneManager.LoadScene ("Test");
	// 	}
	// }
}