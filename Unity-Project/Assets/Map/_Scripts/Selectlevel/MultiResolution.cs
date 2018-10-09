using System.Collections;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class MultiResolution : MonoBehaviour {
    private Transform m_tranform; // tranform need scale
    private static float BASE_WIDTH = 480f;
    private static float BASE_HEIGHT = 800f;
    private float baseRatio;
    private float percentScale;
    void Start () {
        m_tranform = transform;
        setScale ();
    }

    /// <summary>
    /// scale tranform by width and high of scene
    /// </summary>
    void setScale () {
        // #if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8
        // if(isIphoneX()){
        //     baseRatio = (float)BASE_WIDTH / BASE_HEIGHT * Screen.height;
        //     percentScale = Screen.width / baseRatio;
        //     m_tranform.localScale = new Vector3(m_tranform.localScale.x * percentScale, m_tranform.localScale.x * percentScale, 1);
        //     return;
        // }

        // #endif
        if ((!isIPad () && !isMac ())) {
            baseRatio = (float) BASE_WIDTH / BASE_HEIGHT * Screen.height;
            percentScale = Screen.width / baseRatio;
            m_tranform.localScale = new Vector3 (m_tranform.localScale.x * percentScale, m_tranform.localScale.y, 1);
        }
        // m_tranform.localScale = new Vector3(0.8f, 0.8f, 1);
    }

    public static bool isIphoneX () {
        return Screen.height >= 2436f;
    }
    public static bool isIPad () {
        return SystemInfo.deviceModel.Contains ("iPad");
    }
    public static bool isMac () {
        bool isMac = false;
        //		#if UNITY_STANDALONE_OSX
        //		Debug.Log("Stand Alone OSX");
        //		isMac  =true;
        //		#endif
        if (Application.platform == RuntimePlatform.OSXEditor) {
            isMac = true;
        }
        Debug.Log ("Application.platform-->>" + Application.platform);
        return isMac;

    }

    public static bool isIPhones () {
        return SystemInfo.deviceModel.Contains ("iPhone") || isIPhone ();
    }

    public static bool isIPhone () {
#if UNITY_IOS
        return (DeviceGeneration.iPhoneUnknown == Device.generation ||
            Device.generation == DeviceGeneration.iPhone ||
            Device.generation == DeviceGeneration.iPhone3G ||
            Device.generation == DeviceGeneration.iPhone3GS ||
            Device.generation == DeviceGeneration.iPhone4 ||
            Device.generation == DeviceGeneration.iPhone4S ||
            Device.generation == DeviceGeneration.iPhone5 ||
            Device.generation == DeviceGeneration.iPhone5C ||
            Device.generation == DeviceGeneration.iPhone5S ||
            Device.generation == DeviceGeneration.iPhone6 ||
            Device.generation == DeviceGeneration.iPhone6Plus ||
            Device.generation == DeviceGeneration.iPhone6S ||
            Device.generation == DeviceGeneration.iPhone6SPlus ||
            Device.generation == DeviceGeneration.iPhone7 ||
            Device.generation == DeviceGeneration.iPhone7Plus ||
            Device.generation == DeviceGeneration.iPhoneSE1Gen);
#endif
    return false;
    }
}