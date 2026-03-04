using UnityEngine;
using System;

namespace FullMetalAcorn
{
    public class GameController : MonoBehaviour
    {

        [SerializeField] private GameObject playerPrefab;
        private GameObject player;
        public static Action<GameObject> OnPlayerSpawned;

         void Awake()
        {
            player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            OnPlayerSpawned?.Invoke(player);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
