using UnityEngine;

namespace Game.Cameras
{
    /// <summary>
    /// Space camera controller class.
    /// </summary>
    [AddComponentMenu("Custom Camera/Space RTS Camera Style")]
    public class CameraController : MonoBehaviour
    {
        public Transform LockedTransform { get; set; }
       
        public float xSpeed = 15f;
        public float ySpeed = 15f;
        public float yMin = -10f;
        public float yMax = 10f;
        public float zoomRate = 5f;
        public float defaultZoom = 20f; // Larger values show larger area

        public bool panMode = false;
        public float panSpeed = 0.3f;
        public int panThresh = 5;
        public float rotationDampening = 5.0f;

        Transform targetRotation;
        float xDeg = 0.0f;
        float yDeg = 0.0f;
        Vector3 desiredPosition;
        Vector3 cameraPlanePoint;
        Vector3 vectorPoint;

        float lastClickTime = 0.0f;
        float catchTime = 0.25f;
        bool isLocked = false;

        Ray ray;
        Vector3 off = Vector3.zero;
        Vector3 offset;
        Mode mode = Mode.IDLE;

        enum Mode { IDLE, ROTATE, ZOOM, PAN }

        void Awake()
        {
            Init();
        }

        void Init()
        {
            targetRotation = new GameObject("Camera TargetRotation").transform;

            xDeg = Vector3.Angle(Vector3.right, transform.right);
            yDeg = Vector3.Angle(Vector3.up, transform.up);

            LinePlaneIntersect(transform.forward.normalized, transform.position, Vector3.up, Vector3.zero, ref cameraPlanePoint);

            targetRotation.position = cameraPlanePoint;
            targetRotation.rotation = transform.rotation;

            LockedTransform = null;
        }

        // Use this for initialization
        void Start()
        {
            //Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            GameObject lockObject = GameObject.FindGameObjectWithTag("Sun");
            LockObject(lockObject.transform);
        }

        void LateUpdate()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            RaycastHit hit;

            int layerMask = 1 << 9;
            layerMask = ~layerMask;

            if (isLocked)
            {
                offset = LockedTransform.position - off;
                off = LockedTransform.position;

                if (Input.GetMouseButton(1) == false)
                {
                    mode = Mode.IDLE;

                    float magnitude = (targetRotation.position - transform.position).magnitude;
                    transform.position = targetRotation.position - (transform.rotation * Vector3.forward * magnitude) + offset;
                    targetRotation.position = targetRotation.position + offset;
                }
            }

            if (Input.GetMouseButton(1))
            {
                mode = Mode.ROTATE;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                yDeg = transform.rotation.eulerAngles.x;
                if (yDeg > 180)
                {
                    yDeg -= 360;
                }

                xDeg = transform.rotation.eulerAngles.y;

                mode = Mode.IDLE;
            }
            else if (MouseXBorder() != 0 || MouseYBorder() != 0)
            {
                mode = Mode.PAN;
            }
            else if (wheel != 0)
            {
                mode = Mode.ZOOM;
            }
            else if (DoubleClick(Time.time) && Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
            {
                //if (LockedTransform != hit.collider.gameObject.transform)
                //{
                //    LockObject(hit.collider.gameObject.transform);
                //}
            }

            switch (mode)
            {
                case Mode.IDLE:
                    break;

                case Mode.ROTATE:
                    xDeg += Input.GetAxis("Mouse X") * xSpeed;
                    yDeg -= Input.GetAxis("Mouse Y") * ySpeed;
                    yDeg = ClampAngle(yDeg, yMin, yMax, 5);

                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(yDeg, xDeg, 0), Time.deltaTime * rotationDampening / Time.timeScale);
                    targetRotation.rotation = transform.rotation;

                    float magnitude = (targetRotation.position - transform.position).magnitude;
                    transform.position = targetRotation.position - (transform.rotation * Vector3.forward * magnitude) + offset;
                    targetRotation.position = targetRotation.position + offset;
                    break;

                case Mode.ZOOM:
                    if (LockedTransform != null)
                    {
                        UnlockObject();
                    }

                    //float s0 = LinePlaneIntersect(transform.forward, transform.position, Vector3.up, Vector3.zero, ref cameraPlanePoint);
                    //targetRotation.position = transform.forward * s0 + transform.position;
                    //float lineToPlaneLength = LinePlaneIntersect(ray.direction, transform.position, Vector3.up, Vector3.zero, ref vectorPoint);

                    if (wheel > 0)
                    {
                        //if (lineToPlaneLength > 1.1f)
                        //{
                        //    desiredPosition = (vectorPoint - transform.position) / 2 + transform.position;
                        //}
                        //Debug.Log(lineToPlaneLength);
                        desiredPosition = (vectorPoint - transform.position) / 2 + transform.position;

                    }
                    else if (wheel < 0)
                    {
                        //desiredPosition = -(targetRotation.position - transform.position) / 2 + transform.position;
                        //Debug.Log(lineToPlaneLength);
                        desiredPosition = -(vectorPoint - transform.position) / 2 + transform.position;

                    }

                    transform.position = Vector3.Lerp(transform.position, desiredPosition, zoomRate);// * Time.deltaTime / Time.timeScale);

                    if (transform.position == desiredPosition)
                    {
                        mode = Mode.IDLE;
                    }
                    break;

                case Mode.PAN:
                    if (panMode == true)
                    {
                        float panNorm = transform.position.y;
                        if (Input.mousePosition.x - Screen.width + panThresh > 0)
                        {
                            targetRotation.Translate(Vector3.right * -panSpeed * Time.deltaTime * panNorm); // wrt Space.Self is default
                            transform.Translate(Vector3.right * -panSpeed * Time.deltaTime * panNorm);
                        }
                        else if (Input.mousePosition.x - panThresh < 0)
                        {
                            targetRotation.Translate(Vector3.right * panSpeed * Time.deltaTime * panNorm);
                            transform.Translate(Vector3.right * panSpeed * Time.deltaTime * panNorm);
                        }

                        if (Input.mousePosition.y - Screen.height + panThresh > 0)
                        {
                            vectorPoint.Set(transform.forward.x, 0, transform.forward.z);
                            targetRotation.Translate(vectorPoint.normalized * -panSpeed * Time.deltaTime * panNorm, Space.World);
                            transform.Translate(vectorPoint.normalized * -panSpeed * Time.deltaTime * panNorm, Space.World);
                        }
                        else if (Input.mousePosition.y - panThresh < 0)
                        {
                            vectorPoint.Set(transform.forward.x, 0, transform.forward.z);
                            targetRotation.Translate(vectorPoint.normalized * panSpeed * Time.deltaTime * panNorm, Space.World);
                            transform.Translate(vectorPoint.normalized * panSpeed * Time.deltaTime * panNorm, Space.World);
                        }
                    }
                    break;
            }

            transform.position = Vector3.ClampMagnitude(transform.position, 500f);
        }

        #region Private Methods
        void LockObject(Transform transformToLock)
        {
            mode = Mode.IDLE;

            isLocked = true;
            LockedTransform = transformToLock;
            off = LockedTransform.position;

            targetRotation.position = LockedTransform.position;
            transform.position = targetRotation.position - new Vector3(defaultZoom * LockedTransform.localScale.x, -defaultZoom * LockedTransform.localScale.x, 0.0f);
        }

        void UnlockObject()
        {
            isLocked = false;
            LockedTransform = null;
            offset = Vector3.zero;
        }

        /// <summary>
        /// Finds the point at which a line intersects a plane.
        /// </summary>
        /// <returns>The plane intersect.</returns>
        /// <param name="u">Line direction vector (p1 - <paramref name="p0"/>)</param>
        /// <param name="p0">A point on the plane.</param>
        /// <param name="n">Normal vector to the plane.</param>
        /// <param name="d">The zero vector.</param>
        float LinePlaneIntersect(Vector3 u, Vector3 p0, Vector3 n, Vector3 d, ref Vector3 point)
        {
            float s = Vector3.Dot(n, (d - p0)) / Vector3.Dot(n, u);
            point = p0 + s * u;
            return s;
        }

        int MouseXBorder()
        {
            if ((Input.mousePosition.x - Screen.width + panThresh) > 0)
            {
                return 1;
            }
            if ((Input.mousePosition.x - panThresh) < 0)
            {
                return -1;
            }
            return 0;
        }

        int MouseYBorder()
        {
            if ((Input.mousePosition.y - Screen.height + panThresh) > 0)
            {
                return 1;
            }
            if ((Input.mousePosition.y - panThresh) < 0)
            {
                return -1;
            }
            return 0;
        }

        static float ClampAngle(float angle, float minOuter, float maxOuter, float inner)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }

            angle = Mathf.Clamp(angle, minOuter, maxOuter);

            if (angle < inner && angle > 0)
            {
                angle -= 2 * inner;
            }
            else if (angle > -inner && angle < 0)
            {
                angle += 2 * inner;
            }

            return angle;
        }

        bool DoubleClick(float t)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - lastClickTime < catchTime * Time.timeScale)
                {
                    lastClickTime = Time.time;
                    return true;
                }

                lastClickTime = Time.time;
                return false;
            }
            return false;
        }

        #endregion
    }
}
