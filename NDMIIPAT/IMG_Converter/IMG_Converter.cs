using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;

using OSGeo.GDAL;
using OSGeo.OSR;

namespace IMG_Converter
{
    /// @class :     IMG_Converter
    /// @brief :     IMG 파일 변환
    /// @author :    SiJun Kim (sjkim@soletop.com)
    /// @date :      2016-10-21
    /// @version :    1.0
    /// @revision : 
    public class IMG_Converter
    {
        private static readonly IMG_Converter instance = new IMG_Converter();
        public static IMG_Converter Instance
        {
            get
            {
                return instance;
            }
        }

        private string _strPngFile = null;

        private IMG_Converter()
        {

        }

        public bool ChangeProjectionByUTM(string inPut, string outPut, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);
            bool isExtif = false;
            bool isExtiff = false;

            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return false;
            else
            {
                //입력 파일이 tif / tiff 파일이 아니라면 종료
                if (inPutFile.Extension != ".tif" && inPutFile.Extension != ".tiff")
                    return false;

                if (inPutFile.Extension == ".tif")
                    isExtif = true;

                if (inPutFile.Extension == ".tiff")
                    isExtiff = true;
            }

            Gdal.AllRegister();

            try
            {
                #region Change projection

                string strWorldSinusoidal = "PROJCS[\"World_Sinusoidal\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]],PROJECTION[\"Sinusoidal\"],PARAMETER[\"longitude_of_center\",0],PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]]]";
                string t_srs_wkt = "GEOGCS[\"WGS84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS84\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.01745329251994328]]";

                Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);
                dataset.SetProjection(strWorldSinusoidal);
                string s_srs_wkt = dataset.GetProjectionRef();

                Dataset dswarp = Gdal.AutoCreateWarpedVRT(dataset, s_srs_wkt, t_srs_wkt, ResampleAlg.GRA_NearestNeighbour, 0);

                string[] options = null;
                OSGeo.GDAL.Driver srcDrv = Gdal.GetDriverByName("GTiff");

                FileInfo outPutFile = new FileInfo(outPut);
                string testPath = string.Empty;

                if (isExtif) testPath = string.Format("{0}\\test_Png_{1}", outPutFile.DirectoryName, inPutFile.Name.Replace(".tif", ".png"));
                else
                    if (isExtiff) testPath = string.Format("{0}\\test_Png_{1}", outPutFile.DirectoryName, inPutFile.Name.Replace(".tiff", ".png"));

                Dataset dstDs = srcDrv.CreateCopy(testPath, dswarp, 0, options, null, null);

                #endregion

                #region Create PNG

                string file_name = Path.GetFileNameWithoutExtension(inPutFile.Name) + ".png";
                //string pngPath = string.Format("{0}\\{1}", outPutFile.DirectoryName, file_name);
                _strPngFile = string.Format("{0}\\{1}", outPutFile.DirectoryName, file_name);

                Band band1 = dstDs.GetRasterBand(band); //특정 벤드 가져오기

                int startX = 0;
                int startY = 0;
                int width = band1.XSize;
                int height = band1.YSize;

                int nBandSpace = Gdal.GetDataTypeSize(DataType.GDT_Byte) / 8;
                int nPixelSpace = nBandSpace * dataset.RasterCount;
                int nLineSpace = nPixelSpace * width;

                double[] buffer = new double[width * height];
                CPLErr d = band1.ReadRaster(startX, startY, width, height, buffer, width, height, 0, 0);

                double[] minmax = new double[2];
                band1.ComputeRasterMinMax(minmax, 0);

                Bitmap bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double value = buffer[x + y * width];
                        if (!Double.IsNaN(value))
                            if (value >= minmax[0] && value <= minmax[1] && value != 0)
                            {
                                double colorValue = (value - minmax[0]) * 255 / (minmax[1] - minmax[0]);
                                Color newColor = Color.FromArgb(Convert.ToInt32(colorValue), Convert.ToInt32(colorValue), Convert.ToInt32(colorValue));
                                bitmap.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                Color newColor = Color.FromArgb(0, 0, 0, 0);
                                bitmap.SetPixel(x, y, newColor);
                            }
                    }
                }
                bitmap.Save(_strPngFile, System.Drawing.Imaging.ImageFormat.Png);
                #endregion

                #region Create a GeoTiff
                double north = 36.16465526448886; //북
                double south = 35.97362616042732; //남
                double east = 127.5672085281825; //동
                double west = 127.3435070025512; //서

                CheckGeoTransform(ref north, ref south, ref east, ref west, inPutFile);
                double[] newGeot = CalcGeoTransform(dstDs, north, south, east, west);

                OSGeo.GDAL.Driver driver = Gdal.GetDriverByName("GTiff");
                Dataset oldDataSet = Gdal.Open(_strPngFile, Access.GA_ReadOnly);
                Dataset newDataSet = srcDrv.CreateCopy(outPut, oldDataSet, 0, null, null, null);

                newDataSet.SetProjection(dswarp.GetProjectionRef());
                newDataSet.SetGeoTransform(newGeot);

                #endregion

                oldDataSet.Dispose();
                newDataSet.Dispose();
                driver.Dispose();
                dataset.Dispose();
                bitmap.Dispose();
                dstDs.Dispose();
                srcDrv.Dispose();
                dswarp.Dispose();
                File.Delete(testPath);
                //File.Delete(pngPath);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        public void TiffToKmlForModis(string inPut, string outPut)
        {
            FileInfo inPutFile = new FileInfo(inPut);

            string copyPngFile = Path.GetDirectoryName(_strPngFile) + @"/" + Path.GetFileNameWithoutExtension(_strPngFile) + "_kml.png";
            string pngFile = Path.GetFileNameWithoutExtension(_strPngFile) + "_kml.png";

            File.Copy(_strPngFile, copyPngFile);

            double north = 36.16465526448886; //북
            double south = 35.97362616042732; //남
            double east = 127.5672085281825; //동
            double west = 127.3435070025512; //서
            CheckGeoTransform(ref north, ref south, ref east, ref west, inPutFile);

            StreamWriter sw = new StreamWriter(outPut);
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sw.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.opengis.net/kml/2.2 http://schemas.opengis.net/kml/2.2.0/ogckml22.xsd http://www.google.com/kml/ext/2.2 http://code.google.com/apis/kml/schema/kml22gx.xsd\">");
            sw.WriteLine("<Document id=\"radar\">");
            sw.WriteLine("\t<name>" + inPutFile.Name + "</name>");
            sw.WriteLine("\t<Snippet></Snippet>");
            sw.WriteLine("\t<GroundOverlay id=\"0\">");
            sw.WriteLine("\t\t<Snippet></Snippet>");
            sw.WriteLine("\t\t<drawOrder>1000</drawOrder>");
            sw.WriteLine("\t\t<name>" + inPutFile.Name + "</name>");
            sw.WriteLine("\t\t<Icon>\r\t\t\t<href>" + pngFile + "</href>\r\t\t\t<viewBoundScale>1.0</viewBoundScale>\r\t\t</Icon>");
            sw.WriteLine("\t\t<LatLonBox>");
            sw.WriteLine("\t\t\t<north>" + north.ToString() + "</north>"); //북
            sw.WriteLine("\t\t\t<south>" + south.ToString() + "</south>"); //남
            sw.WriteLine("\t\t\t<east>" + east.ToString() + "</east>");   //동
            sw.WriteLine("\t\t\t<west>" + west.ToString() + "</west>");   //서
            sw.WriteLine("\t\t\t<rotation>0</rotation>"); //회전
            sw.WriteLine("\t\t</LatLonBox>");
            sw.WriteLine("\t</GroundOverlay>");
            sw.WriteLine("</Document>");
            sw.WriteLine("</kml>");
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public void TiffToJpg(string outPut)
        {
            Image im = Image.FromFile(_strPngFile);
            im.Save(outPut, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        public bool ImgToGeoTiff(string inPut, string outPut, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);

            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return false;
            else
            {
                //입력 파일이 img 파일이 아니라면 종료
                if (inPutFile.Extension != ".img")
                    return false;
            }

            try
            {
                #region Create Tiff
                Gdal.AllRegister();
                Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);

                Band band1 = dataset.GetRasterBand(band); //특정 벤드 가져오기
                int width = band1.XSize;
                int height = band1.YSize;

                int startX = 0;
                int startY = 0;

                int nBandSpace = Gdal.GetDataTypeSize(DataType.GDT_Byte) / 8;
                int nPixelSpace = nBandSpace * dataset.RasterCount;
                int nLineSpace = nPixelSpace * width;

                double[] buffer = new double[width * height];
                CPLErr d = band1.ReadRaster(startX, startY, width, height, buffer, width, height, 0, 0);

                double[] minmax = new double[2];
                band1.ComputeRasterMinMax(minmax, 0);

                #region 복사할 이미지 생성
                Bitmap bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double value = buffer[x + y * width];
                        if (!Double.IsNaN(value))
                            if (value >= minmax[0] && value <= minmax[1] && value != 0)
                            {
                                double colorValue = (value - minmax[0]) * 255 / (minmax[1] - minmax[0]);
                                Color newColor = Color.FromArgb(Convert.ToInt32(colorValue), Convert.ToInt32(colorValue), Convert.ToInt32(colorValue));
                                bitmap.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                Color newColor = Color.FromArgb(0, 0, 0, 0);
                                bitmap.SetPixel(x, y, newColor);
                            }
                    }
                }
                FileInfo outPutFile = new FileInfo(outPut);
                string pngPath = string.Format("{0}\\{1}", outPutFile.DirectoryName, inPutFile.Name.Replace(".img", ".png"));
                bitmap.Save(pngPath, System.Drawing.Imaging.ImageFormat.Png);
                #endregion

                string[] options = null;
                OSGeo.GDAL.Driver srcDrv = Gdal.GetDriverByName("GTiff");
                Dataset srcDs = Gdal.Open(pngPath, Access.GA_ReadOnly);
                Dataset dstDs = srcDrv.CreateCopy(outPut, srcDs, 0, options, null, null);

                dstDs.SetProjection(dataset.GetProjection());

                double[] geot = new double[6];
                dataset.GetGeoTransform(geot);
                dstDs.SetGeoTransform(geot);

                dstDs.FlushCache();
                dstDs.Dispose();
                srcDs.Dispose();
                srcDrv.Dispose();
                bitmap.Dispose();

                File.Delete(pngPath);

                #endregion
            }
            catch(Exception e)
            {
                return false;
            }

            return true;
        }

        public bool ImgToPng(string inPut, string outPut, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);

            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return false;
            else
            {
                //입력 파일이 img 파일이 아니라면 종료
                if (inPutFile.Extension != ".img")
                    return false;
            }

            try
            {
                #region Create PNG
                Gdal.AllRegister();
                Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);

                Band band1 = dataset.GetRasterBand(band); //특정 벤드 가져오기
                int width = band1.XSize;
                int height = band1.YSize;

                int startX = 0;
                int startY = 0;

                int nBandSpace = Gdal.GetDataTypeSize(DataType.GDT_Byte) / 8;
                int nPixelSpace = nBandSpace * dataset.RasterCount;
                int nLineSpace = nPixelSpace * width;

                double[] buffer = new double[width * height];
                CPLErr d = band1.ReadRaster(startX, startY, width, height, buffer, width, height, 0, 0);

                double[] minmax = new double[2];
                band1.ComputeRasterMinMax(minmax, 0);

                Bitmap bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double value = buffer[x + y * width];
                        if (!Double.IsNaN(value))
                            if (value >= minmax[0] && value <= minmax[1] && value != 0)
                            {
                                double colorValue = ((value - minmax[0]) / (minmax[1] - minmax[0])) * 255;
                                Color newColor = Color.FromArgb(Convert.ToInt32(colorValue), Convert.ToInt32(colorValue), Convert.ToInt32(colorValue));
                                bitmap.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                Color newColor = Color.FromArgb(0, 0, 0, 0);
                                bitmap.SetPixel(x, y, newColor);
                            }
                    }
                }
                bitmap.Save(outPut, System.Drawing.Imaging.ImageFormat.Png);

                dataset.Dispose();
                bitmap.Dispose();
                #endregion

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool ImgToJpg(string inPut, string outPut, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);

            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return false;
            else
            {
                //입력 파일이 img 파일이 아니라면 종료
                if (inPutFile.Extension != ".img")
                    return false;
            }

            try
            {
                #region Create JPG
                Gdal.AllRegister();
                Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);

                Band band1 = dataset.GetRasterBand(band); //특정 벤드 가져오기
                int width = band1.XSize;
                int height = band1.YSize;

                int startX = 0;
                int startY = 0;

                int nBandSpace = Gdal.GetDataTypeSize(DataType.GDT_Byte) / 8;
                int nPixelSpace = nBandSpace * dataset.RasterCount;
                int nLineSpace = nPixelSpace * width;

                double[] buffer = new double[width * height];
                CPLErr d = band1.ReadRaster(startX, startY, width, height, buffer, width, height, 0, 0);

                double[] minmax = new double[2];
                band1.ComputeRasterMinMax(minmax, 0);

                Bitmap bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double value = buffer[x + y * width];
                        if (!Double.IsNaN(value))
                            if (value >= minmax[0] && value <= minmax[1] && value != 0)
                            {
                                double colorValue = (value - minmax[0]) * 255 / (minmax[1] - minmax[0]);
                                Color newColor = Color.FromArgb(Convert.ToInt32(colorValue), Convert.ToInt32(colorValue), Convert.ToInt32(colorValue));
                                bitmap.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                Color newColor = Color.FromArgb(0, 0, 0, 0);
                                bitmap.SetPixel(x, y, newColor);
                            }
                    }
                }
                bitmap.Save(outPut, System.Drawing.Imaging.ImageFormat.Jpeg);

                dataset.Dispose();
                bitmap.Dispose();
                #endregion
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool ImgToKml(string inPut, string outPut, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);
            
            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return false;
            else
            {
                //입력 파일이 img 파일이 아니라면 종료
                if (inPutFile.Extension != ".img")
                    return false;
            }

            Gdal.AllRegister();

            try
            {
                #region 임시 이미지 생성
                Dataset pngDs = Gdal.Open(inPut, Access.GA_ReadOnly);

                Band pngBand1 = pngDs.GetRasterBand(band); //특정 벤드 가져오기
                int pngWidth = pngBand1.XSize;
                int pngHeight = pngBand1.YSize;

                int pngStartX = 0;
                int pngStartY = 0;

                double[] pngBuffer = new double[pngWidth * pngHeight];
                CPLErr pngD = pngBand1.ReadRaster(pngStartX, pngStartY, pngWidth, pngHeight, pngBuffer, pngWidth, pngHeight, 0, 0);

                double[] pngMinmax = new double[2];
                pngBand1.ComputeRasterMinMax(pngMinmax, 0);

                //이미지 생성
                Bitmap pngBitmap = new Bitmap(pngWidth, pngHeight);
                for (int x = 0; x < pngWidth; x++)
                {
                    for (int y = 0; y < pngHeight; y++)
                    {
                        double value = pngBuffer[x + y * pngWidth];
                        if (!Double.IsNaN(value))
                            if (value >= pngMinmax[0] && value <= pngMinmax[1] && value != 0)
                            {
                                double colorValue = ((value - pngMinmax[0]) / (pngMinmax[1] - pngMinmax[0])) * 255;
                                Color newColor = Color.FromArgb(Convert.ToInt32(colorValue), Convert.ToInt32(colorValue), Convert.ToInt32(colorValue));
                                pngBitmap.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                Color newColor = Color.FromArgb(0, 0, 0, 0);
                                pngBitmap.SetPixel(x, y, newColor);
                            }
                    }
                }
                FileInfo outPutFile_test = new FileInfo(outPut);
                string pngDsPath = string.Format("{0}\\test_{1}", outPutFile_test.DirectoryName, inPutFile.Name.Replace(".img", ".png"));
                pngBitmap.Save(pngDsPath, System.Drawing.Imaging.ImageFormat.Png);
                pngBitmap.Dispose();
                //이미지 생성 완료

                string[] testTiffoptions = null;
                OSGeo.GDAL.Driver srcPngDrv = Gdal.GetDriverByName("GTiff");
                Dataset srcPngDs = Gdal.Open(pngDsPath, Access.GA_ReadOnly);

                string tifDsPath = string.Format("{0}\\test_{1}", outPutFile_test.DirectoryName, inPutFile.Name.Replace(".img", ".tif"));
                Dataset dstPngDs = srcPngDrv.CreateCopy(tifDsPath, srcPngDs, 0, testTiffoptions, null, null);

                dstPngDs.SetProjection(pngDs.GetProjection());

                double[] png_geot = new double[6];
                pngDs.GetGeoTransform(png_geot);
                dstPngDs.SetGeoTransform(png_geot);

                dstPngDs.FlushCache();
                dstPngDs.Dispose();
                srcPngDs.Dispose();
                srcPngDrv.Dispose();
                pngDs.Dispose();
                File.Delete(pngDsPath);
                #endregion

                #region Get Coordinate
                double north = 36.16465526448886; //북
                double south = 35.97362616042732; //남
                double east = 127.5672085281825; //동
                double west = 127.3435070025512; //서

                Dataset dataset = Gdal.Open(tifDsPath, Access.GA_ReadOnly);

                string t_srs_wkt = "GEOGCS[\"WGS84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS84\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.01745329251994328]]";
                string s_srs_wkt = dataset.GetProjectionRef();

                Dataset dswarp = Gdal.AutoCreateWarpedVRT(dataset, s_srs_wkt, t_srs_wkt, ResampleAlg.GRA_NearestNeighbour, 0.125);


                string[] options = null;
                OSGeo.GDAL.Driver srcDrv = Gdal.GetDriverByName("GTiff");

                FileInfo outPutFile = new FileInfo(outPut);
                string testPath = string.Format("{0}\\test_{1}", outPutFile.DirectoryName, inPutFile.Name.Replace(".img", ".png"));
                Dataset dstDs = srcDrv.CreateCopy(testPath, dswarp, 0, options, null, null);

                dstDs.FlushCache();
                #endregion

                #region Create PNG

                string file_name = Path.GetFileNameWithoutExtension(inPutFile.Name) + "_kml.png";
                string pngPath = string.Format("{0}\\{1}", outPutFile.DirectoryName, file_name);

                Band band1 = dstDs.GetRasterBand(band); //특정 벤드 가져오기

                int startX = 0;
                int startY = 0;
                int width = band1.XSize;
                int height = band1.YSize;

                double[] geot = new double[6];
                dstDs.GetGeoTransform(geot);

                double Xp = geot[0] + width * geot[1] + height * geot[2]; //우하단
                double Yp = geot[3] + width * geot[4] + height * geot[5]; //우하단

                north = geot[3];        //북
                west = geot[0];         //서

                south = Yp;             //남
                east = Xp;              //동

                int nBandSpace = Gdal.GetDataTypeSize(DataType.GDT_Byte) / 8;
                int nPixelSpace = nBandSpace * dataset.RasterCount;
                int nLineSpace = nPixelSpace * width;

                double[] buffer = new double[width * height];
                CPLErr d = band1.ReadRaster(startX, startY, width, height, buffer, width, height, 0, 0);

                double[] minmax = new double[2];
                band1.ComputeRasterMinMax(minmax, 0);

                Bitmap bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double value = buffer[x + y * width];
                        if (!Double.IsNaN(value))
                            if (value >= minmax[0] && value <= minmax[1] && value != 0)
                            {
                                double colorValue = (value - minmax[0]) * 255 / (minmax[1] - minmax[0]);
                                Color newColor = Color.FromArgb(Convert.ToInt32(colorValue), Convert.ToInt32(colorValue), Convert.ToInt32(colorValue));
                                bitmap.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                Color newColor = Color.FromArgb(0, 0, 0, 0);
                                bitmap.SetPixel(x, y, newColor);
                            }
                    }
                }
                bitmap.Save(pngPath, System.Drawing.Imaging.ImageFormat.Png);
                #endregion

                #region Create KML
                StreamWriter sw = new StreamWriter(outPut);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.opengis.net/kml/2.2 http://schemas.opengis.net/kml/2.2.0/ogckml22.xsd http://www.google.com/kml/ext/2.2 http://code.google.com/apis/kml/schema/kml22gx.xsd\">");
                sw.WriteLine("<Document id=\"radar\">");
                sw.WriteLine("\t<name>" + inPutFile.Name + "</name>");
                sw.WriteLine("\t<Snippet></Snippet>");
                sw.WriteLine("\t<GroundOverlay id=\"0\">");
                sw.WriteLine("\t\t<Snippet></Snippet>");
                sw.WriteLine("\t\t<drawOrder>1000</drawOrder>");
                sw.WriteLine("\t\t<name>" + inPutFile.Name + "</name>");
                sw.WriteLine("\t\t<Icon>\r\t\t\t<href>" + file_name + "</href>\r\t\t\t<viewBoundScale>1.0</viewBoundScale>\r\t\t</Icon>");
                sw.WriteLine("\t\t<LatLonBox>");
                sw.WriteLine("\t\t\t<north>" + north.ToString() + "</north>"); //북
                sw.WriteLine("\t\t\t<south>" + south.ToString() + "</south>"); //남
                sw.WriteLine("\t\t\t<east>" + east.ToString() + "</east>"); //동
                sw.WriteLine("\t\t\t<west>" + west.ToString() + "</west>"); //서
                sw.WriteLine("\t\t\t<rotation>0</rotation>"); //회전
                sw.WriteLine("\t\t</LatLonBox>");
                sw.WriteLine("\t</GroundOverlay>");
                sw.WriteLine("</Document>");
                sw.WriteLine("</kml>");
                sw.Flush();
                sw.Close();
                sw.Dispose();
                #endregion

                dataset.Dispose();
                bitmap.Dispose();
                dstDs.Dispose();
                srcDrv.Dispose();
                dswarp.Dispose();
                File.Delete(testPath);
                File.Delete(tifDsPath);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool TiffToPng(string inPut, string outPut, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);

            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return false;
            else
            {
                //입력 파일이 tif / tiff 파일이 아니라면 종료
                if (inPutFile.Extension != ".tif" && inPutFile.Extension != ".tiff")
                    return false;
            }

            try
            {
                #region Create Png
                Gdal.AllRegister();
                Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);

                Band band1 = dataset.GetRasterBand(band); //특정 벤드 가져오기
                int width = band1.XSize;
                int height = band1.YSize;

                int startX = 0;
                int startY = 0;

                int nBandSpace = Gdal.GetDataTypeSize(DataType.GDT_Byte) / 8;
                int nPixelSpace = nBandSpace * dataset.RasterCount;
                int nLineSpace = nPixelSpace * width;

                double[] buffer = new double[width * height];
                CPLErr d = band1.ReadRaster(startX, startY, width, height, buffer, width, height, 0, 0);

                double[] minmax = new double[2];
                band1.ComputeRasterMinMax(minmax, 0);

                Bitmap bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double value = buffer[x + y * width];
                        if (!Double.IsNaN(value))
                            if (value >= minmax[0] && value <= minmax[1] && value != 0)
                            {
                                double colorValue = (value - minmax[0]) * 255 / (minmax[1] - minmax[0]);
                                Color newColor = Color.FromArgb(Convert.ToInt32(colorValue), Convert.ToInt32(colorValue), Convert.ToInt32(colorValue));
                                bitmap.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                Color newColor = Color.FromArgb(0, 0, 0, 0);
                                bitmap.SetPixel(x, y, newColor);
                            }
                    }
                }
                bitmap.Save(outPut, System.Drawing.Imaging.ImageFormat.Png);

                dataset.Dispose();
                bitmap.Dispose();
                #endregion
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool TiffToJpg(string inPut, string outPut, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);

            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return false;
            else
            {
                //입력 파일이 tif / tiff 파일이 아니라면 종료
                if (inPutFile.Extension != ".tif" && inPutFile.Extension != ".tiff")
                    return false;
            }

            try
            {
                #region Create Jpg
                Gdal.AllRegister();
                Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);

                Band band1 = dataset.GetRasterBand(band); //특정 벤드 가져오기
                int width = band1.XSize;
                int height = band1.YSize;

                int startX = 0;
                int startY = 0;

                int nBandSpace = Gdal.GetDataTypeSize(DataType.GDT_Byte) / 8;
                int nPixelSpace = nBandSpace * dataset.RasterCount;
                int nLineSpace = nPixelSpace * width;

                double[] buffer = new double[width * height];
                CPLErr d = band1.ReadRaster(startX, startY, width, height, buffer, width, height, 0, 0);

                double[] minmax = new double[2];
                band1.ComputeRasterMinMax(minmax, 0);

                Bitmap bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double value = buffer[x + y * width];
                        if (!Double.IsNaN(value))
                            if (value >= minmax[0] && value <= minmax[1] && value != 0)
                            {
                                double colorValue = (value - minmax[0]) * 255 / (minmax[1] - minmax[0]);
                                Color newColor = Color.FromArgb(Convert.ToInt32(colorValue), Convert.ToInt32(colorValue), Convert.ToInt32(colorValue));
                                bitmap.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                Color newColor = Color.FromArgb(0, 0, 0, 0);
                                bitmap.SetPixel(x, y, newColor);
                            }
                    }
                }
                bitmap.Save(outPut, System.Drawing.Imaging.ImageFormat.Jpeg);

                dataset.Dispose();
                bitmap.Dispose();
                #endregion
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool TiffToKml(string inPut, string outPut, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);
            bool isExtif = false;
            bool isExtiff = false;

            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return false;
            else
            {
                //입력 파일이 tif / tiff 파일이 아니라면 종료
                if (inPutFile.Extension != ".tif" && inPutFile.Extension != ".tiff")
                    return false;

                if (inPutFile.Extension == ".tif")
                    isExtif = true;

                if (inPutFile.Extension == ".tiff")
                    isExtiff = true;
            }

            Gdal.AllRegister();

            try
            {
                #region Get Coordinate
                double north = 36.16465526448886; //북
                double south = 35.97362616042732; //남
                double east = 127.5672085281825; //동
                double west = 127.3435070025512; //서

                Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);

                string t_srs_wkt = "GEOGCS[\"WGS84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS84\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.01745329251994328]]";
                string s_srs_wkt = dataset.GetProjectionRef();

                Dataset dswarp = Gdal.AutoCreateWarpedVRT(dataset, s_srs_wkt, t_srs_wkt, ResampleAlg.GRA_NearestNeighbour, 0.125);


                string[] options = null;
                OSGeo.GDAL.Driver srcDrv = Gdal.GetDriverByName("GTiff");

                FileInfo outPutFile = new FileInfo(outPut);
                string testPath = string.Empty;
                if (isExtif)
                    testPath = string.Format("{0}\\test_Png_{1}", outPutFile.DirectoryName, inPutFile.Name.Replace(".tif", ".png"));
                else if (isExtiff)
                    testPath = string.Format("{0}\\test_Png_{1}", outPutFile.DirectoryName, inPutFile.Name.Replace(".tiff", ".png"));
                Dataset dstDs = srcDrv.CreateCopy(testPath, dswarp, 0, options, null, null);

                dstDs.FlushCache();
                #endregion

                #region Create PNG

                string file_name = Path.GetFileNameWithoutExtension(inPutFile.Name) + "_kml.png";
                string pngPath = string.Format("{0}\\{1}", outPutFile.DirectoryName, file_name);

                Band band1 = dstDs.GetRasterBand(band); //특정 벤드 가져오기

                int startX = 0;
                int startY = 0;
                int width = band1.XSize;
                int height = band1.YSize;

                double[] geot = new double[6];
                dstDs.GetGeoTransform(geot);

                double Xp = geot[0] + width * geot[1] + height * geot[2]; //우하단
                double Yp = geot[3] + width * geot[4] + height * geot[5]; //우하단

                north = geot[3];        //북
                west = geot[0];         //서

                south = Yp;             //남
                east = Xp;              //동

                int nBandSpace = Gdal.GetDataTypeSize(DataType.GDT_Byte) / 8;
                int nPixelSpace = nBandSpace * dataset.RasterCount;
                int nLineSpace = nPixelSpace * width;

                double[] buffer = new double[width * height];
                CPLErr d = band1.ReadRaster(startX, startY, width, height, buffer, width, height, 0, 0);

                double[] minmax = new double[2];
                band1.ComputeRasterMinMax(minmax, 0);

                Bitmap bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double value = buffer[x + y * width];
                        if (!Double.IsNaN(value))
                            if (value >= minmax[0] && value <= minmax[1] && value != 0)
                            {
                                double colorValue = (value - minmax[0]) * 255 / (minmax[1] - minmax[0]);
                                Color newColor = Color.FromArgb(Convert.ToInt32(colorValue), Convert.ToInt32(colorValue), Convert.ToInt32(colorValue));
                                bitmap.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                Color newColor = Color.FromArgb(0, 0, 0, 0);
                                bitmap.SetPixel(x, y, newColor);
                            }
                    }
                }
                bitmap.Save(pngPath, System.Drawing.Imaging.ImageFormat.Png);
                #endregion

                #region Create KML
                StreamWriter sw = new StreamWriter(outPut);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.opengis.net/kml/2.2 http://schemas.opengis.net/kml/2.2.0/ogckml22.xsd http://www.google.com/kml/ext/2.2 http://code.google.com/apis/kml/schema/kml22gx.xsd\">");
                sw.WriteLine("<Document id=\"radar\">");
                sw.WriteLine("\t<name>" + inPutFile.Name + "</name>");
                sw.WriteLine("\t<Snippet></Snippet>");
                sw.WriteLine("\t<GroundOverlay id=\"0\">");
                sw.WriteLine("\t\t<Snippet></Snippet>");
                sw.WriteLine("\t\t<drawOrder>1000</drawOrder>");
                sw.WriteLine("\t\t<name>" + inPutFile.Name + "</name>");
                sw.WriteLine("\t\t<Icon>\r\t\t\t<href>" + file_name + "</href>\r\t\t\t<viewBoundScale>1.0</viewBoundScale>\r\t\t</Icon>");
                sw.WriteLine("\t\t<LatLonBox>");
                sw.WriteLine("\t\t\t<north>" + north.ToString() + "</north>"); //북
                sw.WriteLine("\t\t\t<south>" + south.ToString() + "</south>"); //남
                sw.WriteLine("\t\t\t<east>" + east.ToString() + "</east>"); //동
                sw.WriteLine("\t\t\t<west>" + west.ToString() + "</west>"); //서
                sw.WriteLine("\t\t\t<rotation>0</rotation>"); //회전
                sw.WriteLine("\t\t</LatLonBox>");
                sw.WriteLine("\t</GroundOverlay>");
                sw.WriteLine("</Document>");
                sw.WriteLine("</kml>");
                sw.Flush();
                sw.Close();
                sw.Dispose();
                #endregion

                dataset.Dispose();
                bitmap.Dispose();
                dstDs.Dispose();
                srcDrv.Dispose();
                dswarp.Dispose();
                File.Delete(testPath);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public double[] GetImageInfo(string inPut)
        {
            double[] rtn = new double[6];

            FileInfo inPutFile = new FileInfo(inPut);

            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return null;
            else
            {
                //입력 파일이 img 파일이 아니라면 종료
                if (inPutFile.Extension != ".img")
                    return null;
            }

            Gdal.AllRegister();
            Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);

            dataset.GetGeoTransform(rtn);

            return rtn;
        }

        public ImgUtmData[,] ImgToImgDataList(string inPut, ref int row, ref int col, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);
            
            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return null;
            else
            {
                //입력 파일이 img 파일이 아니라면 종료
                if (inPutFile.Extension != ".img")
                    return null;
            }

            Gdal.AllRegister();
            Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);

            int width = col = dataset.RasterXSize;
            int height = row = dataset.RasterYSize;

            Band band1 = dataset.GetRasterBand(band); //특정 벤드 가져오기

            double[] buffer = new double[width * height];
            band1.ReadRaster(0, 0, width, height, buffer, width, height, 0, 0);

            double[] geot = new double[6];
            dataset.GetGeoTransform(geot);

            ImgUtmData[,] latlonArr = new ImgUtmData[width, height];
            bool err_flag = false;
            double[] minmax = new double[2];
            try
            {
                band1.ComputeRasterMinMax(minmax, 0);
            }
            catch (Exception e)
            {
                err_flag = true;
            }
            

            /*
            int min = -1;
            int max = 1;
             */
            double min = minmax[0];
            double max = minmax[1];

            for (int x = 0; x < width; x++)
            { 
                for (int y = 0; y < height; y++)
                {
                    double value = buffer[x + y * width];

                    double colorValue = 0;
                    if (!Double.IsNaN(value))
                    {
                        if (err_flag)
                            colorValue = -999;
                        else if (value >= minmax[0] && value <= minmax[1])
                            colorValue = (value - min) * 255 / (max - min);
                    }

                    double Xp = geot[0] + x * geot[1] + y * geot[2];
                    double Yp = geot[3] + x * geot[4] + y * geot[5];

                    latlonArr[x, y] = new ImgUtmData(Xp, Yp, colorValue);

                    //latlonArr[x, y] = new ImgUtmData(Xp, Yp, buffer[x + y * width]);
                }
            }

            dataset.Dispose();

            return latlonArr;
        }
        
        public bool LatLon_TiffToUTM_Tiff(string inPut, string outPut, int band = 1)
        {
            FileInfo inPutFile = new FileInfo(inPut);

            //입력 파일이 없다면 종료
            if (!inPutFile.Exists)
                return false;
            else
            {
                //입력 파일이 tif / tiff 파일이 아니라면 종료
                if (inPutFile.Extension != ".tif" && inPutFile.Extension != ".tiff")
                    return false;
            }

            #region Create Tiff
            Gdal.AllRegister();
            Dataset dataset = Gdal.Open(inPut, Access.GA_ReadOnly);

            //SpatialReference dstt_srs = new SpatialReference("");
            //dstt_srs.SetFromUserInput("+proj=utm +zone=21 +ellps=WGS84 +datum=WGS84 +units=m +no_defs");
            string t_srs_wkt = "GEOGCS[\"WGS84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS84\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.01745329251994328]]";
            //dstt_srs.ExportToWkt(out t_srs_wkt);
            //SpatialReference src_srs = new SpatialReference("");
            //src_srs.SetFromUserInput("+proj=longlat");
            string s_srs_wkt = dataset.GetProjectionRef();
            //src_srs.ExportToWkt(out s_srs_wkt);

            Dataset dswarp = Gdal.AutoCreateWarpedVRT(dataset, s_srs_wkt, t_srs_wkt, ResampleAlg.GRA_NearestNeighbour, 0.125);
            

            string[] options = null;
            OSGeo.GDAL.Driver srcDrv = Gdal.GetDriverByName("GTiff");
            Dataset dstDs = srcDrv.CreateCopy(outPut, dswarp, 0, options, null, null);

            //dstDs.SetProjection(dataset.GetProjection());

            //double[] geot = new double[6];
            //dataset.GetGeoTransform(geot);
            //dstDs.SetGeoTransform(geot);

            dstDs.FlushCache();
            dstDs.Dispose();
            srcDrv.Dispose();


            #endregion

            return true;
        }

        private double[] CalcGeoTransform(Dataset dataSet, double north, double south, double east, double west)
        {
            Band band1 = dataSet.GetRasterBand(1);
            int width = band1.XSize;
            int height = band1.YSize;

            double[] newGeot = new double[6];
            double[] oldGeot = new double[6];

            dataSet.GetGeoTransform(oldGeot);

            newGeot[0] = west;
            newGeot[1] = (east - newGeot[0] - height * oldGeot[2]) / width;
            newGeot[2] = (east - newGeot[0] - width * oldGeot[1]) / height;
            newGeot[3] = north;
            newGeot[4] = (south - newGeot[3] - height * oldGeot[5]) / width;
            newGeot[5] = (south - newGeot[3] - width * oldGeot[4]) / height;

            return newGeot;
        }

        private void CheckGeoTransform(ref double north, ref double south, ref double east, ref double west, FileInfo fInfo)
        {
            if (fInfo.Name.IndexOf("sdci_asia") != -1)
            {
                north = 46.786566; //북
                south = 25.343909; //남
                east = 175.194848; //동
                west = 88.517499;  //서
            }
            else
            {
                north = 40.014640; //북
                south = 29.993614; //남
                east = 143.625186; //동
                west = 115.455945; //서
            }
        }
    }
}
