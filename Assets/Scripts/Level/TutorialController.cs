using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public List<Moment> moments;
    private IEnumerator<Moment> momentEnumerator;
    //public Moment currentMoment;
    public Transform mask;
    public Vector3 originalMaskScale = new Vector3(17.5f, 16.5f, 0);
    public float additionalScalePercentage = 0.15f;
    public GameObject curtain;
    //private Vector3 initialPosition;
    public bool moving;
    public float speed = 5;
    public float timer = 0;
    //public int messageIndex = 0;
    public GameObject tutorialWindow;
    public TMP_Text tutorialText;
    //private bool[] messages;
    private GameEngine gameEngine;
    public float timeThreshold = 0.5f;
    public float curtainThreshold = 2f;


    private void Awake()
    {
        gameEngine = FindObjectOfType<GameEngine>();
    }
    private void Start()
    {
        //moments.ForEach(m => print(m));

        momentEnumerator = moments.GetEnumerator();
        momentEnumerator.Reset();
        momentEnumerator.MoveNext();
        
        // This action is performed with priority to prevent timer to update sooner than it should
        if (momentEnumerator.Current.eventType == EventType.ChangeSpeed && momentEnumerator.Current.time == 0)
            ChangeSpeed(momentEnumerator.Current.newSpeed);
    }

    private void Update()
    {
        int speed = (int)gameEngine.GetGameSpeed();
        timer += Time.deltaTime * speed;
        Moment moment = momentEnumerator.Current;
        float nextTimestamp = momentEnumerator.Current.time;
        //print(moment);
        if (moment.asap || (timer >= nextTimestamp && nextTimestamp <= nextTimestamp + timeThreshold))
        {
            switch(moment.eventType)
            {
                case EventType.ChangeSpeed:
                    ChangeSpeed(moment.newSpeed);
                    break;
                case EventType.HideCurtain:
                    HideCurtain();
                    break;
                case EventType.HideMessageWindow:
                    HideMessageWindow();
                    break;
                case EventType.ShowMessage:
                    ShowTutorialMessage(moment.messageIndex);
                    break;
                case EventType.MoveToTarget:
                    StartMovingCurtain();
                    break;
            }
        }
       // Debug.Break();
        if (moving)
            MoveCurtain(moment.target);
    }

    private void HideMessageWindow()
    {
        tutorialWindow.SetActive(false);
        momentEnumerator.MoveNext();
    }

    public void HideCurtain()
    {
        curtain.SetActive(false);
        momentEnumerator.MoveNext();
    }

    [ContextMenu("Move to target")]
    public void StartMovingCurtain()
    {
        curtain.SetActive(true);
        //  initialPosition = mask.position;
        moving = true;
    }

    public void MoveCurtain(Transform target)
    {
        //  print($"TargetTransform ");
        //  print(target);
        // print("mask: " + mask.position);
        //  print("target: " + target.position);
        //  print("init: " + initialPosition);

        //   print("Lerp: " + Vector3.Lerp(mask.position, target.position, speed * Time.deltaTime));

        //sprint(Vector3.Distance(mask.position, target.position));

        Vector3 gameObjectSize = GameObjectSize(target);
        float maxValue = Mathf.Max(gameObjectSize.x, gameObjectSize.y);
        gameObjectSize = new Vector3(maxValue, maxValue, 0);
        mask.transform.localScale = /* originalMaskScale +*/ gameObjectSize + gameObjectSize * additionalScalePercentage;
        //print(target.GetComponent<Renderer>().bounds.center);
        //print(target.GetComponent<Image>().)
       // print("MOVIENDO");
        //RectTransform rect = (RectTransform) target; //.GetComponent<RectTransform>();
       //Vector3 center = new Vector3(rect.rect.width * rect.pivot.x, rect.rect.height * rect.pivot.y, 0);
        // print("Min "+rect.offsetMin);
        // print("Max "+rect.offsetMax);

        //Vector3 endPos = rect.position;
       // print(endPos);
        mask.position = Vector3.Lerp(mask.position, target.position, speed * Time.deltaTime);
        moving = Vector3.Distance(mask.position, target.position) >= curtainThreshold;
        if (!moving)
        {
            ChangeSpeed(GameEngine.GameSpeed.Paused); // ChangeSpeed does move the IEnumerator to next
        }
    }

    private Vector3 GameObjectSize(Transform transform)
    {
        Renderer rend = transform.GetComponent<Renderer>() ?? transform.GetComponentInChildren<Renderer>();
        if (rend)
            return rend.bounds.size;
        Image im = transform.GetComponent<Image>() ?? transform.GetComponentInChildren<Image>();
        RectTransform rect = im?.GetComponent<RectTransform>();
        Canvas canvas = rect?.GetComponentInParent<Canvas>();
        CanvasScaler canvasScaler = canvas?.GetComponent<CanvasScaler>();
        if (rect && canvas && canvasScaler)
            //return (RectTransformUtility.PixelAdjustRect(rect, canvas).size * (2.54f / Screen.dpi)) / canvasScaler.referencePixelsPerUnit;
            return rect.rect.size / (canvasScaler.referencePixelsPerUnit / 10);
        return Vector3.one;
    }

    public void AcceptTutorialWindow()
    {
        //ChangeSpeed(GameEngine.GameSpeed.Normal);
        tutorialWindow.SetActive(false);
        momentEnumerator.MoveNext();
    }

    [ContextMenu("Set Tutorial Message")]
    public void ShowTutorialMessage()
    {
        ShowTutorialMessage(momentEnumerator.Current.messageIndex);
    }
    public void ShowTutorialMessage(int messageIndex)
    {
        tutorialWindow.SetActive(true);
        tutorialText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial Messages", messageIndex.ToString("D3"));
    }

    public void ChangeSpeed(GameEngine.GameSpeed newSpeed)
    {
        gameEngine.ChangeSpeed((int)newSpeed);
        momentEnumerator.MoveNext();
    }

    public void ResetTutorial()
    {
        momentEnumerator.Reset();
        momentEnumerator.MoveNext();
        if (momentEnumerator.Current.eventType == EventType.ChangeSpeed && momentEnumerator.Current.time == 0)
            ChangeSpeed(momentEnumerator.Current.newSpeed);
    }

    [System.Serializable]
    public struct Moment
    {
        public float time;
        public bool asap;           // Ignores time and plays as soon as posible (default: false)
        public EventType eventType;
        public Transform target;    // Only used when eventType = MoveToTarget, otherwise, ignored
        public int messageIndex;    // Only used when eventType = ShowMessage, otherwise ignored
        public GameEngine.GameSpeed newSpeed; // Only used when eventType = ChangeSpeed, otherwise ignored
        public override string ToString()
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
    }

    [ContextMenu("Current moment")]
    public void PrintCurrentMoment()
    {
        print(momentEnumerator.Current);
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
