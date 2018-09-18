using UnityEngine;
using UnityEditor;

public class SuperpowWindowEditor
{
    [MenuItem("Superpow/Clear all playerprefs")]
    static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("Superpow/Unlock all levels")]
    static void UnlockAllLevel()
    {
       
        // add by zcw in 2018/05/30
        // PlayerPrefs.SetInt(PrefKeys.CURRENT_LEVEL, Const.MAX_LEVEL * Const.WORLD_NAME.Length);

    }

    [MenuItem("Superpow/Credit balance (ruby, hint..)")]
    static void AddRuby()
    {
       
    }

    [MenuItem("Superpow/Set balance to 0")]
    static void SetBalanceZero()
    {
      
    }
}