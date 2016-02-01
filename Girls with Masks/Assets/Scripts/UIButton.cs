using UnityEngine;
using System.Collections;

public class UIButton : MonoBehaviour
{
    public GameObject targetObject;
    public string targetMessage;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnMouseUp()
    {
        print("click");
        if (targetObject != null)
        {
            print("click");
            targetObject.SendMessage(targetMessage);
        }
    }
}
