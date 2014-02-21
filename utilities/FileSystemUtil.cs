using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Labs.Utility {
    public class FileSystem {

        public static void CreateDirectoryIfNeededAndAllowed(string path) {
            if (!Directory.Exists(path)) {
                if (DirectoryAllowed(path)) {
                    Logger.Log("CreateDirectoryIfNeededAndAllowed:" + path);
                    Directory.CreateDirectory(path);
                }
            }
        }

        public static bool DirectoryAllowed(string path) {
            bool allowCreate = true;
            return allowCreate;
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool versioned) {

            FileSystem.EnsureDirectory(sourceDirName, false);
            FileSystem.EnsureDirectory(destDirName, false);

            CreateDirectoryIfNeededAndAllowed(sourceDirName);

            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            CreateDirectoryIfNeededAndAllowed(destDirName);

            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files) {
                if (file.Extension != ".meta"
                    && file.Extension != ".DS_Store") {

                    string temppath = Paths.Combine(destDirName, file.Name);

                    if (!CheckFileExists(temppath)) {
                        file.CopyTo(temppath, true);
                    }
                }
            }

            if (copySubDirs) {

                foreach (DirectoryInfo subdir in dirs) {
                    string temppath = Paths.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, versioned);
                }
            }
        }

        public static void EnsureDirectory(string filePath) {
            EnsureDirectory(filePath, true);
        }

        public static void EnsureDirectory(string filePath, bool filterFileName) {

            string directory = filePath;
            if (filePath.IndexOf('.') > -1 && filterFileName) {
                directory = filePath.Replace(Path.GetFileName(filePath), "");
            }
            CreateDirectoryIfNeededAndAllowed(directory);
        }

        public static bool CheckFileExists(string path) {
            return File.Exists(path);
            ;
        }

        public static void CopyFile(string dataFilePath, string persistenceFilePath) {
            CopyFile(dataFilePath, persistenceFilePath, false);
        }

        public static void CopyFile(string dataFilePath, string persistenceFilePath, bool force) {

            EnsureDirectory(dataFilePath);
            EnsureDirectory(persistenceFilePath);
            if (CheckFileExists(dataFilePath) && (!CheckFileExists(persistenceFilePath) || force)) {
                File.Copy(dataFilePath, persistenceFilePath, true);
            }
        }

        public static void MoveFile(string dataFilePath, string persistenceFilePath) {
            MoveFile(dataFilePath, persistenceFilePath, false);
        }

        public static void MoveFile(string dataFilePath, string persistenceFilePath, bool force) {

            EnsureDirectory(dataFilePath);
            EnsureDirectory(persistenceFilePath);
            //LogUtil.Log("dataFilePath: " + dataFilePath);
            //LogUtil.Log("persistenceFilePath: " + persistenceFilePath);
            if (CheckFileExists(dataFilePath) && (!CheckFileExists(persistenceFilePath) || force)) {
                File.Move(dataFilePath, persistenceFilePath);
            }
        }

        public static byte[] ReadAllBytes(string fileName) {
            return File.ReadAllBytes(fileName);
        }

        public static void WriteAllBytes(string fileName, byte[] buffer) {
            EnsureDirectory(fileName);
            File.WriteAllBytes(fileName, buffer);
        }

        public static byte[] ReadStream(string fileName) {
            byte[] buffer = null;
            if (CheckFileExists(fileName)) {
                FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                long length = new FileInfo(fileName).Length;
                buffer = br.ReadBytes((int)length);
                br.Close();
                fs.Close();
            }
            return buffer;
        }

        public static void WriteStream(string fileName, byte[] data) {
            EnsureDirectory(fileName);
            StreamWriter sw = new StreamWriter(fileName, false, Encoding.ASCII);
            sw.Write(data);
            sw.Flush();
            sw.Close();
        }

        public static string ReadString(string fileName) {
            string contents = "";
            if (CheckFileExists(fileName)) {
                StreamReader sr = new StreamReader(fileName, true);
                contents = sr.ReadToEnd();
                sr.Close();
            }
            return contents;
        }

        public static void WriteString(string fileName, string data) {
            WriteString(fileName, data, false);
        }

        public static void WriteString(string fileName, string data, bool append) {
            EnsureDirectory(fileName);
            StreamWriter sw = new StreamWriter(fileName, append);
            sw.Write(data);
            sw.Flush();
            sw.Close();
        }

        public static void RemoveFile(string file) {
            if (CheckFileExists(file)) {
                File.Delete(file);
            }
        }

        public static void RemoveFilesLikeRecursive(DirectoryInfo dirInfo, string fileKey) {
            foreach (FileInfo fileInfo in dirInfo.GetFiles()) {
                if (fileInfo.FullName.Contains(fileKey)) {
                    File.Delete(fileInfo.FullName);
                }
            }

            foreach (DirectoryInfo dirInfoItem in dirInfo.GetDirectories()) {
                RemoveFilesLikeRecursive(dirInfoItem, fileKey);
            }
        }

        public static void CopyFilesLikeRecursive(
            DirectoryInfo dirInfoCurrent,
            DirectoryInfo dirInfoFrom,
            DirectoryInfo dirInfoTo,
            string filter,
            List<string> excludeExts) {

            foreach (FileInfo fileInfo in dirInfoCurrent.GetFiles()) {
                if (fileInfo.FullName.Contains(filter)) {
                    string fileTo = fileInfo.FullName.Replace(dirInfoFrom.FullName, dirInfoTo.FullName);
                    if (!CheckFileExtention(fileTo, excludeExts)) {
                        string directoryTo = Path.GetDirectoryName(fileTo);

                        if (!Directory.Exists(directoryTo)) {
                            Directory.CreateDirectory(directoryTo);
                        }

                        File.Copy(fileInfo.FullName, fileTo, true);
                    }
                }
            }

            foreach (DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
                CopyFilesLikeRecursive(dirInfoItem, dirInfoFrom, dirInfoTo, filter, excludeExts);
            }
        }

        public static bool CheckFileExtention(string path, List<string> extensions) {
            foreach (string ext in extensions) {
                if (path.ToLower().EndsWith(ext.ToLower())) {
                    return true;
                }
            }
            return false;
        }

        public static void MoveFilesLikeRecursive(
            DirectoryInfo dirInfoCurrent,
            DirectoryInfo dirInfoFrom,
            DirectoryInfo dirInfoTo,
            string filter,
            List<string> excludeExts) {
            foreach (FileInfo fileInfo in dirInfoCurrent.GetFiles()) {
                if (fileInfo.FullName.Contains(filter)) {
                    string fileTo = fileInfo.FullName.Replace(dirInfoFrom.FullName, dirInfoTo.FullName);
                    if (!CheckFileExtention(fileTo, excludeExts)) {
                        string directoryTo = Path.GetDirectoryName(fileTo);

                        if (!Directory.Exists(directoryTo)) {
                            Directory.CreateDirectory(directoryTo);
                        }

                        if (CheckFileExists(fileTo)) {
                            File.Delete(fileTo);
                        }

                        File.Move(fileInfo.FullName, fileTo);
                    }
                }
            }

            foreach (DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
                MoveFilesLikeRecursive(dirInfoItem, dirInfoFrom, dirInfoTo, filter, excludeExts);
            }
        }

        public static void RemoveDirectoriesLikeRecursive(
            DirectoryInfo dirInfoCurrent,
            string filterLike,
            string filterNotLike) {

            foreach (DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
                RemoveDirectoriesLikeRecursive(dirInfoItem, filterLike, filterNotLike);
            }

            if (dirInfoCurrent.FullName.Contains(filterLike)
                && !dirInfoCurrent.FullName.Contains(filterNotLike)) {
                Directory.Delete(dirInfoCurrent.FullName, true);
            }
        }
        public static bool CheckSignatureFile(string filepath, int signatureSize, string expectedSignature) {
            if (String.IsNullOrEmpty(filepath))
                throw new ArgumentException("Must specify a filepath");
            if (String.IsNullOrEmpty(expectedSignature))
                throw new ArgumentException("Must specify a value for the expected file signature");
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                if (fs.Length < signatureSize)
                    return false;
                byte[] signature = new byte[signatureSize];
                int bytesRequired = signatureSize;
                int index = 0;
                while (bytesRequired > 0) {
                    int bytesRead = fs.Read(signature, index, bytesRequired);
                    bytesRequired -= bytesRead;
                    index += bytesRead;
                }
                string actualSignature = BitConverter.ToString(signature);
                if (actualSignature == expectedSignature)
                    return true;
                else
                    return false;
            }
        }

        public static bool CheckSignatureString(string data, int signatureSize, string expectedSignature) {

            byte[] datas = Encoding.ASCII.GetBytes(data);
            using (MemoryStream ms = new MemoryStream(datas)) {
                if (ms.Length < signatureSize)
                    return false;
                byte[] signature = new byte[signatureSize];
                int bytesRequired = signatureSize;
                int index = 0;
                while (bytesRequired > 0) {
                    int bytesRead = ms.Read(signature, index, bytesRequired);
                    bytesRequired -= bytesRead;
                    index += bytesRead;
                }
                string actualSignature = BitConverter.ToString(signature);
                if (actualSignature == expectedSignature)
                    return true;
                else
                    return false;
            }
        }
    }
}
