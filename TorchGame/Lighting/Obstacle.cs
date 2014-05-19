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
    public abstract class Obstacle
    {
        private float[] myVerts;
        private bool myVertsChanged;

        public Obstacle()
        {
            UpdateVertexes();
        }

        protected void UpdateVertexes()
        {
            myVertsChanged = true;
        }

        public virtual Vector2d FindClosestPoint( Vector2d vec )
        {
            throw new NotImplementedException();
        }

        public virtual Line FindClipLine( LightRay ray )
        {
            throw new NotImplementedException();
        }

        public virtual LightRay[] Interact( LightRay ray )
        {
            throw new NotImplementedException();
        }

        public virtual float[] FindVerts()
        {
            throw new NotImplementedException();
        }

        public void Render( ShaderProgram shader )
        {
            if ( myVertsChanged )
            {
                myVerts = FindVerts();
                myVertsChanged = false;
            }

            shader.Render( myVerts );
        }
    }
}
