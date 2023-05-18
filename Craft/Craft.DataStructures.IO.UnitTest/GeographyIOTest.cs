using System.Collections;
using Craft.DataStructures.IO.gml;
using Craft.DataStructures.IO.ogr;
using Xunit;

namespace Craft.DataStructures.IO.UnitTest
{
    public class GeographyIOTest
    {
        [Fact]
        public void WriteDataToGMLFile()
        {
            // Arrange
            var featureCollection = new FeatureCollection
            {
                FeatureCollectionElements = new ArrayList
                {
                    new BoundedBy
                    {
                        Envelope = new Envelope
                        {
                            SrsName = "urn:ogc:def:crs:EPSG::4326",
                            LowerCorner = "-34.600556 -118.181926",
                            UpperCorner = "55.75411 139.749462"
                        }
                    },
                    new FeatureMember(),
                    new FeatureMember()
                }
            };

            var outputFile = @"C:\Temp\myfeatures.gml";

            // Act
            DataIOHandler.SerializeGMLFeature(featureCollection, outputFile);
        }
    }
}
