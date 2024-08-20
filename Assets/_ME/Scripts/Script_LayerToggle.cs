using UnityEngine;

public class Script_LayerToggle : MonoBehaviour
{
    [SerializeField] int layerIniIndex;
    [SerializeField] string[] layer;
    [SerializeField] GameObject[] obj;
    [SerializeField] int layerAtualIndex;

    void Start()
    {
        layerAtualIndex = layerIniIndex;

        foreach (GameObject _obj in obj)
        {
            _obj.layer = LayerMask.NameToLayer(layer[layerAtualIndex]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Layer_Toggle();
        }
    }

    void Layer_Toggle()
    {
        layerAtualIndex = layerAtualIndex == 0 ? 1 : 0;

        foreach (GameObject _obj in obj)
        {
            _obj.layer = LayerMask.NameToLayer(layer[layerAtualIndex]);
        }
    }
}