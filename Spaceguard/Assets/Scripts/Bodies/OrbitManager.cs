//using System.Collections;
//using System.Collections.Generic;
//using Game.IO;
//using Game.Orbits;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Game.Bodies
//{
//    [AddComponentMenu("Game Managers/Orbit Manager")]
//    public class OrbitManager : MonoBehaviour
//    {
//        [Header("Mission Parameters")]
//        public LaunchVehicleData LaunchVehicleType;
//        public int NumLaunches = 1;
//        public int EffectMultiplier = 1;
//        public double TimeOfDeflection = 1280.0;
//        public double TransferTime = 500.0;

//        [Header("NEO Characteristics")]
//        public double Diameter = 0.14;
//        public double Density = 1.5;
//        public double Beta = 1.0;
//        public double MassNeo { get { return OrbitUtils.MassOfSphere(Diameter, Density); }}

//        double LaunchEpoch { get { return ImpactEpoch - TimeOfDeflection - TransferTime; } }
//        double DeflectionEpoch { get { return ImpactEpoch - TimeOfDeflection; } }
//        double MassKI;

//        double ImpactEpoch = 2461607.836805556;

//        [Header("Orbiting Bodies (0 = Earth)")]
//        [SerializeField]
//        List<Body> bodies;

//        double warningTime = 10 * 365.25; // 10 years
//        //double massNeo;

//        //public void CalculateMassNeo(double diameter, double density)
//        //{
//        //    massNeo = OrbitUtils.MassOfSphere(diameter, density);
//        //}

//        double impactEpoch = 2461607.836805556; // for PDC17a
//        double initEpoch;
//        double currentEpoch { get { return bodies[0].Orbit.Epoch; } }
//        double maxEpoch;
//        double simulationSpan;

//        bool _inited = false;
//        public bool IsInited { get { return _inited; } }

//        void Init()
//        {
//            initEpoch = impactEpoch - warningTime;
//            maxEpoch = impactEpoch + (0.2 * warningTime); // show up to 2 years past impact
//            foreach (Body body in bodies)
//            {
//                body.InitOrbit(initEpoch);
//            }
//            simulationSpan = 1.2 * warningTime;
//        }

//        public float GetProgressPercent()
//        {
//            if (currentEpoch > initEpoch)
//            {
//                return (float)((currentEpoch - initEpoch) / simulationSpan);
//            }
//            return 0.0f;
//        }

//        public void SkipToPercent(float percent)
//        {
//            double epoch = (simulationSpan * percent) + initEpoch;
//            foreach (Body body in bodies)
//            {
//                body.Orbit.Epoch = epoch;
//                body.UpdateTransformPositionSkip();
//            }
//        }

//        #region Fields

//        bool _paused;
//        public bool IsPaused
//        {
//            get { return _paused; }
//            set
//            {
//                if (_paused != value)
//                {
//                    _paused = value;
//                    foreach (var obj in hideOnPause)
//                    {
//                        obj.SetActive(!_paused);
//                    }
//                    foreach (var obj in showOnPause)
//                    {
//                        obj.SetActive(_paused);
//                    }
//                }
//            }
//        }

//        bool InReverse;
//        double stepSize = 1.0;

//        double CurrentEpoch { get { return Bodies[0].Orbit.Epoch; } }

//        #endregion

//        Body[] Bodies;

//        GameObject[] showOnPause;
//        GameObject[] hideOnPause;
//        GameObject spacecraftObject;

//        /// <summary>
//        /// Sets all GameObjects in the scene active.
//        /// </summary>
//        /// <remarks>
//        /// Call this method at the beginning of Start() to ensure that
//        /// <see cref="GameObject.FindGameObjectsWithTag(string)"/> will
//        /// retrieve all tagged objects, even if those objects were not
//        /// active in the scene when the scene is played.
//        /// </remarks>
//        void SetActiveAllManaged()
//        {
//            foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
//            {
//                if (obj.scene == gameObject.scene)
//                {
//                    if (obj.tag == "Menu")
//                    {
//                        obj.SetActive(false); // Hide all menus
//                    }
//                    else
//                    {
//                        obj.SetActive(true); // Activate all other scene objects
//                    }
//                }
//            }
//        }

//        #region Unity Methods

//        void Start()
//        {
//            //SetActiveAllManaged(); // GameObject.FindGameObject only returns active

//            // Check that text references are assigned.
//            if (EpochText == null || StepSizeText == null || MessageText == null || DistanceText == null)
//            {
//                Debug.LogError("Missing one or more text references");
//            }
//            MessageText.enabled = false; // Disabled on start

//            // Get list of pause-sensitive objects and initialize visibility.
//            showOnPause = GameObject.FindGameObjectsWithTag("ShowOnPause");
//            hideOnPause = GameObject.FindGameObjectsWithTag("HideOnPause");
//            foreach (var obj in hideOnPause)
//            {
//                obj.SetActive(false);
//            }

//            // TODO: Eventually spacecraft will be initialized by prefab
//            spacecraftObject = GameObject.FindGameObjectWithTag("Spacecraft");
//            spacecraftObject.SetActive(false);

//            // Get list of active bodies and initialize each.
//            Bodies = FindObjectsOfType<Body>();
//            if (Bodies.Length == 0)
//            {
//                Debug.LogError("No bodies active in scene");
//            }
//            //double initEpoch = LaunchEpoch - TransferTime; // Arbitrary.
//            double initEpoch = ImpactEpoch - 5 * 365.25;
//            foreach (Body body in Bodies)
//            {
//                body.InitOrbit(initEpoch);
//            }

//            // Set UI text content.
//            SetEpochText();
//            SetStepSizeText();
//            DistanceText.enabled = false;

//            // Initialize game state and start coroutines.
//            IsPaused = true;
//            StartCoroutine(OrbitLoop());

//            if (UsingUserInput)
//            {
//                StartCoroutine(LaunchFromUserInput());
//            }
//            else if (AutoLaunchOn)
//            {
//                GameObject.FindWithTag("InputPanel").SetActive(false);
//                StartCoroutine(AutoLaunch());
//            }
//        }

//        #endregion

//        #region Coroutines

//        IEnumerator OrbitLoop()
//        {
//            while (true)
//            {
//                if (!IsPaused)
//                {
//                    StepAllBodyOrbits();
//                }
//                yield return null;
//            }
//        }

//        IEnumerator AutoLaunch()
//        {
//            while (true)
//            {
//                if (!IsPaused && CurrentEpoch >= LaunchEpoch)
//                {
//                    LaunchSpacecraft();
//                    break;
//                }
//                yield return null;
//            }
//        }

//        IEnumerator LaunchFromUserInput()
//        {
//            InputManager inputManager = FindObjectOfType<InputManager>();
//            if (inputManager == null)
//            {
//                Debug.LogError("No InputManager active in scene");
//            }
//            if (StartButton == null)
//            {
//                Debug.LogError("Start button does not have button component");
//            }

//            while (true)
//            {
//                if (StartButton == null)
//                {
//                    break;
//                }
//                IsPaused = true;
//                //StartButton.SetActive(inputManager.IsValidMission);
//                if (inputManager.IsValidMission())
//                {
//                    Debug.Log("Valid mission!");
//                    SetMissionParamsFromUserInput();
//                    Debug.Log("D = " + TimeOfDeflection + ", TOF = " + TransferTime);
//                    StartButton.interactable = LambertSolutionExists();
//                }
//                else
//                {
//                    StartButton.interactable = false;
//                    LambertText.text = "Set mission parameter(s)";
//                }
//                yield return null;
//            }
//        }

//        public void OnStartButtonPressed()
//        {
//            //StartButton.gameObject.SetActive(false);
//            StartButton = null;

//            //SetMissionParamsFromUserInput();

//            foreach (GameObject panel in GameObject.FindGameObjectsWithTag("InputPanel"))
//            {
//                panel.SetActive(false);
//            }

//            //double initEpoch = LaunchEpoch - 365.25; // Arbitrary.
//            //foreach (Body body in Bodies)
//            //{
//            //    body.InitOrbit(initEpoch);
//            //}

//            StartCoroutine(AutoLaunch());
//            IsPaused = false;
//        }

//        void SetMissionParamsFromUserInput()
//        {
//            InputManager manager = FindObjectOfType<InputManager>();
//            if (manager == null || !manager.IsValidMission())
//                return;
//            TimeOfDeflection = manager.TimeOfDeflection;
//            TransferTime = manager.TransferTime;
//            //Diameter = manager.Diameter;
//            //Density = manager.Density;
//            // MassNEO is a property calculated from diameter and density
//            LaunchVehicleType = manager.LaunchVehicleType;
//        }

//        void StepAllBodyOrbits()
//        {
//            foreach (Body body in Bodies)
//            {
//                body.Orbit.Step(ScaledTimeStep);
//                body.UpdateTransformPosition();
//            }
//            SetEpochText();
//        }

//        #endregion

//        #region UI Clickable Behaviors

//        /// <summary>
//        /// If <see cref="IsPaused"/>, steps the orbits by <see cref="ScaledTimeStep"/>,
//        /// else pauses the simulation.
//        /// </summary>
//        void StepOrPause()
//        {
//            if (IsPaused)
//            {
//                StepAllBodyOrbits();
//            }
//            else
//            {
//                IsPaused = true;
//            }
//        }

//        /// <summary>
//        /// Step the orbits forward a single <see cref="ScaledTimeStep"/>.
//        /// Routine for the "Step Forward" button.
//        /// </summary>
//        public void StepForward()
//        {
//            InReverse = false;
//            StepOrPause();
//        }

//        /// <summary>
//        /// Steps the orbits backward a single <see cref="ScaledTimeStep"/>.
//        /// Routine for the "Step Backward" button.
//        /// </summary>
//        public void StepBackward()
//        {
//            InReverse = true;
//            StepOrPause();
//        }

//        /// <summary>
//        /// Plays the orbit simuilation. Routine for the "Play" button.
//        /// </summary>
//        public void PlayForward()
//        {
//            IsPaused = false;
//            InReverse = false;
//        }

//        /// <summary>
//        /// Rewind all bodies in the scene; play backwards. Routine for the
//        /// "Play Backward" button.
//        /// </summary>
//        public void PlayBackward()
//        {
//            IsPaused = false;
//            InReverse = true;
//        }

//        /// <summary>
//        /// Pauses all bodies in the scene. Routine for the "Pause" button.
//        /// </summary>
//        public void Pause()
//        {
//            IsPaused = true;
//            InReverse = false;
//        }

//        /// <summary>
//        /// Increases the step size for the orbiting bodies; increases the speed.
//        /// </summary>
//        public void IncreaseStepSize()
//        {
//            if (stepSize == MaxStepSize)
//                return;

//            if (stepSize < 2.0f)
//                stepSize *= 2.0f;
//            else
//                stepSize += 2.0f;

//            if (stepSize > MaxStepSize)
//                stepSize = MaxStepSize;

//            SetStepSizeText();
//        }

//        /// <summary>
//        /// Decreases the step size for the orbiting bodies; slows the speed.
//        /// </summary>
//        public void DecreaseStepSize()
//        {
//            if (stepSize == MinStepSize)
//                return;

//            if (stepSize > 2.0f)
//                stepSize -= 2.0f;
//            else
//                stepSize /= 2.0f;

//            if (stepSize < MinStepSize)
//                stepSize = MinStepSize;

//            SetStepSizeText();
//        }

//        bool LambertSolutionExists()
//        {
//            // Make local copies of departure and arrival orbits.
//            OrbitData earth = null;
//            OrbitData asteroid = null;
//            foreach (Body body in Bodies)
//            {
//                if (body.tag == "Earth")
//                {
//                    earth = new OrbitData(body.Orbit);
//                }
//                else if (body.tag == "Asteroid")
//                {
//                    asteroid = new OrbitData(body.Orbit);
//                }
//            }
//            if (earth == null || asteroid == null)
//            {
//                Debug.LogError("Error finding departure/arrival orbits from bodies list by tag");
//                LambertText.text = "Error finding departure/arrival orbits";
//                return false;
//            }

//            // Advance orbits to epoch of impact for Lambert solver
//            earth.Epoch = ImpactEpoch;
//            asteroid.Epoch = ImpactEpoch;

//            // Get the transfer orbit and C3 from the Lambert solution
//            LambertSolution lambertSolution = Lambert.GetTransferOrbit(earth, asteroid, TimeOfDeflection, TransferTime, -1);
//            if (lambertSolution == null)
//            {
//                LambertText.text = "No Lambert solution found\nTry different mission values.";
//                return false;
//            }

//            if (lambertSolution.C3 <= 0.0)
//            {
//                LambertText.text = "Lambert solution requires negative C3";
//                return false;
//            }

//            // MassKI calculated here will be used in ProcessCollisionEnter
//            MassKI = LaunchVehicleType.ComputeDeliverableMass(lambertSolution.C3, NumLaunches * EffectMultiplier);
//            if (MassKI <= 0.0)
//            {
//                LambertText.text = string.Format("Launch C3 = {0:F4}\nLaunch Vehicle unable to obtain required launch energy",
//                                                 lambertSolution.C3);
//                return false;
//            }
//            LambertText.text = string.Format("Launch C3 = {0:F4}\nImpact Mass = {1:F4}\nImpact Vel. = {2:E4}",
//                                             lambertSolution.C3, MassKI, lambertSolution.arrDV.magnitude);
//            return true;
//        }

//        /// <summary>
//        /// Launches a spacecraft from Earth to intercept/deflect the asteroid.
//        /// </summary>
//        public void LaunchSpacecraft()
//        {
//            // Make local copies of departure and arrival orbits.
//            OrbitData earth = null;
//            OrbitData asteroid = null;
//            foreach (Body body in Bodies)
//            {
//                if (body.tag == "Earth")
//                {
//                    earth = new OrbitData(body.Orbit);
//                }
//                else if (body.tag == "Asteroid")
//                {
//                    asteroid = new OrbitData(body.Orbit);
//                }
//            }
//            if (earth == null || asteroid == null)
//            {
//                Debug.LogError("Error finding departure/arrival orbits from bodies list by tag");
//                return;
//            }

//            // Advance orbits to epoch of impact for Lambert solver
//            earth.Epoch = ImpactEpoch;
//            asteroid.Epoch = ImpactEpoch;

//            // Get the transfer orbit and C3 from the Lambert solution
//            LambertSolution lambertSolution = Lambert.GetTransferOrbit(earth, asteroid, TimeOfDeflection, TransferTime, -1);
//            if (lambertSolution == null || lambertSolution.C3 <= 0.0)
//            {
//                DisplayTextWithFadeEffect(MessageText, "No Lambert solution found");
//                return;
//            }

//            // MassKI calculated here will be used in ProcessCollisionEnter
//            MassKI = LaunchVehicleType.ComputeDeliverableMass(lambertSolution.C3, NumLaunches * EffectMultiplier);
//            if (MassKI <= 0.0)
//            {
//                DisplayTextWithFadeEffect(MessageText, string.Format("Launch vehicle unable to achieve C3 {0:E3} (km/s)^2", lambertSolution.C3));
//                return;
//            }

//            // TODO: Initialize spacecraft from prefab here
//            //GameObject prefab = Instantiate(Resources.Load("Spacecraft.prefab", typeof(GameObject)) as GameObject);
//            Body spacecraft = spacecraftObject.GetComponent<Body>();
//            if (spacecraft == null)
//                Debug.LogError("Instantiated spacecraft prefab does not have Body component");
//            spacecraft.InitOrbit(lambertSolution.TransferOrbit);
//            spacecraft.gameObject.SetActive(true);

//            Bodies = FindObjectsOfType<Body>(); // Update bodies list

//            string message = string.Format("Launch successful!\nC3 = {0:F3} (km/s)^2\nMass = {1:F1} kg", lambertSolution.C3, MassKI);
//            DisplayTextWithFadeEffect(MessageText, message);
//        }

//        #endregion

//        #region Collision-Handling Methods

//        public void ProcessCollisionEnter(Body larger, Body smaller)
//        {
//            if (IsPaused || InReverse)
//            {
//                Debug.Log("Detected collision will not be processesed while paused or in reverse");
//                return;
//            }

//            // Earth-Asteroid Impact
//            if (larger.tag == "Earth" && smaller.tag == "Asteroid")
//            {
//                StartCoroutine(FadeTextToFullAlpha(MessageText, 0f));
//                DistanceText.enabled = true;
//                ProcessCollisionStay(larger, smaller);
//            }

//            // Asteroid-KI Deflection
//            if (larger.tag == "Asteroid" && smaller.tag == "Spacecraft")
//            {
//                smaller.gameObject.SetActive(false);

//                larger.SetTrailColor(Color.red, Color.magenta);

//                // Save the current epoch before making any changes (just in case)
//                double originalEpoch = CurrentEpoch;

//                // Do delta V calculation at true time of deflection
//                larger.Orbit.Epoch = DeflectionEpoch;
//                smaller.Orbit.Epoch = DeflectionEpoch;

//                // Calculate the delta V and apply it to the deflected asteroid
//                Vector3d deltaV = OrbitUtils.CalculateDeflectionDeltaVEcliptic(larger.Orbit, smaller.Orbit, MassNeo, MassKI, Beta);
//                //triggerClone.GetComponent<Body>().Orbit.Velocity += deltaV;
//                larger.Orbit.Velocity += deltaV;

//                // Return orbits to original epoch (position change should not be noticeable)
//                larger.Orbit.Epoch = originalEpoch;
//                smaller.Orbit.Epoch = originalEpoch; // Not really necessary because not active

//                Debug.LogFormat("Delta V: {0:E5} {1:E5} {2:E5}", deltaV.x, deltaV.y, deltaV.z);

//                // Update list of active bodies
//                Bodies = FindObjectsOfType<Body>();
//                return;
//            }
//            Debug.Log("Detected unrecognized collision: " + larger.name + "-" + smaller.name);
//        }

//        public void ProcessCollisionStay(Body larger, Body smaller)
//        {
//            if (IsPaused || InReverse)
//            {
//                return;
//            }

//            if (larger.tag == "Earth" && smaller.tag == "Asteroid")
//            {
//                double distance = ClosestApproachDistance(larger.Orbit, smaller.Orbit, ScaledTimeStep) * OrbitUtils.KM2AU;
//                DistanceText.text = distance.ToString("F4") + " AU";
//                double missDistance = 0.01;
//                Debug.Log("Distance: " + distance);
//                if (distance < missDistance)
//                {
//                    MessageText.text = "Game Over :(";
//                    Debug.LogWarning("Impact @ " + distance + " AU on " + EpochText.text);
//                    IsPaused = true;
//                    smaller.gameObject.SetActive(false);
//                }
//                else
//                {
//                    MessageText.text = "Close-Approach\n< " + missDistance.ToString("F4") + " AU";
//                }

//                distance = (larger.transform.position - smaller.transform.position).magnitude;
//                Debug.Log("Transform distance: " + distance.ToString("F4") + " AU");
//            }
//        }

//        public void ProcessCollisionExit()
//        {
//            DisplayTextWithFadeEffect(MessageText, "Deflection successful! You win!");
//        }

//        /// <summary>
//        /// Determines the closest approach distance between two orbiting bodies
//        /// over the specified time interval.
//        /// </summary>
//        /// <returns>The approach distance.</returns>
//        /// <param name="collider1">Collider1.</param>
//        /// <param name="collider2">Collider2.</param>
//        /// <param name="numSteps">Number steps.</param>
//        /// <param name="interval">The time interval to check.</param>
//        double ClosestApproachDistance(OrbitData collider1, OrbitData collider2, double interval, int numSteps = 30)
//        {
//            double minDistance = double.PositiveInfinity;
//            OrbitData orbit1 = new OrbitData(collider1);
//            OrbitData orbit2 = new OrbitData(collider2);

//            double intervalStepSize = interval / numSteps;
//            double time = 0.0;
//            while (time <= interval)
//            {
//                orbit1.Step(intervalStepSize);
//                orbit2.Step(intervalStepSize);
//                double distance = (orbit1.Position - orbit2.Position).magnitude;
//                if (distance < minDistance)
//                {
//                    minDistance = distance;
//                }
//                time += intervalStepSize;
//            }
//            return minDistance;
//        }

//        #endregion

//        #region UI Text Methods

//        public void DisplayTextWithFadeEffect(Text text, string message, float displayTime = 8f, float fadeTime = 1f)
//        {
//            StartCoroutine(TextFadeEffect(text, message, displayTime, fadeTime));
//        }

//        IEnumerator TextFadeEffect(Text text, string message, float displayTime, float fadeTime = 1.0f)
//        {
//            text.text = message;
//            text.enabled = true;
//            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
//            while (text.color.a < 1.0f)
//            {
//                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / fadeTime));
//                yield return null;
//            }

//            yield return new WaitForSecondsRealtime(displayTime);

//            while (text.color.a > 0.0f)
//            {
//                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / fadeTime));
//                yield return null;
//            }
//        }

//        IEnumerator FadeTextToFullAlpha(Text text, float time)
//        {
//            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
//            while (text.color.a < 1.0f)
//            {
//                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / time));
//                yield return null;
//            }
//        }

//        IEnumerator FadeTextToZeroAlpha(Text text, float time)
//        {
//            text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
//            while (text.color.a > 0.0f)
//            {
//                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / time));
//                yield return null;
//            }
//        }

//        void SetEpochText()
//        {
//            EpochText.text = OrbitUtils.JDN2DateTime(CurrentEpoch).ToShortDateString();
//        }

//        void SetStepSizeText()
//        {
//            StepSizeText.text = stepSize.ToString();
//        }

//        void SetDistanceText(Body body1, Body body2)
//        {
//            DistanceText.text = (body1.Orbit.Position - body2.Orbit.Position).magnitude.ToString("E5");
//        }

//        #endregion
//    }
//}
