using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

//
// SplashScreen Script
//
// Version 0.1 by Martijn Dekker
// martijn.pixelstudio@gmail.com
//
// Version 0.2 by Ferdinand Joseph Fernandez, 2010Sep7 16:45 GMT + 8
// Changes:
//  * changed sceneToLoad to a string, for easier usage
//  * added waitTime, which adds a pause after fade in, and before fade
//    out (during fade waiting)
//  * added option to either automatically fade out after waitTime
//    seconds (default), or wait for user input (press any key to continue)
//  * added option to wait until fade out is complete before loading next
//    level, instead of the default, which is to load the next level
//    before fade out
//
// Version 0.3 by Ferdinand Joseph Fernandez, 2010Sep8 01:13 GMT + 8
// Changes:
//  * splash screen itself is now fading without the need for a solid
//    background color
//  * optimized some code
//
// Version 0.4 by Ferdinand Joseph Fernandez, 2010Sep14 14:09 GMT + 8
// Changes:
//  * splash screen picture can now be either centered (default) or
//    stretched on the screen
//
// Version 0.5 by Ferdinand Joseph Fernandez, 2010Sep15 18:27 GMT + 8
// Changes:
//  * now has option to start automatically or not. if not started
//    automatically, the splash screen can be started by calling
//    the StartSplash function
//  * code acknowledges if the sceneToLoad is blank, in that case,
//    the code simply does not attempt to load a level
//
// Version 0.6 by Ferdinand Joseph Fernandez, 2010Sep29 13:43 GMT + 8
// Changes:
//  * added the property "gui depth" so you can control at which depth the
//    splash screen shows in
//

public class SplashScreen : MonoBehaviour
{
    public int guiDepth = 0;
    public string sceneToLoad = ""; // this has to correspond to a level (file>build settings)
    public Texture2D splashLogo; // the logo to splash;
    public float fadeSpeed = 0.3f;
    public float waitTime = 0.5f; // seconds to wait before fading out
    public bool waitForInput = false; // if true, this acts as a "press any key to continue"
    public bool startAutomatically = true;
    private float timeFadingInFinished = 0.0f;

    public enum SplashType
    {
        LoadNextLevelThenFadeOut,
        FadeOutThenLoadNextLevel
    }
    public SplashType splashType;

    private float alpha = 0.0f;

    private enum FadeStatus
    {
        Paused,
        FadeIn,
        FadeWaiting,
        FadeOut
    }
    private FadeStatus status = FadeStatus.FadeIn;

    private Camera oldCam;
    private GameObject oldCamGO;

    private Rect splashLogoPos = new Rect();
    public enum LogoPositioning
    {
        Centered,
        Stretched
    }
    public LogoPositioning logoPositioning;
    private float logoMargin = 20.0f;

    private bool loadingNextLevel = false;

    void Start()
    {
        if (startAutomatically)
        {
            status = FadeStatus.FadeIn;
        }
        else
        {
            status = FadeStatus.Paused;
        }
        oldCam = Camera.main;
        oldCamGO = Camera.main.gameObject;

        if (logoPositioning == LogoPositioning.Centered)
        {
            float logoMarginWidth = splashLogo.width + 2.0f * logoMargin;
            float logoMarginHeight = splashLogo.height + 2.0f * logoMargin;

            if (logoMarginWidth / logoMarginHeight > Screen.width / Screen.height)
            {
                splashLogoPos.x = logoMargin;
                splashLogoPos.width = Screen.width - 2.0f * logoMargin;

                splashLogoPos.height = splashLogoPos.width * splashLogo.height / splashLogo.width;
                splashLogoPos.y = (Screen.height - splashLogoPos.height) / 2.0f;
            }
            else
            {
                splashLogoPos.y = logoMargin;
                splashLogoPos.height = Screen.height - 2.0f * logoMargin;

                splashLogoPos.width = splashLogoPos.height * splashLogo.width / splashLogo.height;
                splashLogoPos.x = (Screen.width - splashLogoPos.width) / 2.0f;
            }
        }
        else
        {
            splashLogoPos.x = 0;
            splashLogoPos.y = 0;

            splashLogoPos.width = Screen.width - 2.0f * logoMargin;
            splashLogoPos.height = Screen.height - 2.0f * logoMargin;
        }



        if (splashType == SplashType.LoadNextLevelThenFadeOut)
        {
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(Camera.main);
        }
        if ((SceneManager.sceneCountInBuildSettings <= 1) || (sceneToLoad == ""))
        {
            Debug.LogWarning("Invalid sceneToLoad value.");
        }
    }

    public void StartSplash()
    {
        status = FadeStatus.FadeIn;
    }

    void Update()
    {
        switch (status)
        {
            case FadeStatus.FadeIn:
                alpha += fadeSpeed * Time.deltaTime;
                break;
            case FadeStatus.FadeWaiting:
                if ((!waitForInput && Time.time >= timeFadingInFinished + waitTime) || (waitForInput && Input.anyKey))
                {
                    status = FadeStatus.FadeOut;
                }
                break;
            case FadeStatus.FadeOut:
                alpha += -fadeSpeed * Time.deltaTime;
                break;
        }
    }

    void OnGUI()
    {
        GUI.depth = guiDepth;
        if (splashLogo != null)
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Clamp01(alpha));
            GUI.DrawTexture(splashLogoPos, splashLogo);
            if (alpha > 1.0f)
            {
                status = FadeStatus.FadeWaiting;
                timeFadingInFinished = Time.time;
                alpha = 1.0f;
                if (splashType == SplashType.LoadNextLevelThenFadeOut)
                {
                    oldCam.depth = -1000;
                    loadingNextLevel = true;
                    if ((SceneManager.sceneCountInBuildSettings >= 1) && (sceneToLoad != ""))
                    {
                        SceneManager.LoadScene(sceneToLoad);
                    }
                }
            }
            if (alpha < 0.0f)
            {
                if (splashType == SplashType.FadeOutThenLoadNextLevel)
                {
                    if ((SceneManager.sceneCountInBuildSettings >= 1) && (sceneToLoad != ""))
                    {
                        SceneManager.LoadScene(sceneToLoad);
                    }
                }
                else
                {
                    Destroy(oldCamGO); // somehow this doesn't work
                    Destroy(this);
                }
            }
        }
    }

    void OnLevelWasLoaded(int lvlIdx)
    {
        if (loadingNextLevel)
        {
            Destroy(oldCam.GetComponent<AudioListener>());
            Destroy(oldCam.GetComponent<GUILayer>());
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, .5f);
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}