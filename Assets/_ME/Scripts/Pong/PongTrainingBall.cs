using UnityEngine;

// Bola da mesa de treino. Mesma logica de rebatida do Script_BolaPong (OnTriggerEnter),
// mas com posicoes locais da mesa e sem som, raycast de trajetoria ou singleton.
public class PongTrainingBall : MonoBehaviour
{
    [SerializeField] Transform t_arena;

    Rigidbody rb;

    public void Configurar(Transform _t_arena)
    {
        t_arena = _t_arena;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        float _x = t_arena.InverseTransformPoint(transform.position).x;

        if (other.gameObject.CompareTag("Player"))
        {
            Raquete_Rebater(other, _x < -8.7f);
        }

        if (other.gameObject.CompareTag("IA"))
        {
            Raquete_Rebater(other, _x > 8.7f);
        }

        if (other.gameObject.CompareTag("Parede"))
        {
            MoveSpeedVertical_Toggle();
        }
    }

    void Raquete_Rebater(Collider _raquete, bool _passouDaRaquete)
    {
        MoveSpeedHorizontal_Toggle();
        if (_passouDaRaquete) MoveSpeedVertical_Toggle();
        if (transform.position.z + 0.5f < _raquete.transform.position.z && rb.linearVelocity.z > 0) MoveSpeedVertical_Toggle();
        if (transform.position.z - 0.5f > _raquete.transform.position.z && rb.linearVelocity.z < 0) MoveSpeedVertical_Toggle();

        if (_raquete.TryGetComponent(out AgentPong _agente)) _agente.Rebateu();
    }

    void MoveSpeedHorizontal_Toggle()
    {
        rb.linearVelocity = new Vector3(-rb.linearVelocity.x, 0, rb.linearVelocity.z);
    }

    void MoveSpeedVertical_Toggle()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, -rb.linearVelocity.z);
    }
}
