using System.Collections;
using UnityEngine;
[RequireComponent(typeof(ColorTeamData))]

public class TeamDestroy : MonoBehaviour
{
    private int _teamCollisions = 0;
    private ColorTeamData _myTeamData; 
    private AudioClip _destructionSound; // Destruction AudioClip
    private AudioSource _audioSource;
    void Start()
    {
        _myTeamData = GetComponent<ColorTeamData>();
        //_audioSource = gameObject.AddComponent<AudioSource>(); //not working for now , i had to use camera audiosource
        _destructionSound=Resources.Load<AudioClip>("destroy_sound");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return;//that's a quick fix i should use a status flag (didn't know that OnCollision method still worked when disabled)
        
        // Get components from collided gameObject
        ColorTeamData otherTeamData = collision.gameObject.GetComponent<ColorTeamData>();
        TeamDestroy otherTeamDestroy = collision.gameObject.GetComponent<TeamDestroy>(); // Controllo su TeamDestroy
        
        // Team Collision increment only with same colorTeam objects (and with same component enabled, to avoid destruction durin drag session
        if (enabled && otherTeamData != null && otherTeamDestroy != null && otherTeamDestroy.enabled
            && otherTeamData.ColorTeam == _myTeamData.ColorTeam) // colorTeam match
        {
            _teamCollisions++;
            Debug.Log($"TeamCollisions: {_teamCollisions}");
            if (_teamCollisions >= 2)
            {
                Debug.Log($"SELF DESTRUCTION");
                if (_destructionSound != null && _audioSource != null)
                {
                    Debug.Log($"SOUND DESTRUCTION");
                    //audioSource.PlayOneShot(destructionSound); // Not Working for now, need to applicate sound on camera
                    //get camera audio source
                    _audioSource= Camera.main.gameObject.GetComponent<AudioSource>();
                    _audioSource.PlayOneShot(_destructionSound); 
                }
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!enabled) return;//that's a quick fix i should use a status flag (didn't know that OnCollision method still worked when disabled)
        
        // Get components from collided gameObject
        ColorTeamData otherTeamData = collision.gameObject.GetComponent<ColorTeamData>();
        TeamDestroy otherTeamDestroy = collision.gameObject.GetComponent<TeamDestroy>();

        // Team Collision decrement only with same colorTeam objects (and with same component enabled, to avoid destruction durin drag session
        if (enabled && otherTeamData != null && otherTeamDestroy != null && otherTeamDestroy.enabled 
            && otherTeamData.ColorTeam == _myTeamData.ColorTeam) // match colorTeam
        {
            if(_teamCollisions>0) {_teamCollisions--;}
            Debug.Log($"TeamCollisions: {_teamCollisions}");
        }
    }

    public void DestroyMeAndTeam()
    {
        // Quick solusion from the web and not very precise , probably i had to save colliders before but i'm still not sure of how onCollition exactly work
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4f); // Sphere overlap collider for a rough estimate of nearby colliders

        //Probably thats cost too much than save colliders or components before(?)
        foreach (Collider collider in colliders)
        {
            TeamDestroy teamDestroy = collider.GetComponent<TeamDestroy>();
            ColorTeamData otherTeamData = collider.GetComponent<ColorTeamData>();

            // Verify the object to have the same colorTeam objects (and with same component enabled, to avoid destruction durin drag session
            if ( enabled && teamDestroy != null && teamDestroy.enabled && otherTeamData != null && otherTeamData.ColorTeam == _myTeamData.ColorTeam)
            {
                // Destroying other object before destroying this
                teamDestroy.DestroyMeAndTeam();
            }
        }

        // Quick solusion from the web : Using a coroutine that apply the destroy function at the next frame 
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        // Wait next frame elaboration
        yield return new WaitForEndOfFrame();
        //Disable the gameObject for now
        gameObject.SetActive(false);
        // Real Destruction
        Destroy(gameObject);
    }
    
}
