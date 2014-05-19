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
    public class ReflectLine : SolidLine
    {
        public RayColour Filter;

        public ReflectLine( Line line,
            RayColour filter = RayColour.White | RayColour.Inverse )
            : base( line )
        {
            Filter = filter;
        }

        public ReflectLine( Vector2d start, Vector2d end,
            RayColour filter = RayColour.White | RayColour.Inverse )
            : base( start, end )
        {
            Filter = filter;
        }

        public ReflectLine( double startX, double startY, double endX, double endY,
            RayColour filter = RayColour.White | RayColour.Inverse )
            : base( startX, startY, endX, endY )
        {
            Filter = filter;
        }

        public override LightRay[] Interact( LightRay ray )
        {
            Vector2d normal = Line.Direction.PerpendicularRight;

            
            if( !ray.Focusing )
            {
                return new LightRay[]
                {
                    new LightRay( ( ray.Origin - Line.Start ).Mirror( Line.Direction, normal ) + Line.Start,
                        ray.FieldRight.Mirror( Line.Direction, normal ),
                        ray.FieldLeft.Mirror( Line.Direction, normal ),
                        ray.ProjRight, ray.ProjLeft,
                        LightRay.DefaultRange, LightRay.DefaultRange,
                        ray, ray.Colour & Filter, this )
                };
            }
            else
            {
                return new LightRay[]
                {
                    new LightRay( ( ray.Origin - Line.Start ).Mirror( Line.Direction, normal ) + Line.Start,
                        ray.FieldRight.Mirror( Line.Direction, normal ),
                        ray.FieldLeft.Mirror( Line.Direction, normal ),
                        ray.ProjRight, ray.ProjLeft,
                        0.0, 0.0,
                        ray, ray.Colour & Filter, this )
                };
            }
        }
    }
}
