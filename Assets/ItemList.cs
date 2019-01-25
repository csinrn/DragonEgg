using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//an Item type for Serialize
[System.Serializable]
public class ItemListP
{
    public ItemList[] itemarray;
}

[System.Serializable]
public class ItemList
{
    public string engName;
    public string chineseName;
    public string dragonPic;
    public string squarePic;
    public string content;
    public string part;

    public static ItemList Equal(Item it)
    {
        ItemList item = new ItemList
        {
            chineseName = it.chineseName,
            engName = it.engName,
            dragonPic = it.dragonPic,
            squarePic = it.squarePic,
            content = it.content,
            part = it.part
        };
        return item;
    }
}

