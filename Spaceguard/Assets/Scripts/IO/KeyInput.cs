using Game.Audio;
using Game.Menus;
using UnityEngine;

public class KeyInput : MonoBehaviour {

    /// <summary>
    /// Keyboard input by individual key.
    /// See https://docs.unity3d.com/ScriptReference/KeyCode.html
    /// which contains a list of keycodes.
    /// KeyCode.Escape is equivalent to "escape" for GetKeyDown() or GetKeyUp()
    /// When testing in the Unity IDE, you must click the game window for keyboard events to be active.
    /// </summary>
    void Update () {
        if (Input.GetKeyDown("["))
        {
            FindObjectOfType<MenuManager>().LoadBuildScreen();
        }
        if (Input.GetKeyDown("]"))
        {
            FindObjectOfType<MenuManager>().LoadMissionScreen();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            callMute();
            //Debug.Log("M (MUTE) key was pressed.");
        }
    }

    public void callMute()
    {
        FindObjectOfType<AudioManager>().SetMasterMute();        
    }
}
