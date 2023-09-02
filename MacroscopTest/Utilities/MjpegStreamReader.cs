using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MacroscopTest.Utilities
{
    public class MjpegStreamReader
    {
        private BinaryReader _reader;

        public MjpegStreamReader(Stream stream)
        {
            _reader = new BinaryReader(stream);
        }

        public byte[] GetNextFrame()
        {
            try
            {
                // начальный маркер 0xFFD8
                while (_reader.ReadByte() != 0xFF || _reader.ReadByte() != 0xD8) { }

                // конечный маркер 0xFFD9
                using (MemoryStream frameStream = new MemoryStream())
                {
                    frameStream.WriteByte(0xFF);
                    frameStream.WriteByte(0xD8);

                    byte prevByte = 0xD8;
                    byte currentByte = _reader.ReadByte();
                    frameStream.WriteByte(currentByte);

                    while (!(prevByte == 0xFF && currentByte == 0xD9))
                    {
                        prevByte = currentByte;
                        currentByte = _reader.ReadByte();
                        frameStream.WriteByte(currentByte);
                    }

                    return frameStream.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
