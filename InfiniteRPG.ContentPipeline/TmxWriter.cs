using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteRPG.Data.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using TWrite = InfiniteRPG.Data.Map.MapSection;

namespace InfiniteRPG.ContentPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class TmxWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            output.Write(value.Width);
            output.Write(value.Height);
            output.Write(Enum.GetValues(typeof(MapLayer)).Length);
            foreach (var tileRef in value.Cells.SelectMany(cell => cell))
                output.Write(tileRef);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "InfiniteRPG.Data.Map.MapSectionReader, InfiniteRPG.Data";
        }
    }
}
