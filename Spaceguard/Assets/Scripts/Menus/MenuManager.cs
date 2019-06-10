using System.Collections.Generic;
using Game.Cameras;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Menus
{
    /// <summary>
    /// Provides methods to "navigate" between different menu states/views.
    /// </summary>
    /// <remarks>
    /// The original implementation utilizes a single canvas containing all screens
    /// and menus, and simply hides/shows menu GameObjects to "switch" between states.
    /// TODO: Menus and screens would be better implemented as individual scene assets.
    /// </remarks>
    [AddComponentMenu("Game Managers/Menu Manager")]
    public class MenuManager : MonoBehaviour
    {
        /// <summary>
        /// Key-value pairs for each menu state and its associated GameObject.
        /// </summary>
        public Dictionary<MenuState, GameObject> Menus;

        /// <summary>
        /// The (default) menu state to open each time the application starts.
        /// </summary>
        public MenuState StateOnApplicationStart;

        /// <summary>
        /// The current menu GameObject that is active in the game.
        /// </summary>
        GameObject current;
        /// <summary>
        /// The previous menu GameObject that was active in the game.
        /// </summary>
        /// <remarks>
        /// TODO: This field is intended to be used as a simple way to implement "Back" button functionality.
        /// </remarks>
        GameObject previous;

        void Awake()
        {
            Menus = new Dictionary<MenuState, GameObject>();
        }

        void Start()
        {
            foreach (Menu menu in FindObjectsOfType<Menu>())
            {
                if (Menus.ContainsKey(menu.State))
                {
                    Debug.LogError("Multiple game objects assigned to menu state " + menu.State);
                }
                else
                {
                    Menus.Add(menu.State, menu.gameObject);
                    if (menu.State == StateOnApplicationStart)
                    {
                        menu.gameObject.SetActive(true);
                        current = menu.gameObject;
                    }
                    else
                    {
                        menu.gameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Activates specified menu.
        /// </summary>
        /// <remarks>
        /// Hides the current menu (old) and unhides the indicated menu state.
        /// </remarks>
        /// <param name="key">Key.</param>
        void SetActiveMenu(MenuState key)
        {
            if (key == current.GetComponent<Menu>().State)
            {
                Debug.Log("Menu " + key + " is already active");
                return;
            }

            GameObject temp = Menus[key];
            if (temp != null)
            {
                previous = current;
                current.SetActive(false);
                temp.SetActive(true);
                current = temp;
            }
            else
            {
                Debug.Log("Error retrieving Menus[" + key + "]");
            }
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        public void LoadMainMenu()
        {
            //Debug.Log("Loading Main Menu");
            SetActiveMenu(MenuState.MainMenu);
        }

        /// <summary>
        /// Loads the options menu.
        /// </summary>
        public void LoadOptionsMenu()
        {
            ////make current scene the previous one
            //previous = current;
            //Debug.Log(previous);

            //Debug.Log("Loading Options Menu");
            SetActiveMenu(MenuState.OptionsMenu);

            //Debug.Log(current);
            ////and then find gameobject of menustate
            //foreach (Menu menu in FindObjectsOfType<Menu>())
            //{
            //    if (menu.State == MenuState.OptionsMenu)
            //    {
            //        current = menu.gameObject;
            //        break;

            //    }

            //}

        }

        /// <summary>
        /// Loads the scenario (intro) screen.
        /// </summary>
        public void LoadScenario()
        {
            //Debug.Log("Loading Scenario");
            SetActiveMenu(MenuState.Scenario);
        }

        /// <summary>
        /// Loads the orbit view.
        /// </summary>
        public void LoadOrbitView()
        {
            //Debug.Log("Loading Orbit View");
            SetActiveMenu(MenuState.OrbitView);
        }

        /// <summary>
        /// Loads the end screen.
        /// </summary>
        public void LoadEndScreen()
        {
            //Debug.Log("Loading End Screen");
            SetActiveMenu(MenuState.EndScreen);
        }

        /// <summary>
        /// Loads the mission scene.
        /// </summary>
        public void LoadMissionScreen()
        {
            //Debug.Log("Loading MissionScreen");
            SetActiveMenu(MenuState.MissionScreen);
        }

        /// <summary>
        /// Loads the build screen.
        /// </summary>
        public void LoadBuildScreen()
        {
            //Debug.Log("Loading Build Screen");
            SetActiveMenu(MenuState.BuildScreen);
        }

        /// <summary>
        /// Loads the highscores screen.
        /// </summary>
        public void LoadHighscores()
        {
            Debug.Log("Loading Highscores");
            SetActiveMenu(MenuState.Highscores);
        }

        /// <summary>
        /// Loads the achievements screen.
        /// </summary>
        public void LoadAchievements()
        {
            //Debug.Log("Loading Achievements");
            SetActiveMenu(MenuState.Achievements);
        }

        public void SimulationMenu()
        {
            //Debug.Log("Loading Achievements");
            SetActiveMenu(MenuState.SimulationMenu);
        }

        public void AlertsMenu()
        {
            //Debug.Log("Loading Achievements");
            SetActiveMenu(MenuState.AlertsMenu);
        }

        public void InventoryMenu()
        {
            //Debug.Log("Loading Achievements");
            SetActiveMenu(MenuState.InventoryMenu);
        }

        public void OnFullscreenEnter()
        {
            print("hello");
            SceneManager.LoadScene("SpaceGuard");
        }

        public void OnFullscreenExit()
        {
            print("pls work");
        }

        /// <summary>
        /// Loads the <see cref="previous"/> menu; for use with a "Back" button.
        /// </summary>
        public void OnBack()
        {
            //Debug.Log("Back to " + previous);
            SetActiveMenu(previous.GetComponent<Menu>().State);
        }

        /// <summary>
        /// Activates the camera shake effect script.
        /// </summary>
        public void OnCameraShake()
        {
            CameraShaker shaker = GameObject.FindWithTag("Main Camera").GetComponent<CameraShaker>();
            if (shaker != null)
            {
                shaker.shouldShake = !shaker.shouldShake;
            }
        }

        /// <summary>
        /// Simulates exiting the application.
        /// </summary>
        public void ApplicationSimulatedExit()
        {
            //Debug.Log("Simulated exiting the application...");
            SetActiveMenu(MenuState.WebEndScreen);
        }

        /// <summary>
        /// Exits (quits) the application.
        /// </summary>
        public void OnApplicationExit()
        {
            //Debug.Log("Exiting the application...");
            Application.Quit();
        }
    }
}