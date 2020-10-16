using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    /// <summary>
    /// Responsible for positioning and orienting the camera
    /// based on user input.
    /// </summary>
    public class CameraControl : MonoBehaviour
    {
        public float rotateSpeed;
        public float speed;
        public float speedBoost;

        private float cursorPosX;
        private float cursorPosY;
        private float horizontalAngle;
        private float verticalAngle;

        void Start()
        {
            Vector3 euler = GetComponent<Transform>().rotation.eulerAngles;
            horizontalAngle = Mathf.Deg2Rad * euler.y;
            verticalAngle = -Mathf.Deg2Rad * euler.x;
        }

        void FixedUpdate()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            if (Input.GetKey("space"))
            {
                moveHorizontal *= speedBoost;
                moveVertical *= speedBoost;
            }


            if (Input.GetButton("Fire2"))
            {
                cursorPosX = Input.GetAxis("Mouse X");
                cursorPosY = Input.GetAxis("Mouse Y");

                Cursor.lockState = CursorLockMode.Locked;

                horizontalAngle += rotateSpeed * cursorPosX;
                verticalAngle += rotateSpeed * cursorPosY;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }

            verticalAngle = Mathf.Clamp(verticalAngle, -Mathf.PI / 2.0f, Mathf.PI / 2.0f);


            Vector3 direction = new Vector3(
                Mathf.Cos(verticalAngle) * Mathf.Sin(horizontalAngle),
                Mathf.Sin(verticalAngle),
                Mathf.Cos(verticalAngle) * Mathf.Cos(horizontalAngle)
            );

            Vector3 right = new Vector3(
                Mathf.Sin(horizontalAngle - 3.14f / 2.0f),
                0,
                Mathf.Cos(horizontalAngle - 3.14f / 2.0f)
            );

            Vector3 up = Vector3.Cross(right, direction);

            GetComponent<Transform>().Translate(speed * moveHorizontal, 0, speed * moveVertical, Space.Self);
            GetComponent<Transform>().LookAt(GetComponent<Transform>().position + direction, up);
        }
    }
}
