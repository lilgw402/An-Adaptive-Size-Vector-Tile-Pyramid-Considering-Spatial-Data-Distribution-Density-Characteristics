using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
using System.IO;
using System.Runtime.InteropServices;

namespace vectortile_lgw
{
    struct TileStructure
    {
        public int level;
        public int x;
        public int y;
        public OSGeo.OGR.Geometry extentPolygon;
        public string path;
        public OSGeo.OGR.DataSource ds;
        public OSGeo.OGR.Layer layer;

    }
    public class Program
    {
        List<TileStructure> tiles;
        /**public VectorTileTool()
        {
        }*/

        public bool GdTree(OSGeo.OGR.Layer layer1)
        {
            Envelope layerEx1 = new Envelope();
            //Get bounding box (Xmin,Ymin,Xmax,Ymax) via Layer's function GetExtent
            layer1.GetExtent(layerEx1, 0);
            List<List<double>> lon = new List<List<double>>();
            List<List<double>> lat = new List<List<double>>();
            double lon11 = layerEx1.MaxX;
            double lat11 = layerEx1.MaxY;
            double lon12 = (layerEx1.MaxX + layerEx1.MinX) / 2;
            double lat12 = lat11;
            double lon13 = lon12;
            double lat13 = (layerEx1.MaxY + layerEx1.MinY) / 2;
            double lon14 = lon11;
            double lat14 = lat13;
            List<double> lon1 = new List<double> { lon11, lon12, lon13, lon14 };
            List<double> lat1 = new List<double> { lat11, lat12, lat13, lat14 };
            lon.Add(lon1);
            lat.Add(lat1);
            double lon21 = lon12;
            double lat21 = lat12;
            double lon22 = layerEx1.MinX;
            double lat22 = lat11;
            double lon23 = lon22;
            double lat23 = lat13;
            double lon24 = lon13;
            double lat24 = lat13;
            List<double> lon2 = new List<double> { lon21, lon22, lon23, lon24 };
            List<double> lat2 = new List<double> { lat21, lat22, lat23, lat24 };
            lon.Add(lon2);
            lat.Add(lat2);
            double lon31 = lon13;
            double lat31 = lat13;
            double lon32 = lon23;
            double lat32 = lat23;
            double lon33 = lon32;
            double lat33 = layerEx1.MinY;
            double lon34 = lon31;
            double lat34 = lat33;
            List<double> lon3 = new List<double> { lon31, lon32, lon33, lon34 };
            List<double> lat3 = new List<double> { lat31, lat32, lat33, lat34 };
            lon.Add(lon3);
            lat.Add(lat3);
            double lon41 = lon14;
            double lat41 = lat14;
            double lon42 = lon13;
            double lat42 = lat13;
            double lon43 = lon34;
            double lat43 = lat34;
            double lon44 = lon14;
            double lat44 = lat34;
            List<double> lon4 = new List<double> { lon41, lon42, lon43, lon44 };
            List<double> lat4 = new List<double> { lat41, lat42, lat43, lat44 };
            lon.Add(lon4);
            lat.Add(lat4);

            return true;

        }

   

        public static void Shp2Geojson(string srcPath, string destPath)
        {
            OSGeo.OGR.Ogr.RegisterAll();
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            Gdal.SetConfigOption("DXF_ENCODING", "UTF-8");
            OSGeo.OGR.DataSource ds = OSGeo.OGR.Ogr.Open(srcPath, 0);

            OSGeo.OGR.Driver dr2 = OSGeo.OGR.Ogr.GetDriverByName("GeoJSON");
    
            OSGeo.OGR.DataSource ret = dr2.CopyDataSource(ds, destPath, null);

            ret.FlushCache();
            ret.Dispose();
            }

        [DllImport("geos_c.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void initGEOS(GEOSMessageHandler noticefuction,
            GEOSMessageHandler errorfunction);

        [DllImport("geos_c.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static IntPtr GEOSGeomFromWKT(string wkt);

        [DllImport("geos_c.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static char GEOSContains(IntPtr g1, IntPtr g2);

        public delegate void GEOSMessageHandler(string[] epsilon);

        static void Main(string[] args)
        {

            //  new Program().SeprateShpLayer("G:\\wh_road\\District_boundary.shp", "G:\\wh_road\\vector_tile", 7);
            Shp2Geojson(@"G:\wh_road\result\result110_09.shp", @"G:\wh_road\result\result110_09.json");
        }
    }
}
