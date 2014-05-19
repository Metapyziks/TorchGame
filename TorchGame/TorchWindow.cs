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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using ResourceLib;

using TorchGame.Graphics;
using TorchGame.Lighting;
using OpenTK.Input;
using System.Diagnostics;

namespace TorchGame
{
    public class TorchWindow : GameWindow
    {
        private RayShader myRayShader;
        private ObstacleShader myObstacleShader;
        private SpriteShader mySpriteShader;
        
        private Vector2d myLightOrigin;
        private Vector2d myLightTarget;
        private double myFOV;

        private LightRay myLightRay;
        private Obstacle[] myObstacles;

        private Vector2d myNewObsStart;
        private bool myDrawing;

        private delegate Obstacle ObjectCreatorDelegate( Vector2d start, Vector2d end, RayColour filter );

        private int myCurObjectType;
        private readonly String[] myObjectNames =
        {
            "Solid Line",
            "Mirror",
            "Filter",
            "Solid Box",
            "Solid Circle"
        };
        private readonly ObjectCreatorDelegate[] myObjectCreators =
        {
            delegate( Vector2d start, Vector2d end, RayColour filter )
            {
                return new SolidLine( start, end );
            },
            delegate( Vector2d start, Vector2d end, RayColour filter )
            {
                return new ReflectLine( start, end, filter );
            },
            delegate( Vector2d start, Vector2d end, RayColour filter )
            {
                return new FilterLine( start, end, filter );
            },
            delegate( Vector2d start, Vector2d end, RayColour filter )
            {
                double x = Math.Min( start.X, end.X );
                double y = Math.Min( start.Y, end.Y );

                double w = Math.Max( start.X, end.X ) - x;
                double h = Math.Max( start.Y, end.Y ) - y;

                return new SolidRect( x, y, w, h );
            },
            delegate( Vector2d start, Vector2d end, RayColour filter )
            {
                return new SolidCircle( start, ( end - start ).Length );
            }
        };
        private RayColour myFilterColour;

        private Text myInfoText;
        private Text myFPSText;

        private DateTime myLastFPSUpdate;

        private bool myDrawRayOutline;
        private bool myDrawObstacles;
        private bool myRotFixed;

        public TorchWindow()
            : base( 800, 600, new GraphicsMode( new ColorFormat( 8, 8, 8, 8 ), 0, 0, 4 ), "Torch Game" )
        {
            WindowBorder = WindowBorder.Fixed;
            VSync = VSyncMode.Off;

            myDrawRayOutline = false;
            myDrawObstacles = true;
            myRotFixed = false;

            myCurObjectType = 0;
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
#if DEBUG
            Debug.WriteLine( GL.GetString( StringName.Version ) );
            Debug.WriteLine( GL.GetString( StringName.ShadingLanguageVersion ) );
            Debug.WriteLine( GL.GetString( StringName.Vendor ) );
            Debug.WriteLine( GL.GetString( StringName.Extensions ) );
            Debug.WriteLine( GL.GetString( StringName.Renderer ) );
            Debug.WriteLine( "----------------" );
#endif

            GL.ClearColor( Color4.Black );
            GL.Enable( EnableCap.Blend );

            myRayShader = new RayShader( Width, Height );
            myObstacleShader = new ObstacleShader( Width, Height );
            mySpriteShader = new SpriteShader( Width, Height );

            double mar = 0.0;

            myObstacles = new Obstacle[]
            {
                /*
                new SolidLine( 0.0, mar, Width, mar ),
                new SolidLine( Width - mar, 0.0, Width - mar, Height ),
                new SolidLine( Width, Height - mar, 0.0, Height - mar ),
                new SolidLine( mar, Height, mar, 0.0 )
                */
            };

            myLightOrigin = new Vector2d( Width / 4, Height / 2 );
            myFOV = 45.0 * Math.PI / 180.0;

            myLightRay = new LightRay( myLightOrigin, 0.0, myFOV );
            myLightRay.Cast( myObstacles );

            myFilterColour = RayColour.White;

            Mouse.ButtonDown += delegate( object sender, MouseButtonEventArgs me )
            {
                myNewObsStart = new Vector2d( me.X, me.Y );
                myDrawing = true;
                
                List<Obstacle> obs = myObstacles.ToList();
                obs.Add( myObjectCreators[ myCurObjectType ](
                    myNewObsStart, myNewObsStart, myFilterColour ) );
                myObstacles = obs.ToArray();
            };

            Mouse.ButtonUp += delegate( object sender, MouseButtonEventArgs me )
            {
                if ( myDrawing )
                {
                    myObstacles[ myObstacles.Length - 1 ] = myObjectCreators[ myCurObjectType ](
                        myNewObsStart, new Vector2d( me.X, me.Y ), myFilterColour );
                    myDrawing = false;
                }
            };

            myInfoText = new Text( new Font( "gui_fontlarge" ) );
            myInfoText.Position = new Vector2( 4.0f, 20.0f );

            UpdateInfoText();

            myFPSText = new Text( new Font( "gui_fontlarge" ) );
            myFPSText.Position = new Vector2( 4.0f, 4.0f );
            myFPSText.String = "RT: 0.00ms - FT: 0.00ms";

            myLastFPSUpdate = DateTime.Now;
        }

        private void UpdateInfoText()
        {
            myInfoText.String = "Mouse to direct ray, WASD to move origin\n"
                + "L toggles ray outline\n"
                + "Left click + drag to draw an object\n"
                + "Press E to change object type\n"
                + "Press R, G and B to toggle colours in mirrors\n"
                + "Press Q to fix the torch rotation\n"
                + "Press C to colour the torch\n\n"
                + "Current object type: " + myObjectNames[ myCurObjectType ] + "\n"
                + "Current filter colour: " + myFilterColour.ToString();
        }

        protected override void OnKeyPress( KeyPressEventArgs e )
        {
            switch ( e.KeyChar )
            {
                case 'l':
                case 'L':
                    myDrawRayOutline = !myDrawRayOutline;
                    break;
                case 'o':
                case 'O':
                    myDrawObstacles = !myDrawObstacles;
                    break;

                case 'e':
                case 'E':
                    myCurObjectType = ( myCurObjectType + 1 ) % myObjectNames.Length;
                    break;
                case 'q':
                case 'Q':
                    myRotFixed = !myRotFixed;
                    break;

                case 'r':
                case 'R':
                    myFilterColour ^= RayColour.Red;
                    break;
                case 'g':
                case 'G':
                    myFilterColour ^= RayColour.Green;
                    break;
                case 'b':
                case 'B':
                    myFilterColour ^= RayColour.Blue;
                    break;

                case 'c':
                case 'C':
                    myLightRay.Colour = myFilterColour;
                    break;
            }

            UpdateInfoText();
        }

        protected override void OnUpdateFrame( FrameEventArgs e )
        {
            double moveSpeed = 4.0;

            if ( Keyboard[ Key.W ] )
                myLightOrigin.Y -= moveSpeed;
            if ( Keyboard[ Key.S ] )
                myLightOrigin.Y += moveSpeed;
            if ( Keyboard[ Key.A ] )
                myLightOrigin.X -= moveSpeed;
            if ( Keyboard[ Key.D ] )
                myLightOrigin.X += moveSpeed;

            if( !myRotFixed )
                myLightTarget = new Vector2d( Mouse.X, Mouse.Y );

            myLightRay.Origin = myLightOrigin;
            myLightRay.SetDirection( myLightTarget - myLightOrigin );

            myLightRay.Cast( myObstacles );

            if ( myDrawing )
                myObstacles[ myObstacles.Length - 1 ] = myObjectCreators[ myCurObjectType ](
                        myNewObsStart, new Vector2d( Mouse.X, Mouse.Y ), myFilterColour );

            if ( ( DateTime.Now - myLastFPSUpdate ).TotalMilliseconds >= 500.0 )
            {
                myFPSText.String = "RT: " + ( RenderTime * 1000.0 ).ToString( "F" ) + "ms"
                    + " - FT: " + ( UpdateTime * 1000.0 ).ToString( "F" ) + "ms";
                myLastFPSUpdate = DateTime.Now;
            }
        }

        protected override void OnRenderFrame( FrameEventArgs e )
        {
            MakeCurrent();

            // Clear the colour buffer
            GL.Clear( ClearBufferMask.ColorBufferBit );

            myRayShader.Begin();
            myLightRay.Render( myRayShader );

            myRayShader.End();

            myObstacleShader.Begin();
            if( myDrawRayOutline )
                myLightRay.Render( myObstacleShader );

            if( myDrawObstacles )
                foreach ( Obstacle obs in myObstacles )
                    obs.Render( myObstacleShader );
            
            myObstacleShader.End();

            mySpriteShader.Begin();
            myInfoText.Render( mySpriteShader );
            myFPSText.Render( mySpriteShader );
            mySpriteShader.End();

            // Display the rendered frame
            SwapBuffers();
        }
    }
}