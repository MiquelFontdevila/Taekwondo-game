using UnityEngine;

public enum Estat
{
    inici,
    tornJugador,
    tornRival,
    resolucioTorn,
    victoria,
    empat,
    derrota,
}

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

    // Temps que dura l'animació de cop rebut (estat 7) abans de continuar
    public float duradaCop = 0.5f;

    // Temps que dura l'animació de sortir del ring (estat 10) abans de continuar
    public float duradaFora = 0.5f;

    // Temps màxim d'espera per animació (seguretat): normalment l'script detecta quan acaba l'animació,
    // però si alguna no acaba mai (per exemple, una transició mal posada) no es queda bloquejat
    public float duradaAnimacio = 3f;
    private Coroutine resetAnimacio;

    // Posicions finals del torn: es calculen a resolucioTorn però NO es mostren fins que
    // acaben totes les animacions (si algú ha sortit del ring ja són 3 i 4)
    private bool posicionsPendents = false;
    private int posPendentJugador;
    private int posPendentRival;

    private AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Si és true, el combat comença sol a Start (escenes sense selector de lluitador).
    // A Level1Scene es posa a false: el SeleccioLluitador crida IniciarBatalla quan el jugador confirma.
    public bool iniciarAutomaticament = true;

    void Start()
    {
        GameObject audioGO = GameObject.FindGameObjectWithTag("Audio");
        if (audioGO != null)
        {
            audioManager = audioGO.GetComponent<AudioManager>();
        }

        estat = Estat.inici;

        if (iniciarAutomaticament)
        {
            IniciarBatalla(lluitadorJugador);
        }
    }

    void LateUpdate()
    {
        // Mentre no ha començat el combat (selecció de lluitador) no cal moure ningú
        if (estat == Estat.inici || lluitadorJugador == null || lluitadorRival == null)
        {
            return;
        }
        ActualitzarPosicions();
    }

    // Comença el combat amb el lluitador triat pel jugador
    public void IniciarBatalla(Lluitador jugadorTriat)
    {
        if (estat != Estat.inici)
        {
            return; // evita iniciar-lo dos cops
        }

        lluitadorJugador = jugadorTriat;
        animatorJugador = lluitadorJugador.GetComponentInChildren<Animator>(true);
        animatorRival = lluitadorRival.GetComponentInChildren<Animator>(true);

        SetupBatalla();
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
        ReproduirSonsAccio(accioJugador, lluitadorJugador);
        ReproduirSonsAccio(accioRival, lluitadorRival);
        ReproduirSonsResultatPatada(accioJugador, lluitadorJugador, accioRival, lluitadorRival);
        ReproduirSonsResultatPatada(accioRival, lluitadorRival, accioJugador, lluitadorJugador);

        // --- ANIMACIONS ---
        if (resetAnimacio != null)
        {
            StopCoroutine(resetAnimacio);
            resetAnimacio = null;
        }

        // Si la corrutina anterior s'ha tallat abans d'acabar, apliquem ja les posicions que li quedaven
        AplicarPosicionsPendents();

        // Cada lluitador fa l'animació de la seva acció. Si un esquiva i l'altre fa una patada,
        // es fan les dues animacions (patada + esquivar)
        int estatAnimJugador = EstatAnimacio(accioJugador);
        int estatAnimRival = EstatAnimacio(accioRival);

        PosarEstatAnimator(animatorJugador, estatAnimJugador);
        PosarEstatAnimator(animatorRival, estatAnimRival);

        // Qui ha rebut una patada que no ha bloquejat ni esquivat (estat 7 després de l'animació de l'acció)
        bool jugadorRebCop = PatadaImpacta(accioRival, accioJugador);
        bool rivalRebCop = PatadaImpacta(accioJugador, accioRival);

        // Qui ha sortit del ring en aquest torn (estat 10 després de les animacions anteriors)
        bool jugadorForaRing = false;
        bool rivalForaRing = false;

        // Posicions abans d'aplicar els efectes del torn (les tornarem a posar després de calcular les noves)
        int posIniciJugador = lluitadorJugador.posicio;
        int posIniciRival = lluitadorRival.posicio;

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

        // Posicions noves calculades (encara no es mostren)
        posPendentJugador = lluitadorJugador.posicio;
        posPendentRival = lluitadorRival.posicio;

        // Tornem les posicions visibles a les d'abans: no es mouran fins que acabin totes les animacions
        lluitadorJugador.posicio = posIniciJugador;
        lluitadorRival.posicio = posIniciRival;

        // Comprovar si algun lluitador surt de la zona de combat (amb les posicions noves)
        if (posPendentJugador < 1)
        {
            Debug.Log("El jugador ha sortit de la zona! Punt per al rival.");

            lluitadorRival.AugmentarPunts(1);

            // Anirà a l'estat 10 i, en acabar totes les animacions, es reiniciaran les posicions
            jugadorForaRing = true;
            posPendentJugador = 3;
            posPendentRival = 4;
        }
        else if (posPendentRival > 6)
        {
            Debug.Log("El rival ha sortit de la zona! Punt per al jugador.");

            lluitadorJugador.AugmentarPunts(1);

            // Anirà a l'estat 10 i, en acabar totes les animacions, es reiniciaran les posicions
            rivalForaRing = true;
            posPendentJugador = 3;
            posPendentRival = 4;
        }

        posicionsPendents = true;

        if (lluitadorJugador.energia > 10)
        {
            Debug.Log(
                "Energia maxima es 10 per tant tornem a 10, borrar misatge al acabar el joc. (Jugador)"
            );

            lluitadorJugador.energia = 10;
        }
        if (lluitadorRival.energia > 10)
        {
            Debug.Log(
                "Energia maxima es 10 per tant tornem a 10, borrar misatge al acabar el joc. (Rival)"
            );

            lluitadorRival.energia = 10;
        }
        if (lluitadorJugador.energia < 0)
        {
            Debug.Log(
                "La energia no podra ser menys de 0, es desactivaran botons quan no puguis utilitzar un moviment, de moment la posem a 0, eliminar en producte final(Jugador)"
            );

            lluitadorJugador.energia = 0;
        }
        if (lluitadorRival.energia < 0)
        {
            Debug.Log(
                "La energia no podra ser menys de 0, es desactivaran botons quan no puguis utilitzar un moviment, de moment la posem a 0, eliminar en producte final(Rival)"
            );

            lluitadorRival.energia = 0;
        }

        Debug.Log("--- RESULTAT DEL TORN ---");
        Debug.Log("Punts jugador: " + lluitadorJugador.puntuacio);
        Debug.Log("Punts rival: " + lluitadorRival.puntuacio);
        Debug.Log("Energia jugador: " + lluitadorJugador.energia);
        Debug.Log("Energia rival: " + lluitadorRival.energia);
        Debug.Log("Posicio Jugador (nova): " + posPendentJugador);
        Debug.Log("Posicio Rival (nova): " + posPendentRival);

        torn++;

        combatUI.MostrarEstamina(lluitadorJugador.energia);

        // Sempre es mostra el resultat del torn (també l'últim).
        // El final del combat es decideix a AcabarMostrarResultatTorn(), quan la UI ja ha acabat el missatge.
        estat = Estat.resolucioTorn;
        combatUI.MostrarResultatTorn(accioJugador, accioRival);

        // Animació de l'acció, després estat 7 (cop rebut), després estat 10 (sortir del ring),
        // i quan han acabat totes s'actualitzen les posicions i els estats tornen a 0
        resetAnimacio = StartCoroutine(
            TornarAIdle(jugadorRebCop, rivalRebCop, jugadorForaRing, rivalForaRing)
        );
    }

    // El crida CombatUIManager quan acaba de mostrar el missatge del torn
    public void AcabarMostrarResultatTorn()
    {
        if (estat != Estat.resolucioTorn)
        {
            return;
        }

        if (torn >= tornsMaxims)
        {
            // Si l'animació encara no havia acabat, la tallem i apliquem les posicions que quedaven
            if (resetAnimacio != null)
            {
                StopCoroutine(resetAnimacio);
                resetAnimacio = null;
            }
            AplicarPosicionsPendents();

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
            accioJugador = "";
            estat = Estat.tornJugador;
        }
    }

    // POSICIONS PENDENTS

    // Aplica les posicions guardades a resolucioTorn (empenta / esquivar / reinici per sortir del ring).
    // Es crida quan han acabat totes les animacions del torn.
    void AplicarPosicionsPendents()
    {
        if (!posicionsPendents)
            return;

        lluitadorJugador.posicio = posPendentJugador;
        lluitadorRival.posicio = posPendentRival;

        posicionsPendents = false;
        ActualitzarPosicions();
    }

    // ANIMACIONS

    // Valor de l'int "estat" de l'Animator per a cada acció
    // 1 = patada alta, 2 = patada al cos, 3 = empenta, 4 = bloqueig alt, 5 = bloqueig al cos, 6 = esquivar
    // 7 = cop rebut, 8 = victòria, 9 = derrota/empat, 10 = sortir del ring
    int EstatAnimacio(string accio)
    {
        switch (accio)
        {
            case "patadaAlta":
                return 1;
            case "patadaAlCos":
                return 2;
            case "empenta":
                return 3;
            case "bloqueigAlt":
                return 4;
            case "bloqueigAlCos":
                return 5;
            case "esquivar":
                return 6;
            default:
                return 0;
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

    private void MostrarFeedbackPatada(string atac, string defensa, Lluitador defensor)
    {
        bool atacAlt = atac == "patadaAlta";
        bool atacCos = atac == "patadaAlCos";

        if (!atacAlt && !atacCos)
            return;

        if (defensa == "esquivar")
            return;

        bool bloquejat =
            (atacAlt && defensa == "bloqueigAlt") || (atacCos && defensa == "bloqueigAlCos");

        if (defensor == null)
            return;

        FeedbackLluitador feedback = defensor.GetComponent<FeedbackLluitador>();

        if (feedback != null)
        {
            feedback.MostrarImpacte(bloquejat, atacAlt);
        }
    }

    void PosarEstatAnimator(Animator animator, int valor)
    {
        if (animator == null)
            return;
        animator.SetInteger("estat", valor);
    }

    // Espera duradaAccio segons (animació de l'acció, estats 1-6), posa l'estat 7 a qui ha rebut cop,
    // després l'estat 10 a qui ha sortit del ring i, quan han acabat totes les animacions,
    // actualitza les posicions i torna els dos lluitadors a l'estat 0
    System.Collections.IEnumerator TornarAIdle(
        bool jugadorRebCop,
        bool rivalRebCop,
        bool jugadorForaRing,
        bool rivalForaRing
    )
    {
        // 1) Animació de l'acció (estats 1-6): dura un temps fix
        float tempsContacte = duradaAccio * 0.65f;
        yield return new WaitForSeconds(tempsContacte);
        MostrarFeedbackPatada(accioJugador, accioRival, lluitadorRival);
        MostrarFeedbackPatada(accioRival, accioJugador, lluitadorJugador);
        yield return new WaitForSeconds(duradaAccio - tempsContacte);

        // 2) Animació de cop rebut (estat 7), només per a qui ha rebut la patada
        if (jugadorRebCop)
            PosarEstatAnimator(animatorJugador, 7);
        if (rivalRebCop)
            PosarEstatAnimator(animatorRival, 7);

        // L'estat 7 dura duradaCop segons abans de continuar
        if (jugadorRebCop || rivalRebCop)
            yield return new WaitForSeconds(duradaCop);

        // 3) Animació de sortir del ring (estat 10), només per a qui ha sortit de la zona
        if (jugadorForaRing)
            PosarEstatAnimator(animatorJugador, 10);
        if (rivalForaRing)
            PosarEstatAnimator(animatorRival, 10);

        // L'estat 10 dura duradaFora segons abans de continuar
        if (jugadorForaRing || rivalForaRing)
            yield return new WaitForSeconds(duradaFora);

        // 4) Han acabat totes les animacions: ara s'actualitzen les posicions
        AplicarPosicionsPendents();

        // 5) Tornem a l'estat 0 (idle)
        PosarEstatAnimator(animatorJugador, 0);
        PosarEstatAnimator(animatorRival, 0);
        resetAnimacio = null;
    }

    // Espera que l'animació que s'està reproduint acabi una vegada (normalizedTime >= 1)
    System.Collections.IEnumerator EsperarFiAnimacio(Animator animator)
    {
        if (animator == null)
            yield break;

        // Esperem un frame perquè l'Animator processi el canvi d'estat
        yield return null;
        float temps = Time.deltaTime;

        while (temps < duradaAnimacio)
        {
            // Mentre hi ha transició entre animacions, seguim esperant
            if (
                !animator.IsInTransition(0)
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
            )
            {
                yield break;
            }

            temps += Time.deltaTime;
            yield return null;
        }
    }

    // SONS

    // So de cada acció: depèn de l'acció i de qui la fa (segons el seu tag)
    void ReproduirSonsAccio(string accio, Lluitador qui)
    {
        if (audioManager == null)
            return;

        string tag = qui.gameObject.tag;

        switch (accio)
        {
            case "patadaAlta":
                audioManager.PlaySFX(TipusSo.AtacAlt, tag);
                break;

            case "patadaAlCos":
                audioManager.PlaySFX(TipusSo.AtacCos, tag);
                break;

            case "empenta":
                audioManager.PlaySFX(TipusSo.VeuEmpenta, tag);
                break;

            case "bloqueigAlt":
            case "bloqueigAlCos":
                audioManager.PlaySFX(TipusSo.Bloqueig, tag);
                break;

            case "esquivar":
                audioManager.PlaySFX(TipusSo.Esquiva, tag);
                break;
        }
    }

    // So del resultat d'una patada: si impacta, el defensor fa la veu de cop rebut (amb 1 segon de retard)
    // Si la patada és bloquejada o esquivada no s'afegeix res: ja ha sonat Bloqueig/Esquiva a l'acció del defensor
    void ReproduirSonsResultatPatada(
        string accioAtacant,
        Lluitador atacant,
        string accioDefensor,
        Lluitador defensor
    )
    {
        if (audioManager == null)
            return;

        bool esPatadaAlta = (accioAtacant == "patadaAlta");
        bool esPatadaAlCos = (accioAtacant == "patadaAlCos");

        // Només les patades fan aquest so (l'empenta no)
        if (!esPatadaAlta && !esPatadaAlCos)
            return;

        bool bloquejadaOEsquivada =
            (esPatadaAlta && accioDefensor == "bloqueigAlt")
            || (esPatadaAlCos && accioDefensor == "bloqueigAlCos")
            || accioDefensor == "esquivar";

        if (bloquejadaOEsquivada)
            return;

        audioManager.PlaySFXDelayed(TipusSo.VeuRebutCop, defensor.gameObject.tag, 1f);
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
        Transform[] posicions = { Posicio1, Posicio2, Posicio3, Posicio4, Posicio5, Posicio6 };

        // Moure el jugador
        int posJugador = lluitadorJugador.posicio;

        if (posJugador >= 1 && posJugador <= 6)
        {
            lluitadorJugador.transform.position = posicions[posJugador - 1].position;
        }

        // Moure el rival
        int posRival = lluitadorRival.posicio;

        if (posRival >= 1 && posRival <= 6)
        {
            lluitadorRival.transform.position = posicions[posRival - 1].position;
        }
    }
}
