//using System.Collections.Generic;
//using UnityEngine;

//public class ZombieManager : MonoBehaviour
//{
//    // Lista que contiene todos los zombies en la escena
//    public static List<EnemyWandering> allZombies = new List<EnemyWandering>();

//    // Método para que un zombie se registre al manager
//    public static void RegisterZombie(EnemyWandering zombie)
//    {
//        if (!allZombies.Contains(zombie))
//        {
//            allZombies.Add(zombie);
//        }
//    }

//    // Llamado cuando un zombie detecta al jugador
//    public static void AlertAllZombies(Transform player)
//    {
//        foreach (EnemyWandering zombie in allZombies)
//        {
//            zombie.FollowPlayer(player); // Hace que todos los zombies sigan al jugador
//        }
//    }
//}
