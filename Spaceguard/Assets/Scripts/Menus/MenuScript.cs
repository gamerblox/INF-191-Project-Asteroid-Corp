using UnityEngine;
using Game.Cameras;

namespace Game.Menus.Unused
{
    public class MenuScript : MonoBehaviour
    {

        //Menu States
        public enum MenuStates { mainM, optionsM, scenarioS, spaceS, endS, missionS, buildS, alertsM, inventoryM, simulationM };

        public MenuStates currentstate;
        public MenuStates previousstate;


        //Menu Panel Objects
        public GameObject mainMenu;
        public GameObject optionsMenu;
        public GameObject scenarioScreen;
        public GameObject spaceScreen;
        public GameObject endScreen;
        public GameObject missionScreen;
        public GameObject buildScreen;
        public GameObject alertsMenu;
        public GameObject inventoryMenu;
        public GameObject simulationMenu;

        GameObject cameraObj; //for accessing the script on the main camera *temp
        CameraShaker cameraShake; //for accessing the cameraShake boolean on the main camera script *temp

        void Start()
        {
            cameraObj = GameObject.Find("Main Camera"); //find the main camera object *temp
            cameraShake = cameraObj.GetComponent<CameraShaker>(); //get access to the CameraShaker class *temp
                                                                  //default at start is main menu
            currentstate = MenuStates.mainM;
            mainMenu.SetActive(true);
            optionsMenu.SetActive(false);
            scenarioScreen.SetActive(false);
            spaceScreen.SetActive(false);
            endScreen.SetActive(false);
            missionScreen.SetActive(false);
            buildScreen.SetActive(false);
            alertsMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            simulationMenu.SetActive(false);
            previousstate = currentstate;
        }

        void Awake()
        {
            //default at start is main menu
            currentstate = MenuStates.mainM;
            mainMenu.SetActive(true);
            optionsMenu.SetActive(false);
            scenarioScreen.SetActive(false);
            spaceScreen.SetActive(false);
            endScreen.SetActive(false);
            missionScreen.SetActive(false);
            buildScreen.SetActive(false);
            alertsMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            simulationMenu.SetActive(false);
            previousstate = currentstate;
        }

        void Update()
        {
            //check current screen state
            switch (currentstate)
            {
                case MenuStates.mainM:
                    //set active gameobject for main menu
                    mainMenu.SetActive(true);
                    optionsMenu.SetActive(false);
                    scenarioScreen.SetActive(false);
                    spaceScreen.SetActive(false);
                    endScreen.SetActive(false);
                    missionScreen.SetActive(false);
                    buildScreen.SetActive(false);
                    alertsMenu.SetActive(false);
                    inventoryMenu.SetActive(false);
                    simulationMenu.SetActive(false);
                    break;

                case MenuStates.optionsM:
                    //set active gameobject for options menu
                    mainMenu.SetActive(false);
                    optionsMenu.SetActive(true);
                    scenarioScreen.SetActive(false);
                    spaceScreen.SetActive(false);
                    endScreen.SetActive(false);
                    missionScreen.SetActive(false);
                    buildScreen.SetActive(false);
                    alertsMenu.SetActive(false);
                    inventoryMenu.SetActive(false);
                    simulationMenu.SetActive(false);
                    break;

                case MenuStates.scenarioS:
                    mainMenu.SetActive(false);
                    optionsMenu.SetActive(false);
                    scenarioScreen.SetActive(true);
                    spaceScreen.SetActive(false);
                    endScreen.SetActive(false);
                    missionScreen.SetActive(false);
                    buildScreen.SetActive(false);
                    alertsMenu.SetActive(false);
                    inventoryMenu.SetActive(false);
                    simulationMenu.SetActive(false);
                    break;

                case MenuStates.spaceS:
                    mainMenu.SetActive(false);
                    optionsMenu.SetActive(false);
                    scenarioScreen.SetActive(false);
                    spaceScreen.SetActive(true);
                    endScreen.SetActive(false);
                    missionScreen.SetActive(false);
                    buildScreen.SetActive(false);
                    alertsMenu.SetActive(false);
                    inventoryMenu.SetActive(false);
                    simulationMenu.SetActive(false);
                    break;

                case MenuStates.endS:
                    mainMenu.SetActive(false);
                    optionsMenu.SetActive(false);
                    scenarioScreen.SetActive(false);
                    spaceScreen.SetActive(false);
                    endScreen.SetActive(true);
                    missionScreen.SetActive(false);
                    buildScreen.SetActive(false);
                    alertsMenu.SetActive(false);
                    inventoryMenu.SetActive(false);
                    simulationMenu.SetActive(false);
                    break;

                case MenuStates.missionS:
                    mainMenu.SetActive(false);
                    optionsMenu.SetActive(false);
                    scenarioScreen.SetActive(false);
                    spaceScreen.SetActive(false);
                    endScreen.SetActive(false);
                    missionScreen.SetActive(true);
                    buildScreen.SetActive(false);
                    alertsMenu.SetActive(false);
                    inventoryMenu.SetActive(false);
                    simulationMenu.SetActive(false);
                    break;

                case MenuStates.buildS:
                    mainMenu.SetActive(false);
                    optionsMenu.SetActive(false);
                    scenarioScreen.SetActive(false);
                    spaceScreen.SetActive(false);
                    endScreen.SetActive(false);
                    missionScreen.SetActive(false);
                    buildScreen.SetActive(true);
                    alertsMenu.SetActive(false);
                    inventoryMenu.SetActive(false);
                    simulationMenu.SetActive(false);
                    break;

                case MenuStates.alertsM:
                    mainMenu.SetActive(false);
                    optionsMenu.SetActive(false);
                    scenarioScreen.SetActive(false);
                    spaceScreen.SetActive(false);
                    endScreen.SetActive(false);
                    missionScreen.SetActive(false);
                    buildScreen.SetActive(false);
                    alertsMenu.SetActive(true);
                    inventoryMenu.SetActive(false);
                    simulationMenu.SetActive(false);
                    break;

                case MenuStates.inventoryM:
                    mainMenu.SetActive(false);
                    optionsMenu.SetActive(false);
                    scenarioScreen.SetActive(false);
                    spaceScreen.SetActive(false);
                    endScreen.SetActive(false);
                    missionScreen.SetActive(false);
                    buildScreen.SetActive(false);
                    alertsMenu.SetActive(false);
                    inventoryMenu.SetActive(true);
                    simulationMenu.SetActive(false);
                    break;

                case MenuStates.simulationM:
                    mainMenu.SetActive(false);
                    optionsMenu.SetActive(false);
                    scenarioScreen.SetActive(false);
                    spaceScreen.SetActive(false);
                    endScreen.SetActive(false);
                    missionScreen.SetActive(false);
                    buildScreen.SetActive(false);
                    alertsMenu.SetActive(false);
                    inventoryMenu.SetActive(false);
                    simulationMenu.SetActive(true);
                    break;
            }
        }

        public void OnStartGame()
        {
            Debug.Log("You pressed start game.");
            //Add load level for new scene here.
            currentstate = MenuStates.scenarioS;
        }

        public void OnOptions()
        {
            Debug.Log("You pressed options.");
            //Display options menu.
            currentstate = MenuStates.optionsM;
        }

        public void OnWindowedMode()
        {
            Debug.Log("You pressed windowed mode.");
            //change resolutiont to window/fullscreen.
            currentstate = MenuStates.mainM;
        }

        public void OnMainMenu()
        {
            Debug.Log("You pressed windowed mode.");
            //retreat to main/start menu
            currentstate = MenuStates.mainM;
        }


        public void OnMissionScreen()
        {
            Debug.Log("You slected the mission screen.");
            //
            currentstate = MenuStates.missionS;
        }


        public void OnBuildScreen()
        {
            Debug.Log("You pressed the build screen.");
            //
            currentstate = MenuStates.buildS;
        }


        public void OnEndScreen()
        {
            Debug.Log("You selected the end screen.");
            //
            currentstate = MenuStates.endS;
        }


        public void OnScenarioScreen()
        {
            Debug.Log("You pressed scenario screen.");
            //change to scenario screen 
            currentstate = MenuStates.scenarioS;
        }


        public void OnSpaceScreen()
        {
            Debug.Log("You pressed Space Screen.");
            //Display Space/Play screen
            currentstate = MenuStates.spaceS;
        }

        public void ShakeButton()
        { //*temp method for testing camera shaking
            cameraShake.shouldShake = !cameraShake.shouldShake;
        }


        public void OnApplicationExit()
        {
            Debug.Log("You pressed Exit Game.");
            //Exit the application back to OS
            Application.Quit();
        }
    }//class end
}