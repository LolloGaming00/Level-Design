using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    public UniversalDoor portaDaSbloccare;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            portaDaSbloccare.UnlockDoor(); // La porta ora è utilizzabile!
            Destroy(gameObject); // Distruggi il trigger così non si ripete
        }
    }
}