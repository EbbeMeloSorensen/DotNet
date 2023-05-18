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
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.0",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "24.871938 66.988063",
                                    UpperCorner = "24.871938 66.988063"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                Point = new Point
                                {
                                    Id = "Dummy.geom.0",
                                    Position = "24.871938 66.988063"
                                }
                            },
                            Name = "Karachi"
                        }
                    }
                }
            };

            var outputFile = @"C:\Temp\myfeatures.gml";

            // Act
            DataIOHandler.SerializeGMLFeature(featureCollection, outputFile);
        }
    }
}
