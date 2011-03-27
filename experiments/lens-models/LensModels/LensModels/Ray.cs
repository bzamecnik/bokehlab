﻿using System;
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

            Ray otherRay = (Ray) obj;
            return Vector3d.Equals(Origin, otherRay.Origin) &&
                Vector3d.Equals(Direction, otherRay.Direction);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new NotImplementedException();
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[{0} -> {1}]", Origin, Direction);
        }
    }

    class RayEqualityComparer : EqualityComparer<Ray>
    {
        double Epsilon { get; set; }

        public RayEqualityComparer(double precision)
        {
            Epsilon = precision;
        }
        public override bool Equals(Ray x, Ray y)
        {
            if ((x == null) && (y == null))
            {
                return true;
            }
            
            // TODO: this is not a nice epsilon comparison :/
            if ((x.Origin.X - y.Origin.X) > Epsilon)
            {
                return false;
            }
            if ((x.Origin.Y - y.Origin.Y) > Epsilon)
            {
                return false;
            }
            if ((x.Origin.Z - y.Origin.Z) > Epsilon)
            {
                return false;
            }
            if ((x.Direction.X - y.Direction.X) > Epsilon)
            {
                return false;
            }
            if ((x.Direction.Y - y.Direction.Y) > Epsilon)
            {
                return false;
            }
            if ((x.Direction.Z - y.Direction.Z) > Epsilon)
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode(Ray obj)
        {
            return obj.GetHashCode();
        }
    }
}
