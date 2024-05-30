using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] clips;

    public AudioSource audioSource;
     
    // Start is called before the first frame update
    void Start()
    {
        // videoPlayer.url = videoUrl;
        // videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        // videoPlayer.EnableAudioTrack (0, true);
        // videoPlayer.Prepare ();
        // videoPlayer.play();


       videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        playClip(clips[0]);
    
 

    
    }


    public void playClip(VideoClip clip) {
            //We want to play from video clip not from url
        videoPlayer.source = VideoSource.VideoClip;

        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        videoPlayer.clip = clip;
        videoPlayer.Prepare();

        //Wait until video is prepared
        // while (!videoPlayer.isPrepared)
        // {
        //     Debug.Log("Preparing Video");
        // }

        // Debug.Log("Done Preparing Video");

        //Assign the Texture from Video to RawImage to be displayed

        //Play Video
        videoPlayer.Play();

        //Play Sound
        audioSource.Play();

        }
}


