using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cameraTransform;

    void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        //MAKE THAT SHIT LOOK AT THE CAMERA
        transform.forward = cameraTransform.forward;
    }
}
