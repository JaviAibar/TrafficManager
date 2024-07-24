using Level;
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeSpeedTutorial", menuName = "ScriptableObjects/ChangeSpeedTutorial")]
[System.Serializable]
public class ChangeSpeedTutorialScriptableObject : TutorialEvent
{
    public GameEngine.GameSpeed speed;
    public override void Execute()
    {
        GameEngine.Instance.Speed = speed;
        Completed = true;
    }
}
