/**
 * Copyright (c) 2014 James King [metapyziks@gmail.com]
 *
 * This file is part of TorchGame.
 * 
 * TorchGame is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * TorchGame is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with TorchGame. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace TorchGame
{
    public struct Circle
    {
        public static Circle operator +( Circle circ, Vector2d vec )
        {
            return new Circle( circ.Origin + vec, circ.Radius );
        }
        public static Circle operator +( Vector2d vec, Circle circ )
        {
            return new Circle( circ.Origin + vec, circ.Radius );
        }

        public static Circle operator -( Circle circ, Vector2d vec )
        {
            return new Circle( circ.Origin - vec, circ.Radius );
        }
        public static Circle operator -( Vector2d vec, Circle circ )
        {
            return new Circle( circ.Origin - vec, circ.Radius );
        }

        public readonly Vector2d Origin;
        public readonly double Radius;

        public Circle( Vector2d origin, double radius )
        {
            Origin = origin;
            Radius = radius;
        }

        public Circle Mirror( Vector2d tangent )
        {
            return Mirror( tangent, tangent.PerpendicularRight );
        }

        public Circle Mirror( Vector2d tangent, Vector2d normal )
        {
            return new Circle( Origin.Mirror( tangent, normal ), Radius );
        }

        public Circle Rotate( double angle )
        {
            return new Circle( Origin.Rotate( angle ), Radius );
        }

        public Circle Rotate( Vector2d axis, double angle )
        {
            return new Circle( Origin.Rotate( axis, angle ), Radius );
        }

        public bool IsIntersecting( Circle circle )
        {
            double dist = ( circle.Origin - Origin ).Length;

            return dist <= Radius + circle.Radius && dist >= Radius - circle.Radius;
        }

        public Vector2d[] FindIntersections( Circle circle )
        {
            Vector2d diff = circle.Origin - Origin;
            double dist = diff.Length;

            if ( dist > Radius + circle.Radius || dist < Radius - circle.Radius )
                return new Vector2d[ 0 ];

            if ( dist == Radius + circle.Radius )
                return new Vector2d[] { Origin + diff * ( Radius / dist ) };

            double a = ( Radius * Radius - circle.Radius * circle.Radius + dist * dist ) / ( 2 * dist );
            double h = Math.Sqrt( Radius * Radius - a * a );

            Vector2d mid = Origin + a * diff / dist;

            return new Vector2d[]
            {
                new Vector2d
                {
                    X = mid.X + h * diff.Y / dist,
                    Y = mid.Y - h * diff.X / dist
                },
                new Vector2d
                {
                    X = mid.X - h * diff.Y / dist,
                    Y = mid.Y + h * diff.X / dist
                }
            };
        }
    }
}
