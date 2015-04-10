using UnityEngine;
using System.Collections;
using System.Text;

public class TestBehaviour : MonoBehaviour {
    public GameObject prefab;
    GameObject prova;
    GameObject prova1;

	void Start ()
    {
        prova = Instantiate(prefab) as GameObject;
        prova1 = Instantiate(prova) as GameObject;
        //prova.renderer.material.color = new Color(0.5f, 0.5f, 0.5f);

        prova1.renderer.materials = prova.renderer.materials;

       // StartCoroutine(SendData());
	}

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            prova.renderer.material.color = Color.red;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            prova1.renderer.material.color = Color.white;
        }
    }

    IEnumerator SendData()
    {
        Debug.Log("SendData");
        string ourPostData = "{\"someJSON\":42}";
        byte[] pData = Encoding.ASCII.GetBytes(ourPostData.ToCharArray());
        WWW www = new WWW("http://172.19.6.254/www_test/services/httpEcho.php", pData);
        yield return www;
        Debug.Log("www.text: " + www.text);
    }
}
