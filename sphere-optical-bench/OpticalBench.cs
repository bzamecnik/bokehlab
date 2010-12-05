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

        public List<SphericalCap> Elements { get; set; }

        public List<IntersectionResult> IntersectionResults { get; private set; }

        public OpticalBench()
        {
            IntersectionResults = new List<IntersectionResult>();
            Elements = new List<SphericalCap>();
        }

        public void Update()
        {
            IntersectionResults.Clear();

            IntersectionResult previousResult = new IntersectionResult()
            {
                OutgoingRay = new Ray(IncidentRay)
            };

            double lastRefractiveIndex = RefractiveIndices.AIR;

            int maxIntersections = Elements.Count;
            for (int i = 0; i < maxIntersections; i++)
            {
                IntersectionResult result = new IntersectionResult();
                SphericalCap element = Elements[i];

                result.IncidentRay = new Ray(previousResult.OutgoingRay);

                //ComputeIntersection();

                Point intersection = null;
                result.Intersected = element.IntersectRay(result.IncidentRay, out intersection);
                if (!result.Intersected)
                {
                    break;
                }
                result.Intersection = intersection;

                //ComputeRefractedRay();

                Vector normal = Vector.FromPoint(result.Intersection);
                Vector outgoingDirection = result.IncidentRay.Direction.Length * -Vector.refract(result.IncidentRay.Direction, -normal, lastRefractiveIndex, element.NextRefractiveIndex);
                result.OutgoingRay = new Ray(result.Intersection, outgoingDirection);
                result.Refracted = true; // TODO: differ refraction and TIR
                IntersectionResults.Add(result);
                previousResult = result;
                lastRefractiveIndex = element.NextRefractiveIndex;
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
