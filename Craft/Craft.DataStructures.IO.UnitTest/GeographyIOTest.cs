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
                                AbstractGeometricPrimitive = new Point
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
                                AbstractGeometricPrimitive = new Point
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
                                AbstractGeometricPrimitive = new Point
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
                                AbstractGeometricPrimitive = new Point
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
                                AbstractGeometricPrimitive = new Point
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
                                AbstractGeometricPrimitive = new MultiSurface
                                {
                                    Id = "Dummy.geom.0",
                                    SrsName = "urn:ogc:def:crs:EPSG::4326",
                                    SurfaceMembers = new List<SurfaceMember>
                                    {
                                        new SurfaceMember
                                        {
                                            AbstractSurface = new Polygon
                                            {
                                                Id = "Dummy.geom.0.0",
                                                Exterior = new Exterior
                                                {
                                                    LinearRing = new LinearRing
                                                    {
                                                        PosList = new PosList
                                                        {
                                                            value = "50.12099609375 6.11650390625002 50.09423828125 6.10830078125002 50.034375 6.10976562500002 49.97431640625 6.13818359375 49.91513671875 6.20488281250002 49.87216796875 6.25605468750001 49.837890625 6.32460937500002 49.805322265625 6.44091796875 49.798486328125 6.4873046875 49.75439453125 6.49375000000001 49.7078125 6.48476562500002 49.68203125 6.44462890625002 49.644970703125 6.40673828125 49.599609375 6.37832031250002 49.5126953125 6.34843750000002 49.452734375 6.34433593750001 49.4775390625 6.27734375 49.4943359375 6.2421875 49.49892578125 6.18105468750002 49.485205078125 6.11992187500002 49.454638671875 6.07412109375002 49.445458984375 6.01142578125001 49.454638671875 5.95947265625 49.4775390625 5.92890625000001 49.48974609375 5.9013671875 49.505078125 5.82343750000001 49.53828125 5.78974609375001 49.55380859375 5.8154296875 49.5783203125 5.83759765625001 49.612841796875 5.85654296875001 49.644775390625 5.88037109375 49.732177734375 5.8037109375 49.75888671875 5.78798828125002 49.80830078125 5.72500000000002 49.833349609375 5.72578125000001 49.857177734375 5.74082031250001 49.875634765625 5.73525390625002 49.91962890625 5.74404296875002 49.96123046875 5.7880859375 50.0126953125 5.8173828125 50.0828125 5.86689453125001 50.1671875 5.97626953125001 50.154296875 6.05478515625001 50.15458984375 6.08906250000001 50.123779296875 6.11005859375001 50.12099609375 6.11650390625002"
                                                        }
                                                    }
                                                }
                                            }
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
