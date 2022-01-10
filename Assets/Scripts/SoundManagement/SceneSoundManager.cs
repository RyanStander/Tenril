using UnityEngine;

/// <summary>
/// This will handle sounds and audio genrally for things in the scene, such as swapping to combat music when entering combat or playing ui sounds
/// </summary>
public class SceneSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource, ambienceAudioSource, menuSoundsAudioSource;
    [SerializeField] private AudioClip interactSound;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.PlayInteractSound, OnPlayInteractSound);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.PlayInteractSound, OnPlayInteractSound);
    }

    private void OnPlayInteractSound(EventData eventData)
    {
        if (eventData is PlayInteractSound)
        {
            menuSoundsAudioSource.PlayOneShot(interactSound);
        }
        else
        {
            Debug.LogWarning("Event was not of PlayInteractSound for event type");
        }
    }
}
