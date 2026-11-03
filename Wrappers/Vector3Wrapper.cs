using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WarDotCom.Wrappers
{
    public class Vector3Wrapper(Vector3 source)
    {
        public float x { get; set; } = source.x;
        public float y { get; set; } = source.y;
        public float z { get; set; } = source.z;
    }
}
