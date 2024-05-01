using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraEvent : MonoBehaviour
{
    public static PlayerCameraEvent Instance;
    [HideInInspector] public bool IsLookingAtObject = false;
    [HideInInspector] public bool AllowedToMove = true;

    private void Awake()
    {
        Instance = this;
    }

    public void RotateToObjects(CameraObject[] _objects, bool _stopMovement)
    {
        StartCoroutine(moveCameraToObjects(_objects, _stopMovement));
    }

    IEnumerator moveCameraToObjects(CameraObject[] _objects, bool _stopMovement)
    {
        IsLookingAtObject = true;
        // disabling the rotation calculations controlled by the mouse
        PlayerMovement.Instance.AllowedToLook = false;

        // disabling the player's movement if required 
        if (_stopMovement)
        {
            PlayerMovement.Instance.AllowMovement(false);
        }

        // looping through all objects that we want to look at
        for (int i = 0; i < _objects.Length; i++)
        {
            CameraObject objectToLookAt = _objects[i];
            Quaternion bodyRotation = transform.rotation;
            Quaternion cameraRotation = PlayerMovement.Instance.CameraTransform.rotation;
            float time = 0f;
            // create directions for the y and x axis independatly
            Vector3 bodyDir = (objectToLookAt.Object.position - transform.position).normalized;
            Vector3 camDir = (objectToLookAt.Object.position - PlayerMovement.Instance.CameraTransform.position).normalized;
            bodyDir.y = 0f;
            // assign those direction to quaternion vars to apply them to the rotation
            Quaternion bodyRot = Quaternion.LookRotation(bodyDir);
            Quaternion camRot = Quaternion.LookRotation(camDir);

            // type the message to the dialogue
            DialogueManager.Instance.TypeMessage(objectToLookAt.Message);

            // move the camera and the player to face the object
            while (time < objectToLookAt.RotationDuration)
            {
                time += Time.deltaTime;
                float curveT = time / objectToLookAt.RotationDuration;
                float t = objectToLookAt.SmoothnessCurve.Evaluate(curveT);
                // gets the interpolation amount
                // applies it to the rotations
                transform.rotation = Quaternion.Lerp(bodyRotation, bodyRot, t);
                PlayerMovement.Instance.CameraTransform.rotation = Quaternion.Lerp(cameraRotation, camRot, t);
                yield return null;
            }

            // wait before moving to the next one
            float waitingTime = DialogueManager.Instance.GetMessageDuration(objectToLookAt.Message.Message) + objectToLookAt.LookingDuration;
            yield return new WaitForSeconds(waitingTime);
        }

        IsLookingAtObject = false;

        PlayerMovement.Instance.RestoreRotations();
        // enabling looking with the mouse
        PlayerMovement.Instance.AllowedToLook = true;

        // enabling player movement again if it was disabled in the first place
        if (_stopMovement)
        {
            PlayerMovement.Instance.AllowMovement(true);
        }
    }
}
