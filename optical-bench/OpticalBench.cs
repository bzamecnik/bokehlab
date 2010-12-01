using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class OpticalBench
    {
        Vector direction = new Vector(0.0, -1.0);
        public Vector Direction
        {
            get { return direction; }
            set {
                direction = value;
                Update();
            }
        }

        public Vector RefractedDirection { get; private set; }

        public double RefractiveIndexGlass { get; set; }
        public double RefractiveIndexAir { get; set; }

        public OpticalBench()
        {
            RefractiveIndexGlass = 1.52; // crown glass
            RefractiveIndexAir = 1.00029;

            RefractedDirection = direction;
        }

        public void Update() {
            ComputeRefractedRay();
        }

        private void ComputeRefractedRay()
        {
            Vector normal = new Vector(0.0, 100.0);
            RefractedDirection = Vector.refract(Direction, normal, RefractiveIndexAir, RefractiveIndexGlass);
            RefractedDirection *= Direction.Length;
        }
    }
}
