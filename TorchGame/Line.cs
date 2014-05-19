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
    public struct Line
    {
        public static Line operator +( Line line, Vector2d vec )
        {
            return new Line( line.Start + vec, line.End + vec );
        }
        public static Line operator +( Vector2d vec, Line line )
        {
            return new Line( line.Start + vec, line.End + vec );
        }

        public static Line operator -( Line line, Vector2d vec )
        {
            return new Line( line.Start - vec, line.End - vec );
        }
        public static Line operator -( Vector2d vec, Line line )
        {
            return new Line( line.Start - vec, line.End - vec );
        }

        public static Line operator *( Line line, double val )
        {
            return new Line( line.Start * val, line.End * val );
        }
        public static Line operator *( double val, Line line )
        {
            return new Line( line.Start * val, line.End * val );
        }

        public static Line operator /( Line line, double val )
        {
            return new Line( line.Start / val, line.End / val );
        }
        public static Line operator /( double val, Line line )
        {
            return new Line( line.Start / val, line.End / val );
        }

        public readonly Vector2d Start;
        public readonly Vector2d End;

        public readonly double Length;
        public readonly Vector2d Direction;

        public Line( Vector2d start, Vector2d end )
        {
            Start = start;
            End = end;

            Vector2d diff = end - start;

            Length = diff.Length;
            Direction = diff / Length;
        }

        public Line( Vector2d start, Vector2d direction, double length )
        {
            Start = start;
            End = start + direction * length;

            Length = length;
            Direction = direction / direction.Length;
        }

        public Line( double startX, double startY, double endX, double endY )
            : this( new Vector2d( startX, startY ), new Vector2d( endX, endY ) )
        {

        }

        public Line ChangeBasis( Vector2d i, Vector2d j )
        {
            return new Line( Start.ChangeBasis( i, j ), End.ChangeBasis( i, j ) );
        }

        public Line RevertBasis( Vector2d i, Vector2d j )
        {
            return new Line( Start.RevertBasis( i, j ), End.RevertBasis( i, j ) );
        }

        public Line Mirror( Vector2d tangent )
        {
            return Mirror( tangent, tangent.PerpendicularRight );
        }

        public Line Mirror( Vector2d tangent, Vector2d normal )
        {
            return new Line( Start.Mirror( tangent, normal ), End.Mirror( tangent, normal ) );
        }

        public Line Rotate( double angle )
        {
            return new Line( Start.Rotate( angle ), End.Rotate( angle ) );
        }

        public Line Rotate( Vector2d axis, double angle )
        {
            return new Line( Start.Rotate( axis, angle ), End.Rotate( axis, angle ) );
        }

        public bool IsParallel( Line line )
        {
            return Direction.IsParallel( line.Direction );
        }

        public bool Intersects( Line line )
        {
            Vector2d od = this.Start - line.Start;

            double denom = this.Direction.X * line.Direction.Y - this.Direction.Y * line.Direction.X;

            double a = ( line.Direction.X * od.Y - line.Direction.Y * od.X ) / denom;
            double b = ( this.Direction.X * od.Y - this.Direction.Y * od.X ) / denom;

            return ( a >= 0.0 && b >= 0.0 && a <= this.Length && b <= line.Length );
        }

        public double FindIntersectionTravel( Line line )
        {
            Vector2d od = this.Start - line.Start;
            double denom = this.Direction.X * line.Direction.Y - this.Direction.Y * line.Direction.X;
            return ( line.Direction.X * od.Y - line.Direction.Y * od.X ) / denom;
        }

        public Vector2d FindIntersection( Line line )
        {
            return Start + Direction * FindIntersectionTravel( line );
        }

        public Vector2d FindClosestPoint( Vector2d vec )
        {
            Vector2d perp = Direction.PerpendicularRight;
            Line line = new Line( vec, vec + perp );

            double travel = Tools.Clamp( FindIntersectionTravel( line ), 0.0, Length );

            return Start + Direction * travel;
        }

        public override string ToString()
        {
            return Start.ToString() + "->" + End.ToString();
        }

        public override bool Equals( object obj )
        {
            if ( obj is Line )
            {
                Line line = (Line) obj;
                return Start.Equals( line.Start ) && End.Equals( line.End );
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ( Start.GetHashCode() & 0xFFFF ) | ( ( End.GetHashCode() & 0xFF ) << 16 );
        }
    }
}
