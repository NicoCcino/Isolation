using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "TimedEvent", menuName = "Scriptable Objects/TimedEvent")]


public class TimedEvent : ScriptableObject
{
    public int RunDurationPercentageStart = 0;
    public AudioClip audioClip;
    
}
