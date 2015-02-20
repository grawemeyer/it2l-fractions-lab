using UnityEngine;
using System.Collections;
using System.Text;

public class TestBehaviour : MonoBehaviour {

	void Start ()
    {
        StartCoroutine(SendData());
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
