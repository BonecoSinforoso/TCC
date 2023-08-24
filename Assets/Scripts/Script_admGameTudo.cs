using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Script_admGameTudo : MonoBehaviour
{
    [SerializeField] Text txt_playerDist;
    [SerializeField] Text txt_iaDist;
    [SerializeField] Text txt_mlDist;

    [SerializeField] GameObject obj_player;
    [SerializeField] GameObject obj_ia;
    [SerializeField] GameObject obj_ml;

    [SerializeField] Transform[] t_camera;
    [SerializeField] float distanciaZ;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        t_camera[0].position = Vector3.Lerp(t_camera[0].position, new Vector3(t_camera[0].position.x, 3, obj_player.transform.position.z - distanciaZ), 1f);
        t_camera[1].position = Vector3.Lerp(t_camera[1].position, new Vector3(t_camera[1].position.x, 3, obj_ia.transform.position.z - distanciaZ), 1f);
        t_camera[2].position = Vector3.Lerp(t_camera[2].position, new Vector3(t_camera[2].position.x, 3, obj_ml.transform.position.z - distanciaZ), 1f);

        txt_playerDist.text = obj_player.transform.position.z.ToString() + "m";
        txt_iaDist.text = obj_ia.transform.position.z.ToString() + "m";
        txt_mlDist.text = obj_ml.transform.position.z.ToString() + "m";
    }
}