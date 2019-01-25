using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Dragon : MonoBehaviour
{
    //public GameObject panel;
    public Sprite image = null;
    static public int dragonTypeNum = 4;
    private string breed;
    private bool getEgg;
    private bool hatched;
    private string dragonName;
    private int strength;
    private int intelligence;
    private int hp;
    public List<ItemList> status;
    public List<ItemList> equipped;

    public bool Hatched
    {
        get
        {
            return hatched;
        }

        set
        {
            hatched = value;
        }
    }

    public string DragonName
    {
        get
        {
            return dragonName;
        }

        set
        {
            dragonName = value;
        }
    }

    public static int DragonTypeNum
    {
        get
        {
            return dragonTypeNum;
        }

        set
        {

        }
    }

    public string Breed
    {
        get
        {
            return breed;
        }

        set
        {
            breed = value;
        }
    }

    public int Strength
    {
        get
        {
            return strength;
        }

        set
        {
            strength = value;
        }
    }

    public int Intelligence
    {
        get
        {
            return intelligence;
        }

        set
        {
            intelligence = value;
        }
    }

    public int Hp
    {
        get
        {
            return hp;
        }

        set
        {
            hp = value;
        }
    }

    public bool GetEgg
    {
        get
        {
            return getEgg;
        }

        set
        {
            getEgg = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        // load data if find a stored json in persistentDataPath, 
        // otherwise load a new dragon from Resources file
        print(Application.persistentDataPath + "/dragon.json");
        if (File.Exists(Application.persistentDataPath + "/dragon.json"))
        {
            var jsonData = File.ReadAllText(Application.persistentDataPath + "/dragon.json");
            print("in load data" + jsonData);
            DragonP temp = JsonUtility.FromJson<DragonP>(jsonData);
            print("test LoadData : int = " + temp.intelligence);
            gameObject.GetComponent<Dragon>().Equal(temp);
        }
        else
        {
            var jsonData = Resources.Load<TextAsset>("Dragon");
            DragonP temp = JsonUtility.FromJson<DragonP>(jsonData.text);
            this.Equal(temp);
        }
        print("in Start");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // turn hatched to true and render
    public bool Hatch(string breed)
    {
        try
        {
            Hatched = true;
            this.Breed = breed;
            if (breed == "redDragon")
            {
                this.strength = 40;
                this.intelligence = 20;
                this.hp = 33;
            }
            else if (breed == "greenDragon")
            {
                this.strength = 20;
                this.intelligence = 40;
                this.hp = 30;
            }
            else if (breed == "yellowDragon")
            {
                this.strength = 20;
                this.intelligence = 28;
                this.hp = 40;
            }
            else if (breed == "dragon")
            {
                this.strength = 30;
                this.intelligence = 30;
                this.hp = 30;
            }
            //////  need to be delete when dragon pic gotten
            this.Breed = "dragon";
            //////

            //load drgon image 
            this.image = Resources.Load<Sprite>(this.Breed);
            gameObject.GetComponent<SpriteRenderer>().sprite = this.image;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);

            RenderPanel();

            Save();
            return true;
        }
        catch
        {
            print("Hatch err");
            return false;
        }
    }

    // 
    public bool GetItem(ItemList item)
    {
        try
        {
            print("in GetItem");
            bool contain = false;

            // check if item is gotten before, if yes, contain = true
            for (int i = 0; i < status.Count; i++)
            {
                if (status[i].engName == item.engName)
                {
                    contain = true;
                    break;
                }
            }

            //if is not yet got , get this item
            if (!contain)
            {
                // create square item and choose parent
                GameObject t = Resources.Load<GameObject>("ItemSet");
                GameObject itemset = GameObject.Instantiate<GameObject>(t);
                itemset.GetComponent<Item>().Equal(item);

                // record item gotten and save data
                status.Add(item);
                this.Save();
                return true;
            }
            else
            {
                return false;
            }

        }
        catch
        {
            print("GetItem err");
            return false;
        }
    }

    // delete Item , if success, return true
    public bool DeleteItem(string it)
    {
        try
        {
            bool delSuc = false;

            //find if in status
            for (int i = 0; i < status.Count; i++)
            {
                if (status[i].engName == it)
                {
                    status.RemoveAt(i);
                    print(GameObject.Find(it));
                    Destroy(GameObject.Find(it));
                    print(GameObject.Find(it));

                    delSuc = true;
                    break;
                }
            }
            if (delSuc)
            {
                Save();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            print("Dragon Delete err");
            return false;
        }
    }

    // check if an item equipped or if same part equipped
    public bool CheckItem(Item item)
    {
        try
        {
            ItemList itemList = ItemList.Equal(item);
            // if already equipped or same part item already equipped, strip the item
            if (item.part != "Skills")
            {
                for (int i = 0; i < equipped.Count; i++)
                {
                    if (equipped[i].part == itemList.part)
                    {
                        if (equipped[i].engName == item.engName)
                        {
                            Strip(item);
                            return true;
                        }
                        else
                        {
                            GameObject t = GameObject.Find(equipped[i].engName);
                            Strip(t.GetComponent<Item>());
                        }
                    }
                }
            }
            else
            {
                int t = 0;
                int at = 0;
                for(int i = 0; i < equipped.Count; i++)
                {
                    if (equipped[i].engName == item.engName)
                    {
                        Strip(item);
                        return true;
                    }
                    else if(equipped[i].part == item.part && t == 0)
                    {
                        t += 1;
                        at = i;
                    }
                    else if(equipped[i].part == item.part)
                    {
                        GameObject k = GameObject.Find(equipped[at].engName);
                        Strip(k.GetComponent<Item>());
                    }
                }
            }

            //else, equipped it
            Equip(item);
            return true;
        }
        catch
        {
            return false;
        }
    }

    //equip the item
    public bool Equip(Item item)
    {
        try
        {
            //if (item.part != "Skills")
            //{
            // add to dragon.equipped list
            this.equipped.Add(ItemList.Equal(item));
            item.Equip();
            this.Save();
            //}
            return true;
        }
        catch
        {
            print("Equip err");
            return false;
        }
    }

    public bool Strip(Item item)
    {
        try
        {
            //remove from dragon.equipped list
            for (int i = 0; i < equipped.Count; i++)
            {
                if (equipped[i].engName == item.engName)
                {
                    equipped.RemoveAt(i);
                }
            }
            item.Strip();
            this.Save();
            return true;
        }
        catch
        {
            print("Equip err");
            return false;
        }
    }

    //set all pic, sprite, and item pic after scene loaded
    public void RenderOnload()
    {
        try
        {
            print("in Render");

            //create all item and item sprite
            for (int i = 0; i < status.Count; i++)
            {
                GameObject t = Resources.Load<GameObject>("ItemSet");
                GameObject itemSet = GameObject.Instantiate(t);
                itemSet.GetComponent<Item>().Equal(status[i]);
                for (int j = 0; j < equipped.Count; j++)
                {
                    if (equipped[j].engName == status[i].engName)
                    {
                        itemSet.GetComponent<Item>().Equip();
                    }
                }
            }
            RenderPanel();

            if (this.hatched)
            {
                // change to dragon image
                this.image = Resources.Load<Sprite>(this.Breed);
                gameObject.GetComponent<SpriteRenderer>().sprite = this.image;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
            }
            else if(this.getEgg == true)
            {
                // show egg image
                this.image = Resources.Load<Sprite>("egg");
                gameObject.GetComponent<SpriteRenderer>().sprite = this.image;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
            }
            else 
            {
                // load egg image
                this.image = Resources.Load<Sprite>("egg");
                gameObject.GetComponent<SpriteRenderer>().sprite = this.image;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
            }

        }
        catch
        {
            print("Dragon RenderOnload err");
        }
    }

    // render state panel
    public void RenderPanel()
    {
        if (this.hatched == true)
        {
            GameObject.Find("StrengthValue").GetComponent<Text>().text = Strength.ToString();
            GameObject.Find("IntelligenceValue").GetComponent<Text>().text = Intelligence.ToString();
            GameObject.Find("HpValue").GetComponent<Text>().text = Hp.ToString();
        }
        GameObject.Find("NameValue").GetComponent<InputField>().text = DragonName;
    }

    // make Dragon equals to DragonP
    public void Equal(DragonP dragon)
    {
        try
        {
            //this.image = dragon.image;
            this.Breed = dragon.breed;
            this.GetEgg = dragon.getEgg;
            this.Hatched = dragon.hatched;
            this.DragonName = dragon.dragonName;
            this.Strength = dragon.strength;
            this.Intelligence = dragon.intelligence;
            this.Hp = dragon.hp;
            for (int i = 0; i < dragon.status.Length; i++)
            {
                this.status.Add(dragon.status[i]);
            }
            for (int i = 0; i < dragon.equipped.Length; i++)
            {
                this.equipped.Add(dragon.equipped[i]);
            }
        }
        catch
        {
            print("Dragon Equal err");
        }
    }

    public void GetAbilityPoint(string part, int num)
    {
        if(part.Contains("Hp"))
        {
            Hp += num;
        }else if(part.Contains("Intelligence"))
        {
            Intelligence += num;
        }else if(part.Contains("Strength"))
        {
            Strength += num;
        }
        RenderPanel();
        Save();
    }

    // save current dragon into "dragon.json"
    public void Save()
    {
        try
        {
            print("in save");
            DragonP dra = DragonP.Equal(gameObject.GetComponent<Dragon>());
            string jsonData = JsonUtility.ToJson(dra);
            print(jsonData);
            File.WriteAllText(Application.persistentDataPath + "/dragon.json", jsonData);
        }
        catch
        {
            print("Dragon Save err");
        }
    }
}

// a Dragon type for Serialize
[System.Serializable]
public class DragonP
{
    public string dragonName;
    public string image;
    static public int dragonTypeNum = 3;
    public bool hatched;
    public string breed;
    public bool getEgg;
    public int strength;
    public int intelligence;
    public int hp;
    public ItemList[] status;
    public ItemList[] equipped;

    // return a DragonP equals to Dragon
    public static DragonP Equal(Dragon dragon)
    {
        DragonP d = new DragonP
        {
            image = dragon.Breed,
            breed = dragon.Breed,
            getEgg = dragon.GetEgg,
            hatched = dragon.Hatched,
            dragonName = dragon.DragonName,
            strength = dragon.Strength,
            intelligence = dragon.Intelligence,
            hp = dragon.Hp
        };

        d.status = dragon.status.ToArray();
        d.equipped = dragon.equipped.ToArray();

        return d;
    }
}