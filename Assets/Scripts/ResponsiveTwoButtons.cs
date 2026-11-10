
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveTwoButtons : MonoBehaviour
{
    [SerializeField] private float ampleMaxim = 350f;
    [SerializeField] private float alcada = 150f;
    [SerializeField] private float separacio = 35f;
    [SerializeField] private float marge = 15f;

    private GridLayoutGroup grid;
    private RectTransform rect;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 2;
        grid.childAlignment = TextAnchor.MiddleCenter;
    }

    private void OnEnable()
    {
        if (grid == null)
            grid = GetComponent<GridLayoutGroup>();

        if (rect == null)
            rect = GetComponent<RectTransform>();

        Ajustar();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (grid != null && rect != null)
            Ajustar();
    }

    private void Ajustar()
    {
        float ampleDisponible =
            rect.rect.width - marge * 2f - separacio;

        float ampleBoto = Mathf.Min(
            ampleMaxim,
            Mathf.Max(1f, ampleDisponible / 2f)
        );

        grid.cellSize = new Vector2(
            ampleBoto,
            Mathf.Min(alcada, rect.rect.height)
        );

        grid.spacing = new Vector2(separacio, 0f);
    }
}
