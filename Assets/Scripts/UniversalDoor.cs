using UnityEngine;

public class UniversalDoor : MonoBehaviour, IInteractable
{
    [Header("Impostazioni Base")]
    public bool isLocked = false; // Se è chiusa a chiave/narrativamente
    public string lockedMessage = "La porta non si apre...";

    private bool isOpen = false;

    public void Interact()
    {
        // Se la porta è bloccata, non si apre e mostra un messaggio
        if (isLocked)
        {
            EntityIdentity identity = GetComponent<EntityIdentity>();

            if (identity != null)
            {
                DialogueUI.Instance.ShowDialogue(lockedMessage, identity.entityPortrait);
            }
            else
            {
                DialogueUI.Instance.ShowDialogue(lockedMessage);
            }

            return;
        }

        // Se è sbloccata, si comporta come una porta normale
        ToggleDoor();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        float angle = isOpen ? 90f : -90f;
        transform.Rotate(0, angle, 0);
        Debug.Log(isOpen ? "Porta Aperta" : "Porta Chiusa");
    }

    // Questa funzione verrà chiamata dai tuoi Trigger o Dialoghi per sbloccarla
    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("La porta è stata sbloccata narrativamente!");
    }
}