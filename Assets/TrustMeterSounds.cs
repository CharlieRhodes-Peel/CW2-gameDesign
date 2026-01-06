using UnityEngine;

public class TrustMeterSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] openPopupSounds;
    [SerializeField] private AudioClip[] closePopupSounds;
    
    public void OpenSound()
    {
        SoundManager.Instance.PlayRandomSoundEffect(openPopupSounds, transform, 1);
    }

    public void CloseSound()
    {
        SoundManager.Instance.PlayRandomSoundEffect(closePopupSounds, transform, 1);
    }
}
