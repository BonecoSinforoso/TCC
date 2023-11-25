using UnityEngine;

public class Script_iaPongTudo : MonoBehaviour
{
    [SerializeField] Transform t_bola;
    
    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, t_bola.position.z);
    }
}
