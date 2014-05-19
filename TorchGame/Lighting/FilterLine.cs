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
    public class FilterLine : SolidLine
    {
        public RayColour Filter;

        public FilterLine( Line line,
            RayColour filter = RayColour.White | RayColour.Inverse )
            : base( line )
        {
            Filter = filter;
        }

        public FilterLine( Vector2d start, Vector2d end,
            RayColour filter = RayColour.White | RayColour.Inverse )
            : base( start, end )
        {
            Filter = filter;
        }

        public FilterLine( double startX, double startY, double endX, double endY,
            RayColour filter = RayColour.White | RayColour.Inverse )
            : base( startX, startY, endX, endY )
        {
            Filter = filter;
        }

        public override Line FindClipLine( LightRay ray )
        {
            if ( ray.Colour == Filter )
                return new Line( 0.0, 0.0, 0.0, 0.0 );

            return base.FindClipLine( ray );
        }

        public override LightRay[] Interact( LightRay ray )
        {
            return new LightRay[]
            {
                new LightRay( ray.Origin,
                    ray.FieldLeft, ray.FieldRight,
                    ray.ProjLeft, ray.ProjRight,
                    LightRay.DefaultRange, LightRay.DefaultRange,
                    ray, ray.Colour & Filter, this )
            };
        }
    }
}
