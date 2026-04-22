using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace SABI{
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instence;

    [SerializeField]
    private AudioObject audioGameObject;
    Queue<AudioObject> audioSourcesQueue = new Queue<AudioObject>();
    int queuLength = 5;

    [SerializeField]
    private bool useObjectPool = true;

    public void Play(
        AudioClip audioClip,
        Vector3? location = null,
        float randomPitchRange = 0,
        bool isContinues = false,
        float? length = null,
        float volume = 1
    )
    {
        if (audioClip == null)
            return;
        AudioObject pooledObject = GetPooledAudioSource();
        pooledObject.SetPropertys(
            audioClipArg: audioClip,
            locationArg: location,
            randomPitchRangeArg: randomPitchRange,
            loopArg: isContinues,
            legthArg: length,
            volumeArg: volume
        );
        CommonPlayLogic(pooledObject);
    }

    public AudioObject PlayAndGetAudioObject(
        AudioClip audioClip,
        Vector3? location = null,
        float randomPitchRange = 0,
        bool isContinues = false,
        float? length = null,
        float volume = 1
    )
    {
        AudioObject pooledObject = GetPooledAudioSource();
        pooledObject.SetPropertys(
            audioClipArg: audioClip,
            locationArg: location,
            randomPitchRangeArg: randomPitchRange,
            loopArg: isContinues,
            legthArg: length,
            volumeArg: volume
        );
        CommonPlayLogic(pooledObject);
        return pooledObject;
    }

    private void CommonPlayLogic(AudioObject pooledObject)
    {
        AudioClip audioClip = pooledObject.audioClip;
        bool isContinues = pooledObject.shouldLoop;
        Vector3? location = pooledObject.location;
        float volume = pooledObject.volume;
        float? length = pooledObject.legth;
        float randomPitchRange = pooledObject.randomPitchRange;

        if (audioClip == null)
            return;

        pooledObject.name = $"AudioObject : {audioClip.name}";

        AudioSource audioSource = pooledObject.audioSource;
        audioSource.clip = audioClip;

        // Volume
        audioSource.volume = volume;

        // Set property : location

        if (location == null)
        {
            audioSource.spatialBlend = 0;
        }
        else
        {
            audioSource.spatialBlend = 0.75f;
            pooledObject.transform.position = location ?? Vector3.zero;
        }

        // Set property : loop

        audioSource.loop = isContinues;

        // Set property : pitch

        audioSource.pitch =
            randomPitchRange == 0 ? 1f : Random.Range(-randomPitchRange, randomPitchRange);

        pooledObject.gameObject.SetActive(true);

        pooledObject.SetAudioObjectActive();

        audioSource.Play();

        if (!isContinues)
        {
            ReturnPooledAudioSource(pooledObject, audioClip.length);
        }
        else if (isContinues && length != null)
        {
            ReturnPooledAudioSource(pooledObject, length.Value);
        }
    }

    public void StopAudio(AudioObject audioObject)
    {
        if (audioObject == null)
            return;
        AudioSource audio = audioObject.audioSource;
        audio.Stop();
        ReturnPooledAudioSource(audioObject);
    }

    #region OBJECT POOLING

    private void Awake()
    {
        Instence = this;
        if (useObjectPool)
            SetupObjectPool();
    }

    private void SetupObjectPool()
    {
        for (int i = 0; i < queuLength; i++)
        {
            AudioObject newAudioSource = Instantiate(audioGameObject, transform);
            newAudioSource.ResetData();
            audioSourcesQueue.Enqueue(newAudioSource);
            audioQuieitems++;
        }
    }

    int audioQuieitems = 0;

    public AudioObject GetPooledAudioSource()
    {
        if (useObjectPool && audioSourcesQueue.Count > 0)
        {
            AudioObject audioObject = audioSourcesQueue.Dequeue();
            audioQuieitems--;
            return audioObject;
        }
        AudioObject newAudioObject = Instantiate(audioGameObject, transform);
        return newAudioObject;
    }

    public void ReturnPooledAudioSource(AudioObject audioObject)
    {
        if (audioObject == null)
        {
            Debug.LogError("Null not expected");
            return;
        }
        if (useObjectPool)
        {
            audioObject.ResetData();
            audioSourcesQueue.Enqueue(audioObject);
            audioQuieitems++;
        }
        else
        {
            audioObject.DestroyGameObject();
        }
    }

    public void ReturnPooledAudioSource(AudioObject audioObject, float delay)
    {
        this.DelayedExecution(delay, () => ReturnPooledAudioSource(audioObject));
    }

    #endregion
}
}