using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using Fungus;

// item 刪除後不要再能夠重複獲得
// 正確獲得物品可能會出現Wrong password 提示
// 尾巴要在頭髮後面，氣球要在尾巴後面
// 道具欄在拿到蛋前就要能輸入


public class ButtonClicked : MonoBehaviour
{
    public PasswordP passlist = new PasswordP();
    public ItemListP itemlist = new ItemListP();
    string passwordPath = "Assets";
    private string jsonData;
    public Dragon dragon;
    private bool passwordPanelOut = true;
    private bool ItemPanelOut = true;
    private static bool rendered = false;

    //public Flowchart flowchart;

    void Start()
    {
        //load password.json into passlist as PasswordP
        var t = Resources.Load<TextAsset>("password");
        passlist = JsonUtility.FromJson<PasswordP>(t.text);

        //load itemList.json into itemlist as ItemListP
        t = Resources.Load<TextAsset>("itemList");
        itemlist = JsonUtility.FromJson<ItemListP>(t.text);
        print(t.text);


        // find DragonSprite for following button usage 
        this.dragon = GameObject.Find("DragonSprite").GetComponent<Dragon>();

        // if get into dragon Scene, render dragon
        if (SceneManager.GetActiveScene().name == "dragon" && rendered == false)
        {
            rendered = true;
            dragon.RenderOnload();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // when start btn clicked, show password panel in flowchart
    // if getEgg , load to dragon scene 
    public void StartBtnClicked()
    {
        print("in StartBtn Clicked");
        DontDestroyOnLoad(dragon);

        Flowchart flowchart = GameObject.FindObjectOfType<Flowchart>();
        flowchart.SetBooleanVariable("Started", true);
    }

    public void FirstEnterClicked()
    {
        InputField input = GameObject.FindObjectOfType<InputField>();
        Text textBox = input.GetComponentInChildren<Text>();

        // check if inputText == getEgg password , if yes, load dragon Scene
        // if not, show "wrong Password"
        if (passlist.pwarray[0].key == textBox.text)
        {
            dragon.GetEgg = true;

            //save dragon as getEgg == true
            dragon.Save();

            DontDestroyOnLoad(dragon);
            Flowchart flowchart = GameObject.FindObjectOfType<Flowchart>();
            flowchart.SetBooleanVariable("getEgg", true);
            print(flowchart.GetBooleanVariable("getEgg"));
        }
        else
        {
            input.text = "";
            GameObject.Find("Placeholder").GetComponent<Text>().text = "Wrong Password";
        }
    }

    // if passwordBtn clicked, show password Panel if password Panel isnt shown,
    // else close the password Panel
    public void PasswordBtnClicked()
    {
        GameObject inputPanel = GameObject.Find("passwordPanel");
        if (passwordPanelOut)
        {
            Vector3 pos = new Vector3(-55, -10f, 0);
            inputPanel.transform.localPosition = pos;
            passwordPanelOut = false;
        }
        else
        {
            Vector3 pos = new Vector3(250f, 0.5f, 0);
            inputPanel.transform.localPosition = pos;
            passwordPanelOut = true;
        }
        // clear the inputBox
        inputPanel.GetComponentInChildren<InputField>().text = "";
        inputPanel.GetComponentInChildren<InputField>().GetComponentsInChildren<Text>()[1].text = "";
    }

    // if item Clicked, go to dragon.checked for equipped check
    public void ItemClicked()
    {
        Item item = EventSystem.current.currentSelectedGameObject.GetComponent<Item>();
        dragon.CheckItem(item);
    }

    public void DeleteBtnClicked()
    {
        InputField inputField = GameObject.Find("DeleteInputField").GetComponent<InputField>();
        Text placeholder = inputField.GetComponentsInChildren<Text>()[1];
        string input = inputField.text;
        print("Delete :" + input);

        string content = IsPassword(input);
        ItemList itemList = GetItemList(content);

        inputField.text = "";

        // if enter wrong pw, show "Wrong Password"
        if (content == null || itemList == null)
        {
            placeholder.text = "Wrong Password";
        }
        // if enter right pw, pass to dragon.DeleteItem()
        else if( itemList.part == "Item")
        {
            bool delSuc = false;
            delSuc = dragon.DeleteItem(content);
            if (delSuc)
            {
                placeholder.text = "Delete " + content;
            }
            else
            {
                placeholder.text = "Cant Find " + content;
            }
        }else
        {
            placeholder.text = "Cant delete " + content;
        }
    }
    
    public void DeleteBtnBackClicked()
    {
        GameObject.Find("dlPlaceholder").GetComponent<Text>().text = "";
    }

    public void EditBtnCkicked()
    {
        InputField[] InputFields = GameObject.FindObjectsOfType<InputField>();
        InputField nameInputField = null;
        //get nameUnputField = "NameValue" inputField
        for (int i = 0; i < InputFields.Length; i++)
        {
            if (InputFields[i].name == "NameValue")
            {
                nameInputField = InputFields[i];
                break;
            }
        }
        //set nameInputField, if new Name entered, change ame and save
        if (nameInputField.IsInteractable())
        {
            GameObject.Find("EditBtn").GetComponentInChildren<Text>().text = "Edit";
            string t = nameInputField.GetComponentsInChildren<Text>()[0].text;
            print("Input name :" + t);
            dragon.DragonName = t;
            dragon.Save();
            nameInputField.interactable = false;
            dragon.RenderPanel();
        }
        else
        {
            nameInputField.interactable = true;
            GameObject.Find("EditBtn").GetComponentInChildren<Text>().text = "OK";
        }
    }

    // password check
    public void EnterBtnClicked()
    {
        InputField inputfield = GameObject.Find("PwInputField").GetComponent<InputField>();
        Text input = inputfield.GetComponentInChildren<Text>();
        Text placeHolder = GameObject.Find("pwPlaceholder").GetComponent<Text>();
        string content = IsPassword(input.text);
        ItemList itemList = GetItemList(content);

        inputfield.text = "";
        if (content == null)
        {
            GameObject.Find("pwPlaceholder").GetComponent<Text>().text = "Wrong Password";
            return;
        }


        if (dragon.Hatched)
        {
            if (itemList != null)
            {
                placeHolder.text = "get " + content;
                if (itemList.part == "AbilityPoint")
                {
                    dragon.GetAbilityPoint(itemList.engName, int.Parse(itemList.content));
                }
                else    // item or back or head or ....
                {
                    if (!dragon.GetItem(itemList))
                    {
                        placeHolder.text = "reEnter " + content;
                    }
                }
            }
            else  // reEnter getEgg or dragon Code
            {
                placeHolder.text = "reEnter " + content;
            }
        }
        else  // if not Hatched 
        {
            if (itemList != null)
            {
                placeHolder.text = "get " + content;
                if (itemList.part == "Item")
                {
                    if (  !dragon.GetItem(itemList)  )
                        placeHolder.text = "reEnter " + content;
                }
                else {
                    placeHolder.text = "not Hatched";
                }
            }
            else   // check getEgg & dragon Code
            {
                if(content == "getEgg" && dragon.GetEgg == false)
                {
                    dragon.GetEgg = true;
                    dragon.Save();
                    dragon.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
                    placeHolder.text = "get Egg";
                }
                else if( content == "getEgg" && dragon.GetEgg == true){
                    placeHolder.text = "reEnter getEgg";
                }else{  // check dragon Code
                    if (dragon.GetEgg == true)
                    {
                        bool hatchSuc = dragon.Hatch(content);
                        if (hatchSuc) placeHolder.text = "Hatch";
                        else placeHolder.text = "Hatch err";
                    }
                    else
                    {
                        placeHolder.text = "haven't getEgg";
                    }
                }
            }
        }

    }

    // if is pw, return content, if not, return null
    private string IsPassword(string pw)
    {
        //check whether user enter a correct password
        string content = null;
        for (int i = 0; i < passlist.pwarray.Length; i++)
        {
            if (pw == passlist.pwarray[i].key)
            {
                content = passlist.pwarray[i].content;
                break;
            }
        }

        //if enter wrong pw , return null
        if (content == null)
        {
            return null;
        }

        return content;
    }

    private ItemList GetItemList(string content)
    {
        ItemList itemList = null;
        for (int i = 0; i < itemlist.itemarray.Length; i++)
        {
            if (content == itemlist.itemarray[i].engName)
            {
                itemList = itemlist.itemarray[i];
                return itemList;
            }
        }
        return null;
    }
}
