using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (Manager.Instance.activeSnake == true)
        {
            //Add 1 node to snake
            Manager.Instance.AddBodyPart();

            //Add 1 to score
            Manager.Instance.scoreCounter += 1;
            Manager.Instance.speed += 0.025f;
            Manager.Instance.SetCountText();

            //Recycle sphere to a different position
            Vector3 BallPosition = new Vector3(Random.Range(-8, 8), 0, Random.Range(-8, 8));
            Manager.Instance.spawn_sphere(BallPosition);
        }
    }
}
