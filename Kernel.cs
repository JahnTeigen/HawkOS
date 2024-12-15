using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using Sys = Cosmos.System;

namespace SUMS
{
    public class Kernel : Sys.Kernel
    {
        public static Cosmos.System.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();

        protected override void BeforeRun()
        {
            VFSManager.RegisterVFS(fs);
            Systems.START();
        }

        protected override void Run()
        {
            UI.Mode();
        }
    }
}
