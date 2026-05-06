using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private Transform destination; // Trascina qui un oggetto vuoto che indica dove arrivare

    private void OnTriggerEnter(Collider other)
    {
        // Controlliamo se chi è entrato ha il Character Controller
        CharacterController cc = other.GetComponent<CharacterController>();

        if (cc != null)
        {
            // 1. Disabilitiamo temporaneamente il controller per permettere il cambio di posizione
            cc.enabled = false;

            // 2. Spostiamo il player alla destinazione
            other.transform.position = destination.position;

            // Se vuoi che il player guardi nella direzione della destinazione:
            other.transform.rotation = destination.rotation;

            // 3. Riabilitiamo il controller
            cc.enabled = true;

            Debug.Log("Teletrasporto completato!");
        }
    }
}