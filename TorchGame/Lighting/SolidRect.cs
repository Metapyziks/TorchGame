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
    public class SolidRect : Obstacle
    {
        public Vector2d TopLeft { get; private set; }
        public Vector2d BottomRight { get; private set; }
        public Vector2d TopRight { get; private set; }
        public Vector2d BottomLeft { get; private set; }
        public Vector2d Center { get; private set; }

        public double Top
        {
            get { return TopLeft.Y; }
        }
        public double Left
        {
            get { return TopLeft.X; }
        }
        public double Bottom
        {
            get { return BottomRight.Y; }
        }
        public double Right
        {
            get { return BottomRight.X; }
        }

        public SolidRect( Vector2d topLeft, Vector2d bottomRight )
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            TopRight = new Vector2d( Right, Top );
            BottomLeft = new Vector2d( Left, Bottom );

            Center = ( TopLeft + BottomRight ) / 2.0;
        }

        public SolidRect( double x, double y, double width, double height )
            : this( new Vector2d( x, y ), new Vector2d( x + width, y + height ) )
        {

        }

        public override Vector2d FindClosestPoint( Vector2d vec )
        {
            if ( vec.X <= Left )
            {
                if ( vec.Y <= Top )
                    return TopLeft;
                if ( vec.Y >= Bottom )
                    return BottomLeft;

                return new Vector2d( Left, vec.Y );
            }

            if ( vec.X >= Right )
            {
                if ( vec.Y <= Top )
                    return TopRight;
                if ( vec.Y >= Bottom )
                    return BottomRight;

                return new Vector2d( Right, vec.Y );
            }

            if ( vec.Y <= Top )
                return new Vector2d( vec.X, Top );
            if ( vec.Y >= Bottom )
                return new Vector2d( vec.X, Bottom );

            return new Vector2d( vec.X, vec.Y );
        }

        public override Line FindClipLine( LightRay ray )
        {
            Vector2d vec = ray.Origin;

            if ( vec.X <= Left )
            {
                if ( vec.Y <= Top )
                    return new Line( BottomLeft, TopRight );
                if ( vec.Y >= Bottom )
                    return new Line( TopLeft, BottomRight );

                return new Line( TopLeft, BottomLeft );
            }

            if ( vec.X >= Right )
            {
                if ( vec.Y <= Top )
                    return new Line( TopLeft, BottomRight );
                if ( vec.Y >= Bottom )
                    return new Line( BottomLeft, TopRight );

                return new Line( TopRight, BottomRight );
            }

            if ( vec.Y <= Top )
                return new Line( TopLeft, TopRight );
            if ( vec.Y >= Bottom )
                return new Line( BottomLeft, BottomRight );

            return new Line( Center, Center );
        }

        public override LightRay[] Interact( LightRay ray )
        {
            return new LightRay[ 0 ];
        }

        public override float[] FindVerts()
        {
            return new float[]
            {
                (float) Left, (float) Top,
                (float) Right, (float) Top,

                (float) Right, (float) Top,
                (float) Right, (float) Bottom,
                
                (float) Right, (float) Bottom,
                (float) Left, (float) Bottom,
                
                (float) Left, (float) Bottom,
                (float) Left, (float) Top
            };
        }
    }
}
