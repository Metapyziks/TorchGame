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
using OpenTK.Graphics;

namespace TorchGame.Graphics
{
    public class ObstacleShader : ShaderProgram
    {
        public ObstacleShader()
        {
            ShaderBuilder vert = new ShaderBuilder( ShaderType.VertexShader );
            vert.AddAttribute( ShaderVarType.Vec2, "in_position" );
            vert.Logic = @"
                void main( void )
                {
                    gl_Position = in_position;
                }
            ";

            ShaderBuilder frag = new ShaderBuilder( ShaderType.FragmentShader );
            frag.Logic = @"
                void main( void )
                {
                    out_frag_colour = vec4( 1.0, 0.0, 0.0, 1.0 );
                }
            ";

            VertexSource = vert.Generate( GL3 );
            FragmentSource = frag.Generate( GL3 );

            BeginMode = BeginMode.Lines;
        }

        public ObstacleShader( int width, int height )
            : this()
        {
            Create();
            SetScreenSize( width, height );
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            AddAttribute( "in_position", 2 );
        }

        protected override void OnBegin()
        {
            GL.BlendFunc( BlendingFactorSrc.One, BlendingFactorDest.Zero );
        }
    }
}
