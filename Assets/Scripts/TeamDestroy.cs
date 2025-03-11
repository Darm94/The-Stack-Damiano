using System.Collections;
using UnityEngine;
[RequireComponent(typeof(ColorTeamData))]

public class TeamDestroy : MonoBehaviour
{
    private int _teamCollisions = 0;
    private ColorTeamData _myTeamData; 
    public AudioClip destructionSound; // AudioClip da riprodurre
    private AudioSource audioSource; // Riferimento all'AudioSource
    void Start()
    {
        _myTeamData = GetComponent<ColorTeamData>();
        audioSource = gameObject.AddComponent<AudioSource>();
        destructionSound=Resources.Load<AudioClip>("destroy_sound");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return;
        
        // Ottieni il componente ColorTeamData dall'oggetto con cui è avvenuta la collisione
        ColorTeamData otherTeamData = collision.gameObject.GetComponent<ColorTeamData>();
        TeamDestroy otherTeamDestroy = collision.gameObject.GetComponent<TeamDestroy>(); // Controllo su TeamDestroy
        
        // Verifica se il componente ColorTeamData esiste
        if (otherTeamData != null && otherTeamDestroy != null && otherTeamDestroy.enabled
                                     && otherTeamData.ColorTeam == _myTeamData.ColorTeam) // Confronta i colorTeam
        {
            _teamCollisions++;
            Debug.Log($"TeamCollisions: {_teamCollisions}");
            if (_teamCollisions >= 2)
            {
                Debug.Log($"SELF DESTRUCTION");
                if (destructionSound != null && audioSource != null)
                {
                    Debug.Log($"SOUND DESTRUCTION");
                    //audioSource.PlayOneShot(destructionSound); // Not Working for now, need to applicate sound on camera
                    //get camera audio source
                    audioSource= Camera.main.gameObject.GetComponent<AudioSource>();
                    audioSource.PlayOneShot(destructionSound); 
                }
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!enabled) return;
        // Ottieni il componente ColorTeamData dall'oggetto con cui è avvenuta la collisione
        ColorTeamData otherTeamData = collision.gameObject.GetComponent<ColorTeamData>();
        TeamDestroy otherTeamDestroy = collision.gameObject.GetComponent<TeamDestroy>();

        // Verifica se il componente ColorTeamData esiste
        if (otherTeamData != null && otherTeamDestroy != null && otherTeamDestroy.enabled && otherTeamData.ColorTeam == _myTeamData.ColorTeam) // Confronta i colorTeam
        {
            if(_teamCollisions>=0) {_teamCollisions--;}
            Debug.Log($"TeamCollisions: {_teamCollisions}");
        }
    }

    public void DestroyMeAndTeam()
    {
        // Trova tutti gli oggetti con cui questo oggetto era in collisione
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f); // Usa una sfera di sovrapposizione per trovare i collider vicini

        foreach (Collider collider in colliders)
        {
            TeamDestroy teamDestroy = collider.GetComponent<TeamDestroy>();
            ColorTeamData otherTeamData = collider.GetComponent<ColorTeamData>();

            // Verifica se il componente ColorTeamData esiste, il team è lo stesso e il TeamDestroy è attivo
            if (teamDestroy != null && teamDestroy.enabled && otherTeamData != null && otherTeamData.ColorTeam == _myTeamData.ColorTeam)
            {
                // Distruggi l'altro oggetto prima di distruggere questo
                teamDestroy.DestroyMeAndTeam();
            }
        }

        // Esegui il destroy al prossimo frame (o dopo un certo tempo)
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        // Attendere fino al prossimo frame (o personalizzare il tempo di attesa)
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
        // Ora distruggi l'oggetto
        Destroy(gameObject);
    }
    
}
