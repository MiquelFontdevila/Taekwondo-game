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
    [SerializeField] private TextMeshProUGUI textInfo;   // TurnInfoTMP
    [SerializeField] private string missatgePrincipal = "Què farà el jugador?";

    //private Coroutine seleccioPendent;
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

    // MENÚS
    //Mètodes public per poder cridar-los des de l'Inspector
    public void MostrarPanellPrincipal()
    {
        CanviarPanell(panellPrincipal);
        textInfo.text = missatgePrincipal;
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

        /*if (seleccioPendent != null)
        {
            StopCoroutine(seleccioPendent);
        }

        seleccioPendent = StartCoroutine(SeleccioDespresDeFrame(panell));*/
    }

    /*private IEnumerator SeleccioDespresDeFrame(GameObject panell)
    {
        yield return null; //Esperem un frame

        //Selecciona el primer boto interactuable del panell
        foreach (Button boto in panell.GetComponentsInChildren<Button>())
        {
            if (boto.interactable)
            {
                EventSystem.current.SetSelectedGameObject(boto.gameObject);
                break;
            }
        }

        seleccioPendent = null;
    }*/

    public void MostrarInfoTecnica(TecnicaBoto tecnica)
    {
        textInfo.text = tecnica.GenerarText();
    }

    [Header("Barra d'Estamina")]
    [SerializeField] private Image barraEstamina;           // StaminaBar_Fill
    [SerializeField] private TextMeshProUGUI textEstamina;  // StaminaText
    [SerializeField] private int estaminaMaxima = 10;
    [SerializeField] private float velocitatBarra = 1.5f;

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
}
