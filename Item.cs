using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//最後修code和所有加try catch

// 裝備藍的分隔線 會重疊
// 尾巴有四個,鯉魚王看不同顏色
// 加能力值的時候全屏有圖閃現出來再淡出

public class Item : MonoBehaviour {

    public string engName;
    public string chineseName;
    public string dragonPic;
    public string squarePic;
    public string content;
    public string part;
    public GameObject sprite = null;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // equip this item (all Item is hung on an ItemSet Gameobject)
    public void Equip() {
        try
        {
            // show outsideImage Panel, which has an image of red frame
            gameObject.GetComponentsInChildren<Image>()[1].color = new Color(255, 0, 0, 255);

            if (this.part != "Skills" && this.part != "Item")
            {
                // check if item sprite exist, if not, create one
                if (this.sprite == null)
                {
                    print("in item Equip if");
                    //create sprite
                    GameObject sprite = GameObject.Instantiate(Resources.Load<GameObject>("ItemSprite"));
                    sprite.name = this.engName;
                    SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
                    Texture2D texture2D = Resources.Load<Texture2D>(this.dragonPic);
                    Sprite img = Sprite.Create(texture2D, spriteRenderer.sprite.textureRect, new Vector2(0.5f, 0.5f));
                    spriteRenderer.sprite = img;
                    spriteRenderer.color = new Color(255, 255, 255, 255);
                    spriteRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;

                    this.sprite = sprite;
                    sprite.transform.SetParent(gameObject.transform);
                }
                // if exist, show it
                else
                {
                    print("in item Equip else");
                    this.sprite.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
                }

                if(gameObject.name == "hair")
                {
                    sprite.GetComponent<SpriteRenderer>().sortingOrder = 100;
                }
                if (gameObject.name == "ballon")
                {
                    sprite.GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
            }
        }
        catch
        {
            print("Item Equip err");
        }
    }

    // when strip, disShow the outer red square and dragon item sprite
    public void Strip()
    {
        try
        {
            print("in Strip");
            gameObject.GetComponentsInChildren<Image>()[1].color = new Color(255, 0, 0, 0);
            if(this.part != "Skills")
                this.sprite.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        }
        catch
        {
            print("Item Strip err");
        }
    }

    // make this item equals to itemList it
    public void Equal(ItemList it)
    {
        try
        {
            chineseName = it.chineseName;
            engName = it.engName;
            dragonPic = it.dragonPic;
            squarePic = it.squarePic;
            content = it.content;
            part = it.part;
            print("item Name :" + it.engName);
            gameObject.name = engName;

            //update square item
            Sprite image = Resources.Load<Sprite>(squarePic);
            gameObject.GetComponentsInChildren<Image>()[2].sprite = image;
            gameObject.GetComponentsInChildren<Image>()[2].transform.localScale = new Vector3(1, 1, 1);
            gameObject.GetComponentInChildren<Text>().text = chineseName;
            gameObject.transform.SetParent(GameObject.Find(part).transform);
            gameObject.transform.localPosition = new Vector3(0, 0, 0);
            gameObject.transform.localScale = new Vector3(1, 1, 1);

            if (this.part != "Skills" && this.part != "Item")
            {
                // create sprite
                this.sprite = GameObject.Instantiate(Resources.Load<GameObject>("ItemSprite"));
                this.sprite.transform.localPosition = new Vector3(0.515f, -1.43f, 0);
                sprite.name = this.engName;
                SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();

                // disShow the sprite
                Sprite img = Resources.Load<Sprite>(this.dragonPic);
                spriteRenderer.sprite = img;
                spriteRenderer.color = new Color(255, 255, 255, 0);
                spriteRenderer.sortingOrder = 2;

                // set itemSprite to the children of DragonSprite
                sprite.transform.SetParent(GameObject.Find("DragonSprite").transform);
            }
            else if (this.part == "Item")
            {
                // if part == Item, destroy script ButtonClicked 
                GameObject.Destroy(gameObject.GetComponent<ButtonClicked>());
            }
        }
        catch
        {
            print("Item Equal err");
        }
    }
}
