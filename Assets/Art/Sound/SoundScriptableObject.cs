using UnityEngine;

[CreateAssetMenu(fileName = "SoundScriptableObject", menuName = "Scriptable Objects/SoundScriptableObject")]
public class SoundScriptableObject : ScriptableObject
{
    public string Name;
    public AudioClip AudioSource;
    [Range(0f, 1f)]
    public float Volume;
    public bool UseRandomPitch;
}
