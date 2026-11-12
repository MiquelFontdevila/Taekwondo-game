using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class OpcioLluitador
{
    public string nom;
    public Sprite imatge;
    public Lluitador lluitador;

    [Range(0.5f, 2f)]
    public float escalaPreview = 1f;
}

public class SeleccioLluitador : MonoBehaviour
{
    [Header("Opcions")]
    [SerializeField]
    private OpcioLluitador[] opcions;

    [Header("UI")]
    [SerializeField]
    private Image previsualitzacio;

    [SerializeField]
    private GameObject botoInicial;

    [Header("Joc")]
    [SerializeField]
    private SistemaDeBatalla sistemaBatalla;

    [SerializeField]
    private GameObject[] elementsCombat;

    [Header("Transició de previsualització")]
    [SerializeField]
    private float duradaTransicio = 0.15f;
    private Coroutine transicioActual;

    [Header("Audio")]
    [SerializeField]
    private AudioManager audioManager;

    private const string ClauPref = "LluitadorEscollit";
    private int index;
    private Vector3 escalaOriginal;

    void Start()
    {
        escalaOriginal = previsualitzacio.rectTransform.localScale;

        index = Mathf.Clamp(PlayerPrefs.GetInt(ClauPref, 0), 0, opcions.Length - 1);

        OcultarElementsCombat();

        MostrarOpcio();

        EventSystem.current.SetSelectedGameObject(botoInicial);
    }

    // Oculta la interfície i els lluitadors abans del combat.
    public void OcultarElementsCombat()
    {
        foreach (GameObject g in elementsCombat)
        {
            if (g != null)
                g.SetActive(false);
        }

        foreach (OpcioLluitador o in opcions)
        {
            if (o.lluitador != null)
                o.lluitador.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(botoInicial);
        }
    }

    public void Canviar(int direccio)
    {
        if (opcions == null || opcions.Length == 0)
            return;

        index = (index + direccio + opcions.Length) % opcions.Length;

        if (transicioActual != null)
            StopCoroutine(transicioActual);

        transicioActual = StartCoroutine(TransicioPersonatge());
    }

    private void MostrarOpcio()
    {
        OpcioLluitador opcio = opcions[index];

        previsualitzacio.sprite = opcio.imatge;
        previsualitzacio.preserveAspect = true;

        previsualitzacio.rectTransform.localScale = escalaOriginal * opcio.escalaPreview;
    }

    private IEnumerator TransicioPersonatge()
    {
        Color color = previsualitzacio.color;

        float t = 0f;
        float alfaInicial = color.a;

        while (t < duradaTransicio)
        {
            t += Time.unscaledDeltaTime;

            color.a = Mathf.Lerp(alfaInicial, 0f, Mathf.Clamp01(t / duradaTransicio));

            previsualitzacio.color = color;
            yield return null;
        }

        color.a = 0f;
        previsualitzacio.color = color;

        MostrarOpcio();

        t = 0f;

        while (t < duradaTransicio)
        {
            t += Time.unscaledDeltaTime;

            color.a = Mathf.Lerp(0f, 1f, Mathf.Clamp01(t / duradaTransicio));

            previsualitzacio.color = color;
            yield return null;
        }

        color.a = 1f;
        previsualitzacio.color = color;

        transicioActual = null;
    }

    public void Confirmar()
    {
        PlayerPrefs.SetInt(ClauPref, index);
        PlayerPrefs.Save();

        Lluitador triat = opcions[index].lluitador;
        triat.gameObject.SetActive(true);

        foreach (GameObject g in elementsCombat)
        {
            if (g != null)
                g.SetActive(true);
        }

        if (audioManager != null)
        {
            audioManager.IniciarMusicaCombat();
        }

        gameObject.SetActive(false);
        sistemaBatalla.IniciarBatalla(triat);
    }
}
