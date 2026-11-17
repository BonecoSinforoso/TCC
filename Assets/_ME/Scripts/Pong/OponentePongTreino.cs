using UnityEngine;

// Adversario da mesa de treino: copia do agente reativo (Script_IaPong).
// A diferenca eh que a trajetoria da bola eh calculada por conta (no lugar dos raycasts com a tag "Perigo"
// e das variaveis estaticas do Script_BolaPong), para funcionar com varias mesas e dos dois lados.
public class OponentePongTreino : MonoBehaviour
{
    [SerializeField] float moveSpeed = 7;
    [SerializeField] Transform t_arena;
    [SerializeField] Rigidbody rb_bola;

    [SerializeField] float energiaMax = 100;
    [SerializeField] float energiaAtual = 100;
    [SerializeField] float energiaConsumo = 100;
    [SerializeField] float energiaRecarga = 25;

    //medidas da cena do jogo: face do col_perigo (x = 9.5) e face interna das paredes (z = 5.5)
    const float perigoX = 9.5f;
    const float paredeZ = 5.5f;

    Rigidbody rb;
    float lado; //-1 = esquerda, 1 = direita

    public void Configurar(Transform _t_arena, Rigidbody _rb_bola)
    {
        t_arena = _t_arena;
        rb_bola = _rb_bola;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lado = t_arena.InverseTransformPoint(transform.position).x < 0 ? -1 : 1;
    }

    public void EnergiaReset()
    {
        energiaAtual = energiaMax;
    }

    void FixedUpdate()
    {
        float _z = t_arena.InverseTransformPoint(transform.position).z;
        bool _andou = false;

        if (energiaAtual > 5) //verifica a energia
        {
            if (Perigo_Calcular(out float _pos)) //verifica se a bola esta indo em direcao ao gol
            {
                if (_z + 1 < _pos) //bola acima da raquete
                {
                    if (_z < 3.5f)
                    {
                        rb.linearVelocity = Vector3.forward * moveSpeed;
                        _andou = true;
                    }
                }

                if (_z - 1 > _pos) //bola abaixo da raquete
                {
                    if (_z > -3.5f)
                    {
                        rb.linearVelocity = Vector3.back * moveSpeed;
                        _andou = true;
                    }
                }
            }
        }

        if (_andou)
        {
            energiaAtual -= energiaConsumo * Time.fixedDeltaTime;
            if (energiaAtual < 0) energiaAtual = 0;
        }
        else
        {
            energiaAtual += energiaRecarga * Time.fixedDeltaTime;
            if (energiaAtual > energiaMax) energiaAtual = energiaMax;

            rb.linearVelocity = Vector3.zero;
        }
    }

    // Equivalente aos dois raycasts do Script_BolaPong: reto ate o gol, ou com uma rebatida na parede.
    // Igual ao original, quando a bola rebate na parede o alvo eh o ponto da parede (_hit.point.z), nn o ponto final.
    bool Perigo_Calcular(out float _pos)
    {
        _pos = 0;

        Vector3 _bola = t_arena.InverseTransformPoint(rb_bola.position);
        Vector3 _vel = rb_bola.linearVelocity;

        if (_vel.x * lado <= 0) return false; //bola parada ou indo para o outro lado

        float _tempoGol = (perigoX * lado - _bola.x) / _vel.x;
        if (_tempoGol < 0) return false; //bola ja passou

        float _zNoGol = _bola.z + _vel.z * _tempoGol;

        if (Mathf.Abs(_zNoGol) <= paredeZ) //chega direto no gol
        {
            _pos = _zNoGol;
            return true;
        }

        _pos = _vel.z > 0 ? paredeZ : -paredeZ; //bate na parede antes

        float _tempoParede = (_pos - _bola.z) / _vel.z;
        float _zDepois = _pos - _vel.z * (_tempoGol - _tempoParede);

        return Mathf.Abs(_zDepois) <= paredeZ; //com duas rebatidas o original nn enxerga o perigo
    }
}
