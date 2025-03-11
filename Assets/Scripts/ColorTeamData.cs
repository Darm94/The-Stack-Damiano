using UnityEngine;

public class ColorTeamData : MonoBehaviour
{
    private int _colorTeam; // Variabile privata

    public int ColorTeam // Proprietà pubblica get e set
    {
        get { return _colorTeam; }
        set { _colorTeam = value; }
    }
}
