using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public GameObject lightSource;
    public AudioSource audioSource;
    public AudioClip clickSound;

    // Questa variabile decide se il tasto F funziona o no
    [HideInInspector] public bool hasFlashlight = false;

    private bool isOn = false;

    void Start()
    {
        lightSource.SetActive(false);
    }

    void Update()
    {
        // Il tasto F funziona SOLO se hasFlashlight è true
        if (hasFlashlight && Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;
        lightSource.SetActive(isOn);
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}