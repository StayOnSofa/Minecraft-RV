using BlockUtils;
using BlockUtils.Texturing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Render.Block
{
    public class BlockMaterialLoader : MonoBehaviour
    {
        [SerializeField] private Material _material;
        public void Init(BlockHandler blockHandler)
        {
            AtlasObject aObject = blockHandler.GetAtlasObject();

            _material.SetTexture("_MainTex", aObject.GetAtlas());
            _material.SetTexture("_DistanceTex", aObject.GetBlured());
        }
        private void FixedUpdate()
        {
            if (Camera.main != null)
            {
                Vector4 positon = Camera.main.transform.position;
                _material.SetVector("_CameraPositon", positon);
            }
        }
    }
}
