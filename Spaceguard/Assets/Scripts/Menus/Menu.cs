using UnityEngine.UI;
using System.Collections;
using UnityEngine;

namespace Game.Menus
{
    /// <summary>
    /// Set of menu states represented in the game.
    /// </summary>
    public enum MenuState
    {
        MainMenu,
        OptionsMenu,
        Scenario,
        OrbitView,
        EndScreen,        
        MissionScreen,
        BuildScreen,
        Highscores,
        Achievements,
        WebEndScreen
    }

    /// <summary>
    /// Attach this component to an empty game object containing the full set
    /// of game objects associated with a particular <see cref="MenuState"/>.
    /// </summary>
    public class Menu : MonoBehaviour
    {
        public Text masterVolumeText;
        public Text effectsVolumeText;
        public Text musicVolumeText;

        /// <summary>
        /// The menu state that this game object represents.
        /// </summary>
        public MenuState State;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void MasterVolOnValueChanged(float value)
        {
            masterVolumeText.text = value.ToString("F0");
        }

        public void EffectsVolOnValueChanged(float value)
        {
            effectsVolumeText.text = value.ToString("F0");
        }

        public void MusicVolOnValueChanged(float value)
        {
            musicVolumeText.text = value.ToString("F0");
        }

    }
}