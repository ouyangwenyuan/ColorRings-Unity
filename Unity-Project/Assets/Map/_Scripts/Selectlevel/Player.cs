using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Player
{
    public int Level;
    public int RealLevel;
    public string Name;
    public bool Locked;
    public int Stars;
    public int HightScore;
    public int Background;
    public int giftType; //奖励关卡标示
    public int levelType; //特殊关卡标示


    public string ToSaveString()
    {
        // string s = Locked + "," + Stars + "," + HightScore + "," + Background + ",";
        string s = Level + "," + RealLevel + ",";
        return s;
    }

    // public Player(int level, int realLevel)
    // {
    //     this.Level = level;
    //     this.RealLevel = realLevel;
    //     this.Name = level.ToString();
    // }

    public Player(int level)
    {
        this.Level = level;
        if(level<=10){
            this.RealLevel = level;
        }
        this.Name = level.ToString();
    }

}


