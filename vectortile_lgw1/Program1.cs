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
            //通过Layer的函数GetExtent获取边界盒子(Xmin,Ymin,Xmax,Ymax）
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

        public bool SeprateShpLayer(string sourcePath, string resultFolder, int level)
        {
            //SharpMap.GdalConfiguration.ConfigureGdal();
            //SharpMap.GdalConfiguration.ConfigureOgr();
            //初始化类库GDAL ,进行注册,ESRI Shapefile 代表的是shapefile 的格式   而MapInfo File 则代表的是mif 格式的 
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");
            OSGeo.OGR.Ogr.RegisterAll();
            OSGeo.OGR.Driver dr = OSGeo.OGR.Ogr.GetDriverByName("ESRI shapefile");//dr:{OSGeo.ORG.Driver}
            if (dr == null)
            {
                return false;
            }
            OSGeo.OGR.DataSource ds = dr.Open(sourcePath, 0);//ds数据源:{OSGeo.ORG.DataSource}
            int layerCount = ds.GetLayerCount();//layerCount:1
            OSGeo.OGR.Layer layer = ds.GetLayerByIndex(0);
            //投影信息 
            OSGeo.OSR.SpatialReference coord = layer.GetSpatialRef();//cood：{OSGeo.ORG.SpatialReference}
            string coordString;
            coord.ExportToWkt(out coordString);
            //地理范围
            Envelope layerEx = new Envelope();
            //通过Layer的函数GetExtent获取边界盒子(Xmin,Ymin,Xmax,Ymax）
            layer.GetExtent(layerEx, 0);
            //如果瓦块数据存在，全部删除562
            //if (Directory.Exists(resultFolder))
            //{
              //  Directory.Delete(resultFolder,true);
            //}
            List< List<double>> lon=new List<List<double>>(); 
            List< List<double>> lat=new List<List<double>>(); 
            double lon11 = layerEx.MaxX;
            double lat11=layerEx.MaxY;
            double lon12=(layerEx.MaxX+layerEx.MinX)/2;
            double lat12=lat11;
            double lon13=lon12;
            double lat13=(layerEx.MaxY+layerEx.MinY)/2;
            double lon14=lon11;
            double lat14=lat13;
            List<double> lon1= new List<double>{lon11,lon12,lon13,lon14};
            List<double> lat1= new List<double>{lat11,lat12,lat13,lat14};
            lon.Add(lon1);
            lat.Add(lat1);
            double lon21 = lon12;
            double lat21=lat12;
            double lon22=layerEx.MinX;
            double lat22=lat11;
            double lon23=lon22;
            double lat23=lat13;
            double lon24=lon13;
            double lat24=lat13;
             List<double> lon2= new List<double>{lon21,lon22,lon23,lon24};
            List<double> lat2= new List<double>{lat21,lat22,lat23,lat24};
            lon.Add(lon2);
            lat.Add(lat2);
            double lon31 = lon13;
            double lat31=lat13;
            double lon32=lon23;
            double lat32=lat23;
            double lon33=lon32;
            double lat33=layerEx.MinY;
            double lon34=lon31;
            double lat34=lat33;
             List<double> lon3= new List<double>{lon31,lon32,lon33,lon34};
            List<double> lat3= new List<double>{lat31,lat32,lat33,lat34};
            lon.Add(lon3);
            lat.Add(lat3);
            double lon41 = lon14;
            double lat41=lat14;
            double lon42=lon13;
            double lat42=lat13;
            double lon43=lon34;
            double lat43=lat34;
            double lon44=lon14;
            double lat44=lat34;
             List<double> lon4= new List<double>{lon41,lon42,lon43,lon44};
            List<double> lat4= new List<double>{lat41,lat42,lat43,lat44};
            lon.Add(lon4);
            lat.Add(lat4);

            tiles = new List<TileStructure>();
            for (int x =0;x<4;x++)
            {
                TileStructure tile;
                    tile.level = level;
                    tile.x = x;
                    tile.y = x;
                    tile.extentPolygon = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbPolygon);
                    //在GDAL中Polygon由LinearRing组成
                    OSGeo.OGR.Geometry geo = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbLinearRing);
                    geo.AddPoint(lon[x][0], lat[x][0], 0);
                    geo.AddPoint(lon[x][1], lat[x][1], 0);
                    geo.AddPoint(lon[x][3], lat[x][2], 0);
                    geo.AddPoint(lon[x][3], lat[x][3], 0);
                    ////在GDAL中Polygon由LinearRing组成
                    tile.extentPolygon.AddGeometryDirectly(geo);
                    tile.extentPolygon.CloseRings();
                    //创建空shp文件
                    string tileFolder = resultFolder + "\\SHP13\\" + level.ToString() ;
                    string fileName = x.ToString() + ".shp";
                    string tilePath = tileFolder + "\\" + fileName;
                    if (!Directory.Exists(tileFolder))
                    {
                        Directory.CreateDirectory(tileFolder);
                    }

                    tile.path = tilePath;

                    tile.ds = dr.CreateDataSource(tilePath, null);
                    //创建一个空的矢量图层datasource.CreateLayer(layer_name, spatial, geomtype)
                    tile.layer = tile.ds.CreateLayer("house", coord, OSGeo.OGR.wkbGeometryType.wkbPolygon, null);
                    //创建属性
                    FieldDefn fd = new FieldDefn("HEIGHT", FieldType.OFTReal);
                    tile.layer.CreateField(fd, 1);
                    tiles.Add(tile);
                    Console.WriteLine("创建第{0}个瓦块空shapefile数据",  x);
            }

/*
            //创建文件夹
            Directory.CreateDirectory(resultFolder + "\\JSON7\\");
            //针对本项目，划分第16级，根据地理范围求出瓦片(瓦片几行几列)
            int y0 = Convert.ToInt32((90 - layerEx.MaxY) * Math.Pow(2, level) / 180.0);//math.pow(a,b)即求a的b次方
            int x0 = Convert.ToInt32((180 + layerEx.MinX) * Math.Pow(2, level)/180.0);
            int y1 = Convert.ToInt32((90 - layerEx.MinY) * Math.Pow(2, level) / 180.0);//layerEx.MinY:30.0036129
            int x1 = Convert.ToInt32((180 + layerEx.MaxX) * Math.Pow(2, level) / 180.0);//layerEx.MaxX:114.99858162387893
            //20160621 ZXQ 创建层行列配置文件
            string filePath = resultFolder + "\\JSON7\\" + "tile.txt";
            FileStream fs = new FileStream(filePath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            //写入层行列
            sw.Write(level.ToString());
            sw.Write(",");
            sw.Write(x0.ToString());
            sw.Write(",");
            sw.Write(x1.ToString());
            sw.Write(",");
            sw.Write(y0.ToString());
            sw.Write(",");
            sw.Write(y1.ToString());
            sw.Write(",");
            sw.Write("json");
            sw.Flush();
            sw.Close();
            fs.Close();
            tiles = new List<TileStructure>();
            for (int x =x0;x<=x1;x++)
            {
                for (int y=y0;y<=y1;y++)
                {
                    TileStructure tile;
                    tile.level = level;
                    tile.x = x;
                    tile.y = y;
                    //根据瓦片的xy（行列号）算出瓦片四个角的坐标
                    double lonMin = -180 + 180 / (Math.Pow(2, level)) * x;
                    double lonMax = -180 + 180 / (Math.Pow(2, level)) * (x + 1);
                    double latMax = 90 - 180 / (Math.Pow(2, level)) * y;
                    double latMin = 90 - 180 / (Math.Pow(2, level)) * (y + 1);
                    tile.extentPolygon = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbPolygon);
                    //在GDAL中Polygon由LinearRing组成
                    OSGeo.OGR.Geometry geo = new OSGeo.OGR.Geometry(OSGeo.OGR.wkbGeometryType.wkbLinearRing);
                    geo.AddPoint(lonMin,latMax,0);
                    geo.AddPoint(lonMax, latMax, 0);
                    geo.AddPoint(lonMin, latMin, 0);
                    geo.AddPoint(lonMax, latMin, 0);
                    ////在GDAL中Polygon由LinearRing组成
                    tile.extentPolygon.AddGeometryDirectly(geo);
                    tile.extentPolygon.CloseRings();
                    //创建空shp文件
                    string tileFolder = resultFolder + "\\SHP7\\" + level.ToString() + "\\" + x.ToString();
                    string fileName = y.ToString() + ".shp";
                    string tilePath = tileFolder + "\\" + fileName;
                    if (!Directory.Exists(tileFolder))
                    {
                        Directory.CreateDirectory(tileFolder);
                    }

                    tile.path = tilePath;
                
                    tile.ds = dr.CreateDataSource(tilePath, null);
                    //创建一个空的矢量图层datasource.CreateLayer(layer_name, spatial, geomtype)
                    tile.layer = tile.ds.CreateLayer("house", coord, OSGeo.OGR.wkbGeometryType.wkbPolygon, null);
                    //创建属性
                    FieldDefn fd = new FieldDefn("HEIGHT", FieldType.OFTReal);
                    tile.layer.CreateField(fd,1);
                    tiles.Add(tile);
                    Console.WriteLine("创建第{0}层第{1}行第{2}列瓦块空shapefile数据", level, x, y);
                }
            }*/
            OSGeo.OGR.Feature feat;
            //读取shp文件
            //feat即为图层的每个要素
            while ((feat = layer.GetNextFeature()) != null)
            {
                int id = (int) feat.GetFID();
                OSGeo.OGR.Geometry geometry = feat.GetGeometryRef();
                OSGeo.OGR.wkbGeometryType goetype = geometry.GetGeometryType();//goetype:wkbLinestring
                
                //判断图层是否为polygon
                //if (goetype != wkbGeometryType.wkbPolygon)
                //{
                 //   continue;
                //}
                //geometry.CloseRings();

                //随机楼层3-15层
                Random random = new Random();
                //返回一个(3,15)内的随机数再×3。
                double height = random.Next(3,15)*3;// feat.GetFieldAsDouble("房屋层数") * 3;
                //循环遍历刚刚创建的所有瓦片，看其是否与当前要素相交
                for (int i = 0; i < tiles.Count;i++ )
                {
                    TileStructure tile = tiles[i];
                    //如果瓦片与要素相交，则将要素放入该瓦片
                    if (tile.extentPolygon.Intersect(geometry))
                    {
                        //创建新要素
                        OSGeo.OGR.Feature poFeature = new Feature(tile.layer.GetLayerDefn());//调用Layer.GetLayerDefn获取要素定义，
                        poFeature.SetField(0, height.ToString());
                        //新要素geometry为与瓦片相交的要素的geometry
                        OSGeo.OGR.Geometry geometry2 =tile.extentPolygon.Intersection(geometry);
                        poFeature.SetGeometry(geometry2);
                        //tile的feature设为新要素
                        tile.layer.CreateFeature(poFeature);
                        Console.WriteLine("写入第{0}个要素入瓦片{1}", id,i);
                    }
                }
            }

            return true;
        }
       /* public static void Intersection(String srcFile, String targetFile, String dstFile)
        {
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");
            OSGeo.OGR.Ogr.RegisterAll();

            OSGeo.OGR.Driver dr = OSGeo.OGR.Ogr.GetDriverByName("ESRI shapefile");
            OSGeo.OGR.DataSource srcDataSource = dr.Open(srcFile, 0);
      

            OSGeo.OGR.DataSource tgtDataSource = dr.Open(targetFile, 0);


            OSGeo.OGR.DataSource dstDataSource = dr.CreateDataSource(dstFile,);
            OSGeo.OGR.Layer srcLayer = srcDataSource.GetLayerByIndex(0);
            OSGeo.OGR.Layer tgtLayer = tgtDataSource.GetLayerByIndex(0);

            OSGeo.OGR.SpatialReference sr;
            sr.ImportFromEPSG(4326);
            OSGeo.OGR.Layer dstLayer = dstDataSource.CreateLayer("Intersection", sr, OSGeo.OGR.wkbGeometryType.wkbPolygon,null);

            srcLayer.Intersection(tgtLayer, dstLayer,null);

            tgtDataSource.delete();
            dstDataSource.delete();
            srcDataSource.delete();
        }*/

        public static void Shp2Geojson(string srcPath, string destPath)
{
    //添加所有配置
    //GdalBase.ConfigureAll();
            OSGeo.OGR.Ogr.RegisterAll();
    //配置单个选项
    Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");//支持中文路径
    Gdal.SetConfigOption("DXF_ENCODING", "UTF-8");

    //读取文件，这里自动区别文件类型
            //OSGeo.OGR.Driver dr = OSGeo.OGR.Ogr.GetDriverByName("ESRI shapefile");
    OSGeo.OGR.DataSource ds = OSGeo.OGR.Ogr.Open(srcPath, 0);
    //using var ds = Ogr.Open(srcPath, 0);

    //根据文件名创建驱动
            OSGeo.OGR.Driver dr2 = OSGeo.OGR.Ogr.GetDriverByName("GeoJSON");
    //using var dv = Ogr.GetDriverByName("GeoJSON");
    
    //拷贝数据与转化
            OSGeo.OGR.DataSource ret = dr2.CopyDataSource(ds, destPath, null);

    // 手动释放,否则destPath一直被占用
    ret.FlushCache();
    ret.Dispose();
}

        [DllImport("geos_c.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void initGEOS(GEOSMessageHandler noticefuction,
            GEOSMessageHandler errorfunction);

        //GEOSGeomFromWKT从WKT字符串返回指向几何对象的指针
        [DllImport("geos_c.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static IntPtr GEOSGeomFromWKT(string wkt);

        //GEOSContains检查两个对象的包含关系
        [DllImport("geos_c.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static char GEOSContains(IntPtr g1, IntPtr g2);

        //用于初始化函数的委托
        public delegate void GEOSMessageHandler(string[] epsilon);

        static void Main(string[] args)
        {

            //  new Program().SeprateShpLayer("G:\\wh_road\\District_boundary.shp", "G:\\wh_road\\vector_tile", 7);
            Shp2Geojson(@"G:\wh_road\result\result110_09.shp", @"G:\wh_road\result\result110_09.json");
        }
    }
}
