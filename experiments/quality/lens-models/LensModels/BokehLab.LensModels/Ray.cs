using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace BokehLab.Lens
{
    public class Ray
    {
        public Vector3d Origin { get; set; }
        public Vector3d Direction { get; set; }

        public Ray(Vector3d origin, Vector3d direction)
        {
            Origin = origin;
            Direction = direction;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Ray otherRay = (Ray)obj;
            return Vector3d.Equals(Origin, otherRay.Origin) &&
                Vector3d.Equals(Direction, otherRay.Direction);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[{0} -> {1}]", Origin, Direction);
        }
    }
}
