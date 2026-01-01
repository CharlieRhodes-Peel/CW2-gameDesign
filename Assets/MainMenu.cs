using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{

    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public AudioSource menuMusic;

    private void Start(){
        //load scene after video played
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void StartGame()
    {
        StartCoroutine(LoadWithDelayPlay());
    }

     private IEnumerator LoadWithDelayPlay()
    {
        // Stop menu music
        if (menuMusic != null)
        {
            menuMusic.Stop();
        }
        
        //delay to let button sound play
        yield return new WaitForSeconds(0.3f);

        rawImage.gameObject.SetActive(true);
        rawImage.enabled = true;
        // Play video
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Video finished, load the next scene
        SceneManager.LoadScene("InitScene");
    }

}
