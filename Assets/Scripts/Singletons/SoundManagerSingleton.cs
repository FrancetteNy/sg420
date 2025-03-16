using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManagerSingleton : SingletonViaPrefab<SoundManagerSingleton>
{

    private List<SoundScriptableObject> _soundDataList;
    private static AudioSource _audioSource;
    protected override void InitializeSingleton()
    {
        base.InitializeSingleton();
        _soundDataList = Resources.LoadAll<SoundScriptableObject>("").ToList();
        _audioSource = Inscene.AddComponent<AudioSource>();
    }
    public void PlaySound(string soundName)
    {
        SoundScriptableObject sound = _soundDataList.Find(s => s.Name == soundName);
        if (sound != null)
        {
            _audioSource.clip = sound.AudioSource;
            _audioSource.volume = sound.Volume;
            if (sound.UseRandomPitch)
            {
                _audioSource.pitch = Random.Range(0.5f, 3f);
            }
            else
            {
                _audioSource.pitch = 1f;
            }
            _audioSource.Play();
        }
    }
}
