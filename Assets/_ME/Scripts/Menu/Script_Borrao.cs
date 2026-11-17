using UnityEngine;

public class Script_Borrao : MonoBehaviour
{
    [SerializeField] private RectTransform rect_borrao;
    [SerializeField] private float moveSpeed;
    private bool descer = false;

    private void Update()
    {
        if (descer)
        {
            if (rect_borrao.offsetMax.y > -1080) rect_borrao.offsetMax += moveSpeed * Time.deltaTime * Vector2.down;
        }
        else
        {
            if (rect_borrao.offsetMax.y < 0) rect_borrao.offsetMax += moveSpeed * Time.deltaTime * Vector2.up;
        }
    }

    private void OnMouseEnter()
    {
        descer = true;
    }

    private void OnMouseExit()
    {
        descer = false;
    }
}