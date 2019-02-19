using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using Game.Bodies;

namespace Game.UI
{
    [System.Serializable]
    public class OrbitProgressBar : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField]
        BodyManager bodyManager;

        [SerializeField]
        Text epochText;

        [SerializeField]
        Text countdownText;

        Image impactMarker;
        Image deflectionMarker;
        Image launchMarker;

        Image progressBar;

        float lastClickTime = 0.0f;
        float catchTime = 0.25f;

        void Awake()
        {
            progressBar = GetComponent<Image>();
        }

        void Update()
        {
            //float percent = bodyManager.GetCurrentProgressPercent();
            //if (percent >= 0.0f && percent <= 1.0f)
            //{
            //    progressBar.fillAmount = percent;
            //}
            UpdateProgressText();
        }

        /// <summary>
        /// Skips orbits on drag event; implementation of <see cref="IDragHandler"/>.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnDrag(PointerEventData eventData)
        {
            TrySkip(eventData);
        }

        /// <summary>
        /// Skips orbits on pointer down event; implementation of <see cref="IPointerDownHandler"/>.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            TrySkip(eventData);
        }

        void TrySkip(PointerEventData eventData)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                progressBar.rectTransform, eventData.position, null, out localPoint))
            {
                float percent = Mathf.InverseLerp(progressBar.rectTransform.rect.xMin, 
                                                  progressBar.rectTransform.rect.xMax, 
                                                  localPoint.x);
                bodyManager.SkipToPercent(percent);
            }
        }

        void UpdateProgressText()
        {
            //epochText.text = Orbits.OrbitUtils.JDN2DateTime(bodyManager.CurrentEpoch).ToShortDateString();
            //int daysToImpact = (int)System.Math.Round(bodyManager.ImpactEpoch - bodyManager.CurrentEpoch);
            //countdownText.text = daysToImpact.ToString();
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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                GameObject receivedPointerDown = eventData.pointerPress;

                if (receivedPointerDown == launchMarker)
                {
                    Debug.Log("Double clicked on Launch marker");
                }
                if (receivedPointerDown == deflectionMarker)
                {
                    Debug.Log("Double clicked on Deflection marker");
                }
                if (receivedPointerDown == impactMarker)
                {
                    Debug.Log("Double clicked on Impact marker");
                }
            }
            else
            {
                Debug.Log("Click count = " + eventData.clickCount);
            }
        }
    }
}
