using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using InfiniteRPG.Data.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TInput = System.String;
using TOutput = System.Collections.Generic.IList<InfiniteRPG.Data.Map.MapCell>;

namespace InfiniteRPG.ContentPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentProcessor(DisplayName = "InfiniteRPG.ContentPipeline.TmxProcessor")]
    public class TmxProcessor : ContentProcessor<TInput, TOutput>
    {
        class MapDescription
        {
            public int Width { get; set; }
            public int Height { get; set; }
            
            public MapDescription(XElement element)
            {
                if (element == null || element.Name != "map")
                    throw new InvalidContentException("TMX file is not well formed: root element must be 'map'");

                var wEl = element.Attributes().FirstOrDefault(x => x.Name == "width");
                var hEl = element.Attributes().FirstOrDefault(x => x.Name == "height");

                if (wEl == null || hEl == null)
                {
                    throw new InvalidContentException("TMX file is not well formed: map elelemnt must define width and height");
                }
            }
        }

        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            XDocument doc = null;

            try
            {
                using (var reader = new StringReader(input))
                {
                    doc = XDocument.Load(reader);
                }
            }
            catch (Exception)
            {
                throw new InvalidContentException("Could not parse map file as XML");
            }

            // Find the root "map" document
            var mapDesc = new MapDescription(doc.Root);
            var encoding = Encoding.GetEncoding(doc.Declaration.Encoding);
            var layers = ProcessLayers(doc.Root.DescendantNodes().OfType<XElement>().Where(x => x.Name == "layer"), encoding);

            return new MapSection();
        }

        private IList<MapCell> ProcessLayers(IEnumerable<XElement> layers, Encoding docEncoding)
        {
            
            foreach (var layer in layers)
            {
                var data = layer.DescendantNodes().OfType<XElement>().FirstOrDefault(x => x.Name == "data");
                if (data == null)
                    throw new InvalidContentException("<layer> contains no <data> tag");

                var compressionAttr = data.Attributes().FirstOrDefault(x => x.Name == "compression");
                var encodingAttr = data.Attributes().FirstOrDefault(x => x.Name == "encoding");

                IEnumerable<XElement> tiles = null;

                if (compressionAttr != null)
                {
                    if (encodingAttr == null)
                        throw new InvalidContentException("data elements with compression must specify their encoding");

                    if (compressionAttr.Value != "gzip")
                        throw new InvalidContentException("Only gzip compressed data is supported");

                    if (encodingAttr.Value != "base64")
                        throw new InvalidContentException("Only base64-encoded compressed data is supported");

                    var databytes = Convert.FromBase64String(data.Value);

                    try
                    {
                        using (var memInStream = new MemoryStream(databytes))
                        using (var memOutStream = new MemoryStream())
                        using (var gzStream = new GZipStream(memInStream, CompressionMode.Decompress))
                        {
                            gzStream.CopyTo(memOutStream);
                            var xmlData = docEncoding.GetString(memOutStream.ToArray());
                            var dataDoc = XDocument.Load(xmlData);
                            tiles = dataDoc.DescendantNodes().OfType<XElement>().Where(x => x.Name == "tile");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new InvalidContentException("Unable to decompress gzipped layer data", e);
                    }

                }

                var cellData = tiles.Select(tile => int.Parse(tile.Attributes().First(x => x.Name == "gid").Value)).ToArray();
            }
            return null;
        }

        private 
    }
}