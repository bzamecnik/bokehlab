using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericLens
{
    public abstract class OpticalElement
    {
        /// <summary>
        /// Distance from this element's apex to the next element's apex.
        /// </summary>
        public double DistanceToNext { get; set; }

        public double NextRefractiveIndex { get; set; }

        public abstract Vector TranslationToLocal();

        public abstract bool IntersectRay(Ray ray, out Point intersection);
    }
}
