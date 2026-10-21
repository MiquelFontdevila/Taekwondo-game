using UnityEngine;

public enum Estat { inici, tornJugador, tornRival, resolucioTorn, victoria, derrota}

//codi basat en // https://www.youtube.com/watch?v=_1pz_ohupPs

//en la batalla, posar 6 "punts de spawn" per la posicio, en canviar la posicio puc spawnear el jugador i el rival en aquella posicio

public class Sistemadebatalla : MonoBehaviour
{

    public Estat estat;
    public int torn;

    public GameObject jugadorPrefab;
    public GameObject rivalPrefab;

    public Transform SpawnDelJugador;
    public Transform SpawnDelRival;
    
    public Lluitador lluitadorJugador;
    public Lluitador lluitadorRival;

    public string accioJugador;
    public string accioRival;


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

        lluitadorJugador = jugadorGO.GetComponent<Lluitador>();
        lluitadorRival = jugadorGO.GetComponent<Lluitador>();
        
        torn = 0;
        if (torn > 10)
        {
            if (lluitadorJugador.puntuacio > lluitadorRival.puntuacio)
            {
                estat = Estat.victoria;
            } else
            {
                estat = Estat.derrota;
            }

            // FiDelJoc();
        }
        estat = Estat.tornJugador;
        TornJugador();
    }

    void TornJugador()
    {
        torn =+ 1;
        //fer que vaguis a menus de eleguir moviments etc...

        estat = Estat.tornRival;
        TornRival();
    }

    void TornRival()
    {
        int numero = Random.Range(1, 7);
        switch (numero)
        {
            case 1:
                accioRival = "patadaAlta";
                break;
            case 2:
                accioRival = "patadaAlCos";
                break;
            case 3:
                accioRival = "empenta";
                break;
            case 4:
                accioRival = "BloqueigAlt";
                break;
            case 5:
                accioRival = "bloqueigAlCos";
                break;
            case 6:
                accioRival = "esquivar";
                break;
            default:
                break;
        }
        estat = Estat.resolucioTorn;
        resolucioTorn();
    }
    
    public void resolucioTorn() //TODO________________________________________________________________________________________________
    {
        switch (accioJugador)
        {
            case "patadaAlta":
                if (accioRival == "bloqueigAlCos" || accioRival == "empenta")
                {
                    
                } else if (accioRival == "BloqueigAlt" || accioRival == "esquivar")
                {
                    
                }
                else
                {
                    
                }
                break;

            case "patadaAlCos":
                break;

            case "empenta":
                break;

            case "BloqueigAlt":
                break;

            case "bloqueigAlCos":
                break;

            case "esquivar":
                break;

            default:
                break;
        }
    }

    public void EnBotoAtac(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        //deshabilitar menu de atac/defensa
        //habilitar menu d'atac
    }
    public void EnBotoPatadaAlta(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "patadaAlta";
        return;
    }
    public void EnBotoPatadaAlCos(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "patadaAlCos";
        return;
    }
    public void EnBotoEmpenta(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "empenta";
        return;
    }


    public void EnBotoDefensa(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        //deshabilitar menu de atac/defensa
        //habilitar menu de defensa
    }
    public void EnBotoBloqueigAlt(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "BloqueigAlt";
        return;
    }
    public void EnBotoBloqueigAlCos(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "bloqueigAlCos";
        return;
    }
    public void EnBotoEsquivar(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "esquivar";
        return;
    }
}
