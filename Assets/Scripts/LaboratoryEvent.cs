using UnityEngine;
using System.Collections;

public class LaboratoryEvent : MonoBehaviour
{
    [Header("Riferimenti")]
    public AudioSource sorgenteAudio;
    public Transform portaTarget; // Il punto verso cui il giocatore si volterà
    public PlayerMovement playerMovement;

    [Header("Dialogo")]
    public string testoDialogo = "Cos'era quel rumore dietro la porta?";

    private bool eventoAttivato = false;

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se è il giocatore e se l'evento non è già successo
        if (other.CompareTag("Player") && !eventoAttivato)
        {
            eventoAttivato = true;
            StartCoroutine(SequenzaPorta());
        }
    }

    private IEnumerator SequenzaPorta()
    {
        // 1. Blocca il movimento (opzionale, dipende dal tuo script)
        playerMovement.enabled = false;

        // 2. Fai partire il suono dietro la porta
        if (sorgenteAudio != null)
        {
            sorgenteAudio.Play();
        }

        // 3. Ruota il giocatore verso la porta (usiamo la funzione fluida che abbiamo già visto)
        yield return StartCoroutine(RuotaFluida(portaTarget, 0.7f));

        // 4. Mostra il dialogo
        DialogueUI.Instance.ShowDialogue(testoDialogo);

        // Aspetta che il dialogo finisca
        yield return new WaitUntil(() => !DialogueUI.Instance.gameObject.activeSelf);

        // 5. Sblocca il giocatore
        playerMovement.enabled = true;
    }

    private IEnumerator RuotaFluida(Transform target, float durata)
    {
        GameObject player = GameObject.FindWithTag("Player");
        float tempo = 0;
        Quaternion rotIniziale = player.transform.rotation;

        Vector3 direzione = target.position - player.transform.position;
        direzione.y = 0;
        Quaternion rotFinale = Quaternion.LookRotation(direzione);

        while (tempo < durata)
        {
            player.transform.rotation = Quaternion.Slerp(rotIniziale, rotFinale, tempo / durata);
            tempo += Time.deltaTime;
            yield return null;
        }

        player.transform.rotation = rotFinale;
        // Sincronizziamo il mouse per evitare scatti
        playerMovement.ResetRotationFromCurrentOrientation();
    }
}
