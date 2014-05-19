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
    public class SolidLine : Obstacle
    {
        public Line Line { get; private set; }

        public SolidLine( Line line )
        {
            Line = line;
        }

        public SolidLine( Vector2d start, Vector2d end )
        {
            Line = new Line( start, end );
        }

        public SolidLine( double startX, double startY, double endX, double endY )
        {
            Line = new Line( startX, startY, endX, endY );
        }

        public override Vector2d FindClosestPoint( Vector2d vec )
        {
            return Line.FindClosestPoint( vec );
        }

        public override Line FindClipLine( LightRay ray )
        {
            return Line;
        }

        public override LightRay[] Interact( LightRay ray )
        {
            return new LightRay[ 0 ];
        }

        public override float[] FindVerts()
        {
            return new float[]
            {
                (float) Line.Start.X, (float) Line.Start.Y,
                (float) Line.End.X,   (float) Line.End.Y
            };
        }
    }
}
