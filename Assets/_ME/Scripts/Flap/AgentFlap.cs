using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

// Agente treinado (ML-Agents) do passaro do Flap.
// Na cena de treino quem reinicia os canos eh o FlapTrainingArena; na cena do jogo o campo "arena" fica vazio.
//
// Observacoes (8): altura, velocidade vertical, distancia X e diferenca de altura do proximo cano,
//                  diferenca de altura do cano seguinte, distancia ao chao, se pode pular,
//                  velocidade vertical do proximo cano (canos moveis).
// Acao (1 ramo discreto de 2): 0 nada, 1 pular.
public class AgentFlap : Agent
{
    public const int observacoesQuant = 8;
    public const int acoesQuant = 2;

    [SerializeField] float puloForca = 5;
    [SerializeField] float puloCd = 0.2f;
    [SerializeField] Transform[] t_cano;
    [SerializeField] float chaoY = -5f;
    [SerializeField] float tetoY = 7f; //acima disso conta como batida (evita passar por cima dos canos)

    [Header("Treino")]
    [SerializeField] FlapTrainingArena arena;
    [SerializeField] float recompensaPonto = 1f;
    [SerializeField] float recompensaBater = -1f;
    [SerializeField] float recompensaPorDecisao = 0.002f;

    //normalizacao das observacoes
    const float alturaNorm = 5f;
    const float velocidadeNorm = 10f;
    const float canoDistNorm = 6f; //distancia entre dois canos
    const float canoPassou = 1f; //meia largura do cano + raio do passaro

    Rigidbody rb;
    Transform t_centro; //a arena no treino, o mundo no jogo
    float puloTempo;
    float[] canoAlturaAnterior;
    float[] canoVelocidadeY;

    public int Pontos { get; private set; }
    public bool Morreu { get; private set; }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        t_centro = arena != null ? arena.transform : null;

        canoAlturaAnterior = new float[t_cano.Length];
        canoVelocidadeY = new float[t_cano.Length];
        for (int i = 0; i < t_cano.Length; i++) canoAlturaAnterior[i] = PosLocal(t_cano[i].position).y;
    }

    public void Configurar(FlapTrainingArena _arena, Transform[] _t_cano)
    {
        arena = _arena;
        t_cano = _t_cano;
    }

    public override void OnEpisodeBegin()
    {
        if (arena == null) return; //no jogo a partida nn reinicia sozinha

        Pontos = 0;
        Morreu = false;
        puloTempo = 0;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(PosMundo(new Vector3(0, Random.Range(-1f, 1f), 0)), Quaternion.identity);

        arena.Reiniciar();

        for (int i = 0; i < t_cano.Length; i++)
        {
            canoAlturaAnterior[i] = PosLocal(t_cano[i].position).y;
            canoVelocidadeY[i] = 0;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 _pos = PosLocal(transform.position);

        CanosProximos_Get(_pos.x, out int _cano1, out int _cano2);

        sensor.AddObservation(Mathf.Clamp(_pos.y / alturaNorm, -1.5f, 1.5f));
        sensor.AddObservation(Mathf.Clamp(rb.linearVelocity.y / velocidadeNorm, -1.5f, 1.5f));

        if (_cano1 >= 0)
        {
            Vector3 _cano = PosLocal(t_cano[_cano1].position);
            sensor.AddObservation(Mathf.Clamp((_cano.x - _pos.x) / canoDistNorm, -1f, 2f));
            sensor.AddObservation(Mathf.Clamp((_cano.y - _pos.y) / alturaNorm, -2f, 2f));
        }
        else
        {
            sensor.AddObservation(2f);
            sensor.AddObservation(0f);
        }

        sensor.AddObservation(_cano2 >= 0 ? Mathf.Clamp((PosLocal(t_cano[_cano2].position).y - _pos.y) / alturaNorm, -2f, 2f) : 0f);
        sensor.AddObservation(Mathf.Clamp((_pos.y - chaoY) / (alturaNorm * 2), 0f, 1.5f));
        sensor.AddObservation(puloTempo <= 0 ? 1f : 0f);
        sensor.AddObservation(_cano1 >= 0 ? Mathf.Clamp(canoVelocidadeY[_cano1], -1f, 1f) : 0f);
    }

    // Proximo cano = o mais perto que ainda nn ficou para tras; seguinte = o que vem depois dele.
    void CanosProximos_Get(float _x, out int _cano1, out int _cano2)
    {
        _cano1 = -1;
        _cano2 = -1;
        float _dist1 = float.MaxValue;
        float _dist2 = float.MaxValue;

        for (int i = 0; i < t_cano.Length; i++)
        {
            float _dist = PosLocal(t_cano[i].position).x - _x;
            if (_dist < -canoPassou) continue;

            if (_dist < _dist1)
            {
                _cano2 = _cano1;
                _dist2 = _dist1;
                _cano1 = i;
                _dist1 = _dist;
            }
            else if (_dist < _dist2)
            {
                _cano2 = i;
                _dist2 = _dist;
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (Morreu) return;

        if (actions.DiscreteActions[0] == 1) Pulo();

        AddReward(recompensaPorDecisao);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> _acoes = actionsOut.DiscreteActions;

        _acoes[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    void FixedUpdate()
    {
        if (puloTempo > 0) puloTempo -= Time.fixedDeltaTime;

        for (int i = 0; i < t_cano.Length; i++)
        {
            float _altura = PosLocal(t_cano[i].position).y;
            canoVelocidadeY[i] = (_altura - canoAlturaAnterior[i]) / Time.fixedDeltaTime;
            canoAlturaAnterior[i] = _altura;
        }

        if (!Morreu && PosLocal(transform.position).y > tetoY) Bateu();
    }

    void Pulo()
    {
        if (puloTempo > 0) return;
        puloTempo = puloCd;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);
    }

    //placar desde o Play (somado em todas as arenas), para medir o modelo
    public static int Episodios;
    public static int PontosTotal;
    public static int PontosMax;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Placar_Reset()
    {
        Episodios = 0;
        PontosTotal = 0;
        PontosMax = 0;
    }

    void Bateu()
    {
        Episodios++;
        PontosTotal += Pontos;
        if (Pontos > PontosMax) PontosMax = Pontos;

        AddReward(recompensaBater);

        if (arena != null) EndEpisode();
        else Morreu = true; //no jogo o passaro so para de agir; quem decide o fim eh o Script_FlapManager
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Morreu) return;

        if (collision.gameObject.CompareTag("Chao")) Bateu();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Morreu) return;

        if (other.CompareTag("Cano")) Bateu();

        if (other.CompareTag("Ponto"))
        {
            Pontos++;
            AddReward(recompensaPonto);
        }
    }

    Vector3 PosLocal(Vector3 _pos)
    {
        return t_centro != null ? t_centro.InverseTransformPoint(_pos) : _pos;
    }

    Vector3 PosMundo(Vector3 _pos)
    {
        return t_centro != null ? t_centro.TransformPoint(_pos) : _pos;
    }
}
