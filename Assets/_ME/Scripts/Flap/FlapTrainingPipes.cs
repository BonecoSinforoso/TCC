using UnityEngine;

// Par de canos da arena de treino. Mesma logica do Script_canosTudo, mas em posicao local da arena
// e sem depender do Script_FlapManager (singleton), para varias arenas rodarem na mesma cena.
public class FlapTrainingPipes : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float subidaSpeed = 1;
    [SerializeField] private float posMinX = -6;
    [SerializeField] private float canosDist = 6;
    [SerializeField] private FlapTrainingPipes canosOutro;
    [SerializeField] private float alturaMax = 2.5f;

    private bool movel;
    private bool subindo;

    public void Configurar(FlapTrainingPipes _canosOutro)
    {
        canosOutro = _canosOutro;
    }

    public void Reiniciar(float _x, bool _movel)
    {
        movel = _movel;
        subindo = Random.Range(0, 2) == 0;

        transform.localPosition = new Vector3(_x, Random.Range(-alturaMax, alturaMax), 0);
    }

    private void FixedUpdate()
    {
        Vector3 _pos = transform.localPosition;

        _pos.x -= moveSpeed * Time.fixedDeltaTime;

        if (_pos.x < posMinX)
        {
            _pos.x = canosOutro.transform.localPosition.x + canosDist;
            _pos.y = Random.Range(-alturaMax, alturaMax);
        }

        if (movel)
        {
            if (subindo)
            {
                _pos.y += subidaSpeed * Time.fixedDeltaTime;
                if (_pos.y >= alturaMax) subindo = false;
            }
            else
            {
                _pos.y -= subidaSpeed * Time.fixedDeltaTime;
                if (_pos.y <= -alturaMax) subindo = true;
            }
        }

        transform.localPosition = _pos;
    }
}
