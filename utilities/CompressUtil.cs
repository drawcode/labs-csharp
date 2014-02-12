using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Labs.Utility {

    public static class CompressUtil {

        public static string ToCompressed(this string val) {
            if (!IsStringCompressed(val)) {
                return CompressString(val);
            }
            return val;
        }

        public static string ToDecompressed(this string val) {
            if (IsStringCompressed(val)) {
                return DecompressString(val);
            }
            return val;
        }

        public static string CompressString(string text) {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true)) {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        public static string DecompressString(string compressedText) {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream()) {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress)) {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static bool IsStringCompressed(string data) {
            if (IsStringCompressedGZip(data) || IsStringCompressedPKZip(data)) {
                return true;
            }
            return false;
        }

        public static bool IsStringCompressedGZip(string data) {
            return FileSystemUtil.CheckSignatureString(data, 3, "1F-8B-08");
        }

        public static bool IsStringCompressedPKZip(string data) {
            return FileSystemUtil.CheckSignatureString(data, 4, "50-4B-03-04");
        }
    }
}
