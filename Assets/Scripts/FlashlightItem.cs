using UnityEngine;

public class FlashlightItem : MonoBehaviour, IInteractable
{
    public FlashlightController playerFlashlight; // Trascina qui il "FlashlightGroup" del player
    public string pickupMessage = "Hai preso la torcia! Premi 'F' per accenderla.";

    public void Interact()
    {
        // 1. Abilita la torcia nel controller del giocatore
        playerFlashlight.hasFlashlight = true;

        // 2. Mostra un messaggio a schermo
        DialogueUI.Instance.ShowDialogue(pickupMessage);

        // 3. Fai sparire l'oggetto dal tavolo
        gameObject.SetActive(false);
    }
}