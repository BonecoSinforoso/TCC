using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

// Agente treinado (ML-Agents) da raquete do Pong.
// Funciona dos dois lados da mesa: as observacoes sao espelhadas pelo lado em que a raquete esta.
// Na cena de treino quem reinicia a partida eh o PongTrainingArena; na cena do jogo o campo "arena" fica vazio.
//
// Observacoes (9): z da raquete, x e z da bola, velocidade x e z da bola, diferenca de z bola-raquete,
//                  energia, bola vindo na minha direcao, ponto previsto de chegada da bola.
// Acao (1 ramo discreto de 3): 0 parado, 1 sobe (+z), 2 desce (-z).
public class AgentPong : Agent
{
    public const int observacoesQuant = 9;
    public const int acoesQuant = 3;

    [SerializeField] private float moveSpeed = 7;
    [SerializeField] private float posLimiteZ = 3.5f;
    [SerializeField] private Transform t_bola;
    [SerializeField] private Rigidbody rb_bola;

    [Header("Energia (mesmas regras do Script_IaPong)")]
    [SerializeField] private float energiaMax = 100;
    [SerializeField] private float energiaAtual = 100;
    [SerializeField] private float energiaConsumo = 100;
    [SerializeField] private float energiaRecarga = 25;
    [SerializeField] private float energiaMinMover = 5;
    [SerializeField] private Transform t_energiaBarra;

    [Header("Treino")]
    [SerializeField] private PongTrainingArena arena;
    [SerializeField] private float recompensaRebater = 1f;
    [SerializeField] private float recompensaGolSofrido = -1f;
    [SerializeField] private float recompensaGolFeito = 1f;
    [SerializeField] private float recompensaPorStep = 0f; //penalidade por existir (opcional); ligada ela premia tomar gol logo

    //normalizacao das observacoes
    private const float campoX = 11f;
    private const float campoZ = 5.5f;
    private const float bolaSpeedMax = 12f;
    private const float bolaLimiteZ = 5.25f; //parede (5.5) menos o raio da bola (0.25)

    private Rigidbody rb;
    private Transform t_centro; //referencia do meio da mesa (a arena no treino, o mundo no jogo)
    private float lado; //-1 = raquete da esquerda, 1 = raquete da direita
    private int acaoAtual;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        t_centro = arena != null ? arena.transform : null;
        lado = PosLocal(transform.position).x < 0 ? -1 : 1;
    }

    public void Configurar(PongTrainingArena _arena, Transform _t_bola, Rigidbody _rb_bola, Transform _t_energiaBarra)
    {
        arena = _arena;
        t_bola = _t_bola;
        rb_bola = _rb_bola;
        t_energiaBarra = _t_energiaBarra;
    }

    public override void OnEpisodeBegin()
    {
        acaoAtual = 0;

        if (arena == null) return; //no jogo quem controla a partida eh o Script_PongManager

        energiaAtual = energiaMax;
        rb.linearVelocity = Vector3.zero;

        Vector3 _pos = PosLocal(transform.position);
        _pos.z = Random.Range(-posLimiteZ, posLimiteZ);
        transform.position = PosMundo(_pos);

        arena.Reiniciar();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 _raquete = PosLocal(transform.position);
        Vector3 _bola = PosLocal(t_bola.position);
        Vector3 _bolaVel = rb_bola.linearVelocity;

        sensor.AddObservation(_raquete.z / posLimiteZ);
        sensor.AddObservation(Mathf.Clamp(_bola.x * lado / campoX, -1f, 1f)); //1 = bola no meu gol
        sensor.AddObservation(Mathf.Clamp(_bola.z / campoZ, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(_bolaVel.x * lado / bolaSpeedMax, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(_bolaVel.z / bolaSpeedMax, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp((_bola.z - _raquete.z) / (campoZ * 2), -1f, 1f));
        sensor.AddObservation(energiaAtual / energiaMax);
        sensor.AddObservation(_bolaVel.x * lado > 0 ? 1f : 0f); //bola vindo em direcao ao meu gol
        sensor.AddObservation(Mathf.Clamp(BolaChegada_Calcular(_raquete, _bola, _bolaVel) / (campoZ * 2), -1f, 1f));
    }

    // Onde a bola vai cruzar a linha da raquete (ja contando as rebatidas nas paredes), relativo ao z da raquete.
    // Eh a mesma informacao que o agente reativo tira dos raycasts de trajetoria do Script_BolaPong.
    // Com a bola indo para o outro lado devolve 0.
    private float BolaChegada_Calcular(Vector3 _raquete, Vector3 _bola, Vector3 _bolaVel)
    {
        if (_bolaVel.x * lado <= 0) return 0;

        float _tempo = (_raquete.x - _bola.x) / _bolaVel.x;
        if (_tempo < 0) return 0; //bola ja passou da raquete

        //"desdobra" o caminho da bola como se as paredes fossem espelhos
        float _z = Mathf.Repeat(_bola.z + _bolaVel.z * _tempo + bolaLimiteZ, bolaLimiteZ * 4);
        _z = _z < bolaLimiteZ * 2 ? _z - bolaLimiteZ : bolaLimiteZ * 3 - _z;

        return _z - _raquete.z;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        acaoAtual = actions.DiscreteActions[0];

        AddReward(recompensaPorStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> _acoes = actionsOut.DiscreteActions;
        float _v = Input.GetAxisRaw("Vertical");

        _acoes[0] = _v > 0 ? 1 : _v < 0 ? 2 : 0;
    }

    private void FixedUpdate()
    {
        float _z = PosLocal(transform.position).z;
        bool _andou = false;

        if (energiaAtual > energiaMinMover)
        {
            if (acaoAtual == 1 && _z < posLimiteZ)
            {
                rb.linearVelocity = Vector3.forward * moveSpeed;
                _andou = true;
            }
            else if (acaoAtual == 2 && _z > -posLimiteZ)
            {
                rb.linearVelocity = Vector3.back * moveSpeed;
                _andou = true;
            }
        }

        if (_andou) //se andou diminui a energia
        {
            energiaAtual -= energiaConsumo * Time.fixedDeltaTime;
            if (energiaAtual < 0) energiaAtual = 0;
        }
        else //se nn andou, aumenta a energia e para a raquete
        {
            energiaAtual += energiaRecarga * Time.fixedDeltaTime;
            if (energiaAtual > energiaMax) energiaAtual = energiaMax;

            rb.linearVelocity = Vector3.zero;
        }

        if (t_energiaBarra != null) t_energiaBarra.localScale = new Vector3(1, 1, energiaAtual / energiaMax);
    }

    //chamados pela arena de treino
    public void Rebateu()
    {
        AddReward(recompensaRebater);
    }

    public void GolSofrido()
    {
        AddReward(recompensaGolSofrido);
        EndEpisode();
    }

    public void GolFeito()
    {
        AddReward(recompensaGolFeito);
        EndEpisode();
    }

    private Vector3 PosLocal(Vector3 _pos)
    {
        return t_centro != null ? t_centro.InverseTransformPoint(_pos) : _pos;
    }

    private Vector3 PosMundo(Vector3 _pos)
    {
        return t_centro != null ? t_centro.TransformPoint(_pos) : _pos;
    }
}
