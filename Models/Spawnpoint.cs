using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WarDotCom.Models
{
    public struct Spawnpoint(int x, int y, int z, int yaw)
    {
        public float X { get; } = x;
        public float Y { get; } = y;
        public float Z { get; } = z;
        public float Yaw { get; } = yaw;

        public Vector3 Position = new(x, y, z);
    }
}
