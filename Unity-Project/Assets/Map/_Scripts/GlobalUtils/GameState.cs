using System.Collections.Generic;
using System.IO;
using UnityEngine;
// using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
public class GameState {

    public static int levelindex = 1;
    public static int realLevel = 1;
    public static int messLevel = 1;
    public static int chosenWorld = getWorldN (realLevel);
    public static int chosenLevel = getLevelN (realLevel);
    public static bool canPlay = true;
    public static int tutorialLevel = 1;
    public static int tempGameLevelsSteps = 0;

    public static StoredValue<int> hint = new StoredValue<int> ("hint", 1);
    public static StoredValue<int> coins = new StoredValue<int> ("coins", 0);

    public static List<Player> levelDatas;

    public static void toMessScene (int messLevel) {
        GameState.messLevel = messLevel;
        SceneManager.LoadScene ("GameSceneTheMess");
    }
    public static void toGameScene (int levelIndex) {
        // 进入游戏

        // TODO: 第二步: map进入游戏,level -> resallevel -->转换与查找传值
        int realLevel = GameState.getRealLevel (levelIndex);
        Debug.Log ("levelIndex =" + levelIndex + ",realLevel =" + realLevel);

        GameState.realLevel = realLevel;
        GameState.levelindex = levelIndex;
        // if (levelindex % 5 == 0) {
        //     SceneManager.LoadScene ("GameSceneTheMess");
        // } else {
        SceneManager.LoadScene ("GameSceneLevel");
        // }

        // if (GameState.realLevel == 31 && !PlayerPrefs.HasKey ("completeTutorial3")) {
        //     //加载31关之前,先进红机关引导
        //     CUtils.LoadTurorialScene (3);
        // } else if (GameState.realLevel == 60 && !PlayerPrefs.HasKey ("completeTutorial4")) {
        //     //加载60关之前,先进绿机关引导
        //     CUtils.LoadTurorialScene (4);
        // } else {
        //     CUtils.LoadScene (3, false);
        // }
    }

    public static int getRealLevel (int mapIndex) {
        return mapIndex -1;
        // if (mapIndex % 5 == 0) {
        //     return mapIndex / 5 - 1;
        // } else {
        //     return (mapIndex % 5) + (mapIndex / 5 * 4) - 1;
        // }

        // if (mapIndex == 0) return 1;

        // if (mapIndex < 10) return mapIndex;

        // if (mapIndex >= 10) {
        //     int real = levelDatas[mapIndex - 1].RealLevel;
        //     if (real == 0) {
        //         return levelDatas[mapIndex - 2].RealLevel + 1;
        //     }
        //     if (real >= CommonConst.default_total_levels) {
        //         return CommonConst.default_total_levels;
        //     }
        //     return real;

        // }

        // return 1;
    }

    public static int getMapLevel (int RealLevel) {
        for (int i = 0; i < levelDatas.Count; i++) {
            if (levelDatas[i].RealLevel == RealLevel) {
                return levelDatas[i].Level;
            }
        }
        return 1;
    }

    // Path Rule : Levels/World_n/Level_n : All n from 1 to n
    public static string GetGameLevelFilePath (int RealLevel) {
        int levelnum = 60;
        int word_n = RealLevel / levelnum;
        int level_n = RealLevel % levelnum;
        string path = "";
        if (level_n == 0) {
            path = "Levels/World_" + (word_n) + "/Level_60";
        } else {
            path = "Levels/World_" + (word_n + 1) + "/Level_" + (level_n);

        }

        return path;
    }

    public static int getWorldN (int gamelevel, int levelnum = CommonConst.MAX_LEVEL) {
        int word_n = gamelevel / levelnum;
        // return word_n + 1;
        if (word_n == 0) {
            return 1;
        } else {
            int temp = gamelevel % levelnum;
            if (temp == 0) {
                return word_n;
            } else {
                return word_n + 1;
            }
        }
    }

    public static int getLevelN (int gamelevel, int levelnum = CommonConst.MAX_LEVEL) {
        int level_n = gamelevel % levelnum;
        if (level_n == 0) return 60;
        // Debug.Log("=====>level_n" + level_n);
        return level_n;
    }

    private static string KEY_DATA = "DATA";

    public static void Save () {
        // PlayerPrefs.DeleteKey(KEY_DATA);
        string data = "";
        foreach (var item in levelDatas) {
            data += item.ToSaveString ();
        }
        PlayerPrefs.SetString (KEY_DATA, data);
    }

    /// <summary>
    /// Load data load by PlayerPrefs, set to buttons level on map scene
    /// </summary>
    /// <returns></returns>
    public static List<Player> Load () {
        List<Player> list = new List<Player> ();
        // TODO: 第一步:读文件的关卡,地图构造
        int currentLevel = PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 1) - 1;

        string data = PlayerPrefs.GetString (KEY_DATA, null);
        string[] dataSplit;
        if (string.IsNullOrEmpty (data)) {
            for (int i = 0; i < CommonConst.default_total_levels; i++) {
                Player p = new Player (i + 1);
                if (i <= currentLevel) {
                    p.RealLevel = i + 1;
                }
                // p.Locked = i != 0 ? true : false;
                p.Locked = i <= currentLevel ? false : true;
                p.Stars = i == currentLevel ? 2 : 0;
                list.Add (p);
            }
        } else {
            dataSplit = data.Split (',');

            for (int i = 0; i < CommonConst.default_total_levels; i++) {
                Player p = new Player (i + 1);
                p.RealLevel = int.Parse (dataSplit[i * 2 + 1]);
                p.Locked = i <= currentLevel ? false : true;
                p.Stars = i == currentLevel ? 2 : 0;
                // p.Locked = bool.Parse(dataSplit[i * 4]);
                // p.Stars = int.Parse(dataSplit[i * 4 + 1]);
                // p.HightScore = int.Parse(dataSplit[i * 4 + 2]);
                // p.Background = int.Parse(dataSplit[i * 4 + 3]);
                list.Add (p);
            }

            // DataLoader.MyData = list;

            // for (int i = 0; i < CommonConst.default_total_levels; i++)
            // {
            //     int levelIndex = i + 1;
            //     int realLevel = CUtils.findRealLevelByLevelIndex(levelIndex);// 找到真实关卡数据索引
            //     Player p = new Player(i + 1, realLevel);
            //     p.Locked = i <= currentLevel ? false : true;
            //     p.Stars = i == currentLevel ? 2 : 0;
            //     // p.Locked = bool.Parse(dataSplit[i * 4]);
            //     // p.Stars = int.Parse(dataSplit[i * 4 + 1]);
            //     // p.HightScore = int.Parse(dataSplit[i * 4 + 2]);
            //     // p.Background = int.Parse(dataSplit[i * 4 + 3]);
            //     list.Add(p);
            // }

        }
        levelDatas = list;

        return list;
    }

    // public static IAPItem[] iapItems;
    // public static void initIapItems()
    // {

    //     // "com.zcwfydev.hexablock.oneoffer_item01","$0.99","500","100","0","1",
    //     string[] productConfig = {
    //     "com.zcwfydev.hexablock.oneoffer_item02","$7.99","4100","100","0","0",
    //     "com.zcwfydev.hexablock.oneoffer_item03","$9.99","6500","150","1","0",
    //     "com.zcwfydev.hexablock.oneoffer_item04","$14.99","11500","200","1","0",
    //     "com.zcwfydev.hexablock.getcoin_item01_new","$1.99","500","0","0","0",
    //     "com.zcwfydev.hexablock.getcoin_item02_new","$4.99","1500","10","0","0",
    //     "com.zcwfydev.hexablock.getcoin_item03_new","$7.99","2500","15","0","0",
    //     "com.zcwfydev.hexablock.getcoin_item04_new","$14.99","5200","35","1","0",
    //     "com.zcwfydev.hexablock.getcoin_item05_new","$23.99","10000","55","1","0",
    //     "com.zcwfydev.hexablock.getcoin_item06","$49.99","21000","65","1","0",
    //     "com.zcwfydev.hexablock.getcoin_removeads","$1.99","0","0","2","1",
    //     "com.zcwfydev.hexablock.NoviceGift","$1.99","1000","200","1","0",

    // };

    //     int fieldCount = 6;
    //     int length = productConfig.Length / fieldCount;
    //     Debug.Log("iapitem lenght = " + length);
    //     iapItems = new IAPItem[length];
    //     //先构造的iapItems设置完成；
    //     for (int i = 0; i < length; i++)
    //     {
    //         iapItems[i] = new IAPItem();

    //         iapItems[i].priceStr = productConfig[i * fieldCount + 1];
    //         iapItems[i].price = float.Parse(productConfig[i * fieldCount + 1].Replace("$", ""));
    //         iapItems[i].productID = productConfig[i * fieldCount];
    //         iapItems[i].value = int.Parse(productConfig[i * fieldCount + 2]);
    //         iapItems[i].productType = ProductType.Consumable;
    //         int type = int.Parse(productConfig[i * fieldCount + 4]);
    //         if (type == 1)
    //         {
    //             iapItems[i].type = BTType.coinAndRemoveAD;
    //         }
    //         else if (type == 2)
    //         {
    //             iapItems[i].type = BTType.removeADOnly;
    //         }
    //         else
    //         {
    //             iapItems[i].type = BTType.coinOnly;
    //         }
    //         iapItems[i].isOnceOffer = productConfig[i * fieldCount].Contains("oneoffer");
    //         iapItems[i].morePercent = int.Parse(productConfig[i * fieldCount + 3]);
    //         iapItems[i].isHiden = int.Parse(productConfig[i * fieldCount + 5]) == 1;
    //         iapItems[i].isNoviceGift = productConfig[i * fieldCount].Contains("NoviceGift");
    //     }
    // }

    public static bool isShouldShowNoviceGift () {
        //升级新版本是否超过24小时
        bool isMoreThan24Hours = false;
        if (PlayerPrefs.HasKey ("installVersion_1.0.11_timestamp")) {
            double installTime = double.Parse (PlayerPrefs.GetString ("installVersion_1.0.11_timestamp"));
            double delta = CUtils.GetCurrentTime () - installTime;
            // 判断间隔时长是否超过36小时
            isMoreThan24Hours = delta > 60 * 60 * 36;
        }

        //安装以来是否付费过
        bool isPayed = PlayerPrefs.HasKey ("paySuccess");

        //新手礼包是否支付过
        string GiftId = PlayerPrefs.GetString ("noviceGiftId");
        bool giftPayed = GiftId.Contains ("com.zcwfydev.hexablock.NoviceGift");
        return isMoreThan24Hours && !isPayed && !giftPayed;
    }
}