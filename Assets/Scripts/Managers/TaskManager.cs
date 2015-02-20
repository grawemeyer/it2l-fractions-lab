using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using fractionslab.utils;
using CodeTitans.JSon;
using SBS.Math;
using taskDependentSupport;

public class TaskManager : MonoBehaviour {

    #region Singleton
    protected static TaskManager instance = null;
    public static TaskManager Instance
    {
        get
        {
            if (null == instance)
                instance = GameObject.FindGameObjectWithTag("TaskManager").GetComponent<TaskManager>();
            return instance;
        }
    }
    #endregion

    public List<Element> elements;
    public Dictionary<string, bool> initialConfiguration;
    public Task task;
    public bool taskDescriptionIsActive = true;
    public bool popupDescriptionIsActive = false;
    public bool isLoad = false;

    void Awake() 
    {
        popupDescriptionIsActive = false;
        taskDescriptionIsActive = true;

        initialConfiguration = new Dictionary<string, bool>();

        initialConfiguration.Add("lab", true);
        initialConfiguration.Add("lines", true);
        initialConfiguration.Add("rectangles", true);
        initialConfiguration.Add("sets", true);
        initialConfiguration.Add("liquids", true);
        initialConfiguration.Add("change_color", true);
        initialConfiguration.Add("copy", true);
        initialConfiguration.Add("partition", true);
        initialConfiguration.Add("highlight", true);
        initialConfiguration.Add("add", true);
        initialConfiguration.Add("subtract", true);

        elements = new List<Element>();
        task = new Task();
    }

    public IEnumerator LoadJson(string _url)
    {
        string url = _url+ "?" + (UnityEngine.Random.Range(1, 100000000)).ToString();
        Application.ExternalCall("ShowMessage", url);

        WWW www = new WWW(url);

        yield return www;
        if (www.error == null)
        {
            ProcessTaskInformationPackage(www.text);
            isLoad = true;
        }
        else
        {
            Debug.Log("ERROR: " + www.error);
            taskDescriptionIsActive = false;
            isLoad = false;
        }
    }

    public void ProcessTaskInformationPackage(string jsonString)
    {
        JSonReader reader = new JSonReader();
        IJSonObject jsonObj = reader.ReadAsJSonObject(jsonString);
        elements = new List<Element>();

        ParseInitialModel(jsonObj);
        ParseInitialConfiguration(jsonObj);
        ParseExtraInformation(jsonString);
         taskDescriptionIsActive = ParseTaskDescription(jsonObj);
    }

    private void ParseExtraInformation(string jsonObj)
    {
        TDSWrapper.SetTaskInformationPackage(jsonObj);
    }

    private bool ParseTaskDescription(IJSonObject jsonObj)
    {
        try
        {
            if (null == jsonObj["task_description"])
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
       
        try
        {
            task.id = jsonObj["task_description"]["id"].ToString();
        }
        catch (Exception ex)
        {
            task.id = "";
        }
        try
        {
            task.title = jsonObj["task_description"]["title"].ToString();
        }
        catch (Exception ex)
        {
            task.title = "";
        }
        try
        {
            task.description = jsonObj["task_description"]["desctiption"].ToString();
        }
        catch (Exception ex)
        {
            task.description = "";
        }
        try
        {
            bool result = Boolean.TryParse(jsonObj["task_description"]["showAtStartup"].ToString(), out popupDescriptionIsActive);
            if (!result)
                popupDescriptionIsActive = false;
        }
        catch (Exception ex)
        {
            popupDescriptionIsActive = false;
        }
       
        return true;
    }

    private bool ParseInitialConfiguration(IJSonObject jsonObj)
    {
        bool result;
        bool haveGotInitialConf = true;
        try
        {
            if (null == jsonObj["initial_configuration"])
            {
                haveGotInitialConf = false;
                return false;
            }
        }
        catch (Exception ex) {
            haveGotInitialConf = false;
            return false;
        }

        bool active;
        for (int i = 0; i < jsonObj["initial_configuration"].Count; i++)
        {
            try
            {
                result = false;
                active = true;
                if (haveGotInitialConf)
                {
                    result = Boolean.TryParse(jsonObj["initial_configuration"][i]["active"].ToString(), out active);
                }
                if (!haveGotInitialConf || !result)
                    active = true;

                if (initialConfiguration.ContainsKey(jsonObj["initial_configuration"][i]["item"].ToString()))
                {
                    initialConfiguration[jsonObj["initial_configuration"][i]["item"].ToString()] = active;
                    int index = (int)Enum.Parse(typeof(configurationName), jsonObj["initial_configuration"][i]["item"].ToString());
                    if(index >= 0  && index < GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().initialConfigurationList.Count)
                        GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().ChangeStateButton(index, initialConfiguration[jsonObj["initial_configuration"][i]["item"].ToString()]);
                }
            }
            catch (Exception ex){
                return false;
            }
        }
        return true;
    }

    private bool ParseInitialModel(IJSonObject jsonObj)
    {
        Element el;
        SBSVector3 pos = new SBSVector3();
        bool result;
        
        try
        {
            if (null == jsonObj["initial_model"])
                return false;
        }
        catch (Exception ex)
        {
            return false;
        }  

        for (int i = 0; i < jsonObj["initial_model"].Count; i++)
        {
            el = new Element();
            //NUMERATOR
            try
            {
                result = Int32.TryParse(jsonObj["initial_model"][i]["numerator"].ToString(), out el.numerator);
                if (!result)
                    el.numerator = 1;
                else
                    el.numerator = Mathf.Max(0, el.numerator);
            }
            catch (Exception ex)
            {
                el.numerator = 1;
            }

            //DENUMERATOR
            try
            {
                result = Int32.TryParse(jsonObj["initial_model"][i]["denominator"].ToString(), out el.denominator);
                if (!result)
                    el.denominator = 1;
                else
                    el.denominator = Mathf.Max(1, el.denominator);
            }
            catch (Exception ex)
            {
                el.denominator = 1;
            }

           

            //PARTTION
            try
            {
                result = Int32.TryParse(jsonObj["initial_model"][i]["partition"].ToString(), out el.partitions);
                if (!result)
                    el.partitions = 1;
                else
                    el.partitions = Mathf.Max(1, el.partitions);
            }
            catch (Exception ex)
            {
                el.partitions = 1;
            }
            el.partNumerator = el.numerator * el.partitions;
            el.partDenominator = el.denominator * el.partitions;

            if ((float)el.partNumerator / (float)el.partDenominator > Workspace.MAXVALUE)
            {
                el.partNumerator = el.partDenominator * Workspace.MAXVALUE;
                el.numerator = el.partDenominator / el.partitions;
            }

            el.state = ElementsState.Fraction;
            el.mode = InteractionMode.Moving;

            //TYPE
            try
            {
                el.type = (ElementsType)Enum.Parse(typeof(ElementsType), jsonObj["initial_model"][i]["type"].ToString());
            }
            catch (Exception ex)
            {
                el.type = ElementsType.HRect;
            }

            //POSITION
            try
            {
                result = float.TryParse(jsonObj["initial_model"][i]["position"]["x"].ToString(), out pos.x);
               
                if (!result)
                    pos.x = 0.0f;
                else
                    pos.x = Mathf.Clamp(pos.x, -8.0f, 8.0f);
                result = float.TryParse(jsonObj["initial_model"][i]["position"]["y"].ToString(), out pos.y);
                if (!result)
                    pos.y = 0.0f;
                else
                    pos.y = Mathf.Clamp(pos.y, -5.0f, 5.0f);
                /* result = float.TryParse(jsonObj["fractions"][i]["position"]["z"].ToString(), out pos.z);
                 if (!result)
                     pos.z = 0.0f;*/
                el.position = pos;
            }
            catch (Exception ex)
            {
                el.position = new SBSVector3(0.0f, 0.0f, 0.0f);
            }

            try
            {
                switch (jsonObj["initial_model"][i]["color"].ToString())
                {
                    case "yellow":
                        el.color = Workspace.Instance.colorList[1];
                        break;
                    case "blue":
                        el.color = Workspace.Instance.colorList[2];
                        break;
                    case "purple":
                        el.color = Workspace.Instance.colorList[3];
                        break;
                    case "red":
                        el.color = Workspace.Instance.colorList[0];
                        break;
                    default:
                        el.color = Workspace.Instance.colorList[0];
                        break;
                }
            }
            catch (Exception ex)
            {
                el.color = Workspace.Instance.colorList[0];
            }
            elements.Add(el);
            // Debug.Log("element i " + i +" "+ el);
        }
        return true;
    }

}
