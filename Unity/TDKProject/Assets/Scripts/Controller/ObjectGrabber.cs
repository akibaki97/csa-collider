using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.CollisionDetection;
using Model.PhysicsSimulation;

namespace Controller
{
    /// <summary>
    /// Responsible for drag and drop 3D objects with
    /// CSACollider component.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ObjectGrabber : MonoBehaviour
    {
        /// <value> The dispatcher holding the list of CSAColliders</value>
        public CollisionDispatcher collisionDispatcher;

        private Camera cam;
        private float objectPlaneDist;
        private GameObject grabbedObj;
        private Vector2 dist;

        // Start is called before the first frame update
        void Start()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (collisionDispatcher.Raycast(ray, out DynamicCollider hit, 150.0f))
                {
                    if (hit.transform)
                    {
                        grabbedObj = hit.gameObject;
                        objectPlaneDist = Vector3.Dot(grabbedObj.transform.position - cam.transform.position, cam.transform.forward);
                        RigidBody rb;
                        dist = cam.WorldToScreenPoint(grabbedObj.transform.position) - Input.mousePosition;
                        if (rb = grabbedObj.GetComponent<RigidBody>())
                        {
                            rb.LockMotion();
                        }
                    }
                }
                else
                {
                    grabbedObj = null;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (grabbedObj)
                {
                    RigidBody rb;
                    if (rb = grabbedObj.GetComponent<RigidBody>())
                    {
                        Vector3 currPt = new Vector3(Input.mousePosition.x + dist.x, Input.mousePosition.y + dist.y, objectPlaneDist);
                        Vector3 worldPt = cam.ScreenToWorldPoint(currPt);

                        Vector3 velocity = (worldPt - rb.transform.position) / (10 * Time.deltaTime) * 2;
                        rb.UnLockMotion(velocity);
                    }
                    grabbedObj = null;
                }
            }

            if (Input.GetButton("Fire1"))
            {
                if (grabbedObj)
                {
                    float angle = 0;
                    if (Input.GetKey(KeyCode.R))
                    {
                        angle = 10 * Input.mouseScrollDelta.y;
                    }
                    else
                    {
                        objectPlaneDist += 3 * Input.mouseScrollDelta.y;
                    }                    

                    Vector3 currPt = new Vector3(Input.mousePosition.x + dist.x, Input.mousePosition.y + dist.y, objectPlaneDist);
                    Vector3 worldPt = cam.ScreenToWorldPoint(currPt);

                    grabbedObj.transform.position = worldPt;
                    grabbedObj.transform.RotateAround(worldPt, cam.transform.forward, angle);
                }
            }
        }
    }
}


