using UnityEngine;

public class OneTimeInteractable : MonoBehaviour, IInteractable
{
    [Header("Impostazioni Dialogo")]
    public string message = "Dopo aver letto questo, qualcosa è cambiato...";

    [Header("Riferimenti Oggetti")]
    public GameObject blackScreen;      // Trascina qui il pannello nero
    public UniversalDoor doorToUnlock;  // Trascina qui la porta da sbloccare

    private bool hasInteracted = false;

    public void Interact()
    {
        // Se abbiamo già interagito, non fare nulla
        if (hasInteracted) return;

        ExecuteEvent();
    }

    private void ExecuteEvent()
    {
        hasInteracted = true;

        // 1. Mostra la schermata nera
        if (blackScreen != null)
            blackScreen.SetActive(true);

        // 2. Sblocca la porta
        if (doorToUnlock != null)
        {
            doorToUnlock.UnlockDoor();
            doorToUnlock.ToggleDoor();
        }

        // 3. Mostra il dialogo (usando il sistema che abbiamo già)
        // Prendiamo l'identità da questo oggetto stesso
        EntityIdentity id = GetComponent<EntityIdentity>();
        DialogueUI.Instance.ShowDialogue(message, id != null ? id.entityPortrait : null);

        Debug.Log("Evento completato: Porta sbloccata e schermo nero attivo.");
    }
}