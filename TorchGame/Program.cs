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

using ResourceLib;

using OpenTK;
using OpenTK.Graphics;
using System.IO;
using System.Diagnostics;
using TorchGame.Lighting;

namespace TorchGame
{
    public class Program
    {
        static void Main( string[] args )
        {
#if DEBUG
            Debug.Listeners.Add( new DebugListener() );
#endif

            Res.RegisterManager( new Graphics.RTextureManager() );

            String dataDir = "Data" + Path.DirectorySeparatorChar;

            Res.MountArchive( Res.LoadArchive( dataDir + "sh_baseui.rsa" ) );
            Res.MountArchive( Res.LoadArchive( dataDir + "sh_main.rsa" ) );

            var window = new TorchWindow();
            window.Run( 60, 60 );
            window.Dispose();

#if DEBUG
            Debug.Flush();
#endif
        }
    }
}
