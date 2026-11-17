using UnityEngine;

public class Script_LayerToggle : MonoBehaviour
{
    [SerializeField] private int layerIniIndex;
    [SerializeField] private string[] layer;
    [SerializeField] private GameObject[] obj;
    [SerializeField] private int layerAtualIndex;

    private void Start()
    {
        layerAtualIndex = layerIniIndex;

        foreach (GameObject _obj in obj)
        {
            _obj.layer = LayerMask.NameToLayer(layer[layerAtualIndex]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Layer_Toggle();
        }
    }

    private void Layer_Toggle()
    {
        layerAtualIndex = layerAtualIndex == 0 ? 1 : 0;

        foreach (GameObject _obj in obj)
        {
            _obj.layer = LayerMask.NameToLayer(layer[layerAtualIndex]);
        }
    }
}