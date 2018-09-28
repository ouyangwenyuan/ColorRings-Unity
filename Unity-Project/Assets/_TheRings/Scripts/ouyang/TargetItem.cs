using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetItem : MonoBehaviour {

	public Image icon;
	public Text labelTx;
	public Text targetTx;
	public Text valueTx;
	// Use this for initialization
	public int colorType;
	int total;
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void setData (string label, string target, int colorType = -1) {
		this.colorType = colorType;
		if (colorType == -1) {
			icon.gameObject.SetActive (false);
		} else {
			string spirtefile = "rings/" + colorType + "-1";
			// icon.color = UIManager.ringColors[colorType];
			icon.GetComponent<Image> ().sprite = Resources.Load<Sprite> (spirtefile);
		}
		targetTx.text = target;
		labelTx.text = label;
	}

	public void setValue (int count) {
		valueTx.text = count.ToString ();
		total = count;
	}

	public void changeValue (int count) {
		total += count;
		valueTx.text = total.ToString ();
	}

}