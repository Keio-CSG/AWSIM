
using UnityEngine;
using AWSIM.TrafficSimulation;

namespace AWSIM.Scenes.RadarValidation
{
    public class EgoController : MonoBehaviour
    {
        [Header("NPC Vehicle Settings")]
        [SerializeField] private NPCVehicleConfig vehicleConfig = NPCVehicleConfig.Default();

        [SerializeField, Tooltip("Vehicle layer for raytracing the collision distances.")]
        private LayerMask vehicleLayerMask;

        [SerializeField, Tooltip("Ground layer for raytracing the collision distances.")]
        private LayerMask groundLayerMask;

        private GameObject dummyEgo;
        private NPCVehicleSimulator npcVehicleSimulator;

        public TrafficLane startLane;

        public void Awake()
        {
            dummyEgo = new GameObject("DummyEgoController");
            npcVehicleSimulator = new NPCVehicleSimulator(vehicleConfig, vehicleLayerMask, groundLayerMask, 1, dummyEgo);
        }

        public void Start()
        {
            NPCVehicleSpawnPoint sp = new NPCVehicleSpawnPoint(startLane, 0);
            npcVehicleSimulator.Register(GetComponent<NPCVehicle>(), sp.Lane, sp.WaypointIndex);
        }

        public void FixedUpdate()
        {
            npcVehicleSimulator.StepOnce(Time.fixedDeltaTime);
            Despawn();
        }

        private void Despawn()
        {
            foreach (var state in npcVehicleSimulator.VehicleStates)
            {
                if (state.ShouldDespawn)
                {
                    Object.DestroyImmediate(state.Vehicle.gameObject);
                }
            }
            npcVehicleSimulator.RemoveInvalidVehicles();
        }

        private void OnDestroy()
        {
            npcVehicleSimulator?.Dispose();
        }
    }
}
