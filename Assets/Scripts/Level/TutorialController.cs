using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class TutorialController : MonoBehaviour
{
    public List<Transform> targets;
    public IEnumerator<Transform> targetsEnumerator;
    public Transform mask;
    public GameObject curtain;
    private Vector3 initialPosition;
    bool moving;
    public float speed = 5;
    public float timer = 0;
    public int messageIndex = 0;
    public GameObject tutorialWindow;
    public TMP_Text tutorialText;
    private bool[] messages;
    private GameEngine gameEngine;



    private void Awake()
    {
        gameEngine = FindObjectOfType<GameEngine>();
    }
    private void Start()
    {
        targetsEnumerator = targets.GetEnumerator();
        print(targets.Count);
        targetsEnumerator.MoveNext();
        print(targetsEnumerator.Current);
        messages = new bool[]{ true, true, false };
        gameEngine.ChangeSpeed((int)GameEngine.GameSpeed.Paused);
        SetNextTutorialMessage();

    }

    private void Update()
    {
        int speed = (int)gameEngine.GetGameSpeed();
        timer += Time.deltaTime * speed;

        if (timer >= 1f && timer <= 1.2f)
        {
            PointToTarget();
        }
        if (moving)
        {
            Transform target = targetsEnumerator.Current;
            //  print($"TargetTransform ");
            //  print(target);
            // print("mask: " + mask.position);
            //  print("target: " + target.position);
            //  print("init: " + initialPosition);

            //   print("Lerp: " + Vector3.Lerp(mask.position, target.position, speed * Time.deltaTime));
            print(Vector3.Distance(mask.position, target.position));
            mask.position = Vector3.Lerp(mask.position, target.position, speed * 5 * Time.deltaTime);
            moving = Vector3.Distance(mask.position, target.position) >= 2f;
            if (!moving)
            {
                targetsEnumerator.MoveNext();
                SetNextTutorialMessage();
            }
        }
    }

    [ContextMenu("Move to target")]
    public void PointToTarget()
    {
        curtain.SetActive(true);
        initialPosition = mask.position;
        moving = true;
    }

    public void AcceptTutorialWindow()
    {
        messageIndex++;
        if (messages[messageIndex])
        {
            SetNextTutorialMessage();
        }else
        {
            gameEngine.ChangeSpeed((int)GameEngine.GameSpeed.Normal);
            tutorialWindow.SetActive(false);
        }
    }

    [ContextMenu("Set Tutorial Message")]
    public void SetNextTutorialMessage()
    {
        gameEngine.ChangeSpeed((int)GameEngine.GameSpeed.Paused);
        tutorialWindow.SetActive(true);
        tutorialText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial Messages", messageIndex.ToString("D3"));// + " " + (i + 1).ToString("D3");
       // LocalizationAsset.Get "Tutorial_Text_" + ;
    }
    /*
    public struct Msg
    {
        public Msg(bool inmediate) { this.inmediate = inmediate; }
        public bool inmediate;
    }*/

    public struct Moments
    {
        public float time;
        public string eventType;
    }
}
