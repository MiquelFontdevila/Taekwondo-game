using UnityEngine;

public enum Estat { inici, tornJugador, tornRival, resolucioTorn, victoria, empat, derrota}

public class Sistemadebatalla : MonoBehaviour
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
        lluitadorRival = rivalGO.GetComponent<Lluitador>();

        lluitadorJugador.posicio = 3;
        lluitadorRival.posicio = 4;

                
        lluitadorJugador.energia = 5;
        lluitadorRival.energia = 5;


        ActualitzarPosicions();
        
        torn = 0;
        estat = Estat.tornJugador;

        //Mira que el combat funciona_
        Debug.Log("=== COMENÇA EL COMBAT ===");
        Debug.Log("Jugador: " + lluitadorJugador.name);
        Debug.Log("Rival: " + lluitadorRival.name);
        Debug.Log("Posicio Jugador: " + lluitadorJugador.posicio);
        Debug.Log("Posicio Rival: " + lluitadorRival.posicio);
        Debug.Log("Torn preparat: " + (torn + 1));
    }

    void TornRival()
    {
        bool energiaSuficient = false;
        while (energiaSuficient == false)
        {
            int numero = Random.Range(1, 7);
            switch (numero)
            {
                case 1:
                    if (lluitadorRival.energia < 3)
                    {
                        energiaSuficient = false;
                    } else
                    {
                        accioRival = "patadaAlta";
                        energiaSuficient = true;
                    }
                    break;
                case 2:
                    if (lluitadorRival.energia < 1)
                    {
                        energiaSuficient = false;
                    } else
                    {
                        accioRival = "patadaAlCos";
                        energiaSuficient = true;
                    }
                    break;
                case 3:
                    if (lluitadorRival.energia < 2)
                    {
                        energiaSuficient = false;
                    } else
                    {
                        accioRival = "empenta";
                        energiaSuficient = true;
                    }
                    break;
                case 4:
                    accioRival = "bloqueigAlt";
                    energiaSuficient = true;
                    break;
                case 5:
                    accioRival = "bloqueigAlCos";
                    energiaSuficient = true;
                    break;
                case 6:
                    accioRival = "esquivar";
                    energiaSuficient = true;
                    break;
                default:
                    break;
            }
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

            case "patadaAlCos":
                lluitadorJugador.DisminuirEnergia(1);
                if (accioRival == "bloqueigAlCos" || accioRival == "esquivar")
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
                if (accioRival == "patadaAlta" || accioRival == "patadaAlCos")
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

            case "bloqueigAlCos":
                lluitadorJugador.AugmentarEnergia(1);
                if (accioRival == "patadaAlCos")
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

            case "patadaAlCos":
                lluitadorRival.DisminuirEnergia(1);
                if (accioJugador == "bloqueigAlCos" || accioJugador == "esquivar")
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
                if (accioJugador == "patadaAlta" || accioJugador == "patadaAlCos")
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

            case "bloqueigAlCos":
                lluitadorRival.AugmentarEnergia(1);
                if (accioJugador == "patadaAlCos")
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

        if (torn >= 10)
        {

            Debug.Log(" ");
            Debug.Log("========== FINAL DEL COMBAT ==========");
            Debug.Log("Puntuació final del jugador: " + lluitadorJugador.puntuacio);
            Debug.Log("Puntuació final del rival: " + lluitadorRival.puntuacio);

            if (lluitadorJugador.puntuacio > lluitadorRival.puntuacio)
            {
                estat = Estat.victoria;
            } else if (lluitadorJugador.puntuacio == lluitadorRival.puntuacio)
            {
                estat = Estat.empat;
            }
            else
            {
                estat = Estat.derrota;
            }

            // FiDelJoc();
        }
        else
        {
            accioJugador = "";
            estat = Estat.tornJugador;
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
        if (lluitadorJugador.energia < 3)
        {
            Debug.Log("Energia insuficient");
            return;
        }
        accioJugador = "patadaAlta";
        ConfirmarAccioJugador();
        return;
    }
    public void EnBotoPatadaAlCos(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        if (lluitadorJugador.energia < 1)
        {
            Debug.Log("Energia insuficient");
            return;
        }
        accioJugador = "patadaAlCos";
        ConfirmarAccioJugador();
        return;
    }
    public void EnBotoEmpenta(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        if (lluitadorJugador.energia < 2)
        {
            Debug.Log("Energia insuficient");
            return;
        }
        accioJugador = "empenta";
        ConfirmarAccioJugador();
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
        accioJugador = "bloqueigAlt";
        ConfirmarAccioJugador();
        return;
    }
    public void EnBotoBloqueigAlCos(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "bloqueigAlCos";
        ConfirmarAccioJugador();
        return;
    }
    public void EnBotoEsquivar(){
        if (estat != Estat.tornJugador)
        {
            return;
        }
        accioJugador = "esquivar";
        ConfirmarAccioJugador();
        return;
    }
    
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




    //Prova de funcionament elimina

    // private bool esperant = false;
    // void Update()
    // {
    //     if (estat != Estat.tornJugador || esperant)
    //     {s
    //         return;
    //     }

    //     StartCoroutine(EsperarAbansDeJugar());
    // }

    // System.Collections.IEnumerator EsperarAbansDeJugar()
    // {
    //     esperant = true;

    //     // Esperar 2 segons
    //     yield return new WaitForSeconds(2f);

    //     // Comprovar que encara és el torn del jugador
    //     if (estat == Estat.tornJugador)
    //     {
    //         // Triar una acció aleatòria per provar el combat
    //         int numero = Random.Range(1, 7);

    //         switch (numero)
    //         {
    //             case 1:
    //                 accioJugador = "patadaAlta";
    //                 break;
    //             case 2:
    //                 accioJugador = "patadaAlCos";
    //                 break;
    //             case 3:
    //                 accioJugador = "empenta";
    //                 break;
    //             case 4:
    //                 accioJugador = "bloqueigAlt";
    //                 break;
    //             case 5:
    //                 accioJugador = "bloqueigAlCos";
    //                 break;
    //             case 6:
    //                 accioJugador = "esquivar";
    //                 break;
    //         }

    //         ConfirmarAccioJugador();
    //     }

    //     esperant = false;
    // }
}
