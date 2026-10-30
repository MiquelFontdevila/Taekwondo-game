using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CombatUIManager : MonoBehaviour
{
    [Header("Panells")]
    [SerializeField] private GameObject panellPrincipal; //ChooseDef_AtkButtons
    [SerializeField] private GameObject panellAtac; //AttackButtons
    [SerializeField] private GameObject panellDefensa; //DeffenseButtons

    [Header("Panell d'informació")]
    [SerializeField] private TextMeshProUGUI textInfoTorn;   // TurnInfoTMP
    [SerializeField] private string missatgePrincipal = "Què farà el jugador?";

    [Header("Barra d'Estamina")]
    [SerializeField] private GameObject barraEstaminaGroup; // EstaminaBar_Group
    [SerializeField] private Image barraEstamina;           // EstaminaBar_Fill
    [SerializeField] private TextMeshProUGUI textEstamina;  // EstaminaText
    [SerializeField] private int estaminaMaxima = 10;
    [SerializeField] private float velocitatBarra = 1.5f;
    
    [Header("Resultat del Torn")]
    [SerializeField] private SistemaDeBatalla sistemaBatalla;

    private GameObject panellActiu;
    private Lluitador jugador;

    // INICI
    public void Inicialitzar(Lluitador j)
    {
        jugador = j;
        MostrarEstamina(jugador.energia, true);
        MostrarPanellPrincipal();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MostrarPanellPrincipal();
    }

    // FORMAT DE TEXTOS
    private void FormatMissatgePanellPrincipal()
    {
        textInfoTorn.fontSizeMax = 48;
        textInfoTorn.alignment = TextAlignmentOptions.Left;
    }

    private void FormatMissatgeDestacat()
    {
        textInfoTorn.fontSizeMax = 38;
        textInfoTorn.alignment = TextAlignmentOptions.Left;
    }

    private void FormatMissatgeInfoTecnica()
    {
        textInfoTorn.fontSizeMax = 32;
        textInfoTorn.alignment = TextAlignmentOptions.TopLeft;
    }

    // Crear efecte d'escriptura de text (typewriter effect)
    private IEnumerator EscriureTextTypewriter(string text, float segonsPerLletra = 0.05f)
    {
        textInfoTorn.text = text;
        textInfoTorn.maxVisibleCharacters = 0;

        textInfoTorn.ForceMeshUpdate(); //Forçar procesar el text abans d'escriure
        int totalLletres = textInfoTorn.textInfo.characterCount;

        for (int i = 0; i <= totalLletres; i++)
        {
            textInfoTorn.maxVisibleCharacters = i;
            yield return new WaitForSeconds(segonsPerLletra);
        }
    }

    private void MostrarTextInstantani(string text)
    {
        textInfoTorn.maxVisibleCharacters = 99999;
        textInfoTorn.text = text;
    }

    // MENÚS
    //Mètodes public per poder cridar-los des de l'Inspector
    public void MostrarPanellPrincipal()
    {
        CanviarPanell(panellPrincipal);
        FormatMissatgePanellPrincipal();
        StartCoroutine(EscriureTextTypewriter(missatgePrincipal));
    }
    public void MostrarPanellAtac()
    {
        CanviarPanell(panellAtac);
    }
    public void MostrarPanellDefensa()
    {
        CanviarPanell(panellDefensa);
    }

    private void CanviarPanell(GameObject nou)
    {
        panellPrincipal.SetActive(nou == panellPrincipal);
        panellAtac.SetActive(nou == panellAtac);
        panellDefensa.SetActive(nou == panellDefensa);
        ActualitzarBotonsDisponibles(nou);
        SeleccionarPrimerBoto(nou);
    }

    private void ActualitzarBotonsDisponibles(GameObject panell)
    {
        foreach (TecnicaBoto tboto in panell.GetComponentsInChildren<TecnicaBoto>(true))
        {
            Button b = tboto.GetComponent<Button>();
            b.interactable = (jugador == null) || (jugador.energia >= tboto.cost);
        }
    }

    private void SeleccionarPrimerBoto(GameObject panell)
    {
        EventSystem.current.SetSelectedGameObject(null);

        foreach (Button b in panell.GetComponentsInChildren<Button>())
        {
            if (b.interactable)
            {
                EventSystem.current.SetSelectedGameObject(b.gameObject);
                return;
            }
        }

    }

    // ESTAMINA I INFO
    public void MostrarInfoTecnica(TecnicaBoto tecnica)
    {
        FormatMissatgeInfoTecnica();
        MostrarTextInstantani(tecnica.GenerarText());
    }

    private float omplirObjectiu = 0.5f;

    public void MostrarEstamina(int estaminaActual, bool immediat = false)
    {
        omplirObjectiu = Mathf.Clamp01((float)estaminaActual / estaminaMaxima);

        if (immediat)
        {
            barraEstamina.fillAmount = omplirObjectiu;
        }
        
        textEstamina.text = estaminaActual + " / " + estaminaMaxima;
    }

    void Update()
    {
        // Mou la barra suaument cap al valor objectiu
        barraEstamina.fillAmount = Mathf.MoveTowards(
            barraEstamina.fillAmount, omplirObjectiu, velocitatBarra * Time.deltaTime);

        if (panellActiu != null && EventSystem.current.currentSelectedGameObject == null)
        {
            SeleccionarPrimerBoto(panellActiu);
        }
    }

    // TORN
    private string NomAccio(string accio)
    {
        switch (accio)
        {
            case "patadaAlta":
                return "Patada Alta";
            case "patadaBaixa":
                return "Patada Baixa";
            case "empenta":
                return "Empenta";
            case "bloqueigAlt":
                return "Bloqueig Alt";
            case "bloqueigBaix":
                return "Bloqueig Baix";
            case "esquivar":
                return "Esquivar";
            default:
                return accio;
        }
    }

    public void MostrarResultatTorn(string accioJugador, string accioRival)
    {
        StartCoroutine(SequenciaResultat(accioJugador, accioRival));
    }

    private IEnumerator SequenciaResultat(string accioJug, string accioRiv)
    {
        panellPrincipal.SetActive(false);
        panellAtac.SetActive(false);
        panellDefensa.SetActive(false);
        panellActiu = null;
        barraEstaminaGroup.SetActive(false);

        FormatMissatgeDestacat();
        string missatgeTornRealitzat = "El jugador ha realitzat <b>" + NomAccio(accioJug) + "</b>!" + 
                                        "\n\nEl rival ha realitzat <b>" + NomAccio(accioRiv) + "</b>!";

        yield return EscriureTextTypewriter(missatgeTornRealitzat);

        yield return new WaitForSeconds(2f);

        barraEstaminaGroup.SetActive(true);
        MostrarPanellPrincipal();
        sistemaBatalla.AcabarMostrarResultatTorn();
    }

    // FINAL DEL COMBAT
    public void AcabarCombat(Estat resultat)
    {
        panellPrincipal.SetActive(false);
        panellAtac.SetActive(false);
        panellDefensa.SetActive(false);
        panellActiu = null;

        switch (resultat)
        {
            case Estat.victoria:
                FormatMissatgeDestacat();
                string missatgeVictoria = "Guanya el jugador!";
                StartCoroutine(EscriureTextTypewriter(missatgeVictoria));
                barraEstaminaGroup.SetActive(false);
                break;
            case Estat.derrota:
                FormatMissatgeDestacat();
                string missatgeDerrota = "Has perdut! Guanya el rival!";
                StartCoroutine(EscriureTextTypewriter(missatgeDerrota));
                barraEstaminaGroup.SetActive(false);
                break;
            case Estat.empat:
                FormatMissatgeDestacat();
                string missatgeEmpat = "Empat!";
                StartCoroutine(EscriureTextTypewriter(missatgeEmpat));
                barraEstaminaGroup.SetActive(false);
                break;
        }
    }
}
