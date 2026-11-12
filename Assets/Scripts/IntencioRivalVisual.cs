
using UnityEngine;

public class IntencioRivalVisual : MonoBehaviour
{
    [Header("Icones")]
    [SerializeField] private Sprite spriteAtac;
    [SerializeField] private Sprite spriteDefensa;

    [Header("Posicio respecte al rival")]
    [SerializeField] private float desplacamentX = 0f;
    [SerializeField] private float desplacamentY = 3.2f;

    [Header("Mida de la icona")]
    [SerializeField, Min(0.1f)]
    private float midaIcona = 0.8f;

    [Header("Animacio")]
    [SerializeField, Min(0.1f)]
    private float durada = 2f;

    [SerializeField, Min(0f)]
    private float distanciaPujada = 0.5f;

    private FeedbackLluitador feedbackRival;
    private GameObject objecteIcona;
    private SpriteRenderer rendererIcona;

    private Vector3 posicioLocalInicial;
    private Transform transformRival;

    private float temps;
    private bool mostrant;

    public void Mostrar(Lluitador rival, string accio)
    {
        Ocultar();

        if (rival == null)
            return;

        bool atac =
            accio == "patadaAlta" ||
            accio == "patadaAlCos" ||
            accio == "empenta";

        bool defensa =
            accio == "bloqueigAlt" ||
            accio == "bloqueigAlCos" ||
            accio == "esquivar";

        if (!atac && !defensa)
            return;

        Sprite sprite = atac ? spriteAtac : spriteDefensa;

        if (sprite == null)
        {
            Debug.LogWarning(
                "Falta assignar un sprite d'intencio."
            );
            return;
        }

        transformRival = rival.transform;

        feedbackRival =
            rival.GetComponent<FeedbackLluitador>();

        // Crear la icona directament DINS del rival.
        objecteIcona = new GameObject("IconaIntencioRival");

        objecteIcona.transform.SetParent(
            transformRival,
            false
        );

        rendererIcona =
            objecteIcona.AddComponent<SpriteRenderer>();

        rendererIcona.sprite = sprite;
        rendererIcona.color = Color.white;

        // Mostrar la icona davant dels personatges.
        SpriteRenderer[] spritesRival =
            rival.GetComponentsInChildren<SpriteRenderer>(true);

        int ordreMaxim = 0;
        int capa = 0;
        bool trobat = false;

        foreach (SpriteRenderer sr in spritesRival)
        {
            if (sr == null || sr == rendererIcona)
                continue;

            if (!trobat || sr.sortingOrder > ordreMaxim)
            {
                ordreMaxim = sr.sortingOrder;
                capa = sr.sortingLayerID;
                trobat = true;
            }
        }

        if (trobat)
            rendererIcona.sortingLayerID = capa;

        rendererIcona.sortingOrder = ordreMaxim + 10;

        // Posicio desitjada en unitats del mon.
        Vector3 offsetMon = new Vector3(
            desplacamentX,
            desplacamentY,
            0f
        );

        // Convertir-la a coordenades locals del rival.
        // Aixi la seva escala no desplaca la icona.
        posicioLocalInicial =
            transformRival.InverseTransformVector(offsetMon);

        objecteIcona.transform.localPosition =
            posicioLocalInicial;

        objecteIcona.transform.localRotation =
            Quaternion.identity;

        float alturaOriginal = sprite.bounds.size.y;

        float escalaPare = Mathf.Max(
            Mathf.Abs(transformRival.lossyScale.y),
            0.0001f
        );

        if (alturaOriginal > 0f)
        {
            float escala =
                midaIcona / (alturaOriginal * escalaPare);

            objecteIcona.transform.localScale =
                Vector3.one * escala;
        }

        if (feedbackRival != null)
            feedbackRival.MostrarIntencio(atac);

        temps = 0f;
        mostrant = true;
    }

    private void Update()
    {
        if (!mostrant)
            return;

        temps += Time.deltaTime;

        float progres = Mathf.Clamp01(temps / durada);
        float opacitat = 1f - progres;

        if (rendererIcona != null)
        {
            Color color = rendererIcona.color;
            color.a = opacitat;
            rendererIcona.color = color;
        }

        if (feedbackRival != null)
            feedbackRival.ActualitzarIntencio(opacitat);

        if (objecteIcona != null && transformRival != null)
        {
            float moviment = Mathf.SmoothStep(
                0f,
                1f,
                progres
            );

            Vector3 pujadaLocal =
                transformRival.InverseTransformVector(
                    Vector3.up * distanciaPujada * moviment
                );

            objecteIcona.transform.localPosition =
                posicioLocalInicial + pujadaLocal;
        }

        if (progres >= 1f)
            Ocultar();
    }

    public void Ocultar()
    {
        mostrant = false;
        temps = 0f;

        if (feedbackRival != null)
            feedbackRival.OcultarIntencio();

        if (objecteIcona != null)
            Destroy(objecteIcona);

        objecteIcona = null;
        rendererIcona = null;
        feedbackRival = null;
        transformRival = null;
    }

    private void OnDisable()
    {
        Ocultar();
    }
}
