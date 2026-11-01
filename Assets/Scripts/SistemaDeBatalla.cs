using UnityEngine;
using System.Collections;

public enum Estat { inici, tornJugador, tornRival, resolucioTorn, victoria, empat, derrota }

//codi basat en // https://www.youtube.com/watch?v=_1pz_ohupPs

//en la batalla, posar 6 "punts de spawn" per la posicio, en canviar la posicio puc spawnear el jugador i el rival en aquella posicio

public class SistemaDeBatalla : MonoBehaviour
{

    public Estat estat;
    public int torn;

    public GameObject jugadorPrefab;
    public GameObject rivalPrefab;

    public Transform SpawnDelJugador;
    public Transform SpawnDelRival;

    public Transform Posicio1;
    public Transform Posicio2;
    public Transform Posicio3;
    public Transform Posicio4;
    public Transform Posicio5;
    public Transform Posicio6;

    public Lluitador lluitadorJugador;
    public Lluitador lluitadorRival;

    public string accioJugador;
    public string accioRival;

    public CombatUIManager combatUI;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        estat = Estat.inici;
        SetupBatalla();
    }

    // CONFIGURACIÓ DE LA BATALLA

    void SetupBatalla()
    {
        GameObject jugadorGO = Instantiate(jugadorPrefab, SpawnDelJugador);
        GameObject rivalGO = Instantiate(rivalPrefab, SpawnDelRival);

        lluitadorJugador = jugadorGO.GetComponent<Lluitador>();
        lluitadorRival = rivalGO.GetComponent<Lluitador>();

        lluitadorJugador.posicio = 3;
        lluitadorRival.posicio = 4;


        lluitadorJugador.energia = 5;
        lluitadorRival.energia = 5;


        ActualitzarPosicions();

        torn = 0;
        estat = Estat.tornJugador;

        combatUI.Inicialitzar(lluitadorJugador);

        //Mira que el combat funciona__________________________________________________________________________________
        Debug.Log("=== COMENÇA EL COMBAT ===");
        Debug.Log("Jugador: " + lluitadorJugador.name);
        Debug.Log("Rival: " + lluitadorRival.name);
        Debug.Log("Posicio Jugador: " + lluitadorJugador.posicio);
        Debug.Log("Posicio Rival: " + lluitadorRival.posicio);
        Debug.Log("Torn preparat: " + (torn + 1));
    }

    // GESTIó DELS TORNS

    void TornRival()
    {
        int numero = Random.Range(1, 7);
        switch (numero)
        {
            case 1:
                accioRival = "patadaAlta";
                break;
            case 2:
                accioRival = "patadaBaixa";
                break;
            case 3:
                accioRival = "empenta";
                break;
            case 4:
                accioRival = "bloqueigAlt";
                break;
            case 5:
                accioRival = "bloqueigBaix";
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

    public void ConfirmarAccioJugador()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }

        if (string.IsNullOrEmpty(accioJugador))
        {
            return;
        }

        estat = Estat.tornRival;
        TornRival();
    }

    public void resolucioTorn()
    {
        Debug.Log(" ");
        Debug.Log("========== TORN " + (torn + 1) + " ==========");
        Debug.Log("Acció del jugador: " + accioJugador);
        Debug.Log("Acció del rival: " + accioRival);


        switch (accioJugador)
        {
            //Atac
            case "patadaAlta":
                lluitadorJugador.DisminuirEnergia(3);
                if (accioRival == "bloqueigAlt" || accioRival == "esquivar")
                {
                    //no fa res, si que una animacio
                }
                else
                {
                    lluitadorJugador.AugmentarPunts(3);
                }
                break;

            case "patadaBaixa":
                lluitadorJugador.DisminuirEnergia(1);
                if (accioRival == "bloqueigBaix" || accioRival == "esquivar")
                {
                    //no fa res, si que una animacio
                }
                else
                {
                    lluitadorJugador.AugmentarPunts(1);
                }
                break;

            case "empenta":
                lluitadorJugador.DisminuirEnergia(2);
                if (accioRival == "patadaAlta" || accioRival == "patadaBaixa")
                {
                    //no fa res, si que una animacio
                }
                else
                {
                    lluitadorJugador.AugmentarPosicio(1);
                    lluitadorRival.AugmentarPosicio(1);
                }
                break;

            //defensa
            case "bloqueigAlt":
                lluitadorJugador.AugmentarEnergia(1);
                if (accioRival == "patadaAlta")
                {
                    lluitadorJugador.AugmentarEnergia(2);
                }
                break;

            case "bloqueigBaix":
                lluitadorJugador.AugmentarEnergia(1);
                if (accioRival == "patadaBaixa")
                {
                    lluitadorJugador.AugmentarEnergia(2);
                }
                break;

            case "esquivar":
                lluitadorJugador.AugmentarEnergia(2);

                lluitadorJugador.DisminuirPosicio(1);
                lluitadorRival.DisminuirPosicio(1);

                break;
            default:
                break;
        }



        switch (accioRival)
        {
            //Atac
            case "patadaAlta":
                lluitadorRival.DisminuirEnergia(3);
                if (accioJugador == "bloqueigAlt" || accioJugador == "esquivar")
                {
                    //no fa res, si que una animacio
                }
                else
                {
                    lluitadorRival.AugmentarPunts(3);
                }
                break;

            case "patadaBaixa":
                lluitadorRival.DisminuirEnergia(1);
                if (accioJugador == "bloqueigBaix" || accioJugador == "esquivar")
                {
                    //no fa res, si que una animacio
                }
                else
                {
                    lluitadorRival.AugmentarPunts(1);
                }
                break;

            case "empenta":
                lluitadorRival.DisminuirEnergia(2);
                if (accioJugador == "patadaAlta" || accioJugador == "patadaBaixa")
                {
                    //no fa res, si que una animacio
                }
                else
                {
                    lluitadorJugador.DisminuirPosicio(1);
                    lluitadorRival.DisminuirPosicio(1);
                }
                break;

            //defensa
            case "bloqueigAlt":
                lluitadorRival.AugmentarEnergia(1);
                if (accioJugador == "patadaAlta")
                {
                    lluitadorRival.AugmentarEnergia(2);
                }
                break;

            case "bloqueigBaix":
                lluitadorRival.AugmentarEnergia(1);
                if (accioJugador == "patadaBaixa")
                {
                    lluitadorRival.AugmentarEnergia(2);
                }
                break;

            case "esquivar":
                lluitadorRival.AugmentarEnergia(2);

                lluitadorJugador.AugmentarPosicio(1);
                lluitadorRival.AugmentarPosicio(1);

                break;
            default:
                break;
        }

        // Comprovar si algun lluitador surt de la zona de combat
        if (lluitadorJugador.posicio < 1)
        {
            Debug.Log("El jugador ha sortit de la zona! Punt per al rival.");

            lluitadorRival.AugmentarPunts(1);

            lluitadorJugador.posicio = 3;
            lluitadorRival.posicio = 4;
        }
        else if (lluitadorRival.posicio > 6)
        {
            Debug.Log("El rival ha sortit de la zona! Punt per al jugador.");

            lluitadorJugador.AugmentarPunts(1);

            lluitadorJugador.posicio = 3;
            lluitadorRival.posicio = 4;
        }

        ActualitzarPosicions();

        if (lluitadorJugador.energia > 10)
        {
            Debug.Log("Energia maxima es 10 per tant tornem a 10, borrar misatge al acabar el joc. (Jugador)");

            lluitadorJugador.energia = 10;
        }
        if (lluitadorRival.energia > 10)
        {
            Debug.Log("Energia maxima es 10 per tant tornem a 10, borrar misatge al acabar el joc. (Rival)");

            lluitadorRival.energia = 10;
        }
        if (lluitadorJugador.energia < 0)
        {
            Debug.Log("La energia no podra ser menys de 0, es desactivaran botons quan no puguis utilitzar un moviment, de moment la posem a 0, eliminar en producte final(Jugador)");

            lluitadorJugador.energia = 0;
        }
        if (lluitadorRival.energia < 0)
        {
            Debug.Log("La energia no podra ser menys de 0, es desactivaran botons quan no puguis utilitzar un moviment, de moment la posem a 0, eliminar en producte final(Jugador)");

            lluitadorRival.energia = 0;
        }


        Debug.Log("--- RESULTAT DEL TORN ---");
        Debug.Log("Punts jugador: " + lluitadorJugador.puntuacio);
        Debug.Log("Punts rival: " + lluitadorRival.puntuacio);
        Debug.Log("Energia jugador: " + lluitadorJugador.energia);
        Debug.Log("Energia rival: " + lluitadorRival.energia);
        Debug.Log("Posicio Jugador: " + lluitadorJugador.posicio);
        Debug.Log("Posicio Rival: " + lluitadorRival.posicio);

        torn++;

        combatUI.MostrarEstamina(lluitadorJugador.energia);

        if (torn >= 10)
        {

            Debug.Log(" ");
            Debug.Log("========== FINAL DEL COMBAT ==========");
            Debug.Log("Puntuació final del jugador: " + lluitadorJugador.puntuacio);
            Debug.Log("Puntuació final del rival: " + lluitadorRival.puntuacio);

            if (lluitadorJugador.puntuacio > lluitadorRival.puntuacio)
            {
                estat = Estat.victoria;
            }
            else if (lluitadorJugador.puntuacio == lluitadorRival.puntuacio)
            {
                estat = Estat.empat;
            }
            else
            {
                estat = Estat.derrota;
            }

            combatUI.AcabarCombat(estat);
        }
        else
        {
            estat = Estat.tornJugador;
            combatUI.MostrarResultatTorn(accioJugador, accioRival);
        }
    }

    public void AcabarMostrarResultatTorn()
    {
        if (estat == Estat.resolucioTorn)
        {
            accioJugador = "";
            estat = Estat.tornJugador;
        }
    }

    // BOTONS
    public void EnBotoAtac()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }
        // UI de selecció de mode deshabilitada a CombatUIManager
    }
    public void EnBotoPatadaAlta()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "patadaAlta";
        ConfirmarAccioJugador();
    }
    public void EnBotoPatadaBaixa()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "patadaBaixa";
        ConfirmarAccioJugador();
    }
    public void EnBotoEmpenta()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "empenta";
        ConfirmarAccioJugador();
    }


    public void EnBotoDefensa()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }
        // UI de selecció de mode deshabilitada a CombatUIManager
    }
    public void EnBotoBloqueigAlt()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "bloqueigAlt";
        ConfirmarAccioJugador();
    }
    public void EnBotoBloqueigBaix()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "bloqueigBaix";
        ConfirmarAccioJugador();
    }
    public void EnBotoEsquivar()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "esquivar";
        ConfirmarAccioJugador();
    }

    // MOVIMENT AL CAMP DE BATALLA
    void ActualitzarPosicions()
    {
        Transform[] posicions =
        {
            Posicio1,
            Posicio2,
            Posicio3,
            Posicio4,
            Posicio5,
            Posicio6
        };

        // Moure el jugador
        int posJugador = lluitadorJugador.posicio;

        if (posJugador >= 1 && posJugador <= 6)
        {
            lluitadorJugador.transform.position =
                posicions[posJugador - 1].position;
        }

        // Moure el rival
        int posRival = lluitadorRival.posicio;

        if (posRival >= 1 && posRival <= 6)
        {
            lluitadorRival.transform.position =
                posicions[posRival - 1].position;
        }
    }




    //Prova de funcionament elimina_______________________________________________________________________

    private bool esperant = false;
    void Update()
    {
        if (estat != Estat.tornJugador || esperant)
        {
            return;
        }

        // StartCoroutine(EsperarAbansDeJugar());
    }

    IEnumerator EsperarAbansDeJugar()
    {
        esperant = true;

        // Esperar 2 segons
        yield return new WaitForSeconds(2f);

        // Comprovar que encara és el torn del jugador
        if (estat == Estat.tornJugador)
        {
            // Triar una acció aleatòria per provar el combat
            int numero = Random.Range(1, 7);

            switch (numero)
            {
                case 1:
                    accioJugador = "patadaAlta";
                    break;
                case 2:
                    accioJugador = "patadaBaixa";
                    break;
                case 3:
                    accioJugador = "empenta";
                    break;
                case 4:
                    accioJugador = "bloqueigAlt";
                    break;
                case 5:
                    accioJugador = "bloqueigBaix";
                    break;
                case 6:
                    accioJugador = "esquivar";
                    break;
            }

            ConfirmarAccioJugador();
        }

        esperant = false;
    }
}
