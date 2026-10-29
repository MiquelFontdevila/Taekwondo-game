using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    [Header("Panells")]
    [SerializeField] private GameObject panellPrincipal; //ChooseDef_AtkButtons
    [SerializeField] private GameObject panellAtac; //AttackButtons
    [SerializeField] private GameObject panellDefensa; //DeffenseButtons

    [Header("Panell d'informació")]
    [SerializeField] private TextMeshProUGUI textInfo;   // TurnInfoTMP
    [SerializeField] private string missatgePrincipal = "Què farà el jugador?";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MostrarPanellPrincipal();
    }

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

    private void CanviarPanell (GameObject nou)
    {
        panellPrincipal.SetActive (nou == panellPrincipal);
        panellAtac.SetActive (nou == panellAtac);
        panellDefensa.SetActive (nou == panellDefensa);
    }

    private void SeleccionarPrimerBoto(GameObject panell)
    {
        EventSystem.current.SetSelectedGameObject(null); //Desselecciona el que hi havia seleccionat

        //Selecciona el primer boto interactuable del panell
        foreach (Button boto in panell.GetComponentsInChildren<Button>())
        {
            if (boto.interactable)
            {
                EventSystem.current.SetSelectedGameObject(boto.gameObject);
                return;
            }
        }
    }

    public void MostrarInfoTecnica(TecnicaBoto tecnica)
    {
        textInfo.text = tecnica.GenerarText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
