using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour {

	public Transform BtnWrapper;
	// Use this for initialization
	void Start () {
		for(int i = 0 ;i < BtnWrapper.childCount ; i++){
			Button btn = BtnWrapper.GetChild(i).GetComponent<Button>();
		    int x = i + 1;
			btn.onClick.AddListener(delegate(){
				switchScene(x);
			});
		}
	}

	// Update is called once per frame
	void Update () {

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

	private void OnGUI () {
		if (GUILayout.Button ("关卡数据导入测试")) {
			SceneManager.LoadScene ("Test");
		}
	}
}