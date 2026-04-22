using UnityEngine;

namespace SABI
{
    public class AudioObject : MonoBehaviour
    {
        public Vector3? location = null;
        public AudioClip audioClip = null;

        public AudioSource audioSource = null;
        public bool shouldLoop;
        public float volume = 1,
            randomPitchRange = 0;
        public float? legth = null;
        bool isActive;

        float activateTime;

        void OnEnable()
        {
            activateTime = Time.time;
        }

        public void SetAudioObjectActive()
        {
            isActive = true;
        }

        public bool GetIsActive() => isActive;

        public void SetPropertys(
            Vector3? locationArg = null,
            AudioClip audioClipArg = null,
            //AudioSource audioSourceArg = null,
            bool loopArg = false,
            float volumeArg = 1,
            float randomPitchRangeArg = 0,
            float? legthArg = null
        )
        {
            location = locationArg;
            audioClip = audioClipArg;
            //audioSource = audioSourceArg;
            shouldLoop = loopArg;
            volume = volumeArg;
            randomPitchRange = randomPitchRangeArg;
            legth = legthArg;
        }

        public void ResetData()
        {
            Debug.Log(
                $"[SAB] AudioManager Reset() audioClip:{audioClip} Reset() after time {Time.time - activateTime} ",
                this
            );
            location = null;
            //audioClip = null;
            shouldLoop = false;
            volume = 1;
            randomPitchRange = 0;
            legth = null;
            isActive = false;
            audioSource.Stop();
            gameObject.SetActive(false);
        }
    }
}
