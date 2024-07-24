using Level;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "ShowMessageTutorial", menuName = "ScriptableObjects/ShowMessageTutorial")]

[System.Serializable]
public class ShowMessageTutorialScriptableObject : TutorialEvent
{
    public string messageIndex;
    private GameObject tutorialWindow;
    private TMP_Text tutorialText;

    public override void Awake()
    {
        base.Awake();
        TutorialController tutController = FindAnyObjectByType<TutorialController>();
        tutorialWindow = tutController.TutorialWindow;
        tutorialText = tutController.TutorialText;
        Completed = false;
    }

    public override void Execute()
    {
        if (StopTime)
            GameEngine.Instance.Speed = GameEngine.GameSpeed.Paused;

        tutorialWindow.SetActive(true);
        tutorialText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial Messages", messageIndex);
    }
}
