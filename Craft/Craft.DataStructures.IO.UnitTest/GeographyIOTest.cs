using System.Collections;
using System.Collections.Generic;
using Craft.DataStructures.IO.gml;
using Craft.DataStructures.IO.ogr;
using Xunit;

namespace Craft.DataStructures.IO.UnitTest
{
    public class GeographyIOTest
    {
        [Fact]
        public void WriteDataToGMLFile_BigCities()
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
                                Geometry = new Point
                                {
                                    Id = "Dummy.geom.0",
                                    Position = "24.871938 66.988063"
                                }
                            },
                            Name = "Karachi"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.1",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "28.671939 77.228058",
                                    UpperCorner = "28.671939 77.228058"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                Geometry = new Point
                                {
                                    Id = "Dummy.geom.1",
                                    Position = "28.671939 77.228058"
                                }
                            },
                            Name = "Delhi"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.2",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "23.725006 90.406634",
                                    UpperCorner = "23.725006 90.406634"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                Geometry = new Point
                                {
                                    Id = "Dummy.geom.2",
                                    Position = "23.725006 90.406634"
                                }
                            },
                            Name = "Dhaka"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.3",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "40.751925 -73.981963",
                                    UpperCorner = "40.751925 -73.981963"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                Geometry = new Point
                                {
                                    Id = "Dummy.geom.3",
                                    Position = "40.751925 -73.981963"
                                }
                            },
                            Name = "New York"
                        }
                    },
                    new FeatureMember
                    {
                        Dummy = new Dummy
                        {
                            Id = "Dummy.4",
                            BoundedBy = new BoundedBy
                            {
                                Envelope = new Envelope
                                {
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    LowerCorner = "30.051906 31.248022",
                                    UpperCorner = "30.051906 31.248022"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                Geometry = new Point
                                {
                                    Id = "Dummy.geom.4",
                                    Position = "30.051906 31.248022"
                                }
                            },
                            Name = "Cairo"
                        }
                    }
                }
            };

            var outputFile = @"C:\Temp\WorldsBiggestCities.gml";

            // Act
            DataIOHandler.SerializeGMLFeature(featureCollection, outputFile);
        }

        [Fact]
        public void WriteDataToGMLFile_Luxembourg()
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
                                    LowerCorner = "49.445458984375 5.72500000000002",
                                    UpperCorner = "50.1671875 6.49375000000001"
                                }
                            },
                            GeometryProperty = new GeometryProperty
                            {
                                Geometry = new MultiSurface
                                {
                                    Id = "Dummy.geom.0",
                                    SurfaceMembers = new List<SurfaceMember>
                                    {
                                        new SurfaceMember
                                        {

                                        },
                                        new SurfaceMember
                                        {

                                        }
                                    }
                                }
                            },
                            Name = "Luxembourg"
                        }
                    }
                }
            };

            var outputFile = @"C:\Temp\Luxembourg.gml";

            // Act
            DataIOHandler.SerializeGMLFeature(featureCollection, outputFile);
        }
    }
}
