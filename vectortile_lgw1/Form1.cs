using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using System.Runtime.InteropServices;
using vectortile_lgw;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Output;
using System.Threading;

namespace vectortile_lgw1
{
    public partial class Form1 : Form
    {

        public struct CPoint
        {
            public double X;
            public double Y;

        }
        public List<CPoint> formerplist = new List<CPoint>();
        public List<CPoint> plist = new List<CPoint>();
        public string path;

        public Form1()
        {
            InitializeComponent();
        }

        double PI = 3.1415926535897932384626;
        double R0 = 6378137;
        double D2R;

       

        int x = 0;
        int j = 0;
        List<string> jsonname = new List<string>();

        List<double> resolutions = new List<double>() { 0.703125, 0.3515625, 0.17578125, 0.087890625, 0.0439453125, 0.02197265625, 0.010986328125, 0.0054931640625, 0.00274658203125, 0.001373291015625, 6.866455078125E-4, 3.4332275390625E-4, 1.71661376953125E-4, 8.58306884765625E-5, 4.291534423828125E-5, 2.1457672119140625E-5, 1.0728836059570312E-5, 5.364418029785156E-6, 2.682209014892578E-6, 1.341104507446289E-6, 6.705522537231445E-7, 3.3527612686157227E-7 };
      

       string extentname = "0829";


        //coded character
        private static char[] CHARS = {'0','1','2','3','4','5','6','7','8','9',
                                          'b','c','d','e','f','g','h','j','k','m',
                                          'n','p','q','r','s','t','u','v','w','x','y','z'};
        //Earth's radius constant
        private const double EARTH_RADIUS = 6378137;
        //Binary length constant, 20 corresponds to a code length of 8, positioning is accurate to the metre scale.
        private const int Binary_Length = 20;




        private void Start_Click(object sender, EventArgs e)
        {
            IWorkspaceFactory pWF;
            pWF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFW;
            pFW = (IFeatureWorkspace)pWF.OpenFromFile("D:\\postgraduate\\vector_tiles\\vectortile_exoriment_data\\polygon2", 0);
            IFeatureClass pFC2;
            pFC2 = pFW.OpenFeatureClass("polygon.shp");
            IFeatureLayer pFlayer2 = new FeatureLayerClass();
            pFlayer2.FeatureClass = pFC2;
            //addshpfrompath("G:\\wh_road\\polygon", "point.shp",pFlayer2,null);
            //vectortile_lgw.Program.Shp2Geojson(@"G:\wh_road\result\result110_09.shp", @"G:\wh_road\result\result110_092.json");
            //Shp2Geojson(@"G:\wh_road\result\result110_09.shp", @"G:\wh_road\result\result110_092.json");

 
           String path = @"D:\\postgraduate\\vector_tiles\\vectortile_exoriment_data\\DP\\szbuilding_UTM49N";

            //Method 1

            var files = Directory.GetFiles(path, "*.shp");

            
            foreach (var file in files)

            {
                string sourcename = System.IO.Path.GetFileNameWithoutExtension(file);
                int level = int.Parse(sourcename);
                string jpg_name = Convert.ToString(level-1);
            //    to_jpg("G:\\wh_road\\data\\result_jpg\\wh_road_UTM49N\\" + jpg_name + ".jpg");
                axMapControl1.ClearLayers();
                // Console.WriteLine(file); //Console outputs the names of all files under the Log folder with a .log suffix
                //-->BenXHCMS.xml
                addshpfrompath(path, sourcename, sourcename, pFlayer2, null, null,level);
                

            }
        }

        public void to_jpg(string Outpath)
        {

                //resolution
                double resulotion = 1000;
                //axMapControl1.ActiveView.ScreenDisplay.DisplayTransformation.Resolution;
                IExport m_export = null;
                
                m_export = new ExportJPEG() as IExport;

                //Setting the output path
                m_export.ExportFileName = Outpath;
                //Setting the resolution of the output
                m_export.Resolution = resulotion;
                tagRECT piexPound;
                piexPound = axMapControl1.ActiveView.ScreenDisplay.DisplayTransformation.get_DeviceFrame();
                IEnvelope m_envelope = new ESRI.ArcGIS.Geometry.Envelope() as IEnvelope;
                m_envelope.PutCoords(piexPound.left, piexPound.bottom, piexPound.right, piexPound.top);
                //Setting the output IEnvelope
                m_export.PixelBounds = m_envelope;

                ITrackCancel m_trackCancel = new CancelTracker();
                //The method of output
                axMapControl1.ActiveView.Output(m_export.StartExporting(), (short)resulotion, ref piexPound, axMapControl1.ActiveView.Extent, m_trackCancel);
                m_export.FinishExporting();
            
        }

        public void addshpfrompath(string filepath,string filename,string sourcename ,IFeatureLayer pFlayer2,IFeature pfeature,IFeatureLayer cliplayer,int level)
        {
            //pFlayer2 is “Polygon.shp”,
            IWorkspaceFactory pWF;
            IFeatureWorkspace pFW;
            IFeatureWorkspace sourcepFW;
            IFeatureClass pFC;
            IFeatureClass sourcepFC;
            IFeatureLayer pFlayer=new FeatureLayerClass(); 
            IFeatureLayer sourcepFlayer= new FeatureLayerClass();
            IFeatureLayer pFlayer3= new FeatureLayerClass();
            // The first parameter is the full pathname, the last is the filename
            pWF = new ShapefileWorkspaceFactoryClass();
            pFW = (IFeatureWorkspace)pWF.OpenFromFile(filepath, 0);
            pFC = pFW.OpenFeatureClass(filename+".shp"); 
            pFlayer.FeatureClass = pFC;     


            ISpatialReference pSpatialRef = (pFC as IGeoDataset).SpatialReference;
            IProjectedCoordinateSystem pProCoordSys = pSpatialRef as IProjectedCoordinateSystem;
           // IProjection pro = pProCoordSys.Projection;
            if (pProCoordSys != null)
            {
                string wgs84_topath = "D:\\postgraduate\\vector_tiles\\vectortile_exoriment_data\\DP\\szbuilding_UTM49N\\to_wgs84\\" + sourcename;
                string wgs84_toname = "Wgs84_" + sourcename+ ".shp";
                //project(pFlayer, sourcename, wgs84_topath, wgs84_toname);
                pFW = (IFeatureWorkspace)pWF.OpenFromFile(wgs84_topath + "\\", 0);
                pFC = pFW.OpenFeatureClass(wgs84_toname);

                pFlayer.FeatureClass = pFC;
            }

            //The sorcelayer is for the final clip3.
            sourcepFW = (IFeatureWorkspace)pWF.OpenFromFile("D:\\postgraduate\\vector_tiles\\vectortile_exoriment_data\\DP\\szbuilding_UTM49N\\to_wgs84\\" + sourcename,0);
            sourcepFC = sourcepFW.OpenFeatureClass("Wgs84_" + sourcename + ".shp");
            sourcepFlayer.FeatureClass = sourcepFC;

            if (pFC.ShapeType == esriGeometryType.esriGeometryPoint)
                {
                    Feacturecount(sourcename, sourcepFlayer, pFlayer2, pFlayer, pfeature, cliplayer,level);
                }
            else
                {
                    axMapControl1.AddLayer(pFlayer);
                    // axMapControl1.Refresh();
                    //  MessageBox.Show("Feature To Point");
                    string topath = "D:\\postgraduate\\vector_tiles\\vectortile_exoriment_data\\feacture_topoint\\sz_building\\" + sourcename;
                    if (!Directory.Exists(topath))
                    {
                        Directory.CreateDirectory(topath);
                    }
                    //FeaturetoPoint(pFlayer, topath + "\\" + sourcename + extentname + "topoint.shp");

                    pFW = (IFeatureWorkspace)pWF.OpenFromFile(topath + "\\", 0);
                    pFC = pFW.OpenFeatureClass(sourcename + extentname + "topoint.shp");
                    pFlayer3.FeatureClass = pFC;//pFlayer3 is topoint

                    Feacturecount(sourcename, sourcepFlayer, pFlayer2, pFlayer3, pfeature, cliplayer,level);
                   //pFlayer3 is the topoint, sourcepFlayer is the original data, pFlayer2 is the polygon type, cliplayer is the determined polygon that can crop the original data.


            }

        }



        private void project(IFeatureLayer pInputFeatureLayer,string sorcename, string wgs84_topath, string wgs84_toname)
        {
            j++;
            Geoprocessor GP = new Geoprocessor();
            //  IGeoProcessor2 GP = new GeoProcessorClass();
            GP.OverwriteOutput = true;


            IFeatureClass inputfeatureclass = pInputFeatureLayer.FeatureClass;
          //  IFeatureClass clipfeatureclass = pClipFeatureLayer.FeatureClass;
            
            if (!Directory.Exists(wgs84_topath))

            {
                Directory.CreateDirectory(wgs84_topath);
            }

          //  string name = "Wgs84_" + sorcename + j + extentname + ".shp";
            string strOutShpPath = wgs84_topath + "\\" + wgs84_toname;

            ESRI.ArcGIS.DataManagementTools.Project project_tool = new ESRI.ArcGIS.DataManagementTools.Project();

            project_tool.in_dataset = inputfeatureclass;
            project_tool.out_dataset= strOutShpPath;
            project_tool.out_coor_system = esriSRGeoCSType.esriSRGeoCS_WGS1984;

            GeoProcessorResult gpResult = new GeoProcessorResult();
            gpResult = GP.Execute(project_tool, null) as GeoProcessorResult;
        }


        public void Feacturecount(string sorcename, IFeatureLayer sourcepFlayer, IFeatureLayer pFeaturelayer2, IFeatureLayer pFeaturelayer_count, IFeature pfeature, IFeatureLayer cliplayer, int level)
        {
            //It shouldn't be the range of data that is fetched each time, it should be the range of polygon tiles
            IGeoFeatureLayer geofeaturelayer = pFeaturelayer_count as IGeoFeatureLayer;
            IFeatureClass featureclass = geofeaturelayer.FeatureClass as IFeatureClass;
            IFeatureCursor featureCursor = featureclass.Search(null, false);
            int featurecount = featureclass.FeatureCount(null);   //Total number of elements
        
            if (featurecount > 1045) //Define the Q value
            {
                    Gdtree(sorcename,featurecount, featureCursor, pFeaturelayer_count, pFeaturelayer2,pfeature,level);
               
            }
            else
            {
                double XMin = 180.0;
                double XMax = 0.0;
                double YMin = 90.0;
                double YMax = 0.0;
                if (pfeature == null)
                {
                        IFeature feature = null;
                        while ((feature = featureCursor.NextFeature()) != null)
                        {
                            if (feature.Extent.XMin < XMin)
                            {
                                XMin = feature.Extent.XMin;
                            }
                            if (feature.Extent.XMax > XMax)
                            {
                                XMax = feature.Extent.XMax;
                            }
                            if (feature.Extent.YMin < YMin)
                            {

                                YMin = feature.Extent.YMin;
                            }
                            if (feature.Extent.YMax > YMax)
                            {
                                YMax = feature.Extent.YMax;
                            }
                        }
                    }
                else{
                    XMin = pfeature.Extent.XMin;
                    XMax = pfeature.Extent.XMax;
                    YMin = pfeature.Extent.YMin;
                    YMax = pfeature.Extent.YMax;
                }
                           
                double X = (XMin + XMax) / 2;
                double Y = (YMin + YMax) / 2;
                string geohash_name = EnCode(Y, X);
                if (cliplayer == null)
                {
                    cliplayer = sourcepFlayer;
                }
                Clip3(sorcename,sourcepFlayer, cliplayer,level,geohash_name);
               
            }
        }

        private void Gdtree(string sorcename, int featurecount, IFeatureCursor featureCursor, IFeatureLayer featurelayer, IFeatureLayer featurelayer2, IFeature feature1, int level)
        {
            double XMin = 180.0;
            double XMax = 0.0;
            double YMin = 90.0;
            double YMax = 0.0;

            if (feature1 == null)
            {
                IFeature feature = null;
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    if (feature.Extent.XMin < XMin)
                    {
                        XMin = feature.Extent.XMin;
                    }
                    if (feature.Extent.XMax > XMax)
                    {
                        XMax = feature.Extent.XMax;
                    }
                    if (feature.Extent.YMin < YMin)
                    {

                        YMin = feature.Extent.YMin;
                    }
                    if (feature.Extent.YMax > YMax)
                    {
                        YMax = feature.Extent.YMax;
                    }
                }
            }
            else
            {
                XMin = feature1.Extent.XMin;
                XMax = feature1.Extent.XMax;
                YMin = feature1.Extent.YMin;
                YMax = feature1.Extent.YMax;
            }
            List<double> extent = new List<double> { XMin, XMax, YMin, YMax };
            IPoint point = new PointClass();
            //First block (top right)
            Ring ring1 = new RingClass();
            object missing = Type.Missing;
            //IPointCollection pCol = new PolygonClass();
            point.X = XMax;
            point.Y = YMax;
            ring1.AddPoint(point, ref missing, ref missing);
            point.X = (XMax + XMin) / 2;
            point.Y = YMax;
            ring1.AddPoint(point, ref missing, ref missing);
            point.X = (XMax + XMin) / 2;
            point.Y = (YMax + YMin) / 2;
            ring1.AddPoint(point, ref missing, ref missing);
            point.X = XMax;
            point.Y = (YMax + YMin) / 2;
            ring1.AddPoint(point, ref missing, ref missing);
            point.X = XMax;
            point.Y = YMax;
            ring1.AddPoint(point, ref missing, ref missing);



            Ring ring2 = new RingClass();
            point.X = (XMax + XMin) / 2;
            point.Y = YMax;
            ring2.AddPoint(point, ref missing, ref missing);
            point.X = XMin;
            point.Y = YMax;
            ring2.AddPoint(point, ref missing, ref missing);
            point.X = XMin;
            point.Y = (YMax + YMin) / 2;
            ring2.AddPoint(point, ref missing, ref missing);
            point.X = (XMax + XMin) / 2;
            point.Y = (YMax + YMin) / 2;
            ring2.AddPoint(point, ref missing, ref missing);
            point.X = (XMax + XMin) / 2;
            point.Y = YMax;
            ring2.AddPoint(point, ref missing, ref missing);

            Ring ring3 = new RingClass();
            point.X = (XMax + XMin) / 2;
            point.Y = (YMax + YMin) / 2;
            ring3.AddPoint(point, ref missing, ref missing);
            point.X = XMin;
            point.Y = (YMax + YMin) / 2;
            ring3.AddPoint(point, ref missing, ref missing);
            point.X = XMin;
            point.Y = YMin;
            ring3.AddPoint(point, ref missing, ref missing);
            point.X = (XMax + XMin) / 2;
            point.Y = YMin;
            ring3.AddPoint(point, ref missing, ref missing);
            point.X = (XMax + XMin) / 2;
            point.Y = (YMax + YMin) / 2;
            ring3.AddPoint(point, ref missing, ref missing);

            Ring ring4 = new RingClass();
            point.X = XMax;
            point.Y = (YMax + YMin) / 2;
            ring4.AddPoint(point, ref missing, ref missing);
            point.X = (XMax + XMin) / 2;
            point.Y = (YMax + YMin) / 2;
            ring4.AddPoint(point, ref missing, ref missing);
            point.X = (XMax + XMin) / 2;
            point.Y = YMin;
            ring4.AddPoint(point, ref missing, ref missing);
            point.X = XMax;
            point.Y = YMin;
            ring4.AddPoint(point, ref missing, ref missing);
            point.X = XMax;
            point.Y = (YMax + YMin) / 2;
            ring4.AddPoint(point, ref missing, ref missing);
            
            List<Ring> rings = new List<Ring> { ring1, ring2, ring3, ring4 };
            for (int i = 0; i < 4; i++)
            {
                x++;
                IFeatureClass featureclass = featurelayer2.FeatureClass as IFeatureClass;

                IGeometryCollection pointPolygon = new PolygonClass();
                pointPolygon.AddGeometry(rings[i] as IGeometry);
                IPolygon polyGonGeo = pointPolygon as IPolygon;

                string FeatureType =  sorcename +x+extentname + ".shp";

                IDataset dataset = featureclass as IDataset;
                IWorkspace workspace = dataset.Workspace;
                IFeatureWorkspace featureworkspce = workspace as IFeatureWorkspace;
             
                IFields polygonfield = featureclass.Fields;
                IFeatureClass featureclassPolygon = featureworkspce.CreateFeatureClass(FeatureType, polygonfield, null, null, esriFeatureType.esriFTSimple, "shape", "");


                //Ifeacture is the feacture of the current tile
                IFeature pFeature = featureclassPolygon.CreateFeature();
                pFeature.Shape = polyGonGeo;
                pFeature.Store();

                IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = featureclassPolygon;

                GdtreeToMapcontrol(pFeatureLayer);
                Clip2(sorcename,featurelayer, pFeatureLayer,i, pFeature,level);
            }
        }

        public void GdtreeToMapcontrol(IFeatureLayer gdfeaturelayer)
        {
            ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbol();  
            pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSNull;    


            //pSimpleFillSymbol.Outline = pSimpleOutline;
            ISimpleRenderer pSimpleRenderer = new SimpleRenderer();  
            pSimpleRenderer.Symbol = (ISymbol)pSimpleFillSymbol;   

            IGeoFeatureLayer pGeoFeatureLayer = (IGeoFeatureLayer)gdfeaturelayer;
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)pSimpleRenderer; 
            this.axMapControl1.AddLayer(pGeoFeatureLayer);
           
           // this.axMapControl1.Refresh();
        }


        public void SaveShpToFile(IFeatureClass pFeatureClass, string ExportFilePath, string ExportFileShortName,string DatasetName)
        {
            //Setting parameters for exporting element classes
            IFeatureClassName pOutFeatureClassName = new FeatureClassNameClass();
            IDataset pOutDataset = (IDataset)pFeatureClass;
            pOutFeatureClassName = (IFeatureClassName)pOutDataset.FullName;
            //Create a workspace for outputting shp files
            IWorkspaceFactory pShpWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspaceName pInWorkspaceName = new WorkspaceNameClass();
            pInWorkspaceName = pShpWorkspaceFactory.Create(ExportFilePath, ExportFileShortName, null, 0);

            //Create an element class
            IFeatureClassName pInFeatureClassName = new FeatureClassNameClass();
            IDatasetName pInDatasetClassName;
            pInDatasetClassName = (IDatasetName)pInFeatureClassName;
            pInDatasetClassName.Name = DatasetName;//as an output parameter
            pInDatasetClassName.WorkspaceName = pInWorkspaceName;
            IFeatureDataConverter pShpToClsConverter = new FeatureDataConverterClass();
            pShpToClsConverter.ConvertFeatureClass(pOutFeatureClassName, null, null, pInFeatureClassName, null, null, "", 1000, 0);
            //MessageBox.Show("Export Successful", "System Alert")

            //Add the exported part to the map
            IFeatureLayer fea = new FeatureLayerClass();
            fea.FeatureClass = pFeatureClass;
            axMapControl1.AddLayer(fea);

        }

       
        public void Clip2(string sorcename, IFeatureLayer pInputFeatureLayer, IFeatureLayer pClipFeatureLayer,int i,IFeature pfeature,int level)
        {
            j++;
            Geoprocessor GP = new Geoprocessor();
           //  IGeoProcessor2 GP = new GeoProcessorClass();
            GP.OverwriteOutput = true;


            IFeatureClass inputfeatureclass = pInputFeatureLayer.FeatureClass;
            IFeatureClass clipfeatureclass = pClipFeatureLayer.FeatureClass;
            

            string Path = "D:\\postgraduate\\vector_tiles\\vectortile_exoriment_data\\clip\\sz_building\\" + sorcename;
            if (!Directory.Exists(Path))

            {
                Directory.CreateDirectory(Path);
            }

            string name = "Clip" + i + x +j+ extentname ;
            string strOutShpPath = Path+ "\\" + name + ".shp";
            ESRI.ArcGIS.AnalysisTools.Clip clipTool = new ESRI.ArcGIS.AnalysisTools.Clip();

            clipTool.in_features = inputfeatureclass;
            clipTool.clip_features = clipfeatureclass;
            clipTool.out_feature_class = strOutShpPath;

            GeoProcessorResult gpResult = new GeoProcessorResult();
            gpResult = GP.Execute(clipTool, null) as GeoProcessorResult;
            //The result of the cut is then used to determine the number of elements.
            IWorkspaceFactory pWF;
            pWF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFW;
            //string filepath = "G:\\wh_road\\polygon";
            pFW = (IFeatureWorkspace)pWF.OpenFromFile("D:\\postgraduate\\vector_tiles\\vectortile_exoriment_data\\polygon2", 0);
            IFeatureClass pFC2;
            pFC2 = pFW.OpenFeatureClass("polygon.shp");
            IFeatureLayer pFlayer2 = new FeatureLayerClass();
            pFlayer2.FeatureClass = pFC2;

            addshpfrompath(Path,  name,sorcename, pFlayer2,pfeature,pClipFeatureLayer, level);
         
            

            //ESRI.ArcGIS.AnalysisTools.Clip clipTool = new ESRI.ArcGIS.AnalysisTools.Clip("G:/wh_road/District_boundary.shp", "G:/wh_road/polygon.shp", "G://wh_road//Clip.shp");
            //clipTool.in_features = pInputFeatureClass;
            //clipTool.clip_features = pClipFeatureClass;
            //clipTool.out_feature_class = outfeatureclass;
            //GP.Execute(clipTool, null);

            //return pInputFeatureClass;
        }
        public void Clip3(string sorcename, IFeatureLayer pInputFeatureLayer, IFeatureLayer pClipFeatureLayer, int level,string geohash_name)
        {
            j++;
            Geoprocessor GP = new Geoprocessor();
            //  IGeoProcessor2 GP = new GeoProcessorClass();
            GP.OverwriteOutput = true;


            IFeatureClass inputfeatureclass = pInputFeatureLayer.FeatureClass;
            IFeatureClass clipfeatureclass = pClipFeatureLayer.FeatureClass;
            string topath = "D:\\postgraduate\\vector_tiles\\vectortile_exoriment_data\\result_clip\\sz_building\\" + sorcename;
            if (!Directory.Exists(topath))

            {
                Directory.CreateDirectory(topath);
            }

            
           
            string name = Convert.ToString(level)+"_"+geohash_name+ ".shp";
            string strOutShpPath = topath + "\\" + name;

            ESRI.ArcGIS.AnalysisTools.Clip clipTool = new ESRI.ArcGIS.AnalysisTools.Clip();
            clipTool.in_features = inputfeatureclass;
            clipTool.clip_features = clipfeatureclass;
            clipTool.out_feature_class = strOutShpPath;
            GeoProcessorResult gpResult = new GeoProcessorResult();
            gpResult = GP.Execute(clipTool, null) as GeoProcessorResult;
            

            
          
            if (File.Exists(strOutShpPath))
            {
                //Thread.Sleep(TimeSpan.FromMinutes(0.5));
                //The result of the cut is then used to determine the number of elements.
                IWorkspaceFactory pWF;
                pWF = new ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace pFW;
                //string filepath = "G:\\wh_road\\polygon";
                pFW = (IFeatureWorkspace)pWF.OpenFromFile(topath, 0);
                IFeatureClass pFC2;

                pFC2 = pFW.OpenFeatureClass(name);
                IFeatureLayer pFlayer2 = new FeatureLayerClass();
                pFlayer2.FeatureClass = pFC2;

                axMapControl1.AddLayer(pFlayer2);
                // axMapControl1.Refresh();

                IFeatureClass featureclass = pFlayer2.FeatureClass;
                ESRI.ArcGIS.ConversionTools.FeaturesToJSON tojsonTool = new ESRI.ArcGIS.ConversionTools.FeaturesToJSON();
                tojsonTool.in_features = featureclass;

                string topath2 = "D:\\postgraduate\\vector_tiles\\vectortile_exoriment_data\\result_json\\sz_building\\" + sorcename;
                if (!Directory.Exists(topath2))

                {
                    Directory.CreateDirectory(topath2);
                }
                tojsonTool.out_json_file = topath2 + "\\" + Convert.ToString(level) + "_" + geohash_name + ".json";

                //Indicates that a geojson file was generated.
                tojsonTool.geoJSON = "true";
                GeoProcessorResult gpResult2 = new GeoProcessorResult();
                gpResult2 = GP.Execute(tojsonTool, null) as GeoProcessorResult;

                jsonname.Add("'" + extentname + "json" + sorcename + j + ".json" + "'");
            }
            else
            {
              
            }

            

        }


        List<string> GetAllFileNames(string path, string pattern = "*")
        {
            List<FileInfo> folder = new DirectoryInfo(path).GetFiles(pattern).ToList();

            return folder.Select(x => x.Name).ToList();
        }

        public void FeaturetoPoint(IFeatureLayer pInputFeatureLayer, string topoint_name)
        {
            Geoprocessor GP = new Geoprocessor();
            //  IGeoProcessor2 GP = new GeoProcessorClass();
            GP.OverwriteOutput = true;
            ESRI.ArcGIS.DataManagementTools.FeatureVerticesToPoints featureVerticesToPoints = new ESRI.ArcGIS.DataManagementTools.FeatureVerticesToPoints();
           // ESRI.ArcGIS.DataManagementTools.FeatureToPoint featureTopoint = new ESRI.ArcGIS.DataManagementTools.FeatureToPoint();
            IFeatureClass inputfeatureclass = pInputFeatureLayer.FeatureClass;
            featureVerticesToPoints.in_features = inputfeatureclass;
            featureVerticesToPoints.out_feature_class = topoint_name;
            GeoProcessorResult gpResult = new GeoProcessorResult();
            gpResult = GP.Execute(featureVerticesToPoints, null) as GeoProcessorResult;
            
        }

        private void writeListToFile(List<string> pList, string myFileName)
        {
            //Creates a file stream to write to or creates a StreamWriter
            System.IO.FileStream fs = new System.IO.FileStream(myFileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.Flush();
            // Using StreamWriter to write to a file
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
            // Write the contents of richTextBox1 to a file.
            for (int i = 0; i < pList.Count; i++)
            {

                m_streamWriter.WriteLine(pList[i]);
                m_streamWriter.Write(",");
            }
            //Close this file
            m_streamWriter.Flush();
            m_streamWriter.Close();

        }
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ToFile_Click(object sender, EventArgs e)
        {
            writeListToFile(jsonname, @"D:\Express\Expresslogin\public\json\jsonname2.txt");
            MessageBox.Show("completed!");
        }

        private void DP_Click(object sender, EventArgs e)
        {
            for (int m = 0; m < resolutions.Count; m++)
            {
                double resolution_m = (resolutions[m] * 2 * Math.PI * 6371004) / 360;
            }

            System.Windows.Forms.OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.Title = "Open SHP file";
            openfiledialog.Filter = "SHP file (*.shp)|*.shp";
            if (openfiledialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileinfo = new FileInfo(openfiledialog.FileName);     // Provides example methods for creating, copying, deleting, moving and opening files     
                string path = fileinfo.Directory.ToString();
                string name = fileinfo.Name.Substring(0, fileinfo.Name.IndexOf("."));
                for (int m = 0; m < resolutions.Count; m++)
                {
                    double resolution_m = (resolutions[m] * 2 * Math.PI * 6371004) / 360;
                    DouglasAnalysis(path + "\\" + name + ".shp", resolution_m, path + "\\DP\\" +m+ name + ".shp");
                }
                MessageBox.Show("completed");
                }
        }

        public void DouglasAnalysis(string inputshpfile, double DisThreshold, string outshpfile)
        {
            //Determine if the input file exists
            if (!System.IO.File.Exists(inputshpfile))
                return;

            //Read input vector file (element set -> element -> node set)
            IFeatureClass pInputFeaCls = GetFeaclsFromFile(inputshpfile);
            int feaCount = pInputFeaCls.FeatureCount(null);//Note the meaning of null

            //New output file
            IFeatureClass pOutFeaCls = CreateShpFromOneShpfile(pInputFeaCls, outshpfile);

            for (int i = 0; i < feaCount; i++)
            {
                IFeature pFea = pInputFeaCls.GetFeature(i);

                IPointCollection pPoints = pFea.Shape as IPointCollection;

                int pointCount = pPoints.PointCount;
                //Starting point-by-point to determine if the key points under the current conditions are met
                //A ----B ----C, determine whether the distance from point B to line segment AC is greater than the DisThreshold value, if true, B is a critical point, otherwise B is a non-critical point
                IPointCollection pNewPoints = null;
                //Instantiate pNewPoints based on the input element types point, line and surface.
                IPolyline pPolyLine = new PolylineClass();
                IPolygon pPolygon = new PolygonClass();
                //Determine the type of input fitness data (here only for line-plane vector data)
                if (pFea.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                {

                    pNewPoints = (IPointCollection)pPolyLine;
                }

                if (pFea.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                {

                    pNewPoints = (IPointCollection)pPolygon;
                }
                //Load the first point into the new point set
                pNewPoints.AddPoint(pPoints.get_Point(0), Type.Missing, Type.Missing);

                int FirstPoint = 0, SecondPoint = 1, SerdPoint = 2;

                for (int k = 0; k < pointCount - 2; k++)
                {

                    double dis = disP2L(pPoints.get_Point(FirstPoint), pPoints.get_Point(SecondPoint),pPoints.get_Point(SerdPoint));
                    if (dis >= DisThreshold)
                    {
                        pNewPoints.AddPoint(pPoints.get_Point(SecondPoint), Type.Missing, Type.Missing);
                        FirstPoint = SecondPoint;
                        SecondPoint++; SerdPoint++;
                    }

                    else
                    {
                        SecondPoint++; SerdPoint++;
                    }
                }

                //Load the last point into the new point set
                pNewPoints.AddPoint(pPoints.get_Point(pointCount - 1), Type.Missing, Type.Missing);
                //Convert pNewPoints to elements to be stored in the output file
                //Instantiate pnewGeometry based on the input element types point, line and surface.
                IGeometry pnewGeometry = null;
                if (pFea.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                {
                    pPolyLine = pNewPoints as IPolyline;
                    pnewGeometry = pPolyLine as IGeometry;
                    AddFeature(pOutFeaCls, pnewGeometry);
                }
                if (pFea.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    pPolygon = pNewPoints as IPolygon;
                    pnewGeometry = pPolygon as IGeometry;
                    AddFeature(pOutFeaCls, pnewGeometry);
                }
            }//end for feaCount
        }

        /// <summary>
        /// Getting the element set
        /// </summary>
        IFeatureClass GetFeaclsFromFile(string shpFile)
        {
            IFeatureClass pFeaCls = null;
            string FilePath = System.IO.Path.GetDirectoryName(shpFile);
            string FileName = System.IO.Path.GetFileNameWithoutExtension(shpFile);
            IWorkspaceFactory pWorkspaceFeatory = new ShapefileWorkspaceFactory();
            IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFeatory.OpenFromFile(FilePath, 0);
            pFeaCls = pFeatureWorkspace.OpenFeatureClass(FileName);
            return pFeaCls;
        }

        /// <summary>
        /// Copy a shpfile to create a blank file
        /// </summary>
        ///
        IFeatureClass CreateShpFromOneShpfile(IFeatureClass pFeacls, string Outfile)
        {
            IDataset pDataset = pFeacls as IDataset;
            if (pDataset == null) return null;

            string pOutShapePath = System.IO.Path.GetDirectoryName(Outfile);
            string pOutShapeName = System.IO.Path.GetFileNameWithoutExtension(Outfile);

            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IWorkspace pWorkspace = pWSF.OpenFromFile(pOutShapePath, 0);

            IFeatureClass pfcls = (IFeatureClass)pDataset.Copy(pOutShapeName, pWorkspace);

            ITable pTable = pfcls as ITable;
            pTable.DeleteSearchedRows(null);
            return pfcls;
        }

        /// <summary>
        /// point-to-point distance
        /// </summary>
        double PointToLineLength(IPoint pPoint, IPoint pLStaPoint, IPoint pLEndPoint)
        {
            double len = 0.0;
            //Calculate the distance from a point to a line using Helen's formula
            double a = Math.Sqrt((pLEndPoint.X - pPoint.X) * (pLEndPoint.X - pPoint.X) + (pLEndPoint.Y - pPoint.Y) * (pLEndPoint.Y - pPoint.Y));
            double b = Math.Sqrt((pLEndPoint.X - pLStaPoint.X) * (pLEndPoint.X - pLStaPoint.X) + (pLEndPoint.Y - pLStaPoint.Y) * (pLEndPoint.Y - pLStaPoint.Y));
            double c = Math.Sqrt((pLStaPoint.X - pPoint.X) * (pLStaPoint.X - pPoint.X) + (pLStaPoint.Y - pPoint.Y) * (pLStaPoint.Y - pPoint.Y));
            double p = (a + b + c) / 2;
            len = 2 * Math.Sqrt(p * (p - a) * (p - b) * (p - c)) / a;
            return len;
        }


        /// <summary>
        /// Adding Elements
        /// </summary>
        IFeatureClass AddFeature(IFeatureClass pFeacls, IGeometry pGeometry)
        {
            IFeatureCursor pFeatCur = pFeacls.Insert(true);
            IFeatureBuffer pFeatBuff = pFeacls.CreateFeatureBuffer();
            SetZMvalue(pFeatBuff, pGeometry);

            pFeatBuff.Shape = pGeometry;
            pFeatCur.InsertFeature(pFeatBuff);

            return pFeacls;
        }

        /// <summary>
        /// Setting the ZM value
        /// </summary>
        void SetZMvalue(IFeatureBuffer pfeabuff, IGeometry pgeometry)
        {
            int tIndex = pfeabuff.Fields.FindField("Shape");
            IGeometryDef pGeometryDef = pfeabuff.Fields.get_Field(tIndex).GeometryDef;

            if (pGeometryDef.HasZ)
            {
                IZAware pZAware = (IZAware)pgeometry;
                pZAware.ZAware = true;
            }
            else
            {
                IZAware pZAware = (IZAware)pgeometry;
                pZAware.ZAware = false;
            }

            if (pGeometryDef.HasM)
            {
                IMAware pMAware = (IMAware)pgeometry;
                pMAware.MAware = true;
            }
            else
            {
                IMAware pMAware = (IMAware)pgeometry;
                pMAware.MAware = false;
            }
        }

        
        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            var a = new Coordinate(lat1, lon1);
            var b = new Coordinate(lat2, lon2);
            if (lat1 == lat2 && lon1 == lon2)
            {
                return 0;
            }

            a.lon = VC(a.lon, -180, 180);
            a.lat = aD(a.lat, -74, 74);
            b.lon = VC(b.lon, -180, 180);
            b.lat = aD(b.lat, -74, 74);
            double ret = JF(JK(a.lon), JK(b.lon), JK(a.lat), JK(b.lat));
            return ret;
        }
        public static double aD(double a, double b, double c)
        {
            if (b != null)
            {
                a = Math.Max(a, b);
            }
            if (b != null)
            {
                a = Math.Min(a, c);
            }
            return a;
        }
        public static double VC(double a, double b, double c)
        {
            if (a > c)
            {
                a -= c - b;
            }
            if (a < b)
            {
                a += c - b;
            }
            return a;
        }
        public static double JK(double a)
        {
            double ret = Math.PI * a / 180.0;
            return ret;
        }
        public static double JF(double a, double b, double c, double e)
        {
            const double Ou = 6370996.81;
            return Ou * Math.Acos(Math.Sin(c) * Math.Sin(e) + Math.Cos(c) * Math.Cos(e) * Math.Cos(b - a));
        }
        public class Coordinate
        {
            public Coordinate(double lat, double lon)
            {
                this.lat = lat;
                this.lon = lon;
            }
            /// <summary>
            /// lat
            /// </summary>
            public double lat { get; set; }
            /// <summary>
            ///lon
            /// </summary>
            public double lon { get; set; }
        }


        
        double disP2L(IPoint pPoint, IPoint pLStaPoint, IPoint pLEndPoin) //The first and last are the ends of the line, and the third is the third point.						
        {
            //x is the longitude lon, +-180; y is the latitude lat, +-90
            double x0 = pLStaPoint.X;       //Obtain the xy coordinates of the two endpoints and the measured distance point.
            double y0 = pLStaPoint.Y;
            double x1 = pLEndPoin.X;
            double y1 = pLEndPoin.Y;
            double x = pPoint.X;
            double y = pPoint.Y;

           // method 1：https://blog.csdn.net/qq_27559331/article/details/94649398?ops_request_misc=&request_id=&biz_id=102&utm_term=C?ops_request_misc=&request_id=&biz_id=102&utm_term=C&utm_medium=distribute.pc_search_result.none-task-blog-2~all~sobaiduweb~default-0-94649398.142^v59^js_top,201^v3^add_ask&spm=1018.2226.3001.4187#%E6%A0%B9%E6%8D%AE%E7%BB%8F%E7%BA%AC%E5%BA%A6%E8%AE%A1%E7%AE%97%E7%82%B9%E5%88%B0%E7%BA%BF%E6%AE%B5%E7%9A%84%E8%B7%9D%E7%A6%BB&utm_medium=distribute.pc_search_result.none-task-blog-2~all~sobaiduweb~default-0-94649398.142^v59^js_top,201^v3^add_ask
            double d1 = Distance(y0, x0, y1, x1);
            double d2 = Distance(y0, x0, y, x);
            double d3 = Distance(y, x, y1, x1);
            double PointToLine = 2 * Halun(d1, d2, d3) / d1;



            //method 2：https://blog.csdn.net/Duanzhengqi/article/details/124620381
            // double d1 = Getdistance(x0, y0,x1 ,y1);
            //double d2 = Getdistance(x0, y0, x, y);
            //double d3 = Getdistance(x, y, x1, y1);
            //double PointToLine = 2 * Halun(d1, d2, d3) / d1;

            //method 3：
            //Calculate the distance from the point to the line according to the formula
            // double disSuqare = ((y0 - y1) * x + (x1 - x0) * y + (x0 * y1 - x1 * y0)) * ((y0 - y1) * x + (x1 - x0) * y + (x0 * y1 - x1 * y0)) / ((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
            //return disSuqare;
            return PointToLine;
        }
        double Halun(double aa, double bb, double cc)
        {
            double pp = (aa + bb + cc) / 2;
            double S =Math.Sqrt (pp * (pp - aa) * (pp - bb) * (pp - cc));//面积
            return S;
        }


        double Getdistance(double Lon1, double Lat1, double Lon2, double Lat2)
        {//（Haversine formula）：
         //Latitude and longitude are first converted to an angular system rad
            D2R = PI / 180.0;
            double rLon1 = Lon1 * D2R;
            double rLon2 = Lon2 * D2R;
            double rLat1 = Lat1 * D2R;
            double rLat2 = Lat2 * D2R;
            double a = rLat1 - rLat2;
            double b = rLon1 - rLon2;
            double below_sqrt = Math.Sin(a / 2) * Math.Sin(a / 2) + Math.Cos(rLat1) * Math.Cos(rLat2) * Math.Sin(b / 2) * Math.Sin(b / 2);
            double Long = 2 * Math.Asin(Math.Sqrt(below_sqrt)) * R0;
            //The range of values for tip:asin is [-pi/2,pi/2].
            return Long;
        }

        public static IPolyline CreatePolyline(IPointCollection PolylineList)
        {
            ISegment pSegment;
            ILine pLine;
            object o = Type.Missing;
            ISegmentCollection pPath = new PathClass();
            for (int i = 0; i < PolylineList.PointCount - 1; i++)
            {
                pLine = CreateLine(PolylineList.get_Point(i), PolylineList.get_Point(i + 1));
                pSegment = pLine as ISegment;
                pPath.AddSegment(pSegment, ref o, ref o);

            }
            IGeometryCollection pPolyline = new PolylineClass();
            pPolyline.AddGeometry(pPath as IGeometry, ref o, ref o);
            return pPolyline as IPolyline;
        }

        public static ILine CreateLine(IPoint from, IPoint to)
        {
            ILine pLine = new LineClass();
            pLine.PutCoords(from, to);
            return pLine;
        }


        #region private method
        /// <summary>
        /// Latitude and longitude converted to binary code
        /// </summary>
        /// <param name="value">longitudes</param>
        /// <param name="min">Latitude and longitude minima</param>
        /// <param name="max">Latitude and longitude maxima</param>
        /// <param name="length">Binary code length</param>
        /// <returns></returns>
        private static bool[] getHashArray(double value, double min, double max, int length)
        {
            if (value < min || value > max)
            {
                return null;
            }
            if (length < 1)
            {
                return null;
            }
            bool[] result = new bool[length];
            for (int i = 0; i < length; i++)
            {
                double mid = (min + max) / 2;
                if (value > mid)
                {
                    result[i] = true;
                    min = mid;
                }
                else
                {
                    result[i] = false;
                    max = mid;
                }
            }
            return result;
        }
        /// <summary>
        /// Combining binary codes for longitude and latitude
        /// </summary>
        /// <param name="latArray">Latitude binary code</param>
        /// <param name="lngArray">binary code for longitude</param>
        /// <returns></returns>
        private static bool[] getMergeArray(bool[] latArray, bool[] lngArray)
        {
            if (latArray == null || lngArray == null)
            {
                return null;
            }
            bool[] result = new bool[lngArray.Length + latArray.Length];
            for (int i = 0; i < lngArray.Length; i++)
            {
                result[2 * i] = lngArray[i];
            }
            for (int i = 0; i < latArray.Length; i++)
            {
                result[2 * i + 1] = latArray[i];
            }
            return result;
        }
        /// <summary>
        /// binary section to character conversion
        /// </summary>
        /// <param name="base32">binary array of sections</param>
        /// <returns></returns>
        private static char getBase32Char(bool[] base32)
        {
            if (base32 == null || base32.Length != 5)
            {
                return ' ';
            }
            int num = 0;
            foreach (bool item in base32)
            {
                num <<= 1;
                if (item)
                {
                    num += 1;
                }
            }
            return CHARS[num % CHARS.Length];
        }

        /// <summary>
        /// Latitude and longitude converted to radians
        /// </summary>
        /// <param name="d">longitude and latitude</param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return (double)d * Math.PI / 180d;
        }

        /// <summary>
        ///  Decoding Refinement Interval
        /// </summary>
        /// <param name="interval">Array to be refined</param>
        /// <param name="cd">character subscript</param>
        /// <param name="mask">delimited index</param>
        private static void refineInterval(ref double[] interval, int cd, int mask)
        {
            if ((cd & mask) != 0)
            {
                interval[0] = (interval[0] + interval[1]) / 2;
            }
            else
            {
                interval[1] = (interval[0] + interval[1]) / 2;
            }
        }
        #endregion


        /// <summary>
        /// Coding of latitude and longitude
        /// </summary>
        /// <param name="lat">lat</param>
        /// <param name="lng">lon</param>
        /// <returns></returns>
        public static string EnCode(double lat, double lng)
        {
            bool[] latArray = getHashArray(lat, -90, 90, Binary_Length);
            bool[] lngArray = getHashArray(lng, -180, 180, Binary_Length);
            bool[] bools = getMergeArray(latArray, lngArray);
            if (bools == null)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bools.Length; i = i + 5)
            {
                bool[] base32 = new bool[5];
                for (int j = 0; j < 5; j++)
                {
                    base32[j] = bools[i + j];
                }
                char cha = getBase32Char(base32);
                if (' ' == cha)
                {
                    return null;
                }
                sb.Append(cha);
            }
            return sb.ToString();
        }

    }
}
