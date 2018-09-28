using UnityEditor;
using UnityEngine;

public class SuperpowWindowEditor {
    [MenuItem ("Superpow/Clear all playerprefs")]
    static void ClearAllPlayerPrefs () {
        PlayerPrefs.DeleteAll ();
        PlayerPrefs.Save ();
    }

    [MenuItem ("Superpow/Unlock all levels")]
    static void UnlockAllLevel () {
        PlayerPrefs.SetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 50);
        // add by zcw in 2018/05/30
        // PlayerPrefs.SetInt(PrefKeys.CURRENT_LEVEL, Const.MAX_LEVEL * Const.WORLD_NAME.Length);

    }

    // [MenuItem ("Superpow/Credit balance (ruby, hint..)")]
    // static void AddRuby () {

    // }

    // [MenuItem ("Superpow/Set balance to 0")]
    // static void SetBalanceZero () {

    // }

    [MenuItem ("Superpow/CreateMessLevel")]
    static void createWorldsLevel () {

    }
    //     Object[] files = Resources.LoadAll ("Levels");
    //     int world = files.Length / 60 + 1;
    //     Debug.Log ("files =" + world);
    //     string folderPath = "Assets/_TheRings/Resources/Levels/World_" + world;
    //     if (!AssetDatabase.IsValidFolder (folderPath))
    //         AssetDatabase.CreateFolder ("Assets/_TheRings/Resources/Levels", "World_" + world);

    //     for (int i = 1; i <= 60; i++) {
    //         GameLevel asset = ScriptableObject.CreateInstance<GameLevel> ();
    //         // 随机生成地图
    //         asset.positions = generalMap ();
    //         // 随机生成切片
    //         asset.pieces = generalPiecesPos (asset.positions);
    //         AssetDatabase.CreateAsset (asset, folderPath + "/Level_" + i + ".asset");
    //         AssetDatabase.SaveAssets ();
    //     }

    //     EditorUtility.FocusProjectWindow ();
    // }
}