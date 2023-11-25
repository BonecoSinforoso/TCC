using UnityEngine;

public class Script_borrarControladorTudo : MonoBehaviour
{
    [SerializeField] RectTransform rect_borrao;
    [SerializeField] float moveSpeed;
    bool descer = false;

    private void Update()
    {
        Debug.Log(rect_borrao.offsetMax.y);
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
        Debug.Log("kasdasd");
        descer = true;
    }

    private void OnMouseExit()
    {
        descer = false;
    }
}