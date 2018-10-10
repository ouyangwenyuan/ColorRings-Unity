using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CSVReader : MonoBehaviour {

	public Text _display;
	// private void OnGUI () {
	// 	if (GUILayout.Button ("Save")) {
	// 		Save ();
	// 	}
	// 	if (GUILayout.Button ("Load")) {
	// 		Load ();
	// 	}
	// }

	private string _fileName = "LevelData";
	private string _messFileName = "MessLevelData";

	string _loadPath;
	string _savePath;
	private const string EXTENSION = ".csv";

	private CSVTable _table;

	private CSVTable _messTable;

	// Bind Component
	void Awake () {
		// #if UNITY_IOS
		_loadPath = Application.streamingAssetsPath + "/";
		// #if UNITY_ANDROID
		// 		_loadPath = "jar:file://" + Application.dataPath + "/!/assets/";
		// #endif

		_savePath = Application.streamingAssetsPath + "/";
		Load ();
		DontDestroyOnLoad (this);
	}

	/// <summary>
	/// 加载文件
	/// </summary>
	private void Load () {
		// if (!Directory.Exists (_loadPath)) {
		// 	Debug.LogError ("The file not be found in this path. path:" + _loadPath);
		// 	return;
		// }

		// if (Application.platform == RuntimePlatform.Android) {
		// LoadWWW ();
		// LoadWWW1 ();
		// } else {
		string fullFileName = _loadPath + _fileName + EXTENSION;
		// StreamReader sr;
		// sr = File.OpenText (fullFileName);
		// string content = sr.ReadToEnd ();
		// sr.Close ();
		// sr.Dispose ();
		string content = LoadFile (fullFileName);
		print ("content  =" + content);
		_table = CSVTable.CreateTable (_fileName, content);
		// 添加测试
		toObject ();

		// sr = File.OpenText (_loadPath + _messFileName + EXTENSION);
		// content = sr.ReadToEnd ();
		// sr.Close ();
		// sr.Dispose ();
		content = LoadFile (_loadPath + _messFileName + EXTENSION);
		_messTable = CSVTable.CreateTable (_messFileName, content);
		print ("content1  =" + content);
		// Test ();
		toMessObject ();
		// }
	}
	public  string  LoadFile (string  url) {
		// string  url  =  Application.streamingAssetsPath  +  "/"  +  filePath;
#if UNITY_ANDROID
		WWW www  =  new  WWW (url);
		while  (!www.isDone)  { }
		return  www.text;
#else
		return  File.ReadAllText (url);
#endif
	}

	void LoadWWW () {
		string fullFileName = _loadPath + _fileName + EXTENSION;
		WWW www = new WWW (fullFileName); //("jar:file://" + Application.dataPath + "/!/assets" + "/Load/" + _fileName + EXTENSION);
		// yield return www;
		while (!www.isDone) {

		}
		string content = www.text; // System.Text.Encoding.Default.GetString (www.bytes);
		_table = CSVTable.CreateTable (_fileName, content);
		print (fullFileName + "content1  =" + content);
		// 添加测试
		toObject ();
	}
	void LoadWWW1 () {
		string fullFileName = _loadPath + _messFileName + EXTENSION;
		WWW www = new WWW (fullFileName); //("jar:file://" + Application.dataPath + "/!/assets" + "/Load/" + _messFileName + EXTENSION);
		while (!www.isDone) {

		}
		// yield return www;
		string content = System.Text.Encoding.Default.GetString (www.bytes);
		_messTable = CSVTable.CreateTable (_messFileName, content);
		print (fullFileName + "content1  =" + _messTable.ToString ());
		toMessObject ();
	}
	//cloumnName
	private string[] cloumnName = { "_id", "Levelname", "targetScore", "targetCombo", "targetRingColor", "targetRingCount", "totalColorCount", "usedTime" };
	public static List<GameLevelData> gameLevelDatas = new List<GameLevelData> ();
	public static List<GameLevelData> gameMessLevelDatas = new List<GameLevelData> ();
	private void toObject () {
		List<string> keys = _table.IdValues; // ids 
		for (int i = 0; i < keys.Count; i++) {
			GameLevelData levelData = ScriptableObject.CreateInstance<GameLevelData> ();
			// Debug.Log (i+" key= " + keys[i]);
			levelData.level = int.Parse (keys[i]);
			levelData._id = keys[i];
			levelData.Levelname = _table[levelData._id]["Levelname"];
			levelData.targetScore = _table[levelData._id]["targetScore"];
			levelData.targetCombo = _table[levelData._id]["targetCombo"];
			levelData.targetRingColors = _table[levelData._id]["targetRingColor"].Split (';');
			levelData.targetRingCounts = _table[levelData._id]["targetRingCount"].Split (';');
			// Debug.Log ("targetRingColors count= " + levelData.targetRingColors.Length + ",targetRingCounts =" + levelData.targetRingCounts.Length);
			levelData.totalColorCount = _table[levelData._id]["totalColorCount"];
			levelData.usedTime = _table[levelData._id]["usedTime"];
			levelData.sizeCount = _table[levelData._id]["RingsType"];
			gameLevelDatas.Add (levelData);
		}
		if (_display) {
			_display.text = _table.ToString ();
		}
	}

	private void toMessObject () {
		List<string> keys = _messTable.IdValues; // ids 
		for (int i = 0; i < keys.Count; i++) {
			GameLevelData levelData = ScriptableObject.CreateInstance<GameLevelData> ();
			levelData.level = int.Parse (keys[i]);
			levelData._id = keys[i];
			levelData.Levelname = _messTable[levelData._id]["Levelname"];
			levelData.RingColorCount = _messTable[levelData._id]["Ring-Color-Count"].Split (';');
			levelData.totalColorCount = _messTable[levelData._id]["TotalColorCount"];
			levelData.TotalRingCount = _messTable[levelData._id]["TotalRingCount"];
			gameMessLevelDatas.Add (levelData);
		}
		if (_display) {
			_display.text = _messTable.ToString ();
		}
	}

	/// <summary>
	/// 存储文件
	/// </summary>
	private void Save () {
		if (_table == null) {
			Debug.LogError ("The table is null.");
			return;
		}
		string tableContent = _table.GetContent ();

		if (!Directory.Exists (_savePath)) {
			Debug.Log ("未找到路径, 已自动创建");
			Directory.CreateDirectory (_savePath);
		}
		string fullFileName = _savePath + _fileName + EXTENSION;

		StreamWriter sw;
		sw = File.CreateText (fullFileName);
		sw.Write (tableContent);
		sw.Close ();
		sw.Dispose ();

		_table = null;
		Debug.Log ("Save file.");

	}

	/// <summary>
	/// 测试方法
	/// </summary>
	private void Test () {
		// 显示所有数据（以调试格式显示)
		// Debug.Log (_table.ToString ());
		List<string> keys = _messTable.IdValues; // ids 
		Debug.Log ("keyCount +" + (keys.Count) + ",_table.Atrribute count = " + _messTable.AtrributeKeys.Count);

		// 显示所有数据（以存储格式显示）
		string text = "_table:" + _messTable.Name; //_table.GetContent ();
		// List<string> ids = new List<string> ();
		foreach (string item in keys) {
			text += item;
			text += "|";
		}
		Debug.Log ("keys= " + text);
		text = "\n===========\n";
		List<string> Atrributes = new List<string> ();
		foreach (string key in _messTable.AtrributeKeys) {
			Atrributes.Add (key);
			text += key;
			text += "|";
		}
		Debug.Log ("AtrributeKeys= " + text);

		// text += "\n======================\n";
		// for (int i = 1; i <= 20; i++) {
		// 	foreach (string key in _table.AtrributeKeys) {
		// 		text += _table[i.ToString ()][key];
		// 		text += "|";
		// 	}
		// 	text += "\n";
		// }

		if (_display) {
			_display.text = _table.ToString ();
		}

		// // 拿到某一数据
		// _display.text += "\n" + "1001的年龄: " + _table["1001"]["年龄"];
		// // 拿到数据对象
		// text += "\n" + "2的数据: " + _table["2"];
		// // 修改某一数据
		// _table["1003"]["年龄"] = "10000";
		// _display.text += "\n" + "1003新的年龄: " + _table["1003"]["年龄"];

		// // 添加一条数据
		// CSVDataObject data = new CSVDataObject ("1005",
		// 	new Dictionary<string, string> () { { "姓名", "hahaha" }, { "年龄", "250" }, { "性别", "随便吧" },
		// 	},
		// 	new string[] { "编号", "姓名", "年龄", "性别" });
		// _table[data.ID] = data;
		// _display.text += "\n" + "新添加的1005的数据: " + _table["1005"].ToString ();

		// // 删除数据
		// _table.DeleteDataObject ("1001");
		// _table.DeleteDataObject ("1002");
		// _display.text += "\n" + "删了两个之后：" + "\n" + _table.GetContent ();

		// // 删除所有数据
		// _table.DeleteAllDataObject ();
		// _display.text += "\n" + "还剩下:" + "\n" + _table.GetContent ();
	}

}