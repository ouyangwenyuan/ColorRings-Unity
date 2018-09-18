//#define DEVELOPMENT
public class CommonConst {

    public const string androidPackageID = "androidPackageID"; //android package name
    public const string iosAppID = "iosAppID"; //ios bundleid

    public const float SCALED_TILE = 0.53f; //底部六边形缩放倍数
    public static readonly string[] WORLD_NAME = { "Beginner", "Advanced", "Master", "Expert", "Platinum", "Gold", "Diamond" };
    public const int MAX_LEVEL = 60;

    public static class PrefKeys {
        public const string CURRENCY = "coins";
        public const string SAVE_VERSION_CODE = "save_version_code";
        public const string CURRENT_LEVEL = "current_level";

        public const string HISTORY_LEVEL = "_history_level";
    }
    //后台配置文件版本号(根据version请求后台配置)
    public const string version = "1.0.x";
    // 是否是测试程序内部统计开关 (上线 false)
    public const bool IS_DEBUG = true;

    public const int int_version = 111;
    // 后台请求url
    public const string request_url = "http://gameof.thrones.trafficmanager.net/legend/init?appid=com.whatevername.hexablock&version=" + version;

    public const int skip_level_num = 200;

    public const string ad_interstitial_id = "174321089944325_198826680827099"; //插屏广告
    public const string ad_rewardedvideo_id = "174321089944325_198815740828193"; //奖励广告

    public const string PREF_AD_REWARD_COUNT_LIMIT = "PREF_AD_REWARD_COUNT_LIMIT"; // 最大个数限制
    public const string PREF_AD_REWARD_TEMP_COUNT = "PREF_AD_REWARD_TEMP_COUNT"; // 程序内部临时统计reward的个数
    public const string PREF_AD_INTERSTITIAL_INTERVAL = "PREF_AD_INTERSTITIAL_INTERVAL"; // 插屏广告的时间间隔

    public const string PREF_AD_INTERSTITIAL_TIME = "PREF_AD_INTERSTITIAL_TIME"; // 程序内部,插屏广告时间flag
    public const string PREF_AD_REWARD_TIME = "PREF_AD_REWARD_TIME"; // 程序内部,奖励广告时间flag

    public const string PREF_OPEN_LOCAL_NOTICE = "PREF_OPEN_LOCAL_NOTICE"; // 内部推送开关

    public const string PREF_AD_REWARD_SWITCH = "PREF_AD_REWARD_SWITCH"; // REWARD开关
    public const string PREF_AD_INTERSTITIAL_SWITCH = "pref_AD_INTERSTITIAL_SWITCH"; //插屏广告开关

    public const string PREF_NOTIFICATION_DELAY_DAYS = "PREF_NOTIFICATION_DELAY_DAYS"; //推送间隔时间

    public const string PREF_ONE_OFFER_UI = "PREF_ONE_OFFER_UI"; //UI ONE TIME OFFER 展示开关

    public const string PREF_SIGINEDIN_DAY = "PREF_SIGINEDIN_DAY"; //记录累计签到的次数

    public const string PREF_RECORD_SIGINEDIN_TIME = "PREF_RECORD_SIGINEDIN_TIME"; // 记录每次签到的时间

    public const string PREF_EXP_LEVEL = "PREF_EXP_LEVEL"; //用户跳关值key temp

    public const string PREF_NEW_REAL_CURRENT_LEVEL = "PREF_NEW_REAL_CURRENT_LEVEL";
    public const string PREF_SKIP_TOTAL_EXP_LEVEL = "PREF_SKIP_TOTAL_EXP_LEVEL"; //用户总共跳过的关卡个数
    public const string PREF_SKIP_FIRST_LEVEL = "PREF_SKIP_FIRST_LEVEL"; //用户第一次开始跳关大于1至少为2的时候的关卡值,只会记录一次
    // public const string PREF_5_10_HINTS = "PREF_5_10_HINTS";

    public const string PREF_LEVEL_5_EXP = "PREF_LEVEL_5_EXP";

    // public const iTween.DimensionMode ITWEEN_MODE = iTween.DimensionMode.mode2D;

#if DEVELOPMENT
    public const int MIN_INVITE_FRIEND = 1;
    public const int MAX_INVITE_FRIEND = 20;
    public const bool ENCRYPTION_PREFS = false;
    public const int MIN_LEVEL_TO_RATE = 1;
    public const int ADS_PERIOD = 5;
#else
    public const int MIN_INVITE_FRIEND = 40;
    public const int MAX_INVITE_FRIEND = 50;
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
    public const bool ENCRYPTION_PREFS = true;
#else
    public const bool ENCRYPTION_PREFS = false;
#endif
    public const int MIN_LEVEL_TO_RATE = 3;
    public const int ADS_PERIOD = 2 * 60;
#endif

    public const int MAX_FRIEND_IN_MAP = 15;
    public const int FACE_AVATAR_SIZE = 100;

    public const int TOTAL_LEVELS = 50;

    public const int PER_TIP_COINS = 100; // 每个提示金币数

    public const int default_total_levels = 42 * 24; //默认关卡数，这个数字和MapScene 的 Mapcreater 对象的mappos必须一一对应，且和游戏中具体关卡一一对应，否则会有数组越界异常。

    public const string rewardedVideoAd_click = "rewardedVideoAd_click";
    public const string rewardedVideoAd_show = "rewardedVideoAd_show";
    public const string interstitialAd_load = "interstitialAd_load";
    public const string interstitialAd_show = "interstitialAd_show";

    public const string purchaseRemoveAD = "REMOVE_AD";

    public const string GAME_START_TIMESTAMP = "game_scene_start_timestamp"; // 统计总时间返回某个scene,每次更新
    public const string GAME_LEVEL_START_TIESTAMP = "game_level_start_timestamp"; // 统计每个Level的时间戳,每次更新

    public const string GAME_LEVEL_HINTS_TJ = "game_level_hints_tj"; // 统计每个Level的时间戳,每次更新
    public const string GAME_LEVEL_COINS_TJ = "game_level_coins_tj"; // 统计每个Level的时间戳,每次更新

    //点击商品条目
    // public const string click_purchase_item = "purchase_item";

    //点击关卡按钮
    public const string click_level_number = "level_number";
    public const string reduce_hint = "reduce_hint";
    public const string reduce_coins = "reduce_coins";

    //点击重玩
    public const string click_replay_btn = "replay_btn";

    //弹当前关完成时
    public const string complete_level_alert = "complete_alert";
    public const string complete_winning = "complete_winning";
    public const string Restore_Successful = "Restore Successful";
    //开始购买
    public const string Purchase_Start = "Purchase Start";
    //购买成功
    public const string Purchase_Successful = "Purchase Successful";
    //购买失败
    public const string Purchase_Fail = "Purchase Fail";
    //购买取消
    public const string Purchase_Canceled = "Purchase Canceled";

    // 插装本地用户分级埋点
    public const string user_level_buy_count = "user_level_buy_count"; //用户消费金币第一周累计统计key

    public const string user_level_coin_count = "user_level_price"; //用户消费金币第一周累计统计key
    public const string user_level_consumer_level = "user_level_consumer_level"; //用户的消费等级key
    public const string user_install_time = "user_install_time"; //用户安装的时间统计key
    public const string user_level = "user_level"; //统计用户等级
    public const string App_Start_At_Hour = "App_Start"; //参数:  time: 小时，24小时制

}