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

using TorchGame.Graphics;

namespace TorchGame.Lighting
{
    public class LightRay
    {
        public const int DepthLimit = 256;
        public const double DefaultRange = 4096.0;

        private float[] myVerts;
        private float[] myEdgeVerts;
        private List<LightRay> myChildren;

        public LightRay Parent { get; private set; }
        public int Depth { get; private set; }

        public RayColour Colour { get; set; }

        public Obstacle SourceObstacle { get; private set; }
        public Obstacle IncidentObstacle { get; private set; }

        public Vector2d Origin { get; set; }

        public Vector2d FieldLeft { get; private set; }
        public Vector2d FieldRight { get; private set; }

        public double ClipLeft { get; private set; }
        public double ClipRight { get; private set; }

        public double ProjLeft { get; private set; }
        public double ProjRight { get; private set; }

        public double MidAngle { get; private set; }

        public double LeftAngle { get; private set; }
        public double RightAngle { get; private set; }

        public bool Focusing
        {
            get { return ClipLeft > ProjLeft || ClipRight > ProjRight; }
        }

        public LightRay( Vector2d origin, Vector2d target,
            double fieldOfView, RayColour colour = RayColour.White )
            : this( origin, Math.Atan2( target.Y - origin.Y, target.X - origin.X ),
            fieldOfView, colour )
        {

        }

        public LightRay( Vector2d origin, double direction, 
            double fieldOfView, RayColour colour = RayColour.White )
        {
            Colour = colour;

            Origin = origin;

            SetDirection( direction, fieldOfView );

            ClipLeft = ClipRight = 0.0;
            ProjLeft = ProjRight = DefaultRange;
        }

        public LightRay( Vector2d origin, Vector2d fieldLeft, Vector2d fieldRight,
           double clipLeft, double clipRight,
           double projLeft, double projRight,
           LightRay parent )
            : this( origin, fieldLeft, fieldRight,
                clipLeft, clipRight, projLeft, projRight,
                parent, parent.Colour, parent.SourceObstacle )
        {

        }

        public LightRay( Vector2d origin, Vector2d fieldLeft, Vector2d fieldRight,
            double clipLeft, double clipRight,
            double projLeft, double projRight,
            LightRay parent, RayColour colour, Obstacle sourceObstacle )
        {
            Parent = parent;
            SourceObstacle = sourceObstacle;

            if ( Parent == null )
                Depth = 0;
            else
                Depth = Parent.Depth + 1;

            Colour = colour;

            if ( Parent == null || Parent.Origin != origin )
                IncidentObstacle = null;
            else
                IncidentObstacle = Parent.IncidentObstacle;

            Origin = origin;

            UpdateField( fieldLeft, fieldRight );

            ClipLeft = clipLeft;
            ClipRight = clipRight;

            ProjLeft = projLeft;
            ProjRight = projRight;
        }

        public void SetDirection( Vector2d dir )
        {
            SetDirection( Math.Atan2( dir.Y, dir.X ), Tools.AngleDif( RightAngle, LeftAngle ) );
        }

        public void SetDirection( Vector2d dir, double fieldOfView )
        {
            SetDirection( Math.Atan2( dir.Y, dir.X ), fieldOfView );
        }

        public void SetDirection( double direction, double fieldOfView )
        {
            double halfFOV = fieldOfView / 2.0;

            FieldLeft = new Vector2d
            {
                X = Math.Cos( direction - halfFOV ),
                Y = Math.Sin( direction - halfFOV )
            };

            FieldRight = new Vector2d
            {
                X = Math.Cos( direction + halfFOV ),
                Y = Math.Sin( direction + halfFOV )
            };

            MidAngle = Tools.WrapAngle( direction );

            LeftAngle = Tools.WrapAngle( direction - halfFOV );
            RightAngle = Tools.WrapAngle( direction + halfFOV );
        }

        private void UpdateField( Vector2d fieldLeft, Vector2d fieldRight )
        {
            Line clipLine = new Line( FieldLeft * ClipLeft, FieldRight * ClipRight );

            FieldLeft  = fieldLeft  / fieldLeft.Length;
            FieldRight = fieldRight / fieldRight.Length;

            ProjLeft  = fieldLeft.Length;
            ProjRight = fieldRight.Length;

            if ( ClipLeft != 0.0 && ClipRight != 0.0 )
            {
                Line left  = new Line( new Vector2d(), fieldLeft  );
                Line right = new Line( new Vector2d(), fieldRight );

                ClipLeft  = clipLine.FindIntersection( left  ).Length;
                ClipRight = clipLine.FindIntersection( right ).Length;
            }

            MidAngle = ( FieldLeft + FieldRight ).Angle();

            LeftAngle = FieldLeft.Angle();
            RightAngle = FieldRight.Angle();
        }

        public Line? FindLineIntersection( Line line )
        {
            if ( line.Length == 0.0 )
                return null;

            Vector2d lsta = line.Start;
            Vector2d lend = line.End;

            Vector2d diff = lsta - Origin;

            Vector2d nedif = ( lend - Origin ).ChangeBasis( diff, diff.PerpendicularLeft );

            bool neg = Focusing;

            if ( nedif.Y < 0 != neg )
            {
                Vector2d temp = lsta;
                lsta = lend;
                lend = temp;
            }

            Vector2d sdif = lsta - Origin;
            Vector2d edif = lend - Origin;

            Vector2d srel = sdif.ChangeBasis( FieldLeft, FieldRight );
            Vector2d erel = edif.ChangeBasis( FieldLeft, FieldRight );

            if ( ( srel.X <= 0.0 && erel.X <= 0.0 ) || ( srel.Y <= 0.0 && erel.Y <= 0.0 ) )
                return null;

            if ( srel.X < 0.0 || srel.Y < 0.0 || erel.X < 0.0 || erel.Y < 0.0 )
            {
                Line left = new Line( 0.0, 0.0, 1.0, 0.0 );
                Line righ = new Line( 0.0, 0.0, 0.0, 1.0 );

                Line lrel = new Line( srel, erel );

                if ( srel.X < 0.0 || srel.Y < 0.0 )
                    srel = lrel.FindIntersection( left );
                if ( erel.X < 0.0 || erel.Y < 0.0 )
                    erel = lrel.FindIntersection( righ );

                if ( srel.X < 0.0 && erel.Y < 0.0 )
                    return null;
            }

            bool sclp = ( ClipLeft > 0.0 && ClipRight > 0.0 && ClipRight * ( 1 - srel.X / ClipLeft ) > srel.Y ) != neg;
            bool eclp = ( ClipLeft > 0.0 && ClipRight > 0.0 && ClipRight * ( 1 - erel.X / ClipLeft ) > erel.Y ) != neg;

            if ( sclp && eclp )
                return null;

            if ( sclp || eclp )
            {
                Line lrel = new Line( srel, erel );
                Vector2d irel = lrel.FindIntersection( new Line( ClipLeft, 0.0, 0.0, ClipRight ) );
                if ( sclp )
                    srel = irel;
                else
                    erel = irel;
            }

            bool sprj = ( ProjLeft == 0.0 || ProjRight == 0.0 || ProjRight * ( 1 - srel.X / ProjLeft ) < srel.Y ) != neg;
            bool eprj = ( ProjLeft == 0.0 || ProjRight == 0.0 || ProjRight * ( 1 - erel.X / ProjLeft ) < erel.Y ) != neg;

            if ( sprj && eprj )
                return null;

            if ( sprj || eprj )
            {
                Line lrel = new Line( srel, erel );
                Vector2d irel = lrel.FindIntersection( new Line( ProjLeft, 0.0, 0.0, ProjRight ) );
                if ( sprj )
                    srel = irel;
                else
                    erel = irel;
            }

            Line lout = new Line( srel, erel ).RevertBasis( FieldLeft, FieldRight );

            return lout.Length > 1.0 / 65536.0 ? lout : (Line?) null;
        }

        public void Cast( Obstacle[] obstacles, int first = 0 )
        {
            if ( Parent == null || !Origin.Equals( Parent.Origin ) )
                obstacles = obstacles.OrderBy( x =>
                    ( x.FindClosestPoint( Origin ) - Origin ).Length ).ToArray();

            myChildren = new List<LightRay>();

            if ( Depth == 0 )
            {
                //LightRay child = new LightRay( Origin, FieldRight, FieldLeft, -1024.0, -1024.0, 1024.0, 1024.0, this );
                //myChildren.Add( child );
                //child.Cast( obstacles, first );

                double field = Tools.AngleDif( RightAngle, LeftAngle, true );
                if ( field == 0.0 )
                    field = Math.PI * 2.0;
                int rays = (int) Math.Ceiling( field * 2.0 / Math.PI );

                double lastAng = LeftAngle;
                Vector2d lastVec = FieldLeft;
                for ( int i = 0; i < rays; ++i )
                {
                    double nextAng = ( i == rays - 1 ? RightAngle : lastAng + Math.PI / 2.0 );
                    Vector2d nextVec = ( i == rays - 1 ? FieldRight
                        : new Vector2d( Math.Cos( nextAng ), Math.Sin( nextAng ) ) );
                    LightRay child = new LightRay( Origin, lastVec, nextVec,
                        ClipLeft, ClipRight, ProjLeft, ProjRight, this );
                    lastAng = nextAng;
                    lastVec = nextVec;
                    myChildren.Add( child );
                    child.Cast( obstacles, first );
                }
            }
            else if ( ClipLeft < 0.0 && ClipRight < 0.0 )
            {
                LightRay focus = new LightRay( Origin, -FieldLeft, -FieldRight, -ClipLeft, -ClipRight, Math.Max( -ProjLeft, 0.0 ), Math.Max( -ProjRight, 0.0 ), this );
                myChildren.Add( focus );
                focus.Cast( obstacles, first );
            }
            else if ( Depth < DepthLimit )
            {
                for ( int i = first; i < obstacles.Length; ++i )
                {
                    Obstacle obs = obstacles[ i ];

                    if ( obs == SourceObstacle )
                        continue;

                    Line? line = FindLineIntersection( obs.FindClipLine( this ) );
                    if ( line.HasValue )
                    {
                        LightRay[] children = Clip( line.Value );
                        myChildren.AddRange( children );

                        foreach ( LightRay ray in children )
                            ray.Cast( obstacles, i + 1 );

                        IncidentObstacle = obs;
                    }
                }

                if ( IncidentObstacle != null )
                {
                    LightRay[] children = IncidentObstacle.Interact( this )
                        .Where( x => x.Colour != RayColour.Black
                            && ( x.ClipLeft < 1024.0 || x.ClipRight < 1024.0 ) ).ToArray();

                    myChildren.AddRange( children );

                    foreach ( LightRay ray in children )
                        ray.Cast( obstacles );
                }

                if ( ProjLeft == 0.0 && ProjRight == 0.0 && ClipLeft > 0.0 && ClipRight > 0.0 )
                {
                    LightRay child = new LightRay( Origin, -FieldRight, -FieldLeft, 0.0, 0.0, 1024.0, 1024.0, this );
                    myChildren.Add( child );
                    child.Cast( obstacles, 0 );
                }
            }

            FindVertices();
        }

        private LightRay[] Clip( Line line )
        {
            Line clipLine = new Line( FieldLeft * ClipLeft, FieldRight * ClipRight );
            Line projLine = new Line( FieldLeft * ProjLeft, FieldRight * ProjRight );

            Line lrel = line.ChangeBasis( FieldLeft, FieldRight );

            LightRay leftChild = null;
            LightRay rightChild = null;

            if ( lrel.Start.Y > 1.0 / 65536.0 )
            {
                Vector2d dir = line.Start;
                dir /= dir.Length;

                double clip = 0.0;
                double proj = ProjRight;

                Line ray = new Line( new Vector2d(), line.Start );

                if ( ClipLeft != 0.0 && ClipRight != 0.0 )
                    clip = clipLine.FindIntersection( ray ).Length;

                if ( ProjLeft != 0.0 && ProjRight != 0.0 )
                    proj = projLine.FindIntersection( ray ).Length;

                leftChild = new LightRay( Origin, FieldLeft, dir,
                    ClipLeft, clip,
                    ProjLeft, proj,
                    this );
            }

            if ( lrel.End.X > 1.0 / 65536.0 )
            {
                Vector2d dir = line.End;
                dir /= dir.Length;

                double clip = 0.0;
                double proj = ProjLeft;

                Line ray = new Line( new Vector2d(), line.End );

                if ( ClipLeft != 0.0 && ClipRight != 0.0 )
                    clip = clipLine.FindIntersection( ray ).Length;

                if ( ProjLeft != 0.0 && ProjRight != 0.0 )
                    proj = projLine.FindIntersection( ray ).Length;

                rightChild = new LightRay( Origin, dir, FieldRight,
                    clip, ClipRight,
                    proj, ProjRight,
                    this );
            }

            UpdateField( line.Start, line.End );

            return ( leftChild != null && rightChild != null )
                ? new LightRay[] { leftChild, rightChild }
                : ( leftChild ?? rightChild ) != null
                    ? new LightRay[] { leftChild ?? rightChild }
                    : new LightRay[ 0 ];
        }

        protected virtual void FindVertices()
        {
            if ( Depth == 0 || ( ClipLeft < 0.0 && ClipRight < 0.0 ) )
            {
                myVerts = new float[ 0 ];
                return;
            }

            Vector2d fLeftVec = Origin + FieldLeft * ClipLeft;
            Vector2d fRighVec = Origin + FieldRight * ClipRight;

            Vector2d pLeftVec = Origin + FieldLeft  * ProjLeft;
            Vector2d pRighVec = Origin + FieldRight * ProjRight;

            if ( ClipLeft == 0.0 || ClipRight == 0.0 )
            {
                myVerts = new float[]
                {
                    (float) Origin.X,   (float) Origin.Y,
                    (float) pLeftVec.X, (float) pLeftVec.Y,
                    (float) pRighVec.X, (float) pRighVec.Y
                };
            }
            else if ( ProjLeft == 0.0 || ProjRight == 0.0 )
            {
                myVerts = new float[]
                {
                    (float) Origin.X,   (float) Origin.Y,
                    (float) fLeftVec.X, (float) fLeftVec.Y,
                    (float) fRighVec.X, (float) fRighVec.Y
                };
            }
            else if ( ( ClipLeft > 0.0 && ClipRight > 0.0 ) || ( ProjLeft < 0.0 && ProjRight < 0.0 ) )
            {
                myVerts = new float[]
                {
                    (float) fLeftVec.X, (float) fLeftVec.Y,
                    (float) pLeftVec.X, (float) pLeftVec.Y,
                    (float) pRighVec.X, (float) pRighVec.Y,

                    (float) fLeftVec.X, (float) fLeftVec.Y,
                    (float) pRighVec.X, (float) pRighVec.Y,
                    (float) fRighVec.X, (float) fRighVec.Y
                };
            }
            else
            {
                myVerts = new float[]
                {
                    (float) Origin.X,   (float) Origin.Y,
                    (float) pLeftVec.X, (float) pLeftVec.Y,
                    (float) pRighVec.X, (float) pRighVec.Y,
                    
                    (float) Origin.X,   (float) Origin.Y,
                    (float) fLeftVec.X, (float) fLeftVec.Y,
                    (float) fRighVec.X, (float) fRighVec.Y
                };
            }

            myEdgeVerts = new float[ myVerts.Length * 2 ];

            for ( int i = 0; i < myVerts.Length / 2; ++i )
            {
                myEdgeVerts[ i * 4 + 0 ] = myVerts[   i * 2 + 0 ];
                myEdgeVerts[ i * 4 + 1 ] = myVerts[   i * 2 + 1 ];
                myEdgeVerts[ i * 4 + 2 ] = myVerts[ ( i * 2 + 2 ) % myVerts.Length ];
                myEdgeVerts[ i * 4 + 3 ] = myVerts[ ( i * 2 + 3 ) % myVerts.Length ];
            }
        }

        public virtual void Render( ShaderProgram shader )
        {
            if ( Depth >= DepthLimit )
                return;

            if ( Depth > 0 )
            {
                if ( shader is RayShader )
                {
                    RayShader rs = (RayShader) shader;
                    rs.Colour = Colour;
                    rs.Origin = Origin;
                    shader.Render( myVerts );
                }
                else
                    shader.Render( myEdgeVerts );
            }

            foreach ( LightRay child in myChildren )
                child.Render( shader );
        }
    }
}
