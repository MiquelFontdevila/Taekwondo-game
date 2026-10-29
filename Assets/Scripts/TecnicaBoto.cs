using UnityEngine;
using UnityEngine.EventSystems;

public class TecnicaBoto : MonoBehaviour, ISelectHandler
{
    public string nomTecnica;
    public bool esDefensa;
    public int cost; // estamina que gasta (0 en defensa)
    public int resultat; // punts (atac) o estamina que recupera (defensa)

    [TextArea]
    public string descripcio;

    private CombatUIManager combatUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Unity crida això quan el botó queda seleccionat
    public void OnSelect(BaseEventData eventData)
    {
        if (combatUI == null)
            combatUI = GetComponentInParent<CombatUIManager>(true);

        combatUI.MostrarInfoTecnica(this);
    }

    public string GenerarText()
    {
        string detall;
        if (esDefensa)
            detall = "Recupera: +" + resultat + " Estamina";
        else
            detall = "Cost: " + cost + " Estamina    Punts: " + resultat;

        string text = "<b>" + nomTecnica + "</b>\n" + detall;

        if (!string.IsNullOrEmpty(descripcio))
            text += "\n" + descripcio;

        return text;

        // Update is called once per frame
        void Update()
        {

        }
    }
}
