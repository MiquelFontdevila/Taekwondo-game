using UnityEngine;

public enum Estat { inici, turnJugadors, turnEnemic, victoria, derrota}

public class Sistemadebatalla : MonoBehaviour
{

    public Estat estat;
    public int turn;

    public GameObject jugadorPrefab;
    public GameObject rivalPrefab;

    public Transform SpawnDelJugador;
    public Transform SpawnDelRival;
    
    // private Lliutador llutadorJugador;
    // private Lliutador llutadorRival;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        estat = Estat.inici;
        SetupBatalla();
    }

    void SetupBatalla()
    {
        GameObject jugadorGO = Instantiate(jugadorPrefab, SpawnDelJugador);
        GameObject rivalGO = Instantiate(rivalPrefab, SpawnDelRival);

        // llutadorJugador = jugadorGO.GetComponent<Lluitador>();
        // llutadorRival = jugadorGO.GetComponent<Lluitador>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
