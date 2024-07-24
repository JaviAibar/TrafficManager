using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static Level.GameEngine;

namespace Level
{
    public class TutorialController : MonoBehaviour
    {
        [TutorialEventTypeSelector]
        /* [SerializeReference] */
        [SerializeField] private List<TutorialEvent> tutorialEvents;
        [SerializeField] private Transform mask;
        [SerializeField] private float additionalScalePercentage = 0.15f;
        [SerializeField] private GameObject curtain;
        [SerializeField] private float speed = 5;
        [SerializeField] private float timer = 0;
        [SerializeField] private GameObject tutorialWindow;
        [SerializeField] private TMP_Text tutorialText;
        [SerializeField] private float timeThreshold = 0.5f;
        [SerializeField] private float curtainThreshold = 2f;

        private IEnumerator<TutorialEvent> tutorialEventEnumerator;
        private GameEngine gameEngine;

        public GameObject TutorialWindow => tutorialWindow;

        public TMP_Text TutorialText => tutorialText;

        private void Awake() => gameEngine = FindObjectOfType<GameEngine>();

        private void Start()
        {
            tutorialEventEnumerator = tutorialEvents.GetEnumerator();
            tutorialEventEnumerator.Reset();
            tutorialEventEnumerator.MoveNext();

            // This action is performed with priority to prevent timer to update sooner than it should
           /* if (TutorialEventEnumerator.Current.eventType == EventType.ChangeSpeed && TutorialEventEnumerator.Current.time == 0)
                ChangeSpeed(TutorialEventEnumerator.Current.newSpeed);*/
        }

        private void Update()
        {
            int speed = (int)gameEngine.Speed;
            timer += Time.deltaTime * speed;
            TutorialEvent tutorialEvent = tutorialEventEnumerator.Current;
            if( tutorialEvent == null )
            {
                Debug.Log("No more events");
                return;
            }
            if (tutorialEvent.initiated == false)
                tutorialEvent.Awake();
            float nextTimestamp = tutorialEventEnumerator.Current.Time;

            if (tutorialEvent.Asap || IsTheTutorialEvent(nextTimestamp))
            {
               /* print($"Tutorial: {tutorialEvent.name}");
                if (tutorialEvent is ShowMessageTutorialScriptableObject)
                {
                    print(((ShowMessageTutorialScriptableObject)tutorialEvent).messageIndex);
                } else
                {
                    print(((ShowMessageTutorialScriptableObject)tutorialEvent).messageIndex);

                }*/
                tutorialEvent.Execute();
                if (tutorialEvent.Completed)
                {
                    tutorialEventEnumerator.MoveNext();
                }
               /* switch (TutorialEvent.eventType)
                {
                    case EventType.ChangeSpeed:
                        ChangeSpeed(TutorialEvent.newSpeed);
                        break;
                    case EventType.HideCurtain:
                        HideCurtain();
                        break;
                    case EventType.HideMessageWindow:
                        HideMessageWindow();
                        break;
                    case EventType.ShowMessage:
                        ShowTutorialMessage(TutorialEvent.messageIndex);
                        break;
                    case EventType.MoveToTarget:
                        StartCoroutine(MoveCurtain(TutorialEvent.target));
                        break;
                }*/
            }

            // TODO: Check if needed condition ShowMessage event
            if (tutorialWindow && Input.GetKeyDown(KeyCode.Escape) && tutorialWindow.activeSelf)
            {
                HideMessageWindow();
            }
        }

        private bool IsTheTutorialEvent(float nextTimestamp)
        {
            return timer >= nextTimestamp && nextTimestamp <= nextTimestamp + timeThreshold;
        }

        private void HideMessageWindow()
        {
            tutorialWindow.SetActive(false);
            tutorialEventEnumerator.MoveNext();
        }

        public void HideCurtain()
        {
            curtain.SetActive(false);
            tutorialEventEnumerator.MoveNext();
        }

        [ContextMenu("Move to target")]
        public IEnumerator MoveCurtain(Transform target)
        {
            curtain.SetActive(true);

            Vector3 gameObjectSize = GameObjectSize(target);
            float maxValue = Mathf.Max(gameObjectSize.x, gameObjectSize.y);
            gameObjectSize = new Vector3(maxValue, maxValue, 0);
            mask.transform.localScale = gameObjectSize + gameObjectSize * additionalScalePercentage;

            ChangeSpeed(GameSpeed.Paused); // ChangeSpeed does move the IEnumerator to next
            Vector3 originalPos = mask.position;
            float t = 0;
            while (Vector3.Distance(mask.position, target.position) >= curtainThreshold)
            {
                t += Time.deltaTime;
                mask.position = Vector3.Lerp(originalPos, target.position, speed * t);
                yield return null;
            }
        }

        private Vector3 GameObjectSize(Transform transform)
        {
            if (!transform.TryGetComponent<Renderer>(out var rend))
            {
                rend = transform.GetComponentInChildren<Renderer>();
            }
            if (rend)
                return rend.bounds.size;
            if (!transform.TryGetComponent<Image>(out var im))
            {
                im = transform.GetComponentInChildren<Image>();
            }
            RectTransform rect = null;
            if (im != null)
            {
                rect = im.GetComponent<RectTransform>();
            }

            Canvas canvas = null;
            if (rect != null)
            {
                canvas = rect.GetComponentInParent<Canvas>();
            }

            CanvasScaler canvasScaler = null;
            if (canvas != null)
            {
                canvasScaler = canvas.GetComponent<CanvasScaler>();
            }
            if (rect && canvas && canvasScaler)
                //return (RectTransformUtility.PixelAdjustRect(rect, canvas).size * (2.54f / Screen.dpi)) / canvasScaler.referencePixelsPerUnit;
                return rect.rect.size / (canvasScaler.referencePixelsPerUnit / 10);
            return Vector3.one;
        }

        public void AcceptTutorialWindow()
        {
            //ChangeSpeed(GameEngine.GameSpeed.Normal);
            tutorialWindow.SetActive(false);
            tutorialEventEnumerator.MoveNext();
        }

       /* [ContextMenu("Set Tutorial Message")]
        public void ShowTutorialMessage()
        {
            ShowTutorialMessage(TutorialEventEnumerator.Current.MessageIndex);
        }*/
        public void ShowTutorialMessage(int messageIndex)
        {
            
        }

        public void ChangeSpeed(GameSpeed newSpeed)
        {
            gameEngine.ChangeSpeed((int)newSpeed);
            tutorialEventEnumerator.MoveNext();
        }

        /*public void ResetTutorial()
        {
            TutorialEventEnumerator.Reset();
            TutorialEventEnumerator.MoveNext();
            if (TutorialEventEnumerator.Current.eventType == EventType.ChangeSpeed && TutorialEventEnumerator.Current.time == 0)
                ChangeSpeed(TutorialEventEnumerator.Current.newSpeed);
        }*/

       /* [System.Serializable]
        public struct Moment
        {
            public float time;
            public bool asap;           // Ignores time and plays as soon as posible (default: false)
            public EventType eventType;
            public Transform target;    // Only used when eventType = MoveToTarget, otherwise, ignored
            public int messageIndex;    // Only used when eventType = ShowMessage, otherwise ignored
            public GameSpeed newSpeed; // Only used when eventType = ChangeSpeed, otherwise ignored
            public override readonly string ToString()
            {
                string msg = eventType + " " + (asap ? "as soon as posible" : time);
                switch (eventType)
                {
                    case EventType.ShowMessage: msg += " with the next message: " + messageIndex.ToString("D3"); break;
                    case EventType.ChangeSpeed: msg += " with a new speed of " + newSpeed; break;
                    case EventType.HideCurtain: msg += " closing the curtain "; break;
                    case EventType.HideMessageWindow: msg += " closing the message window "; break;
                    case EventType.None: msg += " no action at all "; break;
                    case EventType.MoveToTarget: msg += " moving the curtain to " + target.name; break;
                }
                return msg;
            }

            public Moment(float time, bool asap, EventType eventType, Transform target, int messageIndex, GameSpeed newSpeed)
            {
                this.time = time;
                this.asap = asap;
                this.eventType = eventType;
                this.target = target;
                this.messageIndex = messageIndex;
                this.newSpeed = newSpeed;
            }
        }
       */
        [ContextMenu("Current moment")]
        public void PrintCurrentMoment()
        {
            print(tutorialEventEnumerator.Current);
        }

        public enum EventType
        {
            None,
            ShowMessage,
            MoveToTarget,
            HideMessageWindow,
            HideCurtain,
            ChangeSpeed,
        }
    }
}
