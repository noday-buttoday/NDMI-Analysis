using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteDataPolling
{
    // 포인트 타입의 Shape 파일만 가능
    class Point
    {
        public double x { get; set; }
        public double y { get; set; }

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class ShapeWriter
    {
        private int record_nunber_;
        private int shape_type_;
        private List<Point> data_ = null;

        private FileStream fs_shp_;
        private FileStream fs_shx_;

        private int GetWord(int bytes)
        {
            return bytes / 2;
        }

        // type 1 이 포인트
        public void SetShapeData(List<Point> data, int shape_type = 1)
        {
            data_ = data;
            shape_type_ = shape_type;
        }

        public void OpenFile(String file_path)
        {
            String shp_file = file_path + ".shp";
            String shx_file = file_path + ".shx";
            
            fs_shp_ = new FileStream(shp_file, FileMode.Create);
            fs_shx_ = new FileStream(shx_file, FileMode.Create);
        }

        public void CloseFile()
        {
            fs_shp_.Flush(true);
            fs_shp_.Close();

            fs_shx_.Flush(true);
            fs_shx_.Close();
        }

        public void MakeStart()
        {
            int record_number = data_.Count;

            Byte[] header = MakeHeader(record_number, 0);
            fs_shx_.Write(header, 0, header.Length);

            header = MakeHeader(record_number, 1);
            fs_shp_.Write(header, 0, header.Length);

            foreach (Point p in data_)
            {
                Byte[] shx_record = GetShxRecordContent(20);
                Byte[] record_header = GetRecordHeader(20);

                Byte[] record_content = GetRecordContents(20, p.x, p.y);

                fs_shx_.Write(shx_record, 0, shx_record.Length);
                fs_shp_.Write(record_header, 0, record_header.Length);
                fs_shp_.Write(record_content, 0, record_content.Length);

            }
        }

        private int Swap(int number)
        {
            byte[] temp = new byte[4];
            temp = BitConverter.GetBytes(number);
            Array.Reverse(temp);
            return BitConverter.ToInt32(temp, 0);
        }

        private Byte[] MakeHeader(int record_number, int type)
        {
            Byte[] header = new Byte[100];
            MemoryStream ms = new MemoryStream(header);

            ms.Write(BitConverter.GetBytes(Swap(9994)), 0, 4);

            int length = 0;
            
            if (type == 0)      
                // shx
                length = GetWord((record_number * 8 + 100));
            else
                // shp
                length = GetWord((record_number * 28 + 100));

            ms.Seek(24, SeekOrigin.Begin);
            ms.Write(BitConverter.GetBytes(Swap(length)), 0, 4);
            ms.Write(BitConverter.GetBytes(1000), 0, 4);           // Version
            ms.Write(BitConverter.GetBytes(shape_type_), 0, 4);

            return header;
        }

        private Byte[] GetShxRecordContent(int content_length)
        {
            Byte[] content = new Byte[8];
            MemoryStream ms = new MemoryStream(content);

            int offset = GetWord((100 + 28 * record_nunber_));
            ms.Write(BitConverter.GetBytes(Swap(offset)), 0, 4);
            int word = GetWord(content_length);
            ms.Write(BitConverter.GetBytes(Swap(word)), 0, 4);

            return content;
        }

        private Byte[] GetRecordHeader(int content_length)
        {
            Byte[] record_header = new Byte[8];
            MemoryStream ms = new MemoryStream(record_header);

            int word = GetWord(content_length);
            ms.Write(BitConverter.GetBytes(Swap(record_nunber_++)), 0, 4);
            ms.Write(BitConverter.GetBytes(Swap(word)), 0, 4);

            return record_header;
        }

        private Byte[] GetRecordContents(int content_length, double x, double y)
        {
            Byte[] record_contents = new Byte[content_length];
            MemoryStream ms = new MemoryStream(record_contents);

            ms.Write(BitConverter.GetBytes(shape_type_), 0, 4);
            ms.Write(BitConverter.GetBytes(x), 0, 8);
            ms.Write(BitConverter.GetBytes(y), 0, 8);

            return record_contents;
        }
    }
}
