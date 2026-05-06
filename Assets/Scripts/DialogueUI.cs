using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource dialogueSource;
    public AudioClip openSound;

    public static DialogueUI Instance { get; private set; }
    public TextMeshProUGUI textDisplay;
    public Image portraitDisplay;
    public PlayerMovement playerMovement; // Riferimento al movimento del player per bloccarlo durante il dialogo
    public GameObject blackScreen;
    private bool keepBlackScreen = false;

    private void Awake()
    {
        // Se esiste già un'istanza, distruggi questa, altrimenti assegnala
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Nasconde il dialogo all'inizio del gioco
        gameObject.SetActive(false);
    }

    public void ShowDialogue(string message, Sprite portrait = null, bool stayBlack = false)
    {
        gameObject.SetActive(true);
        textDisplay.text = message;
        keepBlackScreen = stayBlack; // Salviamo la scelta

        if (portrait != null)
        {
            portraitDisplay.sprite = portrait;
            portraitDisplay.enabled = true;
        }

        if (playerMovement != null) playerMovement.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (openSound != null) dialogueSource.PlayOneShot(openSound);
    }

    public void CloseDialogue()
    {
        gameObject.SetActive(false);

        if (blackScreen != null)
            blackScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerMovement != null) playerMovement.enabled = true;
    }
}