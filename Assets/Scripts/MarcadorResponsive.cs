using UnityEngine;

public class MarcadorResponsive : MonoBehaviour
{
    [SerializeField]
    private RectTransform marcadorReferencia;

    [SerializeField]
    private RectTransform marcadorUI;

    [SerializeField]
    private Canvas canvasFons;

    [SerializeField]
    private Canvas canvasUI;

    [SerializeField]
    private RectTransform backgroundImage;

    [Header("Escala del marcador")]
    [SerializeField, Range(0.05f, 0.3f)]
    private float proporcioMarcador = 0.14f;

    private void LateUpdate()
    {
        if (
            marcadorReferencia == null
            || marcadorUI == null
            || canvasFons == null
            || canvasUI == null
        )
            return;

        Camera camaraFons =
            canvasFons.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvasFons.worldCamera;

        Camera camaraUI =
            canvasUI.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvasUI.worldCamera;

        Vector2 posicioPantalla = RectTransformUtility.WorldToScreenPoint(
            camaraFons,
            marcadorReferencia.position
        );

        RectTransform pare = marcadorUI.parent as RectTransform;

        if (pare == null)
            return;

        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                pare,
                posicioPantalla,
                camaraUI,
                out Vector2 posicioLocal
            )
        )
        {
            marcadorUI.anchoredPosition = posicioLocal;

            if (backgroundImage != null)
{
    Vector3[] vores = new Vector3[4];
    backgroundImage.GetWorldCorners(vores);

    Vector2 izquierda =
        RectTransformUtility.WorldToScreenPoint(
            camaraFons, vores[0]);

    Vector2 derecha =
        RectTransformUtility.WorldToScreenPoint(
            camaraFons, vores[3]);

    float ampleFonsPantalla =
        Vector2.Distance(izquierda, derecha);

    float ampleObjectiu =
        ampleFonsPantalla * proporcioMarcador;

    float ampleMarcadorNoEscala =
        marcadorUI.rect.width * canvasUI.scaleFactor;

    if (ampleMarcadorNoEscala > 0f)
    {
        float escala = ampleObjectiu / ampleMarcadorNoEscala;

        marcadorUI.localScale =
            new Vector3(escala, escala, 1f);
    }
}
        }
    }
}
