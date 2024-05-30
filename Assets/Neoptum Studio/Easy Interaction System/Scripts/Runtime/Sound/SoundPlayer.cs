using System.Collections.Generic;
using System.Linq;
using EIS.Runtime.Misc;
using UnityEngine;

namespace EIS.Runtime.Sound
{
    /// <summary>
    /// Singleton class responsible for playing sound effects in the scene.
    /// </summary>
    public class SoundPlayer : Singleton<SoundPlayer>
    {
        /// <summary>
        /// List of sound items containing references to audio clips and sound indexes.
        /// </summary>
        [Tooltip("List of sound items containing references to audio clips and sound indexes")] [SerializeField]
        private List<SoundItem> soundItems = new List<SoundItem>();


        /// <summary>
        /// Plays a sound effect one time at the specified position.
        /// </summary>
        /// <param name="position">The position in world space to play the sound from.</param>
        /// <param name="soundItem">The SoundItem containing the audio clip or sound index to play.</param>
        public void PlayOneShot(Vector3 position, SoundItem soundItem)
        {
            if (soundItem.audioClip == null)
            {
                // Handle cases where soundItem doesn't have a direct audio clip
                if (soundItem.soundEffectIndex == SoundItem.SoundEffectIndex.DYNAMIC)
                {
                    PlayOneShot(position, soundItem.soundIndex);
                }
                else
                {
                    PlayOneShot(position, soundItem.soundEffectIndex);
                }
            }
            else
            {
                PlayOneShot(position, soundItem.audioClip);
            }
        }

        /// <summary>
        /// Plays a sound effect one time at the specified position, using a sound index from the SoundItem list.
        /// </summary>
        /// <param name="position">The position in world space to play the sound from.</param>
        /// <param name="soundIndex">The string identifier of the sound effect to play from the SoundItem list.</param>
        public void PlayOneShot(Vector3 position, string soundIndex)
        {
            // Find the SoundItem with matching soundEffectIndex set to DYNAMIC and soundIndex
            SoundItem soundItem = soundItems.FirstOrDefault(si =>
                si.soundEffectIndex == SoundItem.SoundEffectIndex.DYNAMIC && si.soundIndex.Equals(soundIndex));

            if (soundItem != null)
                PlayOneShot(position, soundItem.audioClip);
        }

        /// <summary>
        /// Plays a sound effect one time at the specified position, using a SoundEffectIndex enum value.
        /// </summary>
        /// <param name="position">The position in world space to play the sound from.</param>
        /// <param name="soundIndex">The SoundEffectIndex enum value of the sound effect to play from the SoundItem list.</param>
        public void PlayOneShot(Vector3 position, SoundItem.SoundEffectIndex soundIndex)
        {
            // Find the SoundItem with matching soundEffectIndex
            SoundItem soundItem = soundItems.FirstOrDefault(si => si.soundEffectIndex == soundIndex);

            if (soundItem != null)
                PlayOneShot(position, soundItem.audioClip);
        }

        /// <summary>
        /// Plays a provided audio clip one time at the specified position.
        /// </summary>
        /// <param name="position">The position in world space to play the sound from.</param>
        /// <param name="audioClip">The AudioClip to play.</param>
        public void PlayOneShot(Vector3 position, AudioClip audioClip)
        {
            if (audioClip == null)
                return;

            GameObject soundGO = new GameObject(audioClip.name);
            soundGO.transform.position = position;
            AudioSource source = soundGO.AddComponent<AudioSource>();
            source.clip = audioClip;
            source.Play();
            Destroy(soundGO, audioClip.length);
        }
    }
}