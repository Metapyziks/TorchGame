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

namespace TorchGame.Lighting
{
    public class SolidCircle : Obstacle
    {
        public Circle Circle { get; private set; }

        public Vector2d Origin
        {
            get { return Circle.Origin; }
        }

        public double Radius
        {
            get { return Circle.Radius; }
        }

        public SolidCircle( Circle circle )
        {
            Circle = circle;
        }

        public SolidCircle( Vector2d origin, double radius )
            : this( new Circle( origin, radius ) )
        {

        }

        public override float[] FindVerts()
        {
            int verts = (int) Math.Max( Math.Sqrt( Radius ) * 4.0, 4.0 );

            float[] data = new float[ verts * 4 ];

            for ( int i = 0; i < verts; ++i )
            {
                double omega = ( Math.PI * 2.0 * i ) / verts;
                Vector2d pos = Origin + new Vector2d( Math.Cos( omega ), Math.Sin( omega ) ) * Radius;
                data[ i * 4 + 0 ] = data[ ( ( i + verts - 1 ) * 4 + 2 ) % ( verts * 4 ) ] = (float) pos.X;
                data[ i * 4 + 1 ] = data[ ( ( i + verts - 1 ) * 4 + 3 ) % ( verts * 4 ) ] = (float) pos.Y;
            }

            return data;
        }

        public override Vector2d FindClosestPoint( Vector2d vec )
        {
            Vector2d diff = vec - Origin;
            double dist = diff.Length;

            if ( dist <= Radius )
                return vec;

            return Origin + diff * Radius / diff.Length;
        }

        public override Line FindClipLine( LightRay ray )
        {
            Vector2d diff = ray.Origin - Origin;
            double dist = diff.Length;

            if ( dist <= Radius )
                return new Line( Origin, Origin );

            Vector2d mid = Origin + diff * 0.5;
            Circle perpCircle = new Circle( mid, dist * 0.5 );
            Vector2d[] points = Circle.FindIntersections( perpCircle );

            if ( points.Length < 2 )
                return new Line( Origin, Origin );

            return new Line( points[ 0 ], points[ 1 ] );
        }

        public override LightRay[] Interact( LightRay ray )
        {
            return new LightRay[ 0 ];
        }
    }
}
