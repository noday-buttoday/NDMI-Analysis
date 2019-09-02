using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Threading;

using ShapeFileControl.Event;

namespace ShapeFileControl
{
    public class Coordinate
    {
        public double x { get; set; }
        public double y { get; set; }

        public double lat { get; set; }
        public double lon { get; set; }

        public void xy2ll()
        {
            CoordConversion cc = new CoordConversion();

            LatLon coordi = cc.Tm2Geo(x, y);

            this.lat = coordi.lat * 180.0 / Math.PI; ;
            this.lon = coordi.lon * 180.0 / Math.PI; ;
        }
    }

    public class Polygon
    {
        public int shapeType;
        public int numParts;
        public int numPoints;
        public int[] parts;
        public Coordinate center;
        public Coordinate[] box;
        public Coordinate[] points;

        public Polygon(int nParts, int nPoints)
        {
            shapeType = 0;
            box = new Coordinate[2];
            box[0] = new Coordinate();
            box[1] = new Coordinate();

            numParts = nParts;
            numPoints = nPoints;

            this.parts = new int[nParts];
            this.points = new Coordinate[nPoints];

            for (int i = 0; i < nPoints; ++i)
                points[i] = new Coordinate();

            center = new Coordinate();
        }

        public void CalcCenter()
        {
            center.lat = (box[0].lat + box[1].lat) / 2.0;
            center.lon = (box[0].lon + box[1].lon) / 2.0;

            center.x = (box[0].x + box[1].x) / 2.0;
            center.y = (box[0].y + box[1].y) / 2.0;
        }

    }

    public class ShapeReader
    {
	    public List<Polygon> ReadShapeFile(string path)
	    {
		    List<byte[]> shpList = new List<byte[]>();
		    List<byte[]> shxList = new List<byte[]>();
		    List<Polygon> polyList = new List<Polygon>();
		
		    DirectoryInfo dir = new DirectoryInfo(path);
		    FileInfo[] shpFiles = dir.GetFiles("*.shp");
		    FileInfo[] shxFiles = dir.GetFiles("*.shx");

		    int fileCount = shpFiles.Length;
		
		    for (int i = 0; i < fileCount; i++)
		    {                
			    FileStream shpFile = new FileStream(path + "\\" + shpFiles[i].Name, FileMode.Open);
			    FileStream shxFile = new FileStream(path + "\\" + shxFiles[i].Name, FileMode.Open);
			
			    byte[] shpArray = new byte[shpFile.Length];
			    byte[] shxArray = new byte[shxFile.Length];
			
			    int shpRead = shpFile.Read(shpArray, 0, (int)shpFile.Length);
			    int shxRead = shxFile.Read(shxArray, 0, (int)shxFile.Length);
			
			    shpList.Add(shpArray);
			    shxList.Add(shxArray);
			
			    if (shpRead == 0 && shxRead == 0)
				    return polyList;
			
			    shpFile.Close();
			    shxFile.Close();
		    }

		    for (int i = 0; i < shpList.Count; i++)
		    {
			    int shpLength = BitConverter.ToInt32(shpList[i], 24);
			    shpLength = Swap(shpLength) * 2;             // Shx 의 length는 Word 단위
			
			    int shxLength = BitConverter.ToInt32(shxList[i], 24);
                shxLength = Swap(shxLength) * 2;
			
			    int shxType = BitConverter.ToInt32(shxList[i], 32);

			    if (shxType == 5)
			    {
				    polyList = ReadPolygon(shxLength, shpList[i], shxList[i]);
			    }
			    if (shxType == 3)
			    {
				    polyList = ReadPolygon(shxLength, shpList[i], shxList[i]);
			    }

		    }
            return polyList;
	    }

        public List<Polygon> ReadShapeFile(string shp_file, string shx_file)
        {
            List<Polygon> polygon_list = new List<Polygon>();

            FileStream shpFile = new FileStream(shp_file, FileMode.Open);
            FileStream shxFile = new FileStream(shx_file, FileMode.Open);

            byte[] shpArray = new byte[shpFile.Length];
            byte[] shxArray = new byte[shxFile.Length];

            int shpRead = shpFile.Read(shpArray, 0, (int)shpFile.Length);
            int shxRead = shxFile.Read(shxArray, 0, (int)shxFile.Length);

            shpFile.Close();
            shxFile.Close();

            int shpLength = BitConverter.ToInt32(shpArray, 24);
            shpLength = Swap(shpLength) * 2;             // Shx 의 length는 Word 단위

            int shxLength = BitConverter.ToInt32(shxArray, 24);
            shxLength = Swap(shxLength) * 2;

            int shxType = BitConverter.ToInt32(shxArray, 32);

            if (shxType == 5)
            {
                polygon_list = ReadPolygon(shxLength, shpArray, shxArray);
            }
            if (shxType == 3)
            {
                polygon_list = ReadPolygon(shxLength, shpArray, shxArray);
            }


            return polygon_list;
        }

        public List<Point> ReadPointShapeFile(string shp_file, string shx_file)
        {
            List<Point> point_list = new List<Point>();

            FileStream shpFile = new FileStream(shp_file, FileMode.Open);
            FileStream shxFile = new FileStream(shx_file, FileMode.Open);

            byte[] shpArray = new byte[shpFile.Length];
            byte[] shxArray = new byte[shxFile.Length];

            int shpRead = shpFile.Read(shpArray, 0, (int)shpFile.Length);
            int shxRead = shxFile.Read(shxArray, 0, (int)shxFile.Length);

            shpFile.Close();
            shxFile.Close();

            int shpLength = BitConverter.ToInt32(shpArray, 24);
            shpLength = Swap(shpLength) * 2;             // Shx 의 length는 Word 단위

            int shxLength = BitConverter.ToInt32(shxArray, 24);
            shxLength = Swap(shxLength) * 2;

            int shxType = BitConverter.ToInt32(shxArray, 32);

            if (shxType == 1)
                point_list = ReadPoint(shxLength, shpArray, shxArray);

            return point_list;
        }


        //private List<Point> ReadPoint(int shxLength, byte[] shpArray, byte[] shxArray)
        //{
        //    int count = (shxLength - 100) / 8;
        //    int offset = 0;
        //    List<Point> point_list = new List<Point>();

        //    for (int i = 0; i < count; i++)
        //    {
        //        offset = BitConverter.ToInt32(shxArray, 100 + 8 * i);
        //        offset = Swap(offset) * 2 + 8;           // BigEndian, Word(16bit) 단위 // SHP 파일의 컨텐츠 위치

        //        int shapeType = BitConverter.ToInt32(shpArray, offset);

        //        Point point;
        //        if (shapeType != 0)
        //        {
        //            point = new Point(BitConverter.ToDouble(shpArray, offset + 4 + 8 * 0), BitConverter.ToDouble(shpArray, offset + 4 + 8 * 1));
        //            point_list.Add(point);
        //        }
        //    }

        //    return point_list;
        //}
        //public List<Polygon> ReadShapeFile(string shp_file, string shx_file)
        //{
        //    FileStream shpFile = new FileStream(shp_file, FileMode.Open);
        //    FileStream shxFile = new FileStream(shx_file, FileMode.Open);

        //    byte[] shpArray = new byte[shpFile.Length];
        //    byte[] shxArray = new byte[shxFile.Length];

        //    int shpRead = shpFile.Read(shpArray, 0, (int)shpFile.Length);
        //    int shxRead = shxFile.Read(shxArray, 0, (int)shxFile.Length);

        //    shpFile.Close();
        //    shxFile.Close();

        //    int shpLength = BitConverter.ToInt32(shpArray, 24);
        //    shpLength = Swap(shpLength) * 2;             // Shx 의 length는 Word 단위

        //    int shxLength = BitConverter.ToInt32(shxArray, 24);
        //    shxLength = Swap(shxLength) * 2;

        //    int shxType = BitConverter.ToInt32(shxArray, 32);

        //    if (shxType == 1)
        //        point_list = ReadPoint(shxLength, shpArray, shxArray);

        //    return point_list;
        //}

        private List<Point> ReadPoint(int shxLength, byte[] shpArray, byte[] shxArray)
        {
            int count = (shxLength - 100) / 8;
            int offset = 0;
            List<Point> point_list = new List<Point>();

            for (int i = 0; i < count; i++)
            {
                offset = BitConverter.ToInt32(shxArray, 100 + 8 * i);
                offset = Swap(offset) * 2 + 8;           // BigEndian, Word(16bit) 단위 // SHP 파일의 컨텐츠 위치

                int shapeType = BitConverter.ToInt32(shpArray, offset);

                Point point;
                if (shapeType != 0)
                {
                    point = new Point(BitConverter.ToDouble(shpArray, offset + 4 + 8 * 0), BitConverter.ToDouble(shpArray, offset + 4 + 8 * 1));
                    point_list.Add(point);
                }
            }

            return point_list;
        }

	    private List<Polygon> ReadPolygon(int shxLength, byte[] shpArray, byte[] shxArray)
	    {
		    int count = (shxLength - 100) / 8;
		    int offset = 0;
		    List<Polygon> polyList = new List<Polygon> ();

            CoordConversion cc = new CoordConversion();
		
		    for (int i = 0; i < count; i++)
		    {
                if((i % 100) == 0)
                    EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", i, count));

			    offset = BitConverter.ToInt32(shxArray, 100 + 8 * i);
                offset = Swap(offset) * 2 + 8;           // BigEndian, Word(16bit) 단위 // SHP 파일의 컨텐츠 위치

			    int shapeType = BitConverter.ToInt32(shpArray, offset);
			    int nParts = BitConverter.ToInt32(shpArray, offset + 36);
			    int nPoints = BitConverter.ToInt32(shpArray, offset + 40);

			    if(shapeType != 0)
			    {
				    Polygon poly = new Polygon(nParts, nPoints);
				    poly.shapeType = BitConverter.ToInt32(shpArray, offset);

                    poly.box[0].x = BitConverter.ToDouble(shpArray, offset + 4 + 8 * 0);
                    poly.box[0].y = BitConverter.ToDouble(shpArray, offset + 4 + 8 * 1);

                    poly.box[0].xy2ll();

                    poly.box[1].x = BitConverter.ToDouble(shpArray, offset + 4 + 8 * 2);
                    poly.box[1].y = BitConverter.ToDouble(shpArray, offset + 4 + 8 * 3);

                    poly.box[1].xy2ll();

				    for (int j = 0; j < nParts; j++)            // Parts
					    poly.parts[j] = BitConverter.ToInt32(shpArray, offset + 44 + 4 * j);
				
				    for (int j = 0; j < nPoints; j++)
				    {
                        poly.points[j].x = BitConverter.ToDouble(shpArray, offset + 44 + 4 * nParts + 16 * j);
                        poly.points[j].y = BitConverter.ToDouble(shpArray, offset + 44 + 4 * nParts + 16 * j + 8);

                        poly.points[j].xy2ll();
				    }

                    poly.CalcCenter();
				    polyList.Add(poly);
			    }
		    }
		    ////// 폴리곤 읽기 끝     
            EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", count, count));

		    return polyList;		
	    }

        private int Swap(int bigNumber)
        {
            byte[] temp = new byte[4];
            temp = BitConverter.GetBytes(bigNumber);
            Array.Reverse(temp);
            return BitConverter.ToInt32(temp, 0);
        }

        
    }
}