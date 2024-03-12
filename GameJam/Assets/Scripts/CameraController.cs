using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera camera;
    [SerializeField]
    private Transform target;


    private void Update()
    {
        //while (!SwitchBackToNormal())
        //{
        //    camera.Priority = 0;
        //}
    }
    void SwitchBackToNormal()
    {
            camera.Priority = 2;
            var normalized = (camera.transform.position - target.position).normalized;
            camera.transform.Translate(normalized*5*Time.deltaTime);
    }
}
