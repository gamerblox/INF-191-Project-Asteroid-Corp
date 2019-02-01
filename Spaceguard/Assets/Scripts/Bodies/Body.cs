using Game.Orbits;
using UnityEngine;

namespace Game.Bodies
{
    [AddComponentMenu("Orbits/Orbiting Body")]
    [RequireComponent(typeof(TrailRenderer))]
    [RequireComponent(typeof(SphereCollider))]
    public class Body : MonoBehaviour
    {
        static BodyManager manager;

        TrailRenderer trail;

        public OrbitData Orbit;

        double initEpoch = double.NegativeInfinity;
        double impactEpoch = double.PositiveInfinity;

        #region Unity Methods

        void Awake()
        {
            if (manager == null)
            {
                manager = FindObjectOfType<BodyManager>();
                if (manager == null)
                {
                    Debug.LogError("No BodyManager active in scene");
                }
            }

            trail = GetComponent<TrailRenderer>();
            trail.enabled = true;
            trail.Clear();
        }

        void OnEnable()
        {
            trail.Clear();
            trail.enabled = true;
        }

        void OnDisable()
        {
            trail.Clear();
            trail.enabled = false;
        }

        void Update() 
        {
            if (Orbit.Epoch < initEpoch || Orbit.Epoch > impactEpoch)
            {
                enabled = false;
            }
            else
            {
                enabled = true;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // Make sure the collider also has Body attached
            if (other.GetComponent<Body>() == null)
            {
                Debug.Log("Collider does not have Body component");
                return;
            }

            // Delegate the larger body to handle the collision event.
            // Ignore Earth-Spacecraft "collisions" - occurs at launch.
            if ((tag == "Earth" && other.tag == "Asteroid") ||
                (tag == "Asteroid" && other.tag == "Spacecraft"))
            {
                Debug.Log("Trigger " + tag + " , Collider = " + other.tag);
                manager.ProcessCollisionEnter(this, other.GetComponent<Body>());
            }
        }

        void OnTriggerStay(Collider other)
        {
            // Make sure the collider also has Body attached
            if (other.GetComponent<Body>() == null)
            {
                return;
            }

            // Scale of sphere colliders is not accurate, so additional analysis
            // of the close-approach is required to determine if the two bodies
            // experience an impact or a near-miss.
            if (tag == "Earth" && other.tag == "Asteroid")
            {
                manager.ProcessCollisionStay(this, other.GetComponent<Body>());
            }
        }

        void OnTriggerExit(Collider other)
        {
            // Make sure the collider also has Body attached
            if (other.GetComponent<Body>() == null)
            {
                return;
            }

            // Signal the end of the close-approach or impact.
            // TODO: Check for "win" condition should occur here.
            if ((tag == "Earth" && other.tag == "Asteroid"))
            {
                manager.ProcessCollisionExit();
            }
        }

        #endregion

        /// <summary>
        /// Updates the transform position from the internal state of the orbit.
        /// First converts the orbital position from km to au.
        /// </summary>
        /// <remarks>
        /// Conversion to astronomical units is arbitrary; using AU is simply
        /// a convenient way to scale the position (maintained in high-precision
        /// SI units km and seconds) to Unity-friendly units for rendering. 
        /// </remarks>
        public void UpdateTransformPosition()
        {
            transform.position = (Vector3)(Orbit.Position * OrbitUtils.KM2AU);
        }

        public void UpdateTransformPositionClear()
        {
            UpdateTransformPosition();
            trail.Clear();
        }

        #region Public Methods

        public void InitBody(OrbitData orbit, double epoch)
        {
            Orbit = new OrbitData(orbit);
            Orbit.Epoch = epoch;

            initEpoch = epoch;

            UpdateTransformPositionClear();
            trail.enabled = true;
        }

        /// <summary>
        /// Initialize orbit by this game object's name for specified epoch.
        /// </summary>
        public void InitOrbit(double epoch)
        {
            Orbit = new OrbitData(name);
            Orbit.Epoch = epoch;
            UpdateTransformPosition();
            trail.Clear();
            trail.enabled = true;
            initEpoch = epoch;
        }

        /// <summary>
        /// Initialize orbit to specified orbit.
        /// </summary>
        /// <param name="orbit">Initial orbit.</param>
        public void InitOrbit(OrbitData orbit, bool resetTrail = true)
        {
            Orbit = new OrbitData(orbit); // Force pass-by-value.
            UpdateTransformPosition();
            if (resetTrail)
            {
                trail.Clear();
            }
            trail.enabled = true;
            initEpoch = Orbit.Epoch;
        }

        /// <summary>
        /// Sets the color of the trail.
        /// </summary>
        /// <param name="startColor">Start color.</param>
        /// <param name="endColor">End color.</param>
        public void SetTrailColor(Color startColor, Color endColor)
        {
            if (trail == null)
            {
                trail = GetComponent<TrailRenderer>();
            }
            trail.Clear();
            trail.startColor = startColor;
            trail.endColor = endColor;
        }

        /// <summary>
        /// Sets the width of the trail.
        /// </summary>
        /// <param name="width">Width.</param>
        public void SetTrailWidth(float width)
        {
            SetTrailWidth(width, width);
        }

        /// <summary>
        /// Sets the width of the trail.
        /// </summary>
        /// <param name="startWidth">Start width.</param>
        /// <param name="endWidth">End width.</param>
        public void SetTrailWidth(float startWidth, float endWidth)
        {
            if (trail == null)
            {
                trail = GetComponent<TrailRenderer>();
            }
            trail.startWidth = startWidth;
            trail.endWidth = endWidth;
        }

        public void SetImpactEpoch(double epoch)
        {
            impactEpoch = epoch;
        }

        #endregion

    }
}