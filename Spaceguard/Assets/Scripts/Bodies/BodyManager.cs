#define FORCE_IMPACT_POSITION
using System.Collections;
using Game.IO;
using Game.Orbits;
using UnityEngine;
using UnityEngine.UI;
using Game.Menus;

namespace Game.Bodies
{
    [AddComponentMenu("Game Managers/Body Manager")]
    public class BodyManager : MonoBehaviour
    {
        const double MaxStepSize = 16.0;
        const double MinStepSize = 0.0625;

	    public GameObject manager;

        [Header("Simulation Settings")]
        public double Speed = 20;
        public double WarningTime = 10.0 * 365.25;
        public double ScaledTimeStep
        {
            get
            {
                if (IsPaused)
                {
                    return 0.0;
                }
                double stepScaled = stepSize * Speed * Time.deltaTime * Time.timeScale;
                return InReverse ? -stepScaled : stepScaled;
            }
        }

        [Header("UI Text References")]
        public Text EpochText;
        public Text StepSizeText;
        public Text MessageText;
        public Text DistanceText;
        public GameObject MissionValues;
        public Text LaunchEnergyText;
        public Text DeliverableMassText;
        public Text ImpactVelocityText;
        public Text InfeasibleLaunchText;
        public Text SetMissionParametersText;

        [Header("Mission Parameters")]
        public bool AutoLaunchOn = true;
        public bool UsingUserInput;
        public Button StartButton;
        public LaunchVehicleData LaunchVehicleType;
        public int NumLaunches = 1;
        public int EffectMultiplier = 1;
        public double TimeOfDeflection = 1280.0;
        public double TransferTime = 500.0;

        double missDistance = 1.5 * OrbitUtils.RADIUS_EARTH;

        [Header("NEO Characteristics")]
        public double Diameter = 0.14;
        public double Density = 1.5;
        public double Beta = 1.0;
        public double MassNeo;

        double initEpoch;
        double LaunchEpoch { get { return ImpactEpoch - TimeOfDeflection - TransferTime; } }
        double DeflectionEpoch { get { return ImpactEpoch - TimeOfDeflection; } }
        double MassKI;

        public double ImpactEpoch = 2461607.836805556;
        double totalSimulationTime;

        #region Fields

        bool _paused;
        public bool IsPaused
        {
            get { return _paused; }
            set
            {
                if (_paused != value)
                {
                    _paused = value;
                    foreach (var obj in hideOnPause)
                    {
                        obj.SetActive(!_paused);
                    }
                    foreach (var obj in showOnPause)
                    {
                        obj.SetActive(_paused);
                    }
                }
            }
        }

        bool InReverse;
        double stepSize = 1.0;

        public double CurrentEpoch { get { return Bodies[0].Orbit.Epoch; } }

        #endregion

        Body[] Bodies;

        GameObject[] showOnPause;
        GameObject[] hideOnPause;
        GameObject spacecraftObject;

        /// <summary>
        /// Sets all GameObjects in the scene active.
        /// </summary>
        /// <remarks>
        /// Call this method at the beginning of Start() to ensure that
        /// <see cref="GameObject.FindGameObjectsWithTag(string)"/> will
        /// retrieve all tagged objects, even if those objects were not
        /// active in the scene when the scene is played.
        /// </remarks>
        void SetActiveAllManaged()
        {
            foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (obj.scene == gameObject.scene)
                {
                    if (obj.tag == "Menu")
                    {
                        obj.SetActive(false); // Hide all menus
                    }
                    else
                    {
                        obj.SetActive(true); // Activate all other scene objects
                    }
                }
            }
        }

        #region Unity Methods

        void Start()
        {
            //SetActiveAllManaged(); // GameObject.FindGameObject only returns active

            // Check that text references are assigned.
            if (EpochText == null || StepSizeText == null || MessageText == null || DistanceText == null)
            {
                Debug.LogError("Missing one or more text references");
            }
            MessageText.enabled = false; // Disabled on start

            // Get list of pause-sensitive objects and initialize visibility.
            showOnPause = GameObject.FindGameObjectsWithTag("ShowOnPause");
            hideOnPause = GameObject.FindGameObjectsWithTag("HideOnPause");
            foreach (var obj in hideOnPause)
            {
                obj.SetActive(false);
            }

            // TODO: Eventually spacecraft will be initialized by prefab
            spacecraftObject = GameObject.FindGameObjectWithTag("Spacecraft");
            spacecraftObject.SetActive(false);

            // Get list of active bodies and initialize each.
            Bodies = FindObjectsOfType<Body>();
            if (Bodies.Length == 0)
            {
                Debug.LogError("No bodies active in scene");
            }
            //double initEpoch = LaunchEpoch - TransferTime; // Arbitrary.
            //double initEpoch = ImpactEpoch - 5 * 365.25;
            initEpoch = ImpactEpoch - WarningTime;
            totalSimulationTime = 1.2 * WarningTime;

#if FORCE_IMPACT_POSITION

            OrbitData tempEarth = new OrbitData("Earth");
            OrbitData tempAsteroid = new OrbitData("PDC17a");

            tempEarth.Epoch = ImpactEpoch;
            tempAsteroid.Epoch = ImpactEpoch;
            tempAsteroid.Position = tempEarth.Position;

            foreach (Body body in Bodies)
            {
                if (body.tag == "Earth")
                {
                    body.InitBody(tempEarth, initEpoch);
                }
                else if (body.tag == "Asteroid")
                {
                    body.InitBody(tempAsteroid, initEpoch);
                    body.SetImpactEpoch(ImpactEpoch);
                }
                body.UpdateTransformPositionClear();
            }
#else
            foreach (Body body in Bodies)
            {
                body.InitOrbit(initEpoch);
                body.UpdateTransformPositionClear();
                if (body.tag == "Asteroid")
                {
                    body.SetImpactEpoch(ImpactEpoch);
                }
            }
#endif

            // Set UI text content.
            SetEpochText();
            SetStepSizeText();
            DistanceText.enabled = false;

            // Initialize game state and start coroutines.
            IsPaused = true;
            StartCoroutine(OrbitLoop());

            if (UsingUserInput)
            {
                StartCoroutine(LaunchFromUserInput());
            }
            else if (AutoLaunchOn)
            {
                GameObject.FindWithTag("InputPanel").SetActive(false);
                StartCoroutine(AutoLaunch());
            }
        }

#endregion

#region Coroutines

        IEnumerator OrbitLoop()
        {
            while (true)
            {
                if (!IsPaused)
                {
                    StepAllBodyOrbits();
                }
                yield return null;
            }
        }

        IEnumerator AutoLaunch()
        {
            while (true)
            {
                if (!IsPaused && CurrentEpoch >= LaunchEpoch)
                {
                    LaunchSpacecraft();
                    break;
                }
                yield return null;
            }
        }

        IEnumerator LaunchFromUserInput()
        {
            InputManager inputManager = FindObjectOfType<InputManager>();
            if (inputManager == null)
            {
                Debug.LogError("No InputManager active in scene");
            }
            if (StartButton == null)
            {
                Debug.LogError("Start button does not have button component");
            }

            ShowInfeasibleLaunchWindow();

            while (true)
            {
                if (StartButton == null)
                {
                    break;
                }
                IsPaused = true;
                //StartButton.SetActive(inputManager.IsValidMission());
                if (inputManager.IsValidMission())
                {
                    Debug.Log("Valid mission!");
                    SetMissionParamsFromUserInput();
                    Debug.Log("D = " + TimeOfDeflection + ", TOF = " + TransferTime);
                    StartButton.interactable = LambertSolutionExists();
                }
                else
                {
                    StartButton.interactable = false;
                    //LambertText.text = "Set mission parameter(s)";
                }
                yield return null;
            }
        }

        public void OnStartButtonPressed()
        {
            //StartButton.gameObject.SetActive(false);
            StartButton = null;

            //SetMissionParamsFromUserInput();

            foreach (GameObject panel in GameObject.FindGameObjectsWithTag("InputPanel"))
            {
                panel.SetActive(false);
            }

            //double initEpoch = LaunchEpoch - 365.25; // Arbitrary.
            //foreach (Body body in Bodies)
            //{
            //    body.InitOrbit(initEpoch);
            //}

            StartCoroutine(AutoLaunch());
            IsPaused = false;
        }

        void SetMissionParamsFromUserInput()
        {
            InputManager manager = FindObjectOfType<InputManager>();
            if (manager == null || !manager.IsValidMission())
                return;
            TimeOfDeflection = manager.TimeOfDeflection;
            TransferTime = manager.TransferTimeDays;
            MassNeo = manager.Mass;
            LaunchVehicleType = manager.LaunchVehicleType;
        }

        void StepAllBodyOrbits()
        {
            foreach (Body body in Bodies)
            {
                body.Orbit.Step(ScaledTimeStep);
                body.UpdateTransformPosition();
            }
            SetEpochText();
        }

#endregion

#region UI Clickable Behaviors

        /// <summary>
        /// If <see cref="IsPaused"/>, steps the orbits by <see cref="ScaledTimeStep"/>,
        /// else pauses the simulation.
        /// </summary>
        void StepOrPause()
        {
            if (IsPaused)
            {
                StepAllBodyOrbits();
            }
            else
            {
                IsPaused = true;
            }
        }

        /// <summary>
        /// Step the orbits forward a single <see cref="ScaledTimeStep"/>.
        /// Routine for the "Step Forward" button.
        /// </summary>
        public void StepForward()
        {
            InReverse = false;
            StepOrPause();
        }

        /// <summary>
        /// Steps the orbits backward a single <see cref="ScaledTimeStep"/>.
        /// Routine for the "Step Backward" button.
        /// </summary>
        public void StepBackward()
        {
            InReverse = true;
            StepOrPause();
        }

        /// <summary>
        /// Plays the orbit simuilation. Routine for the "Play" button.
        /// </summary>
        public void PlayForward()
        {
            IsPaused = false;
            InReverse = false;
        }

        /// <summary>
        /// Rewind all bodies in the scene; play backwards. Routine for the
        /// "Play Backward" button.
        /// </summary>
        public void PlayBackward()
        {
            IsPaused = false;
            InReverse = true;
        }

        /// <summary>
        /// Pauses all bodies in the scene. Routine for the "Pause" button.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            InReverse = false;
        }

        public float GetCurrentProgressPercent()
        {
            return (float)((CurrentEpoch - initEpoch) / totalSimulationTime);
        }

        public void SkipToPercent(float percent)
        {
            double epoch = totalSimulationTime * percent + initEpoch;
            foreach (Body body in Bodies)
            {
                body.Orbit.Epoch = epoch;
                body.UpdateTransformPositionClear();
            }
        }

        /// <summary>
        /// Increases the step size for the orbiting bodies; increases the speed.
        /// </summary>
        public void IncreaseStepSize()
        {
            if (stepSize == MaxStepSize)
                return;

            if (stepSize < 2.0f)
                stepSize *= 2.0f;
            else
                stepSize += 2.0f;

            if (stepSize > MaxStepSize)
                stepSize = MaxStepSize;

            SetStepSizeText();
        }

        /// <summary>
        /// Decreases the step size for the orbiting bodies; slows the speed.
        /// </summary>
        public void DecreaseStepSize()
        {
            if (stepSize == MinStepSize)
                return;

            if (stepSize > 2.0f)
                stepSize -= 2.0f;
            else
                stepSize /= 2.0f;

            if (stepSize < MinStepSize)
                stepSize = MinStepSize;

            SetStepSizeText();
        }

        bool LambertSolutionExists()
        {
            // Make local copies of departure and arrival orbits.
            OrbitData earthCopy = null;
            OrbitData asteroidCopy = null;
            foreach (Body body in Bodies)
            {
                if (body.tag == "Earth")
                {
                    earthCopy = new OrbitData(body.Orbit);
                }
                else if (body.tag == "Asteroid")
                {
                    asteroidCopy = new OrbitData(body.Orbit);
                }
            }

            // Advance orbits to epoch of impact for Lambert solver
            earthCopy.Epoch = ImpactEpoch;
            asteroidCopy.Epoch = ImpactEpoch;

            // Get the transfer orbit and C3 from the Lambert solution
            LambertSolution lambertSolution = Lambert.GetTransferOrbit(earthCopy, asteroidCopy, TimeOfDeflection, TransferTime, -1);
            if (lambertSolution == null)
            {
                Debug.Log("No Lambert solution found\nTry different mission values.");
                ShowInfeasibleLaunchWindow();
                return false;
            }

            if (lambertSolution.C3 <= 0.0)
            {
                Debug.Log("Lambert solution requires negative C3");
                ShowInfeasibleLaunchWindow();
                return false;
            }

            double deliverableMass = LaunchVehicleType.ComputeDeliverableMass(lambertSolution.C3, NumLaunches * EffectMultiplier);
            if (deliverableMass <= 0.0)
            {
                Debug.LogFormat("Launch C3 = {0:F4}\nLaunch Vehicle unable to obtain required launch energy", 
                                lambertSolution.C3);
                ShowInfeasibleLaunchWindow();
                return false;
            }
            Debug.LogFormat("Launch C3 = {0:F4}Impact Mass = {1:F4}; Impact Vel. = {2:E4}", 
                            lambertSolution.C3, MassKI, lambertSolution.arrDV.magnitude);
            SetMissionValuesText(deliverableMass, lambertSolution.C3, lambertSolution.arrDV.magnitude);
            return true;
        }

        void SetMissionValuesText(double mass, double c3, double velocity)
        {
            if (mass <= 0.0 || c3 < 0.0)
            {
                ShowInfeasibleLaunchWindow();
                Debug.LogWarning("This shouldn't happen");
            }
            else
            {
                char super2 = '\u00B2';
                DeliverableMassText.text = mass.ToString("E3") + " kg";
                LaunchEnergyText.text = c3.ToString("F3") + " (km/s)" + super2.ToString();
                ImpactVelocityText.text = velocity.ToString("F3") + " km/s";

                HideInfeasibleLaunchWindow();
            }
        }

        static Color defaultColor = new Color(50f / 255f, 50f / 255f, 50f / 255f); // Dark grey
        static Color warningColor = new Color(160f / 255f, 0f, 0f); // Deep red
        static Color normalColor = new Color(0, 115f / 255f, 0f); // Deep green

        void ShowInfeasibleLaunchWindow()
        {
            DeliverableMassText.text = "NONE";
            DeliverableMassText.color = warningColor;
            InfeasibleLaunchText.enabled = true;
            MissionValues.SetActive(false);
        }

        void HideInfeasibleLaunchWindow()
        {
            DeliverableMassText.color = new Color(0f, 115f / 255f, 0f, 1f); // Deep green
            InfeasibleLaunchText.enabled = false;
            MissionValues.SetActive(true);
        }

        /// <summary>
        /// Launches a spacecraft from Earth to intercept/deflect the asteroid.
        /// </summary>
        public void LaunchSpacecraft()
        {
            // Make local copies of departure and arrival orbits.
            OrbitData earth = null;
            OrbitData asteroid = null;
            foreach (Body body in Bodies)
            {
                if (body.tag == "Earth")
                {
                    earth = new OrbitData(body.Orbit);
                }
                else if (body.tag == "Asteroid")
                {
                    asteroid = new OrbitData(body.Orbit);
                }
            }
            if (earth == null || asteroid == null)
            {
                Debug.LogError("Error finding departure/arrival orbits from bodies list by tag");
                return;
            }

            // Advance orbits to epoch of impact for Lambert solver
            earth.Epoch = ImpactEpoch;
            asteroid.Epoch = ImpactEpoch;

            // Get the transfer orbit and C3 from the Lambert solution
            LambertSolution lambertSolution = Lambert.GetTransferOrbit(earth, asteroid, TimeOfDeflection, TransferTime, -1);
            if (lambertSolution == null || lambertSolution.C3 <= 0.0)
            {
                DisplayTextWithFadeEffect(MessageText, "No Lambert solution found");
                return;
            }

            // MassKI calculated here will be used in ProcessCollisionEnter
            MassKI = LaunchVehicleType.ComputeDeliverableMass(lambertSolution.C3, NumLaunches * EffectMultiplier);
            if (MassKI <= 0.0)
            {
                DisplayTextWithFadeEffect(MessageText, string.Format("Launch vehicle unable to achieve C3 {0:E3} (km/s)^2", lambertSolution.C3));
                return;
            }

            // TODO: Initialize spacecraft from prefab here
            //GameObject prefab = Instantiate(Resources.Load("Spacecraft.prefab", typeof(GameObject)) as GameObject);
            Body spacecraft = spacecraftObject.GetComponent<Body>();
            if (spacecraft == null)
                Debug.LogError("Instantiated spacecraft prefab does not have Body component");
            spacecraft.InitOrbit(lambertSolution.TransferOrbit);
            spacecraft.SetImpactEpoch(ImpactEpoch - TimeOfDeflection);
            spacecraft.gameObject.SetActive(true);

            Bodies = FindObjectsOfType<Body>(); // Update bodies list

            string message = string.Format("Launch successful!\nC3 = {0:F3} (km/s)^2\nMass = {1:F1} kg", lambertSolution.C3, MassKI);
            DisplayTextWithFadeEffect(MessageText, message);
        }

#endregion

#region Collision-Handling Methods

        public void ProcessCollisionEnter(Body larger, Body smaller)
        {
            if (IsPaused || InReverse)
            {
                Debug.Log("Detected collision will not be processesed while paused or in reverse");
                return;
            }

            // Earth-Asteroid Impact
            if (larger.tag == "Earth" && smaller.tag == "Asteroid")
            {
		        manager.GetComponent<MenuManager>().LoadEndScreen();
                StartCoroutine(FadeTextToFullAlpha(MessageText, 0f));
                DistanceText.enabled = true;
                ProcessCollisionStay(larger, smaller);
            }

            // Asteroid-KI Deflection
            if (larger.tag == "Asteroid" && smaller.tag == "Spacecraft")
            {
                smaller.gameObject.SetActive(false);

                larger.SetTrailColor(Color.green, new Color(0f, 155f / 255f, 0f));

                // Save the current epoch before making any changes (just in case)
                double originalEpoch = CurrentEpoch;

                // Do delta V calculation at true time of deflection
                larger.Orbit.Epoch = DeflectionEpoch;
                smaller.Orbit.Epoch = DeflectionEpoch;

                // Calculate the delta V and apply it to the deflected asteroid
                Vector3d deltaV = OrbitUtils.CalculateDeflectionDeltaVEcliptic(larger.Orbit, smaller.Orbit, MassNeo, MassKI, Beta);
                //triggerClone.GetComponent<Body>().Orbit.Velocity += deltaV;
                larger.Orbit.Velocity += deltaV;

                // Return orbits to original epoch (position change should not be noticeable)
                larger.Orbit.Epoch = originalEpoch;
                smaller.Orbit.Epoch = originalEpoch; // Not really necessary because not active

                Debug.LogFormat("Delta V: {0:E5} {1:E5} {2:E5}", deltaV.x, deltaV.y, deltaV.z);

                // Update list of active bodies
                Bodies = FindObjectsOfType<Body>();
                return;
            }
            Debug.Log("Detected unrecognized collision: " + larger.name + "-" + smaller.name);
        }

        public void ProcessCollisionStay(Body larger, Body smaller)
        {
            if (IsPaused || InReverse)
            {
                return;
            }

            if (larger.tag == "Earth" && smaller.tag == "Asteroid")
            {
                double distance = ClosestApproachDistance(larger.Orbit, smaller.Orbit, ScaledTimeStep);
                DistanceText.text = (distance * OrbitUtils.KM2ER).ToString("F4") + " Er";
                Debug.Log("Approach Distance: " + distance + " km");
                if (distance < missDistance)
                {
                    MessageText.text = "Game Over :(";
                    Debug.LogWarning("Impact @ " + distance + " km on " + EpochText.text);
                    IsPaused = true;
                    smaller.gameObject.SetActive(false);
                }
                else
                {
                    MessageText.text = "Close-Approach\n< 1.5 Earth-radii";
                }

                //distance = (larger.transform.position - smaller.transform.position).magnitude;
                //Debug.Log("Transform distance: " + distance.ToString("F4") + " AU");
            }
        }

        public void ProcessCollisionExit()
        {
            //FadeTextToZeroAlpha(DistanceText, 8f);
            DisplayTextWithFadeEffect(MessageText, "Deflection successful! You win!");
        }

        /// <summary>
        /// Determines the closest approach distance between two orbiting bodies
        /// over the specified time interval.
        /// </summary>
        /// <returns>The approach distance.</returns>
        /// <param name="collider1">Collider1.</param>
        /// <param name="collider2">Collider2.</param>
        /// <param name="numSteps">Number steps.</param>
        /// <param name="interval">The time interval to check.</param>
        double ClosestApproachDistance(OrbitData collider1, OrbitData collider2, double interval, int numSteps = 30)
        {
            double minDistance = double.PositiveInfinity;
            OrbitData orbit1 = new OrbitData(collider1);
            OrbitData orbit2 = new OrbitData(collider2);

            double intervalStepSize = interval / numSteps;
            double time = 0.0;
            while (time <= interval)
            {
                orbit1.Step(intervalStepSize);
                orbit2.Step(intervalStepSize);
                double distance = (orbit1.Position - orbit2.Position).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
                time += intervalStepSize;
            }
            return minDistance;
        }

#endregion

#region UI Text Methods

        public void DisplayTextWithFadeEffect(Text text, string message, float displayTime = 8f, float fadeTime = 1f)
        {
            StartCoroutine(TextFadeEffect(text, message, displayTime, fadeTime));
        }

        IEnumerator TextFadeEffect(Text text, string message, float displayTime, float fadeTime = 1.0f)
        {
            text.text = message;
            text.enabled = true;
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            while (text.color.a < 1.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / fadeTime));
                yield return null;
            }

            yield return new WaitForSecondsRealtime(displayTime);

            while (text.color.a > 0.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / fadeTime));
                yield return null;
            }
        }

        IEnumerator FadeTextToFullAlpha(Text text, float time)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            while (text.color.a < 1.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / time));
                yield return null;
            }
        }

        IEnumerator FadeTextToZeroAlpha(Text text, float time)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
            while (text.color.a > 0.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / time));
                yield return null;
            }
        }

        void SetEpochText()
        {
            EpochText.text = OrbitUtils.JDN2DateTime(CurrentEpoch).ToShortDateString();
        }

        void SetStepSizeText()
        {
            StepSizeText.text = stepSize.ToString();
        }

        void SetDistanceText(Body body1, Body body2)
        {
            DistanceText.text = (body1.Orbit.Position - body2.Orbit.Position).magnitude.ToString("E5");
        }

#endregion
    }
}
