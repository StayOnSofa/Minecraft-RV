using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BlockUtils.Texturing
{
    public class AtlasObject
    {
        private Texture2D _atlas;
        private Texture2D _blured;

        public AtlasObject(Texture2D atlas, Texture2D blured)
        {
            _atlas = atlas;
            _blured = blured;
        }

        public Texture2D GetAtlas()
        {
            return _atlas;
        }

        public Texture2D GetBlured()
        {
            return _blured;
        }
        
    }
}