using UnityEngine;
using System.Collections;

public class SpecialEventInteractable : MonoBehaviour, IInteractable
{
    [Header("Impostazioni Evento")]
    public GameObject npcPrefab;
    public Transform npcSpawnPoint;
    public string initialDialogue = "C'è qualcuno dietro di me...";

    [Header("Riferimenti")]
    public UniversalDoor doorToRelock;
    public Transform teleportTarget;
    public GameObject blackScreen;
    // Riferimento specifico al movimento per resettare gli angoli
    public PlayerMovement playerMovement;

    private bool eventTriggered = false;

    public void Interact()
    {
        if (!eventTriggered)
        {
            StartCoroutine(SequenzaCinematica());
            eventTriggered = true;
        }
    }

    private IEnumerator SequenzaCinematica()
    {
        // 1. Appare l'NPC
        GameObject npc = Instantiate(npcPrefab, npcSpawnPoint.position, npcSpawnPoint.rotation);

        // 2. Forza lo sguardo ruotando Corpo e Testa
        yield return StartCoroutine(RuotaFluidaVerso(npc.transform, 0.3f));

        // 3. Inizia il dialogo (ricorda di aggiungere true per tenere il nero!)
        EntityIdentity npcId = npc.GetComponent<EntityIdentity>();
        DialogueUI.Instance.ShowDialogue(initialDialogue, npcId != null ? npcId.entityPortrait : null, true);

        yield return new WaitUntil(() => !DialogueUI.Instance.gameObject.activeSelf);

        // 4. Schermo Nero
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(1f);

        // 5. Teletrasporto e Reset Porta
        TeletrasportaPlayer();
        doorToRelock.isLocked = true;
        doorToRelock.transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(1f);
        blackScreen.SetActive(false);
    }

    private IEnumerator RuotaFluidaVerso(Transform target, float durata)
    {
        float altezzaOcchiNpc = 1.6f;
        GameObject player = GameObject.FindWithTag("Player");
        Camera mainCam = Camera.main;
        float tempoTrascorso = 0;

        // --- DATI PER IL CORPO (Asse Y) ---
        Quaternion rotBodyIniziale = player.transform.rotation;
        Vector3 direzioneCorpo = target.position - player.transform.position;
        direzioneCorpo.y = 0;
        Quaternion rotBodyFinale = Quaternion.LookRotation(direzioneCorpo);

        // --- DATI PER LA CAMERA (Asse X) ---
        // Calcoliamo l'angolo verticale verso l'NPC
        Vector3 posizioneFacciaNpc = target.position + (Vector3.up * altezzaOcchiNpc);
        Vector3 direzioneCamera = posizioneFacciaNpc - mainCam.transform.position;
        Quaternion rotCamTarget = Quaternion.LookRotation(direzioneCamera);

        // Estraiamo l'angolo X (pitch). Usiamo eulerAngles e lo normalizziamo
        float pitchFinale = rotCamTarget.eulerAngles.x;
        if (pitchFinale > 180) pitchFinale -= 360;

        // Prendiamo l'angolo X attuale della camera (quello che guarda la botola)
        float pitchIniziale = mainCam.transform.localEulerAngles.x;
        if (pitchIniziale > 180) pitchIniziale -= 360;

        while (tempoTrascorso < durata)
        {
            float t = tempoTrascorso / durata;
            // Rendiamo il movimento più "morbido" con un SmoothStep
            float smoothT = t * t * (3f - 2f * t);

            // 1. Ruota il corpo (Sinistra/Destra)
            player.transform.rotation = Quaternion.Slerp(rotBodyIniziale, rotBodyFinale, smoothT);

            // 2. Ruota la testa (Alto/Basso)
            float pitchCorrente = Mathf.Lerp(pitchIniziale, pitchFinale, smoothT);
            mainCam.transform.localRotation = Quaternion.Euler(pitchCorrente, 0, 0);

            tempoTrascorso += Time.deltaTime;
            yield return null;
        }

        // Fix finale per precisione
        player.transform.rotation = rotBodyFinale;
        mainCam.transform.localRotation = Quaternion.Euler(pitchFinale, 0, 0);

        // 3. SINCRONIZZAZIONE FINALE
        // Diciamo al PlayerMovement che ora siamo allineati
        playerMovement.ResetRotationFromCurrentOrientation();
    }

    private void TeletrasportaPlayer()
    {
        CharacterController cc = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
        cc.enabled = false;
        cc.transform.position = teleportTarget.position;
        cc.enabled = true;
    }
}