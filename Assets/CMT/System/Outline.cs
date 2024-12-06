using UnityEngine;

namespace CMT
{
    public class Outline : MonoBehaviour
    {
        private MaterialPropertyBlock _mbp;
        [SerializeField] private Renderer _renderer;
        private void Start()
        {
            _mbp = new MaterialPropertyBlock();
        }

        public void Highlight()
        {
            _mbp.SetInt("_Enable", 1);
            _renderer.SetPropertyBlock(_mbp);
        }

        public void UnHighlight()
        {
            _mbp.SetInt("_Enable", 0);
            _renderer.SetPropertyBlock(_mbp);
        }
    }
}
