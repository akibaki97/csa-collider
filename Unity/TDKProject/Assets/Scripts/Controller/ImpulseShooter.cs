using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.CollisionDetection;
using Model.PhysicsSimulation;

namespace Controller
{
    /// <summary>
    /// Responsible for shooting impulses onto CSACollider objects
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ImpulseShooter : MonoBehaviour
    {
        public CollisionDispatcher collisionDispatcher;
        public float impulse;

        private Camera cam;
        
        void Start()
        {
            cam = GetComponent<Camera>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (collisionDispatcher.Raycast(ray, out DynamicCollider hit, 150.0f))
                {
                    if (hit.transform)
                    {
                        RigidBody hittedBody = hit.GetComponent<RigidBody>();

                        float objectPlaneDist = Vector3.Dot(hit.transform.position - cam.transform.position, cam.transform.forward);
                        Vector2 dist = cam.WorldToScreenPoint(hit.transform.position) - Input.mousePosition;


                        Vector3 currPt = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectPlaneDist);
                        Vector3 worldPt = cam.ScreenToWorldPoint(currPt);

                        Vector3 relHitPt = worldPt - hit.transform.position;
                        hittedBody.AddImpulse(impulse * cam.transform.forward, relHitPt);
                    }
                }
            }
        }
    }
}
