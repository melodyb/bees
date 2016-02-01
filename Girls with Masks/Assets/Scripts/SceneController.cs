using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class SceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadFieldScene()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadHiveScene()
    {
        SceneManager.LoadScene(3);
    }

}
