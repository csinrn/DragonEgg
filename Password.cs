using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//第一個是getEgg password  後typeNum個是龍的品種
[System.Serializable]
public class PasswordP
{
    public Password[] pwarray;
}


[System.Serializable]
public class Password
{
    public string key;
    public string content;
}
