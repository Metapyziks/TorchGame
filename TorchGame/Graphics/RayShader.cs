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

using OpenTK.Graphics.OpenGL;
using TorchGame.Lighting;
using OpenTK.Graphics;
using OpenTK;

namespace TorchGame.Graphics
{
    public class RayShader : ShaderProgram
    {
        private int myColourLoc = -1;
        private RayColour myColour;

        public RayColour Colour
        {
            get
            {
                return myColour;
            }
            set
            {
                if ( myColour != value )
                {
                    bool wasStarted = Started;

                    if ( wasStarted )
                        End();

                    if ( myColourLoc == -1 )
                        myColourLoc = GL.GetUniformLocation( Program, "light_colour" );

                    float r = ( value & RayColour.Red ) != 0
                        ? 1.0f : 0.0f;
                    float g = ( value & RayColour.Green ) != 0
                        ? 1.0f : 0.0f;
                    float b = ( value & RayColour.Blue ) != 0
                        ? 1.0f : 0.0f;

                    GL.Uniform4( myColourLoc, r, g, b, 0.5f );

                    myColour = value;

                    if ( wasStarted )
                        Begin();
                }
            }
        }

        private int myOriginLoc = -1;
        private Vector2d myOrigin;

        public Vector2d Origin
        {
            get
            {
                return myOrigin;
            }
            set
            {
                if ( !myOrigin.Equals( value ) )
                {
                    bool wasStarted = Started;

                    if ( wasStarted )
                        End();

                    if ( myOriginLoc == -1 )
                        myOriginLoc = GL.GetUniformLocation( Program, "origin" );

                    GL.Uniform2( myOriginLoc, (float) value.X, (float) value.Y );

                    myOrigin = value;

                    if ( wasStarted )
                        Begin();
                }
            }
        }

        public RayShader()
        {
            ShaderBuilder vert = new ShaderBuilder( ShaderType.VertexShader );
            vert.AddAttribute( ShaderVarType.Vec2, "in_position" );
            vert.AddVarying( ShaderVarType.Vec2, "var_position" );
            vert.Logic = @"
                void main( void )
                {
                    var_position = in_position;
                    gl_Position = in_position;
                }
            ";

            ShaderBuilder frag = new ShaderBuilder( ShaderType.FragmentShader );
            frag.AddUniform( ShaderVarType.Vec4, "light_colour" );
            frag.AddUniform( ShaderVarType.Vec2, "origin" );
            frag.AddVarying( ShaderVarType.Vec2, "var_position" );
            frag.Logic = @"
                void main( void )
                {
                    vec2 diff = origin - var_position;
                    float dist2 = diff.x * diff.x + diff.y * diff.y;

                    float mag = 1.0 / ( dist2 / 8192.0 + 1.0 );        

                    if( mag <= 0.0 )
                        discard;

                    out_frag_colour = vec4( light_colour.rgb, mag );
                }
            ";

            VertexSource = vert.Generate( GL3 );
            FragmentSource = frag.Generate( GL3 );

            BeginMode = BeginMode.Triangles;
        }

        public RayShader( int width, int height )
            : this()
        {
            Create();
            SetScreenSize( width, height );
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            AddAttribute( "in_position", 2 );

            Colour = RayColour.White;
            Origin = new Vector2d();
        }

        protected override void OnBegin()
        {
            GL.BlendFunc( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.DstAlpha );
        }
    }
}
