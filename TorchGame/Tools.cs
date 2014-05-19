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

using System.IO;

using OpenTK;
using OpenTK.Graphics;

namespace TorchGame
{
    public static class Tools
    {
        private static Random myRand = new Random();

        public static bool DoesExtend( this Type self, Type type )
        {
            return self.BaseType == type || ( self.BaseType != null && self.BaseType.DoesExtend( type ) );
        }

        public static byte[] ReadBytes( this Stream self, int count )
        {
            byte[] data = new byte[ count ];
            for ( int i = 0; i < count; ++i )
            {
                int bt = self.ReadByte();
                if ( bt == -1 )
                    throw new EndOfStreamException();

                data[ i ] = (byte) bt;
            }

            return data;
        }

        public static void WriteToStream( this Color4 self, BinaryWriter writer )
        {
            writer.Write( (byte) ( self.R * 0xFF ) );
            writer.Write( (byte) ( self.G * 0xFF ) );
            writer.Write( (byte) ( self.B * 0xFF ) );
            writer.Write( (byte) ( self.A * 0xFF ) );
        }

        public static Color4 ReadColor4FromStream( BinaryReader reader )
        {
            return new Color4( reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() );
        }

        public static Double Random()
        {
            return myRand.NextDouble();
        }

        #region Clamps
        public static Byte Clamp( Byte value, Byte min, Byte max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }

        public static UInt16 Clamp( UInt16 value, UInt16 min, UInt16 max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }

        public static UInt32 Clamp( UInt32 value, UInt32 min, UInt32 max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }

        public static UInt64 Clamp( UInt64 value, UInt64 min, UInt64 max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }

        public static SByte Clamp( SByte value, SByte min, SByte max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }

        public static Int16 Clamp( Int16 value, Int16 min, Int16 max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }

        public static Int32 Clamp( Int32 value, Int32 min, Int32 max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }

        public static Int64 Clamp( Int64 value, Int64 min, Int64 max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }

        public static Single Clamp( Single value, Single min, Single max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }

        public static Double Clamp( Double value, Double min, Double max )
        {
            return
                ( value < min ) ? min :
                ( value > max ) ? max :
                value;
        }
        #endregion Clamps

        public static int Min( params int[] values )
        {
            int min = values[ 0 ];
            foreach ( int val in values )
                if ( val < min )
                    min = val;

            return min;
        }

        public static double Min( params double[] values )
        {
            double min = values[ 0 ];
            foreach ( double val in values )
                if ( val < min )
                    min = val;

            return min;
        }

        public static int Max( params int[] values )
        {
            int max = values[ 0 ];
            foreach ( int val in values )
                if ( val > max )
                    max = val;

            return max;
        }

        public static double Max( params double[] values )
        {
            double max = values[ 0 ];
            foreach ( double val in values )
                if ( val > max )
                    max = val;

            return max;
        }

        public static double TicksToSeconds( ulong ticks )
        {
            return ticks / 64.0;
        }

        public static ulong SecondsToTicks( double seconds )
        {
            return (ulong) ( seconds * 64 );
        }

        public static String ApplyWordWrap( this String text, float charWidth, float wrapWidth )
        {
            if ( wrapWidth <= 0.0f )
                return text;

            String newText = "";
            int charsPerLine = (int) ( wrapWidth / charWidth );
            int x = 0, i = 0;
            while ( i < text.Length )
            {
                String word = "";
                while ( i < text.Length && !char.IsWhiteSpace( text[ i ] ) )
                    word += text[ i++ ];

                if ( x + word.Length > charsPerLine )
                {
                    if ( x == 0 )
                    {
                        newText += word.Substring( 0, charsPerLine ) + "\n" + word.Substring( charsPerLine );
                        x = word.Length - charsPerLine;
                    }
                    else
                    {
                        newText += "\n" + word;
                        x = word.Length;
                    }
                }
                else
                {
                    newText += word;
                    x += word.Length;
                }

                if ( i < text.Length )
                {
                    newText += text[ i ];
                    x++;

                    if ( text[ i++ ] == '\n' )
                        x = 0;
                }
            }

            return newText;
        }

        public static double Angle( this Vector2d vec )
        {
            return Math.Atan2( vec.Y, vec.X );
        }

        public static double Dot( this Vector2d self, Vector2d vec )
        {
            return self.X * vec.X + self.Y * vec.Y;
        }

        public static Vector2d Rotate( this Vector2d vec, double angle )
        {
            angle += vec.Angle();
            return new Vector2d( Math.Cos( angle ), Math.Sin( angle ) ) * vec.Length;
        }

        public static Vector2d Rotate( this Vector2d vec, Vector2d axis, double angle )
        {
            return ( vec - axis ).Rotate( angle ) + axis;
        }

        public static Vector2d ChangeBasis( this Vector2d v, Vector2d i, Vector2d j )
        {
            double invdet = 1.0 / ( i.X * j.Y - i.Y * j.X );

            double a = ( v.X * j.Y - v.Y * j.X ) * invdet;
            double b = ( v.Y * i.X - v.X * i.Y ) * invdet;

            return new Vector2d( a, b );
        }

        public static Vector2d RevertBasis( this Vector2d v, Vector2d i, Vector2d j )
        {
            return v.X * i + v.Y * j;
        }

        public static Vector2d Mirror( this Vector2d v, Vector2d tangent )
        {
            return v.Mirror( tangent, tangent.PerpendicularRight );
        }

        public static Vector2d Mirror( this Vector2d v, Vector2d tangent, Vector2d normal )
        {
            double invdet = 1.0 / ( tangent.X * normal.Y - tangent.Y * normal.X );

            return ( tangent * ( v.X * normal.Y  - v.Y * normal.X  )
                    - normal * ( v.Y * tangent.X - v.X * tangent.Y ) ) * invdet;
        }

        public static bool IsParallel( this Vector2d v1, Vector2d v2 )
        {
            return ( v1.X * v2.Y ) == ( v1.Y * v2.X );
        }

        public static Vector2d LineIntersection( Vector2d o1, Vector2d d1, Vector2d o2, Vector2d d2 )
        {
            Vector2d od = o1 - o2;
            double a = ( d2.X * od.Y - d2.Y * od.X ) / ( d1.X * d2.Y - d1.Y * d2.X );
            return o1 + d1 * a;
        }

        public static double WrapAngle( double ang )
        {
            return ang - Math.Floor( ang / ( Math.PI * 2.0 ) + 0.5 ) * Math.PI * 2.0;
        }

        public static double WrapAngle( double ang, double basis )
        {
            return WrapAngle( ang - basis ) + basis;
        }

        public static double AngleDif( double angA, double angB, bool keepPositive = false )
        {
            return WrapAngle( angA - angB, Math.PI );
        }
    }
}
