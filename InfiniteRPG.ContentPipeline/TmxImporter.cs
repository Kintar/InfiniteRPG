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

namespace InfiniteRPG.ContentPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentImporter(".tmx", DisplayName = "TMX Map Importer", DefaultProcessor = "PassThroughProcessor")]
    public class TmxImporter : ContentImporter<MapSection>
    {
        private static int invocationCount = 0;

        class MapDescription
        {
            public int Width { get; set; }
            public int Height { get; set; }

            public MapDescription(XElement element)
            {
                if (element == null || element.Name != "map")
                    throw new InvalidContentException("TMX file is not well formed: root element must be 'map'");

                var wEl = element.Attributes().First(x => x.Name == "width");
                var hEl = element.Attributes().First(x => x.Name == "height");

                if (wEl == null || hEl == null)
                {
                    throw new InvalidContentException("TMX file is not well formed: map element must define width and height");
                }

                Width = int.Parse(wEl.Value);
                Height = int.Parse(hEl.Value);
            }
        }

        public override MapSection Import(string filename, ContentImporterContext context)
        {
            invocationCount++;
            XDocument doc;

            try
            {
                using (var fileStream = File.OpenRead(filename))
                using (var reader = new StreamReader(fileStream))
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
            var layers = ImportLayers(doc.Root.DescendantNodes().OfType<XElement>().Where(x => x.Name == "layer"), encoding);

            return new MapSection(layers.ToArray(), mapDesc.Width, mapDesc.Height);
        }

        private static IEnumerable<MapCell> ImportLayers(IEnumerable<XElement> layers, Encoding docEncoding)
        {
            var cells = new List<IList<int>>();

            var file = File.Open("tmx.debug.txt", invocationCount == 1 ? FileMode.Create : FileMode.Append);
            var writer = new StreamWriter(file);
            writer.Write(invocationCount);
            foreach (var layer in layers)
            {
                var layerName = layer.Attributes().First(x => x.Name == "name").Value;
                writer.Write(layerName);
                MapLayer layerEnum;
                if (!Enum.TryParse(layerName, true, out layerEnum))
                {
                    throw new InvalidContentException("Invalid map layer name: " + layerName);
                }

                var data = layer.DescendantNodes().OfType<XElement>().FirstOrDefault(x => x.Name == "data");
                if (data == null)
                    throw new InvalidContentException("<layer> contains no <data> tag");

                var compressionAttr = data.Attributes().FirstOrDefault(x => x.Name == "compression");
                var encodingAttr = data.Attributes().FirstOrDefault(x => x.Name == "encoding");

                IEnumerable<XElement> tiles = null;

                int[] tileData;

                if (compressionAttr != null)
                {
                    if (encodingAttr == null)
                        throw new InvalidContentException("data elements with compression must specify their encoding");

                    if (compressionAttr.Value != "gzip")
                        throw new InvalidContentException("Only gzip compressed data is supported");

                    if (encodingAttr.Value != "base64")
                        throw new InvalidContentException("Only base64-encoded compressed data is supported");

                    var databytes = Convert.FromBase64String(data.Value.Trim());

                    try
                    {
                        var tileAccum = new List<int>();

                        using (var memOutStream = new MemoryStream())
                        {
                            using (var memInStream = new MemoryStream(databytes))
                            using (var gzStream = new GZipStream(memInStream, CompressionMode.Decompress))
                            {
                                gzStream.CopyTo(memOutStream);
                            }

                            memOutStream.Position = 0;
                            writer.Write(" [{0}]:", memOutStream.Length);
                            while (memOutStream.Position < memOutStream.Length)
                            {
                                var t = memOutStream.ReadByte();
                                for (var i = 0; i < 3; i++ )
                                    t |= memOutStream.ReadByte() << 8;
                                tileAccum.Add(t);
                                writer.Write(t + ",");
                            }
                            writer.WriteLine();
                        }

                        tileData = tileAccum.ToArray();
                    }
                    catch (Exception e)
                    {
                        throw new InvalidContentException("Unable to decompress gzipped layer data: " + e.Message, e);
                    }


                }
                else
                {
                    tileData = tiles.Select(tile => int.Parse(tile.Attributes().First(x => x.Name == "gid").Value)).ToArray();
                }

                for (var i = 0; i < tileData.Length; i++)
                {
                    if (cells.Count <= i)
                        cells.Add(new List<int>());

                    cells[i].Add(tileData[i]);
                }

            }
            writer.WriteLine("Done");
            writer.Flush();
            file.Close();
            return cells.Select(x => new MapCell(x.ToArray())).ToList();
        }
    }
}
