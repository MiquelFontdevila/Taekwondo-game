
using UnityEngine;

public class MarcadorResponsive : MonoBehaviour
{
    [SerializeField]
    private RectTransform marcadorReferencia;

    [SerializeField]
    private RectTransform marcadorUI;

    [SerializeField]
    private Canvas canvasFondo;

    [SerializeField]
    private Canvas canvasUI;

    private void LateUpdate()
    {
        if (marcadorReferencia == null ||
            marcadorUI == null ||
            canvasFondo == null ||
            canvasUI == null)
            return;

        Camera camaraFondo =
            canvasFondo.renderMode ==
            RenderMode.ScreenSpaceOverlay
            ? null : canvasFondo.worldCamera;

        Camera camaraUI =
            canvasUI.renderMode ==
            RenderMode.ScreenSpaceOverlay
            ? null : canvasUI.worldCamera;

        Vector2 posicionPantalla =
            RectTransformUtility.WorldToScreenPoint(
                camaraFondo,
                marcadorReferencia.position
            );

        RectTransform padre =
            marcadorUI.parent as RectTransform;

        if (padre == null)
            return;

        if (RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                padre,
                posicionPantalla,
                camaraUI,
                out Vector2 posicionLocal))
        {
            marcadorUI.anchoredPosition = posicionLocal;
        }
    }
}
