using UnityEngine;

public class PlayerChoice : MonoBehaviour
{
    [SerializeField] private AudioClip[] selectSounds;

    public void ButtonClick()
    {
        SoundManager.Instance.PlayRandomSoundEffect(selectSounds, transform, 1);
    }
}
