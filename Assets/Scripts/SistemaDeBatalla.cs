using UnityEngine;

public enum Estat { inici, tornJugador, tornRival, resolucioTorn, victoria, empat, derrota }

public class SistemaDeBatalla : MonoBehaviour
{
    // Costos d'energia dels atacs
    private const int COST_PATADA_ALTA = 3;
    private const int COST_PATADA_AL_COS = 1;
    private const int COST_EMPENTA = 2;

    public Estat estat;
    public int torn;

    public Transform Posicio1;
    public Transform Posicio2;
    public Transform Posicio3;
    public Transform Posicio4;
    public Transform Posicio5;
    public Transform Posicio6;

    // Arrossega aquí els lluitadors que ja estan a l'escena (des de la Hierarchy)
    public Lluitador lluitadorJugador;
    public Lluitador lluitadorRival;

    public string accioJugador;
    public string accioRival;

    public CombatUIManager combatUI;

    public int tornsMaxims = 10;

    // Animators dels lluitadors
    private Animator animatorJugador;
    private Animator animatorRival;

    // Temps que dura l'animació d'una acció (estats 1-6) abans de passar al següent estat
    public float duradaAccio = 0.5f;

    // Temps que dura l'animació de cop rebut (estat 7) abans de tornar a l'estat 0
    public float duradaCop = 0.5f;

    // Temps màxim d'espera per animació (seguretat): normalment l'script detecta quan acaba l'animació,
    // però si alguna no acaba mai (per exemple, una transició mal posada) no es queda bloquejat
    public float duradaAnimacio = 3f;
    private Coroutine resetAnimacio;

    private AudioManager audioManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        animatorJugador = lluitadorJugador.GetComponentInChildren<Animator>();
        animatorRival = lluitadorRival.GetComponentInChildren<Animator>();

        estat = Estat.inici;
        SetupBatalla();
    }
    void LateUpdate()
    {
        ActualitzarPosicions();
    }

    // CONFIGURACIÓ DE LA BATALLA

    void SetupBatalla()
    {
        lluitadorJugador.posicio = 3;
        lluitadorRival.posicio = 4;

        lluitadorJugador.energia = 5;
        lluitadorRival.energia = 5;

        ActualitzarPosicions();

        torn = 0;
        estat = Estat.tornJugador;

        // Inici de torn: animacions a l'estat 0
        PosarEstatAnimator(animatorJugador, 0);
        PosarEstatAnimator(animatorRival, 0);

        combatUI.Inicialitzar(lluitadorJugador);

        //Mira que el combat funciona__________________________________________________________________________________
        Debug.Log("=== COMENÇA EL COMBAT ===");
        Debug.Log("Jugador: " + lluitadorJugador.name);
        Debug.Log("Rival: " + lluitadorRival.name);
        Debug.Log("Posicio Jugador: " + lluitadorJugador.posicio);
        Debug.Log("Posicio Rival: " + lluitadorRival.posicio);
        Debug.Log("Torn preparat: " + (torn + 1));
    }

    // GESTIÓ DELS TORNS

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
                    }
                    else
                    {
                        accioRival = "patadaAlta";
                        energiaSuficient = true;
                    }
                    break;
                case 2:
                    if (lluitadorRival.energia < 1)
                    {
                        energiaSuficient = false;
                    }
                    else
                    {
                        accioRival = "patadaAlCos";
                        energiaSuficient = true;
                    }
                    break;
                case 3:
                    if (lluitadorRival.energia < 2)
                    {
                        energiaSuficient = false;
                    }
                    else
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

        // --- SONS ---
        ReproduirSonsAccio(accioJugador);
        ReproduirSonsAccio(accioRival);
        ReproduirSonsResultatPatada(accioJugador, accioRival);
        ReproduirSonsResultatPatada(accioRival, accioJugador);

        // --- ANIMACIONS ---
        if (resetAnimacio != null)
        {
            StopCoroutine(resetAnimacio);
            resetAnimacio = null;
        }
        // Si l'altre esquiva, la patada (alta o al cos) no es fa: es queda a l'estat 0
        int estatAnimJugador = EstatAnimacio(accioJugador);
        int estatAnimRival = EstatAnimacio(accioRival);

        if (accioRival == "esquivar" && (accioJugador == "patadaAlta" || accioJugador == "patadaAlCos"))
        {
            estatAnimJugador = 0;
        }
        if (accioJugador == "esquivar" && (accioRival == "patadaAlta" || accioRival == "patadaAlCos"))
        {
            estatAnimRival = 0;
        }

        PosarEstatAnimator(animatorJugador, estatAnimJugador);
        PosarEstatAnimator(animatorRival, estatAnimRival);

        // Qui ha rebut una patada que no ha bloquejat ni esquivat (estat 7 després de l'animació de l'acció)
        bool jugadorRebCop = PatadaImpacta(accioRival, accioJugador);
        bool rivalRebCop = PatadaImpacta(accioJugador, accioRival);

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
            Debug.Log("La energia no podra ser menys de 0, es desactivaran botons quan no puguis utilitzar un moviment, de moment la posem a 0, eliminar en producte final(Rival)");

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

        if (torn >= tornsMaxims)
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

            // Animacions finals (8 = victòria, 9 = derrota/empat)
            switch (estat)
            {
                case Estat.victoria:
                    PosarEstatAnimator(animatorJugador, 8);
                    PosarEstatAnimator(animatorRival, 9);
                    break;
                case Estat.derrota:
                    PosarEstatAnimator(animatorJugador, 9);
                    PosarEstatAnimator(animatorRival, 8);
                    break;
                case Estat.empat:
                    PosarEstatAnimator(animatorJugador, 9);
                    PosarEstatAnimator(animatorRival, 9);
                    break;
            }

            combatUI.AcabarCombat(estat);
        }
        else
        {
            estat = Estat.tornJugador;
            combatUI.MostrarResultatTorn(accioJugador, accioRival);

            // Netejar l'acció del jugador per al proper torn (després d'avisar la UI)
            accioJugador = "";

            // Inici del següent torn: passats duradaAccio segons es posa l'estat 7 a qui ha rebut cop
            // i després els estats tornen a 0
            resetAnimacio = StartCoroutine(TornarAIdle(jugadorRebCop, rivalRebCop));
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

    // ANIMACIONS

    // Valor de l'int "estat" de l'Animator per a cada acció
    // 1 = patada alta, 2 = patada al cos, 3 = empenta, 4 = bloqueig alt, 5 = bloqueig al cos, 6 = esquivar
    // 7 = cop rebut, 8 = victòria, 9 = derrota/empat
    int EstatAnimacio(string accio)
    {
        switch (accio)
        {
            case "patadaAlta":    return 1;
            case "patadaAlCos":   return 2;
            case "empenta":       return 3;
            case "bloqueigAlt":   return 4;
            case "bloqueigAlCos": return 5;
            case "esquivar":      return 6;
            default:              return 0;
        }
    }

    // Retorna true si la patada de l'atacant arriba al defensor (no bloquejada ni esquivada)
    bool PatadaImpacta(string atacant, string defensor)
    {
        if (atacant == "patadaAlta")
            return defensor != "bloqueigAlt" && defensor != "esquivar";

        if (atacant == "patadaAlCos")
            return defensor != "bloqueigAlCos" && defensor != "esquivar";

        return false;
    }

    void PosarEstatAnimator(Animator animator, int valor)
    {
        if (animator == null) return;
        animator.SetInteger("estat", valor);
    }

    // Espera duradaAccio segons (animació de l'acció, estats 1-6), posa l'estat 7 a qui ha rebut cop,
    // espera que acabi aquesta animació i finalment torna els dos lluitadors a l'estat 0
    System.Collections.IEnumerator TornarAIdle(bool jugadorRebCop, bool rivalRebCop)
    {
        // 1) Animació de l'acció (estats 1-6): dura un temps fix
        yield return new WaitForSeconds(duradaAccio);

        // 2) Animació de cop rebut (estat 7), només per a qui ha rebut la patada
        if (jugadorRebCop) PosarEstatAnimator(animatorJugador, 7);
        if (rivalRebCop) PosarEstatAnimator(animatorRival, 7);

        // L'estat 7 dura duradaCop segons abans de tornar a l'estat 0
        if (jugadorRebCop || rivalRebCop) yield return new WaitForSeconds(duradaCop);

        // 3) Tornem a l'estat 0 (idle)
        PosarEstatAnimator(animatorJugador, 0);
        PosarEstatAnimator(animatorRival, 0);
        resetAnimacio = null;
    }

    // Espera que l'animació que s'està reproduint acabi una vegada (normalizedTime >= 1)
    System.Collections.IEnumerator EsperarFiAnimacio(Animator animator)
    {
        if (animator == null) yield break;

        // Esperem un frame perquè l'Animator processi el canvi d'estat
        yield return null;
        float temps = Time.deltaTime;

        while (temps < duradaAnimacio)
        {
            // Mentre hi ha transició entre animacions, seguim esperant
            if (!animator.IsInTransition(0) &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                yield break;
            }

            temps += Time.deltaTime;
            yield return null;
        }
    }

    // SONS

    // So de cada acció (només depèn de l'acció feta)
    void ReproduirSonsAccio(string accio)
    {
        if (audioManager == null) return;

        switch (accio)
        {
            case "patadaAlta":
                audioManager.PlaySFX(audioManager.Veus_Cops_Alts);
                audioManager.PlaySFX(audioManager.Atac_1);
                break;

            case "patadaAlCos":
            case "empenta":
                audioManager.PlaySFX(audioManager.Veus_Cops_Baixos);
                audioManager.PlaySFX(audioManager.Atac_2);
                break;

            case "bloqueigAlt":
            case "bloqueigAlCos":
                audioManager.PlaySFX(audioManager.Veus_Bloqueig);
                break;

            case "esquivar":
                audioManager.PlaySFX(audioManager.Esquiva);
                break;
        }
    }

    // So del resultat d'una patada: bloquejada/esquivada o punt (amb 1 segon de retard)
    void ReproduirSonsResultatPatada(string atacant, string defensor)
    {
        if (audioManager == null) return;

        bool esPatadaAlta = (atacant == "patadaAlta");
        bool esPatadaAlCos = (atacant == "patadaAlCos");

        // Només les patades fan aquests sons (l'empenta no)
        if (!esPatadaAlta && !esPatadaAlCos) return;

        bool bloquejadaOEsquivada =
            (esPatadaAlta && defensor == "bloqueigAlt") ||
            (esPatadaAlCos && defensor == "bloqueigAlCos") ||
            defensor == "esquivar";

        if (bloquejadaOEsquivada)
        {
            audioManager.PlaySFX(audioManager.Bloqueig);
        }
        else if (esPatadaAlta)
        {
            audioManager.PlaySFXDelayed(audioManager.Veus_Rebuts_Alts, 1f);
        }
        else
        {
            audioManager.PlaySFXDelayed(audioManager.Veus_Rebuts_Baixos, 1f);
        }
    }

    // BOTONS

    // Comprova que el jugador tingui prou energia per fer un atac
    private bool TeEnergiaSuficient(int cost)
    {
        if (lluitadorJugador.energia < cost)
        {
            Debug.Log("Energia insuficient");
            return false;
        }
        return true;
    }

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
        if (!TeEnergiaSuficient(COST_PATADA_ALTA))
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
        if (!TeEnergiaSuficient(COST_PATADA_AL_COS))
        {
            return;
        }
        accioJugador = "patadaAlCos";
        ConfirmarAccioJugador();
    }
    public void EnBotoEmpenta()
    {
        if (estat != Estat.tornJugador)
        {
            return;
        }
        if (!TeEnergiaSuficient(COST_EMPENTA))
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
        accioJugador = "bloqueigAlCos";
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
}