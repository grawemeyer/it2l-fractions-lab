using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using fractionslab.meshes;
using CodeTitans.JSon;
using taskDependentSupport;

public class ExternalEventsManager : MonoBehaviour
{
    #region Singleton instance
    protected static ExternalEventsManager instance = null;

    public static ExternalEventsManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    #region Protected Fields
    public Dictionary<string, string> embeddingVariables = new Dictionary<string, string>();
    #endregion

    #region Protected Fields
    protected GameObject workspace;
    protected GameObject interfaces;
    #endregion

    #region External Events
    public void SendBrowserMessage(params object[] args)
    {
#if !UNITY_IPHONE  &&  !UNITY_ANDROID
        Application.ExternalCall("newEvent", args);
#endif
    }

    public void SendMessageToSupport(params object[] args)
    {
#if UNITY_WEBPLAYER && !UNITY_EDITOR
        if (Debug.isDebugBuild)
        {
            string tmp = "";
            foreach (object ob in args)
                tmp += ob.ToString() + " "; 

            Application.ExternalCall("ShowMessage", tmp);
        }
#endif
        //Debug.LogWarning("******** SendMessageToSupport disabled for testing *********");
        //return;
        TDSWrapper.SendMessageToSupport(args);
    }
    #endregion

    #region External Callbacks
    public void SendEvent(string json)
    {
        JSonReader reader = new JSonReader();
        IJSonObject jsonObj = reader.ReadAsJSonObject(json);

        if (jsonObj.Contains("method") && jsonObj.Contains("parameters"))
        {
            switch (jsonObj["method"].ToString())
            {
                case ("PlatformEvent"):
                    if (jsonObj["parameters"].Contains("eventName"))
                        SendMessageToSupport("PlatformEvent", jsonObj["parameters"]["eventName"].ToString());
                    break;
                case ("LowFeedback"):
                    if (jsonObj["parameters"].Contains("message"))
                        if (null != interfaces)
                            interfaces.SendMessage("ShowLowFeedback", jsonObj["parameters"]["message"].ToString(), SendMessageOptions.DontRequireReceiver);
                    break;
                case ("HighFeedback"):
                    if (jsonObj["parameters"].Contains("message"))
                        if (null != interfaces)
                            interfaces.SendMessage("ShowHighFeedback", jsonObj["parameters"]["message"].ToString(), SendMessageOptions.DontRequireReceiver);
                    break;
                case ("Highlight"):
                    if (jsonObj["parameters"].Contains("target"))
                        if (null != workspace)
                            workspace.SendMessage("Highlight", jsonObj["parameters"]["target"].ToString(), SendMessageOptions.DontRequireReceiver);
                    break;
                case ("RemoveHighlight"):
                    if (null != workspace)
                        workspace.SendMessage("DestroyHighlight", SendMessageOptions.DontRequireReceiver);
                    break;
            }
        }
    }
    #endregion

    #region Unity callbacks
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("ExternalEventsManager component must be unique!");
            return;
        }

        instance = this;

        embeddingVariables.Clear();
        string src = Application.srcValue;
        string[] srcSplit = src.Split('?');
        if (srcSplit.Length == 2)
        {
            string[] paramsSplit = srcSplit[1].Split('&');
            for (int i = 0; i < paramsSplit.Length; i++)
            {
                string[] valueSplit = paramsSplit[i].Split('=');
                embeddingVariables.Add(valueSplit[0], valueSplit[1]);
            }
        }

#if UNITY_EDITOR
        //embeddingVariables.Add("language", "es");
        //embeddingVariables.Add("showStartPage", "false");
       // embeddingVariables.Add("username", "Pippo01");
        //embeddingVariables.Add("tip", "http://172.19.6.254/italk2learn/tip/Task01.tip");
#endif
    }

    void Start()
    {
        interfaces = GameObject.FindGameObjectWithTag("Interface");
        workspace = GameObject.FindGameObjectWithTag("Workspace");
        TDSWrapper.eventManager = gameObject;
        if (embeddingVariables.ContainsKey("idtask"))
            TDSWrapper.setTaskID(embeddingVariables["idtask"]);
    }
    #endregion
}
