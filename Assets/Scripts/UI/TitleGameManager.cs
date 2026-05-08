using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleGameManager : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickClip;

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();

    

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickClip != null)
        {
            audioSource.PlayOneShot(buttonClickClip);
        }
    }
}