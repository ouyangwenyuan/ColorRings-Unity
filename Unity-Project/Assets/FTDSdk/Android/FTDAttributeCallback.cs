using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTDAttributeCallback : AndroidJavaProxy
{
    public FTDAttributeCallback() : base("com.ftdsdk.www.api.FTAttributionListener")
    {
    }
    void onAttributionChanged(string channel, string attribution)
    {
        FTDSdk.getInstance().onAttributeCallback(attribution);
    }

}
