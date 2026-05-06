using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }
    public TextMeshProUGUI textDisplay;
    public Image portraitDisplay;
    public PlayerMovement playerMovement; // Riferimento al movimento del player per bloccarlo durante il dialogo

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

    public void ShowDialogue(string message, Sprite portrait = null)
    {
        gameObject.SetActive(true);
        textDisplay.text = message;
        if (portrait != null)
        {
            portraitDisplay.sprite = portrait;
            portraitDisplay.enabled = true;
        }

        playerMovement.enabled = false; // Disabilita il movimento del player durante il dialogo
        // Opzionale: Sblocca il mouse per permettere il click
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseDialogue()
    {
        gameObject.SetActive(false);

        // Ri-blocca il mouse per tornare al movimento del player
        Cursor.lockState = CursorLockMode.Locked;
        playerMovement.enabled = true; // Riabilita il movimento del player dopo il dialogo
    }
}