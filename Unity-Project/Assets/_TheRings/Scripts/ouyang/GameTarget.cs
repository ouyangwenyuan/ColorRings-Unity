using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTarget : MonoBehaviour {

	public Text TimeText; //在UI里显示时间
	// Use this for initialization
	public Image percentageImg;
	private int percentage;
	void Start () {
		int currentLevel = PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 1);
		GameLevelData levelData = CSVReader.gameLevelDatas[currentLevel - 1];
		TotalTime = int.Parse (levelData.usedTime);
		percentage = TotalTime;
		StartCoroutine (startTime ());
		percentageImg.fillAmount = (float) TotalTime / (float) percentage;
	}

	// Update is called once per frame
	void Update () {
		
	}

	private int TotalTime = 90; //总时间

	private int mumite; //分

	private int second; //秒
	public IEnumerator startTime () {

		while (TotalTime > 0) {

			//Debug.Log(TotalTime);//打印出每一秒剩余的时间
			yield return new WaitForSeconds (1); //由于开始倒计时，需要经过一秒才开始减去1秒，
			//所以要先用yield return new WaitForSeconds(1);然后再进行TotalTime--;运算
			TotalTime--;
			TimeText.text = "Time:" + TotalTime;
			if (TotalTime <= 0) { //如果倒计时剩余总时间为0时，就跳转场景
				//游戏结束
				GameManager.Instance.gameOver = true;

			}

			mumite = TotalTime / 60; //输出显示分

			second = TotalTime % 60; //输出显示秒

			// string length = mumite.ToString ();
			percentageImg.fillAmount = (float) TotalTime / (float) percentage;
			if (second >= 10) {
				if (mumite >= 10) {
					TimeText.text = mumite + ":" + second;
				} else {
					TimeText.text = "0" + mumite + ":" + second;
				}
			} //如果秒大于10的时候，就输出格式为 00：00
			else {
				if (mumite >= 10) {
					TimeText.text = mumite + ":0" + second;
				} else {
					TimeText.text = "0" + mumite + ":0" + second; //如果秒小于10的时候，就输出格式为 00：00
				}
			}

		}

	}

	private void OnDestroy () {
		StopCoroutine (startTime ());
	}
}