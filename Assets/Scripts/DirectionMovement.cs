using System;
using UnityEngine;

public class DirectionMovement : MonoBehaviour
{
   [SerializeField] private Vector3 direction;
   [SerializeField] private float speed;
   public bool isGameOver { get; set; }
   private void Update()
   {
      if (isGameOver)
      {
         return;
      }
      transform.position += direction * Time.deltaTime * speed;
   }
}
