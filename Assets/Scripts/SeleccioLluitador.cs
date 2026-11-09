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

    private const string ClauPref = "LluitadorEscollit";
    private int index;
    private Vector3 escalaOriginal;

    void Start()
    {
        escalaOriginal = previsualitzacio.rectTransform.localScale;
        index = Mathf.Clamp(PlayerPrefs.GetInt(ClauPref, 0), 0, opcions.Length - 1);

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

        MostrarOpcio();
        EventSystem.current.SetSelectedGameObject(botoInicial);
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
        index = (index + direccio + opcions.Length) % opcions.Length;
        MostrarOpcio();
    }

    private void MostrarOpcio()
    {
        OpcioLluitador opcio = opcions[index];

        previsualitzacio.sprite = opcio.imatge;
        previsualitzacio.preserveAspect = true;

        previsualitzacio.rectTransform.localScale = escalaOriginal * opcio.escalaPreview;
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

        gameObject.SetActive(false);
        sistemaBatalla.IniciarBatalla(triat);
    }
}
