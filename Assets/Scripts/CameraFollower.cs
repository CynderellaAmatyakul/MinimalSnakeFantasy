using UnityEngine;
using Cinemachine;

public class CameraFollower : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public HeroController heroController;

    void LateUpdate()
    {
        if (heroController == null || heroController.heroChain.Count == 0) return;

        GameObject head = heroController.heroChain[0];
        if (virtualCam.Follow != head.transform)
        {
            virtualCam.Follow = head.transform;
        }
    }
}