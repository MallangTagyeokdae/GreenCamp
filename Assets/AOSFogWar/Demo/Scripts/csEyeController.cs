/*
 * Created :    Winter 2022
 * Author :     SeungGeon Kim (keithrek@hanmail.net)
 * Project :    FogWar
 * Filename :   csEyeController.cs (non-static monobehaviour module)
 * 
 * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
 */



using UnityEngine;  // Monobehaviour



namespace FischlWorks_FogWar
{



    public class csEyeController : MonoBehaviour
    {
        private Vector3 currentSpeed = new Vector3();

        [SerializeField]
        [Range(0, 10)]
        private float acceleration = 6;

        [SerializeField]
        [Range(0, 10)]
        private float speedXLimit = 6;

        [SerializeField]
        [Range(0, 10)]
        private float speedYLimit = 6;



        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (currentSpeed.y < speedYLimit)
                {
                    currentSpeed.y += acceleration * Time.deltaTime;
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (currentSpeed.y > -speedYLimit)
                {
                    currentSpeed.y -= acceleration * Time.deltaTime;
                }
            }
            else
            {
                if (currentSpeed.y > acceleration * Time.deltaTime)
                {
                    currentSpeed.y -= acceleration * Time.deltaTime;
                }
                else if (currentSpeed.y < -acceleration * Time.deltaTime)
                {
                    currentSpeed.y += acceleration * Time.deltaTime;
                }
                else
                {
                    currentSpeed.y = 0;
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                if (currentSpeed.x > -speedXLimit)
                {
                    currentSpeed.x -= acceleration * Time.deltaTime;
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (currentSpeed.x < speedXLimit)
                {
                    currentSpeed.x += acceleration * Time.deltaTime;
                }
            }
            else
            {
                if (currentSpeed.x > acceleration * Time.deltaTime)
                {
                    currentSpeed.x -= acceleration * Time.deltaTime;
                }
                else if (currentSpeed.x < -acceleration * Time.deltaTime)
                {
                    currentSpeed.x += acceleration * Time.deltaTime;
                }
                else
                {
                    currentSpeed.x = 0;
                }
            }

            transform.Translate(new Vector3(currentSpeed.x, 0, currentSpeed.y) * Time.deltaTime);
        }
    }



}