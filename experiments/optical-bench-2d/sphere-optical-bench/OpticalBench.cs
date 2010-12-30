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

        public double LensCenter { get; set; }

        public List<OpticalElement> Elements { get; set; }

        public List<IntersectionResult> IntersectionResults { get; private set; }

        public OpticalBench()
        {
            LensCenter = 0.0;
            IntersectionResults = new List<IntersectionResult>();
            Elements = new List<OpticalElement>();
        }

        public void Update()
        {
            IntersectionResults.Clear();

            IntersectionResult previousResult = new IntersectionResult()
            {
                OutgoingRay = new Ray(IncidentRay)
            };

            double lastRefractiveIndex = RefractiveIndices.AIR;
            Vector translationFromLensCenter = new Vector(0.0, 0.0);

            int maxIntersections = Elements.Count;
            for (int i = 0; i < maxIntersections; i++)
            {
                IntersectionResult result = new IntersectionResult();
                OpticalElement element = Elements[i];

                result.IncidentRay = new Ray(previousResult.OutgoingRay);

                Vector toLocal = element.TranslationToLocal();

                // compute intersection

                Point intersection = null;
                Ray rayIncidentToElement = result.IncidentRay.Translate(translationFromLensCenter + toLocal);
                result.Intersected = element.IntersectRay(rayIncidentToElement, out intersection);
                result.Normal = Vector.FromPoint(intersection);
                intersection = intersection - (translationFromLensCenter + toLocal);
                if (!result.Intersected)
                {
                    break;
                }
                result.Intersection = intersection;

                // compute refracted ray

                Vector outgoingDirection;
                if (Math.Abs(lastRefractiveIndex - element.NextRefractiveIndex) > double.Epsilon)
                {
                    outgoingDirection = result.IncidentRay.Direction.Length * Vector.refract(result.IncidentRay.Direction, result.Normal, lastRefractiveIndex, element.NextRefractiveIndex);
                }
                else
                {
                    // there's no border between different media and thus no refraction
                    outgoingDirection = result.IncidentRay.Direction;
                }
                result.OutgoingRay = new Ray(result.Intersection, outgoingDirection);
                result.Refracted = true; // TODO: differ refraction and TIR
                IntersectionResults.Add(result);
                previousResult = result;
                lastRefractiveIndex = element.NextRefractiveIndex;
                translationFromLensCenter.X += element.DistanceToNext;
            }
        }

        public class IntersectionResult {
            public Ray IncidentRay { get; set; }
            public Ray OutgoingRay { get; set; }
            public Point Intersection { get; set; }
            public bool Intersected { get; set; }
            public bool Refracted { get; set; }
            public bool Reflected { get; set; }
            public Vector Normal { get; set; }
        }
    }
}
