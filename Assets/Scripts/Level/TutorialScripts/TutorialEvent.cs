using UnityEngine;

[System.Serializable]
public abstract class TutorialEvent : ScriptableObject
{
    public float time;
    public bool asap;
    public bool Asap => asap;
    public float Time => time;
    public bool stopTime;
    public bool StopTime => stopTime;

    public bool Completed { get; protected set; } = false;

    public bool initiated = false;

    public virtual void Awake()
    {
        initiated = true;
    }

    public abstract void Execute();
}