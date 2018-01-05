namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [ExecuteInEditMode]
    [AddComponentMenu("Rendering/Homogeneous Fog", -1)]
    public class HomogeneousFog : MonoBehaviour
    {
        public VolumeParameters volumeParameters = new VolumeParameters();

        private void Awake()
        {
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void Update()
        {
        }

        private void OnValidate()
        {
            volumeParameters.Constrain();
        }

        void OnDrawGizmos()
        {
            if (volumeParameters != null && !volumeParameters.IsVolumeUnbounded())
            {
                Gizmos.DrawWireCube(volumeParameters.bounds.center, volumeParameters.bounds.size);
            }
        }
    }
} // UnityEngine.Experimental.Rendering.HDPipeline
