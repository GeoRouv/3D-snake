using System.Collections.Generic;
using UnityEngine;

public class onCollision : MonoBehaviour
{
    [SerializeField] private float force;
    private bool explode = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.layer == 17)
        {
            
            //Edw kanw to explode
            Physics.IgnoreLayerCollision(9, 17);
            Physics.IgnoreLayerCollision(9, 9);

            Manager.Instance.activeSnake = false;

            if (explode)
            {
                List<GameObject> cubes = Manager.Instance.spawnedbodyPart;
                
                foreach (GameObject cube in cubes)
                {
                    cube.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-0.000002f, 0.000002f), 0.000001f, Random.Range(-0.000002f, 0.000002f)) * force);
                }
                explode = false;

                Manager.Instance.Crash();
            }
        }
    }
}
