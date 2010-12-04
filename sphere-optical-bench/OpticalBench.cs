using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public class OpticalBench
    {
        Ray incidentRay = new Ray();
        public Ray IncidentRay
        {
            get { return incidentRay; }
            set
            {
                incidentRay = value;
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

        //public List<IntersectionResult> IntersectionResults { get; private set; }
        public List<IntersectionResult> IntersectionResults { get; private set; }

        //public Point Intersection { get; private set; }
        //public bool RayIntersects { get; private set; }
        //public Ray RefractedRay { get; private set; }

        public double RefractiveIndexGlass { get; set; }
        public double RefractiveIndexAir { get; set; }

        public OpticalBench()
        {
            RefractiveIndexGlass = 1.52; // crown glass
            RefractiveIndexAir = 1.00029;
            IntersectionResults = new List<IntersectionResult>();
        }

        public void Update()
        {
            IntersectionResults.Clear();

            IntersectionResult previousResult = new IntersectionResult()
            {
                OutgoingRay = new Ray(IncidentRay)
            };

            const int maxIntersections = 2;
            for (int i = 0; i < maxIntersections; i++)
            {
                IntersectionResult result = new IntersectionResult();

                result.IncidentRay = new Ray(previousResult.OutgoingRay);

                //ComputeIntersection();

                Point intersection = null;
                result.Intersected = Sphere.IntersectRay(result.IncidentRay, out intersection);
                if (!result.Intersected)
                {
                    break;
                }
                result.Intersection = intersection;

                //ComputeRefractedRay();

                Vector normal = Vector.FromPoint(result.Intersection);
                Vector outgoingDirection = result.IncidentRay.Direction.Length * -Vector.refract(result.IncidentRay.Direction, -normal, RefractiveIndexAir, RefractiveIndexGlass);
                result.OutgoingRay = new Ray(result.Intersection, outgoingDirection);
                result.Refracted = true; // TODO: differ refraction and TIR
                IntersectionResults.Add(result);
                previousResult = result;
            }
        }

        public class IntersectionResult {
            public Ray IncidentRay { get; set; }
            public Ray OutgoingRay { get; set; }
            public Point Intersection { get; set; }
            public bool Intersected { get; set; }
            public bool Refracted { get; set; }
            public bool Reflected { get; set; }
        }
    }
}
