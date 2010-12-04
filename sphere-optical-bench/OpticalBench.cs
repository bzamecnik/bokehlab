using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class OpticalBench
    {
        Ray ray = new Ray();
        public Ray Ray
        {
            get { return ray; }
            set
            {
                ray = value;
                Update();
            }
        }

        Sphere sphere = new Sphere();
        public Sphere Sphere
        {
            get { return sphere; }
            set
            {
                sphere = value;
                Update();
            }
        }

        public Point Intersection { get; private set; }
        public bool RayIntersects { get; private set; }
        public Ray RefractedRay { get; private set; }

        public double RefractiveIndexGlass { get; set; }
        public double RefractiveIndexAir { get; set; }

        public OpticalBench()
        {
            //RefractiveIndexGlass = 1.52; // crown glass
            RefractiveIndexGlass = 2.0; // crown glass
            RefractiveIndexAir = 1.00029;

            RefractedRay = new Ray();
        }

        public void Update()
        {
            ComputeIntersection();
            ComputeRefractedRay();
        }

        private void ComputeIntersection()
        {
            Point intersection;
            RayIntersects = Sphere.IntersectRay(Ray, out intersection);
            Intersection = intersection;
        }

        private void ComputeRefractedRay()
        {
            if (RayIntersects)
            {
                Vector normal = Vector.FromPoint(Intersection);
                RefractedRay.Origin = Intersection;
                RefractedRay.Direction = ray.Direction.Length * -Vector.refract(ray.Direction, -normal, RefractiveIndexAir, RefractiveIndexGlass);
            }
            else
            {
                RefractedRay = new Ray();
            }
        }
    }
}
