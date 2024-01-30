using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenu
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] BridgeManager bridgeManager;
        private Camera cam;

        private Coroutine moveCameraCoroutine;

        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            // Start moving the camera towards the spawner position when it's time to spawn a bridge
            if (moveCameraCoroutine == null)
            {
                Vector3 targetPos = cam.transform.position + Vector3.forward * bridgeManager.GetSpawnerOffsetLength();
                targetPos.y = cam.transform.position.y;
                moveCameraCoroutine = StartCoroutine(MoveCameraCoroutine(targetPos));
            }
        }

        private IEnumerator MoveCameraCoroutine(Vector3 targetPosition)
        {
            float elapsedTime = 0f;
            Vector3 initialPosition = cam.transform.position;

            while (elapsedTime < bridgeManager.GetSpawnTimer())
            {
                // Move the camera towards the target position based on the exact distance to cover
                cam.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / bridgeManager.GetSpawnTimer());
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Ensure the final position is exactly the target position
            cam.transform.position = targetPosition;

            moveCameraCoroutine = null;
        }
    }
}
